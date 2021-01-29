# PruebaCsvImporter_Henry
# Titulo: Csv Importer
---
## Overview: Problema a resolver

La empresa que quiere el programa se llama Acme Corporation (un clásico) y tiene decidido que el nombre del programa sea CsvImporter. Además, le gustaría que el código esté subido a un repositorio de github con el nombre PruebaCsvImporter_<Autor>.

### Alcance(Scope)

Desarrollar un programa de consola .NET Core en C#, que lea un fichero .csv almacenado en una cuenta de almacenamiento de Azure e inserte su contenido en una BD SQL Server local.

#### Casos de uso

* Antes de insertar en la BD, tendrás que eliminar el contenido de una posible previa importación.

* validar el usuario en sesión usando active directory

* Permitir ingresar la url del archivo a migrar

* ver información del proceso de descarga

* ver proceso de migración

* ver información de borrado de db

#### Out of Scope (casos de uso No Soportados)

* archivos que no sean de. Csv
* archivos que no cumplan con la estructura del modelo establecido
* ...
---
## Arquitectura

### Diagrama
https://github.com/
![alt text](https://raw.githubusercontent.com/H3MH/PruebaCsvImporter_Henry/blob/main/assets/arquitectura.png)

### Logica
https://github.com/H3MH/PruebaCsvImporter_Henry/blob/main/assets/logica.png
![alt text](https://raw.githubusercontent.com/H3MH/PruebaCsvImporter_Henry/blob/main/assets/logica.png)

---
## Limitaciones
Lista de limitaciones conocidas. Puede ser en formato de lista.
Ej.
* latencia entre el tiempo de descarga del archivo y tiempo de importación
* peso de archivos 

