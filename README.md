# Gestión de puntos / Fidelización
Este es el backend desarrollado en ASP.NET Core 7 para una aplicación de gestión de puntos entre usuarios. Incluye autenticación JWT, perfiles, historial de puntos, redención y asignación de puntos por parte de un administrador.

## Tecnologías
- .NET 8
- Entity Framework Core
- SQL Server LocalDB
- JWT (Json Web Tokens)

## Como ejecutar el proyecto?
1. Clonar el repositorio
```bash
git clone [link repositorio]
cd gestiondepuntos
```

2. Restaurar paquetes
```bash
dotnet restore
```

3. Crear base de datos y aplicar migraciones
```bash
dotnet ef database update
```

4. Ejecutar el proyecto
```bash
dotnet run
```

Asegurate de tener SQL Server LocalDB instalado y que el archivo appsettings.json tenga la cadena de conexión correcta.
