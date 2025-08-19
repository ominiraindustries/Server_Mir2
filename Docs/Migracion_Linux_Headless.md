# Migración a Linux (Headless) — Opciones y Plan de Cambios

Este documento describe opciones para ejecutar el servidor en Linux (VPS) sin interfaz gráfica, y analiza los cambios mínimos necesarios en el código y en la estructura del proyecto.

## Situación actual (proyectos y dependencias)

- `Server.MirForms/Server.csproj`
  - `TargetFramework: net7.0-windows`
  - `OutputType: WinExe`
  - `UseWindowsForms: true`
  - Referencias a DLLs de UI: `CustomFormControl.dll`, `Microsoft.VisualBasic.PowerPacks.Vs.dll` (Windows-only)
  - Referencias a proyectos: `Server/Server.Library.csproj`, `Shared/Shared.csproj`

- `Server/Server.Library.csproj`
  - `TargetFramework: net7.0-windows`
  - `UseWindowsForms: true` (innecesario para el core)
  - Paquete: `log4net` (compatible cross-platform)
  - Referencia a `Shared`

- `Shared/Shared.csproj`
  - `TargetFramework: net7.0` (multi-plataforma)

- Código revisado: no se detectaron `using System.Windows.Forms` en `Server/` ni P/Invoke `DllImport`. La GUI está encapsulada en `Server.MirForms`.

## Objetivo

Ejecutar el servidor en Linux (Debian/Ubuntu/CentOS/Alma, etc.), como proceso de consola/servicio (systemd), sin dependencia de WinForms.

## Opciones de migración

1) Headless nuevo proyecto (recomendado)
- Crear `Server.Headless` (Console `net7.0`) que referencie `Server` y `Shared`.
- Mover/extraer el arranque del servidor (work loop, networking, carga de mapas, etc.) a un punto de entrada headless (`Program.cs`), idealmente vía .NET Generic Host (`Microsoft.Extensions.Hosting`).
- Mantener `Server.MirForms` para desarrollo/administración en Windows (opcional), pero no requerido en Linux.
- Ventajas: separación limpia de UI y core; empaquetado/publish sencillo para Linux.

2) Multi-target en `Server/Server.Library.csproj`
- Cambiar a `<TargetFrameworks>net7.0;net7.0-windows</TargetFrameworks>`.
- Eliminar `UseWindowsForms` del core.
- Condicionales `#if WINDOWS` si hubiese código específico (no detectado ahora).
- Ventajas: un solo ensamblado core para ambos entornos.

3) Ejecutar `Server.MirForms` bajo Wine (no recomendado en producción)
- Posible pero frágil (drivers, GDI+/WinForms, fuentes, input…).
- Difícil de automatizar como servicio.

4) Contenedor Docker
- Empaquetar `Server.Headless` en imagen oficial `mcr.microsoft.com/dotnet/runtime:7.0`.
- Despliegue reproducible en VPS.

## Cambios de código recomendados

- Core sin WinForms
  - En `Server/Server.Library.csproj`: eliminar `<UseWindowsForms>true</UseWindowsForms>` y retarget a `net7.0` o multi-target.
  - Verificar que ningún archivo del core use `System.Windows.Forms.Timer`. Usar `System.Threading.Timer`/`System.Timers.Timer`.

- Punto de entrada headless
  - Nuevo proyecto `Server.Headless` (Console):
    - `Program.cs` que configure logging (log4net), lea `Settings` y arranque `Envir` y la red (`StartNetwork`, `StartEnvir`/loop).
    - Mecanismo de shutdown limpio (Ctrl+C / SIGTERM): `Console.CancelKeyPress` y `IHostApplicationLifetime`.

- Logging
  - `log4net` es compatible en Linux. Revisar rutas de logs y permisos: usar `Path.Combine` y rutas relativas a `ContentPath`/`AppContext.BaseDirectory`.

- Paths/FS (Linux case-sensitive)
  - Revisar accesos a ficheros (`Settings.MapPath`, assets, SQL):
    - Asegurar uso de `Path.Combine` y nombres de archivos en minúsculas consistentes.
    - Evitar separadores hardcodeados `\\`.

- Red y puertos
  - `MirNetwork/*` debería funcionar en Linux con `System.Net.Sockets`.
  - Asegurar binding en `IPAddress.Any` y configurar puertos en firewall del VPS.

- Timers y tiempo
  - Confirmar que las dependencias del loop usan `Stopwatch`, `Environment.TickCount64` o `DateTime.UtcNow`. Evitar dependencias de `System.Windows.Forms.Application.DoEvents()` (solo UI).

- Señales y servicio
  - Añadir manejo de señales: cerrar sockets, guardar estado (`SaveAccounts`, `SaveGuilds`, etc.) en `Stop`.

## Cambios en estructura de proyecto

- Crear `Server.Headless/Server.Headless.csproj`:
  - `TargetFramework: net7.0`
  - Referencias a `Server/Server.Library.csproj` y `Shared/Shared.csproj`.
  - No `UseWindowsForms`.

- Ajustar `Server/Server.Library.csproj`:
  - Opción A: `TargetFramework: net7.0` (si no hay UI en el core).
  - Opción B: `TargetFrameworks: net7.0;net7.0-windows`.
  - Quitar `<UseWindowsForms>true</UseWindowsForms>` si es innecesario.

- Mantener `Server.MirForms` solo para UI Windows.

## Programa headless (borrador de flujo)

- `Program.cs` (esqueleto conceptual):
  1. Configurar log4net.
  2. Cargar `Settings`.
  3. Instanciar/arrancar `Envir` (método de inicio del servidor ya existente: loop y networking).
  4. Hook para `Ctrl+C`/SIGTERM y llamar a `StopNetwork`, `StopEnvir`, `Save*`.

Si se migra a Generic Host:
- `Host.CreateDefaultBuilder()`
- `ConfigureServices`: registrar `Envir` como `IHostedService` (o adaptador) con `StartAsync/StopAsync`.
- Ventajas: lifecycle y logging integrados.

## Publicación y ejecución en Linux

- Build self-contained (opcional):
  - `dotnet publish Server.Headless -c Release -r linux-x64 --self-contained false`
  - Salida: `bin/Release/net7.0/linux-x64/publish/`

- Servicio systemd (ejemplo):
```
[Unit]
Description=Crystal Mir2 Server
After=network.target

[Service]
WorkingDirectory=/opt/mir2
ExecStart=/usr/bin/dotnet /opt/mir2/Server.Headless.dll
Restart=always
RestartSec=5
User=mir2
Environment=DOTNET_ENVIRONMENT=Production

[Install]
WantedBy=multi-user.target
```

- Logs: redirigir a archivos (log4net) y/o `journalctl`.

## Validaciones previas a producción

- __Pruebas de arranque__: carga de mapas/DB sin errores de path.
- __Pruebas de red__: conexiones de cliente, latencias.
- __Persistencia__: guardados periódicos funcionan; permisos de escritura.
- __Shutdown__: cierre limpio y persistencia final.

## Riesgos y mitigaciones

- __Dependencias Windows inadvertidas__
  - Mitigar con multi-target y CI que compile `net7.0` Linux.

- __Case-sensitivity en rutas de assets__
  - Auditoría de nombres y normalización.

- __Diferencias en rendimiento de timers/schedulers__
  - Medir p50/p95 del tick y ajustar.

## Plan sugerido (fases cortas)

1. Crear `Server.Headless` (Console) y punto de entrada mínimo que arranque el core.
2. Retarget `Server.Library` a `net7.0` o multi-target y quitar `UseWindowsForms`.
3. Auditoría rápida de paths y timers en `Server/` y `Shared/`.
4. Publicar para `linux-x64` y probar en VPS (o Docker) con systemd.
5. Añadir métricas básicas (duración tick, players conectados) para validar estabilidad.
