-- Script para poblar la tabla Provincia con las provincias argentinas
-- Incluye Ciudad Autónoma de Buenos Aires y las 23 provincias

-- IMPORTANTE: Si la tabla ya tiene datos, este script los agregará.
-- Si deseas limpiar primero, descomenta la siguiente línea:
-- DELETE FROM Provincia;

-- Insertar provincias argentinas con sus códigos AFIP
INSERT INTO Provincia (ID, CodigoAFIP, Nombre) VALUES
(NEWID(), '00', 'Ciudad Autónoma de Buenos Aires'),
(NEWID(), '01', 'Buenos Aires'),
(NEWID(), '02', 'Catamarca'),
(NEWID(), '03', 'Córdoba'),
(NEWID(), '04', 'Corrientes'),
(NEWID(), '05', 'Entre Ríos'),
(NEWID(), '06', 'Jujuy'),
(NEWID(), '07', 'Mendoza'),
(NEWID(), '08', 'La Rioja'),
(NEWID(), '09', 'Salta'),
(NEWID(), '10', 'San Juan'),
(NEWID(), '11', 'San Luis'),
(NEWID(), '12', 'Santa Fe'),
(NEWID(), '13', 'Santiago del Estero'),
(NEWID(), '14', 'Tucumán'),
(NEWID(), '16', 'Chaco'),
(NEWID(), '17', 'Chubut'),
(NEWID(), '18', 'Formosa'),
(NEWID(), '19', 'Misiones'),
(NEWID(), '20', 'Neuquén'),
(NEWID(), '21', 'La Pampa'),
(NEWID(), '22', 'Río Negro'),
(NEWID(), '23', 'Santa Cruz'),
(NEWID(), '24', 'Tierra del Fuego');

-- Verificar que se insertaron correctamente
SELECT * FROM Provincia ORDER BY CodigoAFIP;
