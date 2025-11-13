-- Script para crear la tabla Provincia
-- Debe ejecutarse ANTES del script de población (01_Poblar_Provincias.sql)

-- Verificar si la tabla ya existe
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Provincia]') AND type in (N'U'))
BEGIN
    CREATE TABLE [dbo].[Provincia](
        [ID] [uniqueidentifier] NOT NULL,
        [CodigoAFIP] [varchar](2) NULL,
        [Nombre] [varchar](50) NULL,
        CONSTRAINT [PK_Provincia] PRIMARY KEY CLUSTERED 
        (
            [ID] ASC
        ) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]

    PRINT 'Tabla Provincia creada exitosamente'
END
ELSE
BEGIN
    PRINT 'La tabla Provincia ya existe'
END
GO

-- Crear índice único en CodigoAFIP para mejorar búsquedas
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Provincia_CodigoAFIP' AND object_id = OBJECT_ID('dbo.Provincia'))
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX [IX_Provincia_CodigoAFIP] ON [dbo].[Provincia]
    (
        [CodigoAFIP] ASC
    ) WHERE [CodigoAFIP] IS NOT NULL
    WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

    PRINT 'Índice IX_Provincia_CodigoAFIP creado exitosamente'
END
GO
