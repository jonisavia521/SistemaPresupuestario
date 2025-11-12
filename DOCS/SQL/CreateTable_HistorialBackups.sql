-- ============================================
-- Script de Creación de Tabla HistorialBackups
-- Sistema Presupuestario - Backup/Restore
-- ============================================

USE [SistemaPresupuestario]
GO

-- Verificar si la tabla ya existe y eliminarla si es necesario (opcional)
IF OBJECT_ID('dbo.HistorialBackups', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.HistorialBackups
END
GO

-- Crear tabla HistorialBackups
CREATE TABLE dbo.HistorialBackups (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    FechaHora DATETIME NOT NULL DEFAULT GETDATE(),
    RutaArchivo NVARCHAR(500) NOT NULL,
    Estado NVARCHAR(50) NOT NULL, -- 'Exitoso', 'Fallido'
    MensajeError NVARCHAR(1000) NULL,
    UsuarioApp NVARCHAR(100) NOT NULL
);
GO

-- Crear índice en FechaHora para mejorar consultas de historial
CREATE NONCLUSTERED INDEX IX_HistorialBackups_FechaHora
ON dbo.HistorialBackups(FechaHora DESC);
GO

-- Crear índice en Estado para filtrar por estado
CREATE NONCLUSTERED INDEX IX_HistorialBackups_Estado
ON dbo.HistorialBackups(Estado);
GO

-- Comentarios de la tabla
EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'Tabla de historial de operaciones de Backup y Restore de la base de datos', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'HistorialBackups';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'Identificador único del registro', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'HistorialBackups', 
    @level2type=N'COLUMN', @level2name=N'ID';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'Fecha y hora de la operación', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'HistorialBackups', 
    @level2type=N'COLUMN', @level2name=N'FechaHora';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'Ruta completa del archivo de backup', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'HistorialBackups', 
    @level2type=N'COLUMN', @level2name=N'RutaArchivo';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'Estado de la operación: Exitoso o Fallido', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'HistorialBackups', 
    @level2type=N'COLUMN', @level2name=N'Estado';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'Mensaje de error en caso de fallo', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'HistorialBackups', 
    @level2type=N'COLUMN', @level2name=N'MensajeError';
GO

EXEC sys.sp_addextendedproperty 
    @name=N'MS_Description', 
    @value=N'Usuario de la aplicación que ejecutó la operación', 
    @level0type=N'SCHEMA', @level0name=N'dbo', 
    @level1type=N'TABLE', @level1name=N'HistorialBackups', 
    @level2type=N'COLUMN', @level2name=N'UsuarioApp';
GO

PRINT 'Tabla HistorialBackups creada exitosamente'
GO

-- Script de prueba: Insertar un registro de ejemplo
-- DESCOMENTAR SOLO PARA TESTING
/*
INSERT INTO dbo.HistorialBackups (RutaArchivo, Estado, MensajeError, UsuarioApp)
VALUES ('C:\Backups\Test_Backup.bak', 'Exitoso', NULL, 'ADMIN');

-- Verificar
SELECT * FROM dbo.HistorialBackups;
*/
