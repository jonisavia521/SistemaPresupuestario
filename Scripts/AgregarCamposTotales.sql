-- Script para agregar los campos de totales a las tablas Presupuesto y Presupuesto_Detalle
-- Estos campos almacenarán los totales calculados de cada presupuesto y detalle

USE [SistemaPresupuestario]
GO

-- ============================================================
-- TABLA PRESUPUESTO: Agregar campos de totales
-- ============================================================

-- Verificar y agregar campo Subtotal
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[Presupuesto]') 
               AND name = 'Subtotal')
BEGIN
    ALTER TABLE [dbo].[Presupuesto]
    ADD [Subtotal] DECIMAL(18,4) NOT NULL DEFAULT 0;
    
    PRINT 'Campo Subtotal agregado exitosamente a la tabla Presupuesto';
END
ELSE
BEGIN
    PRINT 'El campo Subtotal ya existe en la tabla Presupuesto';
END
GO

-- Verificar y agregar campo TotalIva
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[Presupuesto]') 
               AND name = 'TotalIva')
BEGIN
    ALTER TABLE [dbo].[Presupuesto]
    ADD [TotalIva] DECIMAL(18,4) NOT NULL DEFAULT 0;
    
    PRINT 'Campo TotalIva agregado exitosamente a la tabla Presupuesto';
END
ELSE
BEGIN
    PRINT 'El campo TotalIva ya existe en la tabla Presupuesto';
END
GO

-- Verificar y agregar campo Total
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[Presupuesto]') 
               AND name = 'Total')
BEGIN
    ALTER TABLE [dbo].[Presupuesto]
    ADD [Total] DECIMAL(18,4) NOT NULL DEFAULT 0;
    
    PRINT 'Campo Total agregado exitosamente a la tabla Presupuesto';
END
ELSE
BEGIN
    PRINT 'El campo Total ya existe en la tabla Presupuesto';
END
GO

-- ============================================================
-- TABLA PRESUPUESTO_DETALLE: Agregar campo de total
-- ============================================================

-- Verificar y agregar campo Total
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[Presupuesto_Detalle]') 
               AND name = 'Total')
BEGIN
    ALTER TABLE [dbo].[Presupuesto_Detalle]
    ADD [Total] DECIMAL(18,4) NULL;
    
    PRINT 'Campo Total agregado exitosamente a la tabla Presupuesto_Detalle';
END
ELSE
BEGIN
    PRINT 'El campo Total ya existe en la tabla Presupuesto_Detalle';
END
GO

PRINT 'Script ejecutado exitosamente';
GO
