# Guía de Trabajo Profesional (Git, Versionado, CI, Scripts)

Esta guía resume cómo trabajamos en este repo para mantener calidad y trazabilidad.

## Ramas y flujo de trabajo

- **main (protegida)**: siempre estable. Solo merges vía Pull Request (PR) con build verde.
- **feature/***: una rama por cambio. Ej: `feature/linux-headless`, `feature/optimizaciones-map`.
- **release/*** (opcional): para preparar versiones.
- **hotfix/***: arreglos urgentes sobre producción.

### Ciclo típico
1. Crear rama: `git checkout -b feature/nombre`
2. Commits frecuentes y claros: `feat: ...`, `fix: ...`, `docs: ...`
3. Subir: `git push -u origin feature/nombre`
4. Abrir PR a `main` en GitHub (revisión + CI)
5. Merge con "Squash and merge" cuando la CI esté en verde

## Tags y versiones

- **Tag inicial**: `v0.1.0` (base actual)
- Crear tag: `git tag -a vX.Y.Z -m "Descripción"`
- Enviar: `git push origin vX.Y.Z`
- Usos:
  - Volver a una versión: `git checkout v0.1.0`
  - Crear rama desde una versión: `git checkout -b hotfix/rollback v0.1.0`

## Revertir cambios

- Ver historial: `git log --oneline --graph --decorate --all`
- Revertir uno o varios commits:
  - `git revert <SHA>` o `git revert <SHA1> <SHA2> ...`
- Volver a `main`: `git checkout main`

## CI (Integración continua) — GitHub Actions

Archivo: `.github/workflows/build.yml`
- Ejecuta en **Windows** por WinForms (`net7.0-windows`).
- En **push** y **PR** contra `main`.
- Pasos: restaurar y compilar `Shared`, `Server` y `Server.MirForms`.
- Resultado: PR verde/rojo. Con rama protegida, solo se puede hacer merge si está en verde.

## Scripts locales

- `build-all.ps1`
  - Compila los 3 proyectos en orden.
  - Uso: `./build-all.ps1 -Configuration Release` (o `Debug`), `-Clean` opcional.

## Buenas prácticas

- Commits pequeños, mensajes claros.
- PR con descripción de cambios e impacto.
- Versionar **scripts SQL** y **config** (no `Build/`).
- Mantener **Components/** en control de versiones (DLLs necesarios para GUI).

## Roadmap resumido

- Compatibilidad Linux headless (nuevo proyecto `Server.Headless`).
- Flags de rendimiento y métricas en `Envir`/`Map`.
- Opcional: panel web de administración (solo localhost + túnel SSH al inicio).

## Referencias de estructura

- `Shared/Shared.csproj`: lib multi-plataforma (net7.0)
- `Server/Server.Library.csproj`: core del servidor
- `Server.MirForms/Server.csproj`: GUI (WinForms, `net7.0-windows`, salida en `Build/Server/...`)
- `.github/workflows/build.yml`: CI de compilación
- `build-all.ps1`: build local con un solo comando

---
Si una nueva IA continúa el proyecto, seguir este documento asegura consistencia, reproducibilidad y calidad.
