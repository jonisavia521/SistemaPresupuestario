-- Script para agregar la columna IdProvincia a la tabla Cliente
-- y crear la relación de Foreign Key con Provincia

-- 1. Verificar si la columna IdProvincia (string) existe y eliminarla si es necesario
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'IdProvincia' AND system_type_id = 167) -- 167 = varchar
BEGIN
    -- Eliminar la columna antigua si existe como string
    ALTER TABLE [dbo].[Cliente]
    DROP COLUMN [IdProvincia]
    
    PRINT 'Columna IdProvincia (varchar) eliminada de la tabla Cliente'
END

-- 2. Agregar la nueva columna IdProvincia como Guid nullable
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Cliente]') AND name = 'IdProvincia' AND system_type_id = 36) -- 36 = uniqueidentifier
BEGIN
    ALTER TABLE [dbo].[Cliente]
    ADD [IdProvincia] [uniqueidentifier] NULL
    
    PRINT 'Columna IdProvincia (uniqueidentifier) agregada a la tabla Cliente'
END
ELSE
BEGIN
    PRINT 'La columna IdProvincia (uniqueidentifier) ya existe en la tabla Cliente'
END
GO

-- 3. Crear la Foreign Key entre Cliente y Provincia (si no existe)
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Cliente_Provincia')
BEGIN
    ALTER TABLE [dbo].[Cliente] WITH CHECK 
    ADD CONSTRAINT [FK_Cliente_Provincia] FOREIGN KEY([IdProvincia])
    REFERENCES [dbo].[Provincia] ([ID])
    
    PRINT 'Foreign Key FK_Cliente_Provincia creada exitosamente'
END
ELSE
BEGIN
    PRINT 'La Foreign Key FK_Cliente_Provincia ya existe'
END
GO

-- 4. Crear índice en IdProvincia para mejorar búsquedas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Cliente_IdProvincia' AND object_id = OBJECT_ID('dbo.Cliente'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_Cliente_IdProvincia] ON [dbo].[Cliente]
    (
        [IdProvincia] ASC
    )
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
    
    PRINT 'Índice IX_Cliente_IdProvincia creado exitosamente'
END
ELSE
BEGIN
    PRINT 'El índice IX_Cliente_IdProvincia ya existe'
END
GO

-- 5. Verificar la estructura actualizada
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Cliente')
    AND c.name = 'IdProvincia'

PRINT 'Verificación de estructura completada'
GO
