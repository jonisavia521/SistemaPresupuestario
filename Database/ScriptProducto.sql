-- =============================================
-- Script: Tabla Producto
-- Descripción: Estructura de la tabla Producto para el sistema de presupuestos
-- Autor: Sistema
-- Fecha: 2024
-- =============================================

-- Verificar si la tabla ya existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Producto]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Producto](
        [ID] [uniqueidentifier] NOT NULL,
        [Codigo] [nvarchar](50) NOT NULL,
        [Descripcion] [nvarchar](50) NULL,
        [Inhabilitado] [bit] NOT NULL,
        [FechaAlta] [datetime] NOT NULL,
        [UsuarioAlta] [int] NOT NULL,
        CONSTRAINT [PK_Producto] PRIMARY KEY CLUSTERED ([ID] ASC)
    )
    
    PRINT 'Tabla Producto creada exitosamente'
END
ELSE
BEGIN
    PRINT 'La tabla Producto ya existe'
END
GO

-- Crear índice único en el código del producto
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Producto_Codigo' AND object_id = OBJECT_ID('Producto'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Producto_Codigo]
    ON [dbo].[Producto] ([Codigo] ASC)
    PRINT 'Índice IX_Producto_Codigo creado exitosamente'
END
ELSE
BEGIN
    PRINT 'El índice IX_Producto_Codigo ya existe'
END
GO

-- Insertar productos de ejemplo (solo si la tabla está vacía)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Producto])
BEGIN
    INSERT INTO [dbo].[Producto] ([ID], [Codigo], [Descripcion], [Inhabilitado], [FechaAlta], [UsuarioAlta])
    VALUES 
        (NEWID(), 'PROD001', 'Producto de Ejemplo 1', 0, GETDATE(), 1),
        (NEWID(), 'PROD002', 'Producto de Ejemplo 2', 0, GETDATE(), 1),
        (NEWID(), 'PROD003', 'Producto de Ejemplo 3', 0, GETDATE(), 1),
        (NEWID(), 'SERV001', 'Servicio de Ejemplo 1', 0, GETDATE(), 1),
        (NEWID(), 'SERV002', 'Servicio de Ejemplo 2', 0, GETDATE(), 1)
    
    PRINT 'Datos de ejemplo insertados exitosamente'
END
ELSE
BEGIN
    PRINT 'La tabla Producto ya contiene datos'
END
GO

-- Verificar la estructura
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Producto'
ORDER BY ORDINAL_POSITION
GO

PRINT '================================================'
PRINT 'Script de tabla Producto ejecutado correctamente'
PRINT '================================================'
