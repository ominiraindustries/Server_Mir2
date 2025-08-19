# Informe de Seguridad y Modernización del Servidor Crystal Mir2

Autor: Equipo de modernización
Fecha: Actual

## 1. Resumen ejecutivo

Este documento define la estrategia para modernizar y endurecer la seguridad del servidor Crystal Mir2, con foco en:

- Autenticación segura: hashing robusto de contraseñas, verificación segura y migración transparente desde el esquema legado.
- Endurecimiento de login: bloqueo incremental por cuenta y rate limiting por IP.
- Validación estricta de paquetes, control de tamaños y manejo de errores.
- Observabilidad y auditoría: logs detallados y estructurados.
- Compatibilidad hacia atrás con la base de datos existente.

## 2. Alcance

- Seguridad de contraseñas y autenticación.
- Flujo de login, cambio de contraseña y HTTPLogin.
- Límite anti-flood en login por IP y endurecimiento en `MirConnection`.
- Validaciones y límites en serialización/deserialización de paquetes.
- Pruebas unitarias y de integración para seguridad.

## 3. Estado actual (hallazgos clave)

- `Server/Utils/Crypto.cs`:
  - Legado: PBKDF2-SHA1 con `Iterations=50`, `HashSize=24`, `SaltSize=24`.
  - Almacenamiento inseguro del hash: conversión de bytes a `UTF8` (no base64), potencial corrupción de datos no-UTF8.

- `Server/MirEnvir/Envir.cs`:
  - `Login(ClientPackets.Login p, MirConnection c)`: compara `account.Password` contra `p.Password` tras hashear con legado. Mutaba el contenido del paquete (`p.Password`).
  - `ChangePassword(...)`: similitud en verificación.
  - `HTTPLogin(...)`: comparaba strings directamente, inconsistente con el uso de sal.
  - Bloqueo incremental por cuenta (`WrongPasswordCount` y ban temporal tras 5 fallos) ya implementado.

- `Server/MirNetwork/MirConnection.cs`:
  - `Login(C.Login)` únicamente enruta a `Envir.Login(p, this)` (correcto). Hay anti-flood de paquetes genérico, sin rate limiting específico para login por IP.

- `Shared/Packet.cs`:
  - Validación mínima. Oportunidad para límites de tamaño y manejo de errores más específicos.

## 4. Diseño objetivo

- Hashing fuerte: PBKDF2 con SHA-256, iteraciones altas (100k por defecto), salida en Base64 y formato con prefijo para autoidentificar esquema.
  - Formato: `PBKDF2$sha256$<iterations>$<base64(hash)>`
- Verificación segura: soporte dual (fuerte + legado), comparación en tiempo constante.
- Migración transparente: tras login/cambio exitoso con hash legado, regenerar sal y persistir el nuevo hash fuerte.
- No mutar los paquetes de entrada (`ClientPackets`), mantener inmutabilidad del dato recibido.
- Rate limiting por IP: ventana deslizante (p.ej. 10 intentos/minuto) y cooldown temporal (p.ej. 2 minutos) además del contador por cuenta.
- Logging de auditoría con contexto (accountId, ip, resultado, `wasLegacy`, `migrated`).

## 5. Cambios propuestos por archivo

- `Server/Utils/Crypto.cs` (clase `Crypto`)
  - Añadir:
    - `HashPassword(string password, byte[] salt)` fuerte (PBKDF2-SHA256, Base64, prefijo y `Iterations=100_000`).
    - `HashPasswordLegacy(string password, byte[] salt)` (PBKDF2-SHA1, 50 iteraciones, bytes->UTF8 para compatibilidad).
    - `VerifyPassword(string password, byte[] salt, string storedHash)` (acepta ambos esquemas, comparación tiempo-constante).
    - `IsStrongHash(string stored)`, `GenerateSalt(int size)` y sobrecarga por defecto.
  - Constantes: `LegacySaltSize=24`, `LegacyHashSize=24`, `LegacyIterations=50`; `SaltSize=32`, `HashSize=32`, `Iterations=100_000`.

- `Server/MirEnvir/Envir.cs`
  - `Login(ClientPackets.Login p, MirConnection c)`:
    - Reemplazar mutación de `p.Password` por `Crypto.VerifyPassword(...)` con `account.Salt` y `account.Password`.
    - Si `!IsStrongHash(account.Password)` y verificación OK, migrar: `account.Password = p.Password;` (setter rehash fuerte y nueva sal).
    - Mantener/ajustar bloqueo incremental por cuenta.
    - Añadir rate limiting por IP (ver sección 6) y logs adicionales.
  - `ChangePassword(ClientPackets.ChangePassword p, MirConnection c)`:
    - Verificar `p.CurrentPassword` con `VerifyPassword(...)`.
    - En éxito, `account.Password = p.NewPassword;` (setter rehash fuerte + nueva sal) y limpiar `RequirePasswordChange`.
  - `HTTPLogin(string AccountID, string Password)`:
    - Utilizar `VerifyPassword(...)` y migrar si era legado.

- `Server/MirDatabase/AccountInfo.cs`
  - Revisar el setter de `Password`:
    - Generar sal y almacenar hash usando `Crypto.HashPassword(...)`.
    - Compatibilidad: si la serialización de `Salt` asume 24 bytes fijos, usar `GenerateSalt(Crypto.LegacySaltSize)` temporalmente.
  - Confirmar serialización/versión: comprobar si el writer/reader guarda longitud de la sal o asume fija.

- `Server/Settings.cs`
  - Añadir ajustes para rate limiting:
    - `MaxLoginAttemptsPerWindow = 10`
    - `LoginWindowSeconds = 60`
    - `LoginCooldownSeconds = 120`
  - Opcional: `PasswordHashIterations` para configurar iteraciones.

- `Server/MirNetwork/MirConnection.cs`
  - Mantener enrutar sin mutar paquetes.
  - Opcional: enfriar por conexión si envía múltiples `Login` consecutivos.

- `Shared/Packet.cs`
  - Endurecer `ReceivePacket(...)` y `ReadPacket(...)` en subclases:
    - Verificar longitudes máximas de strings/arrays.
    - Errores de deserialización -> desconexión controlada.

- UI/Admin (`Server.MirForms/*`) – Opcional
  - Exponer métricas de intentos fallidos recientes y listas de IPs bajo cooldown.

## 6. Rate limiting por IP (diseño)

- Estructuras (en `Envir`):
  - `Dictionary<string, SlidingWindow>` por IP.
  - `Dictionary<string, DateTime> _ipCooldownUntil` para cooldowns.
- Lógica:
  - En cada petición de login/HTTPLogin:
    - Si `Now < _ipCooldownUntil[ip]` -> rechazar.
    - Registrar intento en ventana deslizante.
    - Si excede `MaxLoginAttemptsPerWindow` en `LoginWindowSeconds` -> poner en cooldown `LoginCooldownSeconds` y rechazar.
- Configurable por `Settings`.

## 7. Formatos de hash de contraseña

- Legado (actualmente almacenado en DB):
  - Algoritmo: PBKDF2-SHA1, 50 iteraciones, 24 bytes.
  - Representación: bytes crudos convertidos a `UTF8` (no seguro, mantener solo por compatibilidad).

- Fuerte (nuevo):
  - Algoritmo: PBKDF2-SHA256, 100k iteraciones, 32 bytes (tamaño configurable).
  - Formato de almacenamiento: `PBKDF2$sha256$<iterations>$<base64(hash)>`.
  - Detección: `IsStrongHash(hash)`.

## 8. Compatibilidad y migración

- Verificación dual con `VerifyPassword(...)`:
  - Si el hash almacenado tiene prefijo -> verificar con SHA-256 e iteraciones incluidas.
  - Si no -> verificar con legado (SHA1/50) generando string legado.
- Migración automática:
  - Tras verificación exitosa con hash legado, regenerar sal y guardar hash fuerte.
- Tamaño de sal:
  - Riesgo de compatibilidad si la serialización de `AccountInfo.Salt` asume 24 bytes.
  - Mitigación: mantener `LegacySaltSize=24` temporalmente hasta confirmar serialización; luego migrar a 32.

## 9. Logging y auditoría

- Mantener `MessageQueue.Enqueue(...)` y ampliar contexto:
  - Campos recomendados: `accountId`, `ip`, `result`, `wasLegacy`, `migrated`, `wrongPasswordCount`, `rateLimited`.
- Futuro: migrar a `Microsoft.Extensions.Logging` para logs estructurados.

## 10. Plan de pruebas

- Unit tests (`Crypto`):
  - `VerifyPassword` con hash legado correcto/incorrecto.
  - `VerifyPassword` con hash fuerte correcto/incorrecto.
  - Migración: tras verificación con legado, guardar y verificar como fuerte.

- Integración (`Envir`):
  - `Login`:
    - Éxito con legado -> migración y `LoginSuccess`.
    - 5 intentos fallidos -> `LoginBanned` temporal.
    - Rate limiting por IP -> rechazo/cooldown.
  - `ChangePassword`:
    - Fallo de contraseña actual -> `Result=5`.
    - Éxito -> `Result=6`, nuevo hash fuerte.
  - `HTTPLogin`: espejo de `Login`.

- Compatibilidad:
  - Cargar cuentas existentes (hash legado) y verificar que no hay excepciones de deserialización (especialmente en `Salt`).
  - Crear nuevas cuentas y reiniciar servidor.

## 11. Despliegue y rollback

- Paso 1: Confirmar serialización de `AccountInfo.Salt` (longitud fija vs variable).
- Paso 2: Ajustar setter de `AccountInfo.Password` para usar `GenerateSalt(LegacySaltSize)` si fuera necesario.
- Paso 3: Deploy con métricas de CPU/latencia en login.
- Rollback:
  - Si hay problemas: bajar iteraciones temporalmente o revertir verificación a esquema legado (sin modificar datos de cuentas ya migradas).

## 12. Riesgos conocidos

- Rendimiento: 100k iteraciones aumenta coste CPU. Ajustar según hardware y número de logins concurrentes.
- Serialización de `Salt`: confirmar longitud fija/variable.
- `HTTPLogin` cambia a verificación con sal; si existían integraciones externas que enviaban hash ya procesado, deberán enviar password plano (como el cliente del juego).

## 13. Checklist de implementación

- [ ] Revisar `AccountInfo` serialización de `Salt` y ajustar tamaño.
- [ ] Implementar rate limiting por IP en `Envir` + `Settings`.
- [ ] Ampliar logs de auditoría (añadir campos sugeridos).
- [ ] Añadir pruebas unitarias de `Crypto` y `Envir`.
- [ ] Endurecer `Shared/Packet.cs` (límites de tamaño y errores).
- [ ] Documentar parámetros de seguridad en `Settings`.

## 14. Referencias a código

- `Server/Utils/Crypto.cs` – clase `Crypto` (hash/verify, legacy vs strong).
- `Server/MirEnvir/Envir.cs` – métodos `Login(...)`, `ChangePassword(...)`, `HTTPLogin(...)`.
- `Server/MirDatabase/AccountInfo.cs` – campos `Password`, `Salt`, setter de `Password`.
- `Server/MirNetwork/MirConnection.cs` – método `Login(C.Login)`.
- `Shared/Packet.cs` – deserialización/validación de paquetes.
- `Server/Settings.cs` – configuración de seguridad y rate limiting.
