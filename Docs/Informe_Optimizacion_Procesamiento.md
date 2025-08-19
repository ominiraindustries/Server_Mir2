# Informe de Optimización del Procesamiento (Mapas y Monstruos)

Fecha: 2025-08-19 09:03

## Resumen ejecutivo

- El servidor procesa todos los mapas en cada tick y además recorre la lista global de objetos (incl. monstruos), independientemente de la actividad de jugadores.
- Esto provoca consumo de CPU innecesario cuando hay pocos o ningún jugador conectado.
- Se proponen mejoras graduales: throttling de mapas vacíos, gating de respawns sin jugadores, “hibernación” de IA de monstruos cuando no haya jugadores cerca, y métricas para observar impacto.

## Estado actual (cómo funciona hoy)

- __Bucle principal de servidor__: `Server/MirEnvir/Envir.cs`
  - Recorre la lista global de objetos (`Envir.Objects`, `LinkedList<MapObject>`) y llama `current.Value.Process()` según `OperateTime` y otras condiciones (multihilo para monstruos).
  - Tras procesar objetos, recorre todos los mapas `MapList` y llama `map.Process()` en cada tick.
  - Gestiona guardados periódicos, mensajes, y otros subsistemas como `DragonSystem?.Process()`.

- __Procesamiento de mapas__: `Server/MirEnvir/Map.cs`
  - `Map.Process()` ejecuta:
    - `ProcessRespawns()` para respawn de monstruos por `Respawns` (ver `List<MapRespawn>`).
    - Gestión de puertas (`Doors`) y su autocierre por tiempo.
    - Efectos globales del mapa (rayos `Lightning`, fuego `Fire`), que iteran sobre `Players` del mapa para elegir ubicaciones de efectos.
    - `ActionList` (acciones diferidas), llamando `Process(DelayedAction)` y eliminando cada acción ejecutada.
  - `ProcessRespawns()` decide cuándo spawnear más monstruos por cada `MapRespawn`, con soporte de `Envir.RespawnTick` y `RespawnTime`.

- __Monstruos e IA__: `Server/MirObjects/MonsterObject.cs` y `Server/MirObjects/Monsters/*.cs`
  - `MonsterObject` define tiempos (`ActionTime`, `MoveTime`, `AttackTime`, `RegenTime`, `SearchTime`, `RoamTime`…), banderas (`CanMove`, `CanAttack`).
  - Cada clase concreta de monstruo implementa su comportamiento en `Process()`/acciones.
  - Los monstruos son añadidos a `Map` y al índice global de objetos para ser procesados.

## Archivos y símbolos implicados

- __`Server/MirEnvir/Envir.cs`__
  - `public List<Map> MapList`
  - Bucle principal: recorrido de `Objects` y luego `for (var i = 0; i < MapList.Count; i++) MapList[i].Process();`
  - `RespawnTimer RespawnTick`, `DragonSystem?.Process()`

- __`Server/MirEnvir/Map.cs`__
  - `public List<PlayerObject> Players`, `public List<MapRespawn> Respawns`, `public List<DelayedAction> ActionList`
  - `Process()`, `ProcessRespawns()`, manejo de `Doors`, `LightningTime`, `FireTime`.

- __`Server/MirObjects/MonsterObject.cs`__
  - Temporizadores y condiciones de IA: `CanMove`, `CanAttack`, `SearchDelay`, `RoamDelay`, etc.
  - `Spawn(MapRespawn)`, `Die()`, `RefreshAll()`.

- __Otros__ (a revisar al implementar):
  - `Server/MirObjects/Monsters/*.cs` para ajustes finos de IA.
  - `Settings` (configuración de intervalos, flags de activación).
  - `Server.MirDatabase` para `MapInfo`, `MonsterInfo`, `MapRespawnInfo`.

## Cuellos de botella detectados

- __Procesamiento de todos los mapas en cada tick__: `Map.Process()` corre aunque `map.Players.Count == 0`.
- __Respawns en mapas vacíos__: `ProcessRespawns()` evalúa y puede intentar spawnear incluso sin jugadores presentes.
- __IA de monstruos en “off-screen”__: monstruos siguen buscando/merodeando aunque no haya jugadores en cercanías (dependiendo del AI concreto), activando pathfinding, colisiones, etc.
- __Acciones diferidas y efectos globales__: aunque el coste pueda ser menor cuando `Players` está vacío, hay sobrecarga de bucle por mapa.

## Propuestas de mejora (por fases)

### Fase 1: Configuración y gating básico
- __Throttle de mapas vacíos__
  - Añadir flags en `Settings`: `ThrottleEmptyMaps` (bool) y `EmptyMapIntervalMs` (int, p. ej. 3000–10000 ms).
  - En el bucle que recorre `MapList`, si `map.Players.Count == 0`, llamar `map.Process()` solo si han pasado `EmptyMapIntervalMs` desde la última vez.
  - Mantener un sello de tiempo por mapa (p. ej. `LastProcessTime`) para el control del intervalo.

- __Gating de respawns__
  - En `Map.ProcessRespawns()`, si `Players.Count == 0` y no hay condiciones especiales (eventos, mapas persistentes), saltar el cálculo de respawns.
  - Opcional: cuando un jugador entra a un mapa tras estar vacío, realizar “catch-up” parcial de respawns con límite (presupuesto) para evitar picos.

- __Métricas y logging__
  - Contadores por tick: número de mapas procesados, número de respawns evaluados, tiempo del bucle principal (`Stopwatch`).
  - Logs de depuración activables mediante flag para comparar antes/después.

### Fase 2: IA e hibernación de monstruos
- __Hibernación por distancia/visibilidad__
  - Si un monstruo no tiene jugadores visibles en radio configurable, aumentar sus intervalos (`SearchDelay`, `RoamDelay`, `OperateTime`) o saltar `Process` salvo cada N segundos.
  - Persistir estado “Idle/Hibernating” y salir de él cuando un jugador entra en rango.

- __Presupuesto de IA__
  - Límite de monstruos procesados por tick por mapa (fair scheduling), para evitar micro-picos en mapas densos.

- __Despawn/desactivar lejana__ (opcional, depende de diseño de juego)
  - Despawning de monstruos muy alejados y sin actividad durante mucho tiempo, o migrarlos a estado “simulado” de bajo coste.

### Fase 3: Activación espacial por regiones (streaming)
- Dividir el mapa en celdas/regiones activas alrededor de los jugadores.
- Procesar respawns, IA y acciones solo en regiones activas.
- Requiere cambios más profundos, pero ofrece mejor escalabilidad.

## Consideraciones de gameplay y riesgos

- __Eventos y sistemas persistentes__: `DragonSystem?.Process()`, `Conquests`, `SafeZones` y minas pueden requerir procesamiento continuo. Añadir listas blancas de mapas siempre activos.
- __Scripts de NPC/monstruos__: algunos eventos de `Spawn`/`Die` ejecutan scripts (`MonsterNPC`). Asegurar que el gating no rompa secuencias dependientes del tiempo real.
- __Entrada repentina de jugadores__: evitar “storm” de respawns al entrar; usar catch-up limitado y/o spawn paulatino.
- __Sincronización multihilo__: el servidor ya usa multihilo para monstruos. Cualquier gating debe respetar locks/colecciones (`LinkedList<MapObject>`, `Map.Respawns`).

## Plan de implementación (mínima intrusión)

1) __Config y toggles__
- Agregar en `Settings`:
  - `ThrottleEmptyMaps` = true (por defecto configurable).
  - `EmptyMapIntervalMs` = 5000 (tuning).
  - `SkipRespawnOnEmptyMap` = true.
  - `AlwaysActiveMaps` = lista (IDs o nombres) para excluir mapas críticos.

2) __Gating en mapas__
- En el bucle de `Envir` que recorre `MapList`, comprobar `Players.Count` y `AlwaysActiveMaps` para decidir procesar o saltar por intervalo.
- Guardar `LastProcessTime` por mapa para cálculo del intervalo.

3) __Gating en respawns__
- En `Map.ProcessRespawns()`, si el mapa está vacío y no está en `AlwaysActiveMaps`, devolver temprano.
- Añadir flag de “catch-up limitado” cuando un jugador entra (p. ej., máximo X spawns por tick).

4) __Métricas__
- Exponer contadores en el puerto de estado o en logs: `MapsProcessedPerTick`, `RespawnChecksSkipped`, `MonsterAIProcessed`.
- Registrar tiempo del bucle (p50/p95) antes/después.

5) __Fase 2 (opcional)__
- Añadir estado “Idle/Hibernating” a `MonsterObject`, y ajustar `Search/Roam` cuando no hay jugadores cerca.

## Indicadores de éxito

- Reducción del tiempo del tick del servidor y del uso de CPU con 0–5 jugadores conectados.
- Disminución de `MapsProcessedPerTick` cuando la mayoría de mapas están vacíos.
- Sin regresiones de gameplay (respawns y eventos siguen sintiéndose naturales cuando un jugador entra a un mapa inactivo).

## Trabajo futuro

- Activación por regiones y streaming de contenido.
- Persistencia de estado “simulado” de monstruos para grandes poblaciones off-screen.
- Ajuste fino por tipo de mapa/instancia (ciudades, dungeons, eventos).

## Checklist de implementación (fase 1, seguro)

- [ ] Añadir flags en `Server/Settings.cs` (por defecto OFF):
  - `ThrottleEmptyMaps`, `EmptyMapIntervalMs`, `SkipRespawnOnEmptyMap`, `AlwaysActiveMaps`.
- [ ] Instrumentación básica (solo contadores y tiempos, sin cambiar lógica):
  - `MapsProcessedPerTick`, `RespawnChecksEvaluated/Skipped`, `MonsterAIProcessed` (opcional), `TickElapsedMs`.
- [ ] Gating mínimo:
  - Throttle de `Map.Process()` cuando `Players.Count == 0` respetando `AlwaysActiveMaps` y `EmptyMapIntervalMs`.
  - Early-return en `ProcessRespawns()` con `SkipRespawnOnEmptyMap` y mapa vacío.
- [ ] Logs con muestreo (debug/trace) cada N segundos.
- [ ] Validación manual en entorno de pruebas con 0–2 jugadores.
- [ ] Rollback inmediato: desactivar flags y reiniciar.

## Plan de métricas y benchmark

- Escenarios:
  - 0 jugadores (5 min), 1 jugador en ciudad (5 min), 3–5 jugadores en combate (10 min).
- Baseline: todas las flags OFF. Registrar `TickElapsedMs` promedio y p95, `MapsProcessedPerTick`, `RespawnChecks*`, CPU y memoria.
- Comparativa: activar solo una flag a la vez y repetir guion. Documentar resultados.
- Herramientas: `Stopwatch` en `Envir`, logs agregados cada `PerfLogIntervalSeconds`. Opcional `dotnet-counters` o Monitor de recursos de Windows.
