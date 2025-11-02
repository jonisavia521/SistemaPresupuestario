-- Script SQL para agregar los campos necesarios en la tabla Cliente
-- Ejecutar este script en la base de datos SistemaPresupuestario

USE [SistemaPresupuestario]
GO

-- Verificar si las columnas ya existen antes de agregarlas
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'CodigoVendedor')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [CodigoVendedor] VARCHAR(20) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'TipoIva')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [TipoIva] VARCHAR(50) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'CondicionPago')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [CondicionPago] VARCHAR(2) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'Email')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [Email] VARCHAR(100) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'Telefono')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [Telefono] VARCHAR(20) NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'Activo')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [Activo] BIT NOT NULL DEFAULT 1
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'FechaAlta')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [FechaAlta] DATETIME NOT NULL DEFAULT GETDATE()
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'FechaModificacion')
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [FechaModificacion] DATETIME NULL
END
GO

PRINT 'Script ejecutado exitosamente. Campos agregados a la tabla Cliente.'
GO
