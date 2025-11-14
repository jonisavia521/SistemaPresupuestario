-- Script para insertar/actualizar configuración inicial del sistema
-- Este script permite configurar el idioma de la aplicación

USE [Huamani_SistemaPresupuestario]
GO

-- Verificar si ya existe una configuración
IF NOT EXISTS (SELECT 1 FROM [dbo].[Configuracion])
BEGIN
    -- Insertar configuración inicial en español
    INSERT INTO [dbo].[Configuracion]
           ([Id]
           ,[RazonSocial]
           ,[CUIT]
           ,[TipoIva]
           ,[Direccion]
           ,[Localidad]
           ,[IdProvincia]
           ,[Email]
           ,[Telefono]
           ,[Idioma]
           ,[FechaAlta])
     VALUES
           (NEWID()
           ,'Mi Empresa'
           ,'20123456789'
           ,'RESPONSABLE INSCRIPTO'
           ,'Calle Falsa 123'
           ,'Buenos Aires'
           ,NULL  -- Reemplazar con un GUID válido de Provincia si es necesario
           ,'contacto@miempresa.com'
           ,'011-4444-5555'
           ,'es-AR'  -- Idioma español por defecto
           ,GETDATE())
           
    PRINT 'Configuración inicial creada en ESPAÑOL'
END
ELSE
BEGIN
    PRINT 'Ya existe una configuración en el sistema'
    
    -- Mostrar configuración actual
    SELECT 
        RazonSocial,
        CUIT,
        Idioma,
        FechaAlta
    FROM [dbo].[Configuracion]
END
GO

-- Para cambiar el idioma a inglés, ejecutar:
-- UPDATE [dbo].[Configuracion] SET Idioma = 'en-US'

-- Para cambiar el idioma a español, ejecutar:
-- UPDATE [dbo].[Configuracion] SET Idioma = 'es-AR'

-- Verificar configuración actual
SELECT 
    Id,
    RazonSocial,
    CUIT,
    TipoIva,
    Idioma,
    FechaAlta
FROM [dbo].[Configuracion]
