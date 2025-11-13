# Scripts de Base de Datos - Orden de Ejecución

Este directorio contiene los scripts SQL necesarios para crear y poblar la tabla `Provincia` y actualizar la tabla `Cliente`.

## Orden de Ejecución

Ejecutar los scripts en el siguiente orden:

### 1. `00_Crear_Tabla_Provincia.sql`
- **Descripción**: Crea la tabla `Provincia` con las columnas necesarias
- **Acciones**:
  - Crea la tabla `Provincia` (ID, CodigoAFIP, Nombre)
  - Crea índice único en `CodigoAFIP` para búsquedas eficientes
- **Prerequisitos**: Ninguno
- **Idempotente**: Sí (verifica si la tabla ya existe)

### 2. `01_Poblar_Provincias.sql`
- **Descripción**: Inserta todas las provincias argentinas con sus códigos AFIP
- **Acciones**:
  - Inserta 24 registros (23 provincias + CABA)
  - Códigos AFIP del 00 al 24 (sin el 15)
- **Prerequisitos**: Script `00_Crear_Tabla_Provincia.sql` ejecutado
- **Nota**: Si desea limpiar los datos existentes, descomente la línea `DELETE FROM Provincia;`

### 3. `02_Alter_Cliente_Add_Provincia.sql`
- **Descripción**: Agrega la columna `IdProvincia` a la tabla `Cliente` y crea la relación FK
- **Acciones**:
  - Elimina columna `IdProvincia` antigua (varchar) si existe
  - Agrega nueva columna `IdProvincia` (uniqueidentifier, nullable)
  - Crea Foreign Key `FK_Cliente_Provincia`
  - Crea índice en `IdProvincia` para búsquedas eficientes
- **Prerequisitos**: 
  - Script `00_Crear_Tabla_Provincia.sql` ejecutado
  - Tabla `Cliente` debe existir
- **Idempotente**: Sí (verifica si las modificaciones ya existen)

## Verificación

Después de ejecutar todos los scripts, puede verificar que todo se creó correctamente con:

```sql
-- Verificar provincias
SELECT * FROM Provincia ORDER BY CodigoAFIP;

-- Verificar estructura de Cliente
SELECT 
    c.name AS ColumnName,
    t.name AS DataType,
    c.max_length AS MaxLength,
    c.is_nullable AS IsNullable
FROM sys.columns c
INNER JOIN sys.types t ON c.user_type_id = t.user_type_id
WHERE c.object_id = OBJECT_ID('dbo.Cliente')
    AND c.name = 'IdProvincia';

-- Verificar Foreign Key
SELECT 
    fk.name AS FK_Name,
    tp.name AS Parent_Table,
    cp.name AS Parent_Column,
    tr.name AS Referenced_Table,
    cr.name AS Referenced_Column
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.tables tp ON fkc.parent_object_id = tp.object_id
INNER JOIN sys.columns cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
INNER JOIN sys.tables tr ON fkc.referenced_object_id = tr.object_id
INNER JOIN sys.columns cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
WHERE fk.name = 'FK_Cliente_Provincia';
```

## Provincias Incluidas

| Código | Provincia |
|--------|-----------|
| 00 | Ciudad Autónoma de Buenos Aires |
| 01 | Buenos Aires |
| 02 | Catamarca |
| 03 | Córdoba |
| 04 | Corrientes |
| 05 | Entre Ríos |
| 06 | Jujuy |
| 07 | Mendoza |
| 08 | La Rioja |
| 09 | Salta |
| 10 | San Juan |
| 11 | San Luis |
| 12 | Santa Fe |
| 13 | Santiago del Estero |
| 14 | Tucumán |
| 16 | Chaco |
| 17 | Chubut |
| 18 | Formosa |
| 19 | Misiones |
| 20 | Neuquén |
| 21 | La Pampa |
| 22 | Río Negro |
| 23 | Santa Cruz |
| 24 | Tierra del Fuego |

**Nota**: El código 15 no se utiliza según la codificación AFIP.
