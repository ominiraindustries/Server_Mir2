-- Bootstrap MySQL para Crystal Mir2
-- Ejecutar como usuario con privilegios suficientes (ej. root)

-- Ajuste manual de credenciales antes de ejecutar:
--   Usuario:    'mir2_app'
--   Host:       '%' (o IP específica del servidor de la app)
--   Contraseña: 'REEMPLAZAR_POR_PASSWORD_SEGURO'

-- Crear base de datos si no existe (charset/colación acorde al schema)
CREATE DATABASE IF NOT EXISTS `crystal_mir2`
  DEFAULT CHARACTER SET utf8mb4
  DEFAULT COLLATE utf8mb4_unicode_ci;

-- Crear usuario de aplicación (si no existe)
CREATE USER IF NOT EXISTS 'mir2_app'@'%' IDENTIFIED BY 'REEMPLAZAR_POR_PASSWORD_SEGURO';
-- Alternativa con host específico:
-- CREATE USER IF NOT EXISTS 'mir2_app'@'127.0.0.1' IDENTIFIED BY 'REEMPLAZAR_POR_PASSWORD_SEGURO';

-- Otorgar permisos mínimos sobre el esquema de la aplicación
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE, SHOW VIEW
ON `crystal_mir2`.*
TO 'mir2_app'@'%';

-- Si se usa migración/creación de tablas desde la app (EF Core), añadir:
-- GRANT CREATE, ALTER, INDEX, DROP ON `crystal_mir2`.* TO 'mir2_app'@'%';

FLUSH PRIVILEGES;

-- Requisitos de SSL (recomendado):
-- 1) Configurar MySQL con certificados (ca, server-cert, server-key) y activar require_secure_transport=ON.
-- 2) Crear el usuario exigiendo SSL:
--    CREATE USER 'mir2_app'@'<host>' IDENTIFIED BY '...' REQUIRE SSL;
-- 3) En la cadena de conexión (MySqlConnector/Pomelo), usar:
--    Server=<host>;Database=crystal_mir2;User Id=mir2_app;Password=...;SslMode=Required;TrustServerCertificate=False;CertificateFile=<path opcional>;

-- Comprobaciones rápidas
SHOW GRANTS FOR 'mir2_app'@'%';
SHOW DATABASES LIKE 'crystal_mir2';
