-- =============================================
-- Script: Eliminar campo CodigoVendedor de Cliente
-- Descripción: Remueve el campo CodigoVendedor ya que se usará IdVendedor (FK)
-- Autor: Sistema
-- Fecha: 2024
-- =============================================

USE [SistemaPresupuestario]
GO

-- Verificar si existe el campo CodigoVendedor
IF EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Cliente' 
    AND COLUMN_NAME = 'CodigoVendedor'
)
BEGIN
    PRINT 'Eliminando campo CodigoVendedor de la tabla Cliente...'
    
    ALTER TABLE [dbo].[Cliente]
    DROP COLUMN [CodigoVendedor]
    
    PRINT 'Campo CodigoVendedor eliminado exitosamente'
END
ELSE
BEGIN
    PRINT 'El campo CodigoVendedor no existe en la tabla Cliente'
END
GO

-- Verificar que IdVendedor existe como FK
IF NOT EXISTS (
    SELECT 1 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'Cliente' 
    AND COLUMN_NAME = 'IdVendedor'
)
BEGIN
    PRINT 'ADVERTENCIA: El campo IdVendedor no existe. Debe existir para la relación con Vendedor'
END
ELSE
BEGIN
    PRINT 'Campo IdVendedor existe correctamente'
END
GO

-- Verificar estructura final de Cliente
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Cliente'
ORDER BY ORDINAL_POSITION
GO

PRINT '================================================'
PRINT 'Script ejecutado correctamente'
PRINT '================================================'
