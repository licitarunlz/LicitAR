# 🚀 **LicitAR** - Sistema de Gestión de Licitaciones

**LicitAR** es un sistema web desarrollado para gestionar el proceso completo de licitaciones en Argentina. Este proyecto fue creado como parte de la materia **Práctica Profesional Supervisada** de la **Universidad Nacional de Lomas de Zamora (UNLZ)**. El objetivo es proporcionar una plataforma eficiente y fácil de usar para administrar licitaciones, ofertas, postulaciones y pliegos, integrando la participación de diferentes entidades involucradas en el proceso licitatorio.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=licitarunlz_LicitAR&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=licitarunlz_LicitAR)

## 🔧 **Tecnologías utilizadas**

Este proyecto está basado en **.NET Core MVC 9** y utiliza las siguientes tecnologías:

- **ASP.NET Core MVC 9**: Framework principal para el desarrollo web.
- **Entity Framework Core**: Para la interacción con la base de datos.
- **SQL Server**: Base de datos para almacenar toda la información relacionada con licitaciones, ofertas, postulaciones y entidades.
- **Bootstrap**: Para la parte de diseño y frontend responsivo.
- **HTML5, CSS3 y JavaScript**: Para la estructuración y dinamización de las páginas.

## 📊 **Funcionalidades principales**

El sistema cubre los siguientes procesos relacionados con las licitaciones:

- **Gestión de Licitaciones**: Creación, modificación, y seguimiento de licitaciones públicas.
- **Ofertas y Postulaciones**: Permite a las entidades realizar ofertas y postularse en licitaciones activas.
- **Pliegos y Requisitos**: Gestión de los pliegos de licitaciones, con acceso para entidades interesadas.
- **Entidad de Licitantes**: Registro y administración de las entidades que participan en el proceso licitatorio.
- **Panel de Administración**: Un panel para administradores que permite supervisar el proceso completo de las licitaciones.

## 🛠️ **Cómo ejecutar el proyecto**

1. **Clonar el repositorio**

   ```bash
   git clone https://github.com/licitarunlz/licitAR.git
   cd licitAR
   ```
2. **Restaurar las dependencias del proyecto**

   Asegúrate de tener instalada la versión adecuada de **.NET Core SDK** y ejecuta el siguiente comando:

   ```bash
   dotnet restore
   ```
3. **Configurar la base de datos**

  El proyecto utiliza una base de datos alojada en Azure, empleada exclusivamente durante el desarrollo. Si deseas ejecutarlo localmente, no tendrás acceso a esa base, por lo que deberás contar con un servidor SQL Server disponible y configurar la cadena de conexión en el archivo appsettings.json.

4. **Ejecutar la migración de la base de datos**

   Si aún no has creado las migraciones en la base de datos (base nueva), ejecuta:

   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```
   
5. **Ejecutar el proyecto**

   Ejecuta el siguiente comando para iniciar el servidor localmente:

   ```bash
   dotnet build
   dotnet run
   ```

   El proyecto estará disponible en http://localhost:5045.

## 📑 **Documentación**

Si deseas obtener más detalles sobre el funcionamiento de las distintas partes del sistema o contribuciones, puedes consultar la documentación adicional en el repositorio o directamente en las rutas del código fuente.

## 🎓 **Integrantes**

Este proyecto fue desarrollado por los estudiantes de la **Universidad Nacional de Lomas de Zamora (UNLZ)** en el marco de la asignatura **Práctica Profesional Supervisada**:

- Frida Narbó
- Diego Peppert
- Emmanuel Federico
- Pablo Goitisolo

## 📝 **Licencia**

Este proyecto está bajo la licencia MIT. Puedes consultar más detalles en el archivo `LICENSE`.

## 🌐 **Contacto**

Si tienes alguna duda o sugerencia, no dudes en contactarnos:

- Correo electrónico: [unlz.licitar@gmail.com]
- GitHub: [https://github.com/licitarunlz](https://github.com/licitarunlz)

---

¡Gracias por visitar el proyecto **LicitAR**! 🚀


