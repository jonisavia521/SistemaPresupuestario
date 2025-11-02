-- Script SQL para actualizar la tabla Vendedor
-- Ejecutar este script en la base de datos SistemaPresupuestario

USE [SistemaPresupuestario]
GO

-- Agregar campo CodigoVendedor
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'CodigoVendedor')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [CodigoVendedor] VARCHAR(20) NULL
    PRINT 'Campo CodigoVendedor agregado'
END
ELSE
BEGIN
    PRINT 'Campo CodigoVendedor ya existe'
END
GO

-- Agregar campo CUIT
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'CUIT')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [CUIT] VARCHAR(11) NULL
    PRINT 'Campo CUIT agregado'
END
ELSE
BEGIN
    PRINT 'Campo CUIT ya existe'
END
GO

-- Agregar campo PorcentajeComision
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'PorcentajeComision')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [PorcentajeComision] DECIMAL(5,2) NULL DEFAULT 0
    PRINT 'Campo PorcentajeComision agregado'
END
ELSE
BEGIN
    PRINT 'Campo PorcentajeComision ya existe'
END
GO

-- Agregar campo Activo
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'Activo')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [Activo] BIT NOT NULL DEFAULT 1
    PRINT 'Campo Activo agregado'
END
ELSE
BEGIN
    PRINT 'Campo Activo ya existe'
END
GO

-- Agregar campo FechaAlta
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'FechaAlta')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [FechaAlta] DATETIME NOT NULL DEFAULT GETDATE()
    PRINT 'Campo FechaAlta agregado'
END
ELSE
BEGIN
    PRINT 'Campo FechaAlta ya existe'
END
GO

-- Agregar campo FechaModificacion
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Vendedor]') AND name = 'FechaModificacion')
BEGIN
    ALTER TABLE [dbo].[Vendedor]
    ADD [FechaModificacion] DATETIME NULL
    PRINT 'Campo FechaModificacion agregado'
END
ELSE
BEGIN
    PRINT 'Campo FechaModificacion ya existe'
END
GO

-- Actualizar registros existentes con valores por defecto
-- Solo actualizar si los campos están NULL
UPDATE Vendedor
SET 
    CodigoVendedor = CASE WHEN CodigoVendedor IS NULL THEN RIGHT('0' + CAST(ROW_NUMBER() OVER (ORDER BY ID) AS VARCHAR(2)), 2) ELSE CodigoVendedor END,
    CUIT = CASE WHEN CUIT IS NULL THEN '00000000000' ELSE CUIT END,
    PorcentajeComision = CASE WHEN PorcentajeComision IS NULL THEN 0 ELSE PorcentajeComision END,
    FechaAlta = CASE WHEN FechaAlta IS NULL OR FechaAlta = '1900-01-01' THEN GETDATE() ELSE FechaAlta END
WHERE CodigoVendedor IS NULL 
   OR CUIT IS NULL 
   OR PorcentajeComision IS NULL 
   OR FechaAlta IS NULL 
   OR FechaAlta = '1900-01-01'
GO

PRINT '===================='
PRINT 'Script ejecutado exitosamente.'
PRINT 'Campos agregados/actualizados en la tabla Vendedor.'
PRINT '===================='
GO

-- Verificar la estructura de la tabla
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('Vendedor')
ORDER BY c.column_id
GO
