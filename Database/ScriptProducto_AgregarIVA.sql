-- =============================================
-- Script: Agregar campo PorcentajeIVA a Producto
-- Descripción: Agrega el campo para almacenar el porcentaje de IVA del producto
-- Autor: Sistema
-- Fecha: 2024
-- =============================================

USE [SistemaPresupuestario]
GO

-- Verificar si el campo ya existe
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Producto' 
    AND COLUMN_NAME = 'PorcentajeIVA'
)
BEGIN
    PRINT 'Agregando campo PorcentajeIVA a la tabla Producto...'
    
    ALTER TABLE [dbo].[Producto]
    ADD [PorcentajeIVA] DECIMAL(5,2) NOT NULL DEFAULT(21.00)
    
    PRINT 'Campo PorcentajeIVA agregado exitosamente'
    PRINT 'Valores permitidos: 21.00 (IVA 21%), 10.50 (IVA 10.5%), 0.00 (Exento)'
END
ELSE
BEGIN
    PRINT 'El campo PorcentajeIVA ya existe en la tabla Producto'
END
GO

-- Agregar check constraint para validar valores permitidos
IF NOT EXISTS (
    SELECT 1 
    FROM sys.check_constraints 
    WHERE name = 'CK_Producto_PorcentajeIVA'
)
BEGIN
    ALTER TABLE [dbo].[Producto]
    ADD CONSTRAINT [CK_Producto_PorcentajeIVA] 
    CHECK ([PorcentajeIVA] IN (0.00, 10.50, 21.00))
    
    PRINT 'Constraint CK_Producto_PorcentajeIVA creado exitosamente'
END
ELSE
BEGIN
    PRINT 'El constraint CK_Producto_PorcentajeIVA ya existe'
END
GO

-- Verificar estructura actualizada
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    NUMERIC_PRECISION,
    NUMERIC_SCALE,
    IS_NULLABLE,
    COLUMN_DEFAULT
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Producto'
ORDER BY ORDINAL_POSITION
GO

PRINT '================================================'
PRINT 'Script ejecutado correctamente'
PRINT 'Campo PorcentajeIVA agregado a la tabla Producto'
PRINT 'Valores permitidos: 0.00 (Exento), 10.50 (IVA 10.5%), 21.00 (IVA 21%)'
PRINT '================================================'
