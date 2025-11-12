-- ================================================
-- Script para crear tablas de Lista de Precios
-- ================================================

USE [SistemaPresupuestario]
GO

-- ===================================
-- Tabla: ListaPrecio
-- ===================================
CREATE TABLE [dbo].[ListaPrecio](
    [ID] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
    [Codigo] [nvarchar](2) NOT NULL,
    [Nombre] [nvarchar](100) NOT NULL,
    [Activo] [bit] NOT NULL DEFAULT 1,
    [FechaAlta] [datetime] NOT NULL DEFAULT GETDATE(),
    [FechaModificacion] [datetime] NULL,
    CONSTRAINT [PK_ListaPrecio] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UK_ListaPrecio_Codigo] UNIQUE ([Codigo])
) ON [PRIMARY]
GO

-- ===================================
-- Tabla: ListaPrecio_Detalle
-- ===================================
CREATE TABLE [dbo].[ListaPrecio_Detalle](
    [ID] [uniqueidentifier] NOT NULL DEFAULT NEWID(),
    [IdListaPrecio] [uniqueidentifier] NOT NULL,
    [IdProducto] [uniqueidentifier] NOT NULL,
    [Precio] [decimal](18, 2) NOT NULL DEFAULT 0,
    CONSTRAINT [PK_ListaPrecio_Detalle] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ListaPrecio_Detalle_ListaPrecio] FOREIGN KEY([IdListaPrecio])
        REFERENCES [dbo].[ListaPrecio] ([ID]),
    CONSTRAINT [FK_ListaPrecio_Detalle_Producto] FOREIGN KEY([IdProducto])
        REFERENCES [dbo].[Producto] ([ID])
) ON [PRIMARY]
GO

-- Índice para mejorar búsquedas
CREATE NONCLUSTERED INDEX [IX_ListaPrecio_Detalle_IdListaPrecio] 
    ON [dbo].[ListaPrecio_Detalle]([IdListaPrecio] ASC)
GO

CREATE NONCLUSTERED INDEX [IX_ListaPrecio_Detalle_IdProducto] 
    ON [dbo].[ListaPrecio_Detalle]([IdProducto] ASC)
GO

-- ===================================
-- Agregar campo IdListaPrecio a Presupuesto
-- ===================================
ALTER TABLE [dbo].[Presupuesto]
ADD [IdListaPrecio] [uniqueidentifier] NULL,
CONSTRAINT [FK_Presupuesto_ListaPrecio] FOREIGN KEY([IdListaPrecio])
    REFERENCES [dbo].[ListaPrecio] ([ID])
GO

-- Índice para mejorar búsquedas
CREATE NONCLUSTERED INDEX [IX_Presupuesto_IdListaPrecio] 
    ON [dbo].[Presupuesto]([IdListaPrecio] ASC)
GO

-- ===================================
-- Datos de ejemplo (opcional)
-- ===================================
INSERT INTO [dbo].[ListaPrecio] ([ID], [Codigo], [Nombre], [Activo], [FechaAlta])
VALUES 
    (NEWID(), '01', 'Lista Mayorista', 1, GETDATE()),
    (NEWID(), '02', 'Lista Minorista', 1, GETDATE()),
    (NEWID(), '03', 'Lista Promocional', 1, GETDATE())
GO

PRINT 'Tablas de Lista de Precios creadas exitosamente'
GO
