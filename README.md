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
---
## Tecnología
- .Net5
- C# 9
- Azure Storage Blobs 12.8.0
. Entity Framework core 5.0.2

### Requisitos Previos

- Suscripción de Azure: cree una gratis
- Cuenta de almacenamiento de Azure: cree una cuenta de almacenamiento
- SDK de .NET 5 para su sistema operativo. Asegúrese de obtener el SDK y no el tiempo de ejecución.
- Microsoft Visual Studio 
- Sql server 2017 o superior
---
## Configurar el marco de la aplicación
###Copie sus credenciales del portal de Azure

- Cuando la aplicación de muestra realiza una solicitud a Azure Storage, debe estar autorizada. Para autorizar una solicitud, agregue las credenciales de su cuenta de almacenamiento a la aplicación como una cadena de conexión. Vea las credenciales de su cuenta de almacenamiento siguiendo estos pasos:

- Inicie sesión en Azure Portal .

- Busque su cuenta de almacenamiento.

- En la sección Configuración de la descripción general de la cuenta de almacenamiento, seleccione Claves de acceso . Aquí, puede ver las claves de acceso a su cuenta y la cadena de conexión completa para cada clave.

- Busque el valor de la cadena de conexión en key1 y seleccione el botón Copiar para copiar la cadena de conexión. Agregará el valor de la cadena de conexión a una variable de entorno en el siguiente paso.

![alt text](https://docs.microsoft.com/en-us/azure/includes/media/storage-copy-connection-string-portal/portal-connection-string.png)

### Configure su cadena de conexión de almacenamiento
Después de haber copiado la cadena de conexión, escríbala en la variable de entorno que se encuentra en el archivo app.config de la aplicacion <StorageConnectionString> y en el archivo BaseAzure.cs public string ConnectionString => "string de conexion AzureStorage";.
  
por otra parte en el archivo app.config de la aplicacion <DefaultConnection> debe colocar la conexion a su base datos sql.
  ejemplo: "Data Source=192.168.0.1;Initial Catalog=master;User ID=sa;Password=TuContraseña;Application Name=MyApp"

### Uso de la aplicacion
1.	Conectarse al servicio de azure lob storage con string de conexion
2.	Mostrar mensaje de conexion

3.	Listar los contenedores 
4.	Pedir seleccionar un cotenedor o url para migragar
5.	Si selecciona un contenedor listar los archivos del contenedor 
6.	permitir seleccionar archivos .csv  de los archivos listados
7.	Si selecione url permitir ingresar una url y salto a paso 8
8.	Mostrar mensaje de advertencia de que el archivo debe seguir el modelo diseñado
9.	Una vez seleccionado descargar el archivo
10.	Mostrar progreso de descarga
11.	Mostrar mensaje de carga en memoria del archivo para migrar
12.	Preguntar si desea realizar limpieza de la tabla antes de realizar la migración
13.	Si responde que si se limpia la tabla en la db 
14.	Si responde que no salto a paso 15
15.	Se muestra mensaje de inicio de migración de la información
16.	Se muestra mensaje una vez finalizado el proceso de migración
17.	Se presiona tecla para cerrar el programa


---
## Arquitectura

### Diagrama

![alt text](https://github.com/H3MH/PruebaCsvImporter_Henry/blob/main/assets/arquitectura.png)

### Logica
.cs public string ConnectionString => "string de conexion AzureStorage";
![alt text](https://github.com/H3MH/PruebaCsvImporter_Henry/blob/main/assets/logica.png)

---

## Limitaciones
Lista de limitaciones conocidas. Puede ser en formato de lista.
Ej.
* latencia entre el tiempo de descarga del archivo y tiempo de importación
* peso de archivos 

