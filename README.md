# Crystal Mir2 Server

Servidor privado de Legend of Mir 2.

Este repositorio contiene el servidor (lógica de juego y GUI) y documentación de operación.

## Requisitos
- .NET SDK (versión indicada por `global.json` o la usada en los scripts de build)
- Windows (probado en Windows 10/11)
- MariaDB/MySQL accesible (recomendado MariaDB 10.5+). Configura conexión en `Configs/Setup.ini`.

## Compilación
- Script recomendado:
  - `./build-all.ps1` (genera artefactos en `Build/Server/Release/`)

## Ejecución rápida
1) Base de datos: crea el esquema mínimo de cuentas ejecutando `Docs/sql/schema_accounts_only.sql` en tu MariaDB.
2) Copia/ajusta configuración en `Configs/Setup.ini` (ver ejemplo en `Docs/Setup.ini.example`). Asegúrate de completar las claves `MariaDB_*`.
3) Ejecuta `Build/Server/Release/Server.exe` (o `Server.dll`).
4) Logs generales: `Logs/`.

Importante (0.3.4): para persistencia de ítems ejecutar también `Docs/sql/schema_characters_only.sql` y `Docs/sql/schema_character_items.sql` en la misma base de datos.

## Cuentas en MariaDB (0.3.3)
Desde la versión 0.3.3, las cuentas se guardan y leen desde MariaDB como fuente de verdad.

- Lectura/GUI: el servidor carga y muestra únicamente cuentas existentes en MariaDB.
- Creación: nuevas cuentas se crean directamente en la tabla `Accounts` y el `Id` autoincremental pasa a ser `Index` interno.
- Guardado: en cada guardado, `AccountList` se sincroniza con DB mediante upsert.
- Esquema requerido (ver `Docs/sql/schema_accounts_only.sql`):
  - `Accounts.Id` PRIMARY KEY AUTO_INCREMENT
  - `Accounts.AccountID` UNIQUE KEY
- Configuración necesaria en `Configs/Setup.ini`:
  - `MariaDB_Host`, `MariaDB_Port`, `MariaDB_Database`, `MariaDB_User`, `MariaDB_Password`, `MariaDB_SslRequired`

## Seguridad de Economía e Ítems (0.3.x)
Se centralizó el manejo de oro y se agregó auditoría y límites configurables. Además, se amplió la auditoría a movimientos de ítems.

- Archivo de ejemplo: `Docs/Setup.ini.example` (sección `[Security]`).
- Copiar esa sección a `Configs/Setup.ini`.

Claves principales:
- `EnableSecurityLogs` (true/false): activa logs de seguridad.
- `SecurityLogToFile` (true/false): escribe a `Logs/Security_YYYYMMDD.log`.
- `SecurityLogRetentionDays` (int): retención deseada (informativo por ahora).
- `EconomyMinIntervalMs` (int): intervalo mínimo recomendado entre acciones sensibles.
- `MaxGoldPerOp` (uint): tope por operación de oro (protege de overflows/abusos).
- `DropGoldMax` (uint): máximo por acción de dropear oro.
- `TradeGoldMax` (uint): máximo por acción de depositar oro en trade.

Rutas ya protegidas:
- `PlayerObject.DropGold(...)`: aplica `DropGoldMax`, audita intentos cappeados e insuficiencia de fondos; cobra vía `SpendGold("drop")`.
- `PlayerObject.TradeGold(...)`: aplica `TradeGoldMax`; descuenta vía `SpendGold("trade_send")`.
- `PlayerObject.PickUp(...)`: registra pickups de ítems/oro (origen `ground` -> destino `inventory`).
- Trade commit: registra movimientos `trade` -> `inventory` y motivos de oro `trade_receive`.

Logs de seguridad:
- Carpeta: `Logs/`
- Formato: `Security_YYYYMMDD.log`
- Ejemplos:
  - `[2025-08-19T08:20:00Z] ECONOMY account=... char=... action=spend amount=10 before=5000 after=4990 meta=drop`
  - `[2025-08-19T08:22:10Z] ITEM account=... char=... action=drop uid=... index=... count=1 src=equipment dst=ground map=... x=... y=...` 

## Próximos pasos sugeridos
- Migrar compras/ventas NPC/Gameshop, teleport, auction, mail fees, guild bank a `SpendGold/GainGold` con razones.
- Rate-limiting por opcode usando `EconomyMinIntervalMs`.

## Changelog

### 0.3.4
- Persistencia de ítems de personaje (Inventory/Equipment/Quest) en MariaDB usando la tabla `CharacterItems`.
- Repositorio `CharacterItemRepository` para cargar/guardar con serialización binaria de `UserItem`.
- Al cargar, los ítems se "bindean" para resolver `ItemInfo` correctamente.
- Guardado a MariaDB ocurre en cada autosave y al cerrar/guardar el servidor.
- Se inserta el personaje en DB si su `Index` es 0 antes de guardar ítems (asegura FK válida).
- Bugfix: `NullReferenceException` en `CharacterInfo.Save()` al serializar `Heroes` (se añade guardia y se usa `MaximumHeroCount`).

### 0.3.3
- Cuentas migradas completamente a MariaDB (lectura y escritura).
- Creación de cuentas directamente en DB con `Id` como `Index`.
- Guardados realizan upsert de cuentas a la base de datos.

### 0.3.2
- GUI: `PlayerInfoForm` añade recuadro "Gold Actions" (Give/Take) para oro por personaje (`Character.Gold`). Funciona con jugadores online y offline.
- GUI: pestaña "Players Online" muestra columna de oro y refresco en vivo.
- Fix: tipos `int/uint` en índices de personaje para evitar CS1503.

### 0.3.1
- Fix: ahora se pueden soltar al suelo ítems equipados (Equipment -> Ground). Se eliminan correctamente del slot, se refrescan stats y se sincroniza la UI.
- Auditoría: se agregan logs de ítems para `pickup`, `trade commit` y `drop` (incluye `src`, `dst`, mapa y coordenadas).
- Economía: motivos explícitos en oro (ej. `pickup`, `trade_receive`).

### 0.3.0
- Seguridad de economía: control centralizado de oro, límites por operación (`DropGoldMax`, `TradeGoldMax`), y logging estructurado.

## Estructura relevante
- Código servidor: `Server/`
- Formularios GUI: `Server.MirForms/`
- Librería compartida: `Shared/`
- Configs: `Configs/`
- Documentación: `Docs/`
- Scripts: `build-all.ps1`, `run-server.ps1`

## Contribución
- Rama de trabajo: `feature/net8-upgrade`.
- Hacer PRs contra la rama principal cuando corresponda.

## Notas de autosave (0.3.4)
- El servidor realiza autosave cada `SaveDelay` minutos (configurable en `Settings`/`Setup.ini`).
- Durante el autosave, se ejecuta la persistencia en MariaDB de cuentas, personajes e ítems (`BeginSaveAccounts`).

## Licencia
- Ver archivo LICENSE si aplica.
