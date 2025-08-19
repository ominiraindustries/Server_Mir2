# Glosario simple: EF Core, Pomelo, Dapper, MySqlConnector

## ¿Qué es cada cosa?

- **EF Core**: Framework ORM de .NET. Te permite trabajar con la base de datos usando clases C# (entidades) sin escribir SQL la mayor parte del tiempo. Gestiona migraciones de esquema, relaciones, cambios, etc.

- **Pomelo.EntityFrameworkCore.MySql**: Proveedor de EF Core para MySQL/MariaDB. Es el “driver” específico que conecta EF Core con MySQL. Sin él, EF Core no sabría hablar con MySQL.

- **Dapper**: Micro-ORM muy ligero. Tú escribes el SQL, y Dapper mapea resultados a objetos C# automáticamente. Menos “magia” que EF Core, más control y normalmente más rendimiento bruto.

- **MySqlConnector**: Driver ADO.NET para MySQL. Nivel más bajo. Lo usan EF Core (a través del proveedor) o Dapper para abrir conexiones y ejecutar comandos.

## ¿Cuándo usar cada uno?

- **EF Core + Pomelo**
  - Ventajas:
    - Menos SQL manual; productividad alta.
    - Migraciones de esquema integradas (crear/actualizar tablas con comandos).
    - Tracking de entidades y relaciones.
  - Inconvenientes:
    - Curva de aprendizaje mayor.
    - Rendimiento más variable si no se configura bien.
  - Úsalo si: prefieres velocidad de desarrollo, migraciones automáticas y menos SQL.

- **Dapper + MySqlConnector**
  - Ventajas:
    - Muy rápido y predecible.
    - Control total del SQL.
  - Inconvenientes:
    - Escribes tú el SQL (más trabajo).
    - No trae migraciones integradas (tú gestionas DDL/scripts).
  - Úsalo si: quieres máximo control/rendimiento y te sientes cómodo escribiendo SQL.

## Recomendación para este proyecto

- Empezar con **EF Core + Pomelo** para ir rápido en la fase 1 (Accounts/Characters/Storage) y porque necesitamos evolucionar el esquema (migraciones) durante la modernización.
- Mantener abierta la opción de pasar a **Dapper** para rutas “calientes” (hot paths) en el futuro si detectamos cuellos de botella.

## Términos relacionados

- **ORM**: Object-Relational Mapper. Traduce entre objetos C# y tablas SQL.
- **Migraciones**: Scripts generados por herramienta para crear/actualizar el esquema de la base de datos conforme cambian tus entidades.
- **DbContext**: Clase central de EF Core que representa una sesión con la base de datos.
- **POCO**: Clases C# simples (Plain Old CLR Object) que representan tus datos (p. ej. `AccountEntity`).

## Guía rápida de configuración (paquetes, conexión y ejemplos)

- **Paquetes NuGet recomendados (.csproj)**
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.*" />
  <PackageReference Include="Pomelo.EntityFrameworkCore.MySql" Version="8.0.*" />
  <PackageReference Include="Dapper" Version="2.*" />
  <PackageReference Include="MySqlConnector" Version="2.*" />
</ItemGroup>
```
- **Connection String (MySQL, utf8mb4)**
```ini
Server=127.0.0.1;Port=3306;Database=crystalmir2;User ID=mir_user;Password=mir_pass;
CharSet=utf8mb4;SslMode=Required;DefaultCommandTimeout=30;TreatTinyAsBoolean=false
```
- **EF Core (DbContext mínimo con Pomelo)**
```csharp
using Microsoft.EntityFrameworkCore;

public class GameDbContext : DbContext
{
    public DbSet<AccountEntity> Accounts { get; set; }
    public DbSet<CharacterEntity> Characters { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        var cs = "Server=127.0.0.1;Database=crystalmir2;User ID=mir_user;Password=mir_pass;CharSet=utf8mb4;";
        options.UseMySql(cs, ServerVersion.AutoDetect(cs),
            mySql => mySql.CharSetBehavior(CharSetBehavior.AppendToAllColumns));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ejemplo: claves, índices, collation si aplica
        // modelBuilder.Entity<AccountEntity>().HasKey(x => x.AccountID);
    }
}
```
- **Dapper (consulta rápida)**
```csharp
using Dapper;
using MySqlConnector;

const string cs = "Server=127.0.0.1;Database=crystalmir2;User ID=mir_user;Password=mir_pass;CharSet=utf8mb4;";
await using var conn = new MySqlConnection(cs);
await conn.OpenAsync();

var accounts = await conn.QueryAsync<AccountDto>(
    "SELECT AccountID, Username, Gold FROM Accounts WHERE IsActive = 1 LIMIT 50");
```
> Nota: mantener `utf8mb4` en conexión y esquema (`Docs/sql/schema_mysql.sql`) para tildes y ñ.

## Qué implicaría elegir EF Core aquí

- Crear `GameDbContext` con `DbSet<AccountEntity>`, `DbSet<CharacterEntity>`, `DbSet<StorageItemEntity>`.
- Configurar Pomelo con la `ConnectionString` en `Settings`.
- Generar migración inicial (crea tablas) y aplicar.
- Implementar repositorios usando el `DbContext`.

## Qué implicaría elegir Dapper aquí

- Crear una fábrica de conexiones (`MySqlConnection`).
- Escribir a mano el DDL (CREATE TABLE ...) y los SQL CRUD (SELECT/INSERT/UPDATE/DELETE).
- Implementar repositorios ejecutando esos SQL y mapeando con Dapper.
