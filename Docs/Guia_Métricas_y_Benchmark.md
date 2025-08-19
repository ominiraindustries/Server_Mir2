# Guía de Métricas y Benchmark (Sin tocar runtime)

Objetivo: medir CPU, tiempo de tick y actividad por subsistema antes/después de optimizaciones. Todo es opt-in y no altera la lógica del servidor por defecto.

---

## 1) Qué medir

- CPU proceso servidor (% y por núcleo) y memoria (Working Set).
- Duración del tick principal (`TickElapsedMs`).
- Mapas procesados por tick (`MapsProcessedPerTick`).
- Respawns evaluados y saltados (`RespawnChecksEvaluated/Skipped`).
- Monstruos procesados por tick (`MonsterAIProcessed`, opcional).

## 2) Dónde instrumentar (referencias)

- `Server/MirEnvir/Envir.cs` (bucle principal): cronometraje con `Stopwatch` y contadores por tick.
- `Server/MirEnvir/Map.cs`: incrementar contadores en `Process()` y `ProcessRespawns()`.
- `Server/Settings.cs`: flags para activar logs de métricas (`EnablePerfMetricsLogging`, `PerfLogIntervalSeconds`).

## 3) Formato de logging (sugerido)

- Nivel: `Debug`/`Trace`.
- Frecuencia: cada `PerfLogIntervalSeconds` (ej. 30s) emitir línea con agregados p50/p95 si están disponibles o simples promedios.
- Ejemplo de línea:

```
perf tick=30s maps=1200 respawn_eval=800 respawn_skip=400 ai=15000 tick_avg_ms=2.4 tick_p95_ms=5.1 cpu=18% mem=220MB
```

## 4) Procedimiento de benchmark

- Estado inicial (baseline):
  - Iniciar servidor con flags de optimización en OFF.
  - Con 0 jugadores, registrar 5 minutos de métricas.
  - Con 1 jugador quieto en ciudad, 5 minutos.
  - Con 3–5 jugadores moviéndose/peleando en un mapa, 10 minutos.
- Estado optimizado (solo activar 1 cambio a la vez):
  - Activar `ThrottleEmptyMaps` y repetir exactamente el mismo guion.
  - Activar `SkipRespawnOnEmptyMap` y repetir.
- Comparar: CPU promedio, `TickElapsedMs` promedio y p95, `MapsProcessedPerTick`.

## 5) Herramientas externas (opcionales)

- Windows: Monitor de recursos / Performance Monitor (contadores de proceso y CPU por núcleo).
- Dotnet: `EventCounters` y `dotnet-counters monitor` (si se agrega instrumentación).
- Scripts PowerShell para muestrear CPU/memoria del proceso cada N segundos.

## 6) Criterios de éxito

- Con 0–2 jugadores, reducción del `TickElapsedMs` y de CPU sin impacto funcional.
- Con 3–5 jugadores, no peor que baseline; respawns y eventos siguen naturales.

## 7) Checklists de ejecución

- [ ] Log de métricas activado con intervalo fijo (ej. 30s).
- [ ] Misma duración y escenarios en baseline vs optimizado.
- [ ] Captura de logs en archivo para comparar (diff/grep).
- [ ] Reporte breve con tabla antes/después.
