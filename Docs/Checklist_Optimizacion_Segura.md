# Checklist de Optimización Segura (sin romper runtime por defecto)

Este checklist prioriza cambios documentales y con flags apagados por defecto. Sirve para implementar mejoras gradualmente y con riesgo mínimo.

---

## 1) Configuración en `Server/Settings.cs`

- [ ] Añadir flags (por defecto a `false`):
  - `ThrottleEmptyMaps` (bool, default false)
  - `EmptyMapIntervalMs` (int, default 5000)
  - `SkipRespawnOnEmptyMap` (bool, default false)
  - `AlwaysActiveMaps` (lista de nombres/IDs)
- [ ] Documentar en comentarios qué hace cada flag.
- [ ] No activar por defecto en builds de Release iniciales.

## 2) Instrumentación y métricas (solo contadores/logs)

- [ ] En `Envir` (bucle principal):
  - `MapsProcessedPerTick` (int)
  - `RespawnChecksEvaluated` y `RespawnChecksSkipped` (int)
  - `MonsterAIProcessed` (int opcional)
  - `TickElapsedMs` (double, `Stopwatch`)
- [ ] Log nivel `Debug`/`Trace` con muestreo: cada N segundos/minutos.
- [ ] No modificar lógica del juego; solo medir.

## 3) Throttle de mapas vacíos (gating básico)

- [ ] En el loop de `MapList`, si `ThrottleEmptyMaps && map.Players.Count == 0 && !AlwaysActiveMaps.Contains(map)`, procesar el mapa solo si `Now - map.LastProcessTime >= EmptyMapIntervalMs`.
- [ ] Añadir `LastProcessTime` al objeto `Map` (no persistente) o mantener un diccionario en `Envir`.
- [ ] Asegurar que puertas/acciones críticas no dependan de tick continuo (agregar exclusiones por mapa si es necesario).

## 4) Gating de respawns en mapas vacíos

- [ ] En `Map.ProcessRespawns()`, si `SkipRespawnOnEmptyMap && Players.Count == 0 && !AlwaysActiveMaps.Contains(map)`, retornar temprano.
- [ ] (Opcional) Al detectar entrada de jugadores tras vacío, habilitar “catch-up” limitado: máximo X spawns por tick.

## 5) Validaciones y pruebas manuales

- [ ] Con flags en OFF, confirmar que no hay cambios de comportamiento.
- [ ] Activar solo `ThrottleEmptyMaps` en un entorno de pruebas con 0–2 jugadores y verificar CPU.
- [ ] Añadir/quitar un jugador de un mapa y verificar que el mapa vuelve a procesarse con normalidad.
- [ ] Revisar `AlwaysActiveMaps`: ciudades/eventos/dragón.

## 6) Rollback rápido

- [ ] Mantener capacidad de desactivar todos los flags en runtime/reinicio.
- [ ] En caso de problema, apagar flags y volver al comportamiento previo sin tocar código.

## 7) Trabajo futuro (opt-in)

- [ ] Hibernación de IA por distancia/visibilidad con un flag separado.
- [ ] Presupuesto de IA por tick.
- [ ] Activación por regiones (streaming) — cambios más profundos.
