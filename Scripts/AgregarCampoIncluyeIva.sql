-- Script para agregar el campo IncluyeIva a la tabla ListaPrecio
-- Este campo indica si los precios de la lista incluyen IVA o no

USE [SistemaPresupuestario]
GO

-- Verificar si la columna ya existe
IF NOT EXISTS (SELECT * FROM sys.columns 
               WHERE object_id = OBJECT_ID(N'[dbo].[ListaPrecio]') 
               AND name = 'IncluyeIva')
BEGIN
    -- Agregar la columna IncluyeIva con valor por defecto FALSE
    ALTER TABLE [dbo].[ListaPrecio]
    ADD [IncluyeIva] BIT NOT NULL DEFAULT 0;
    
    PRINT 'Campo IncluyeIva agregado exitosamente a la tabla ListaPrecio';
END
ELSE
BEGIN
    PRINT 'El campo IncluyeIva ya existe en la tabla ListaPrecio';
END
GO
