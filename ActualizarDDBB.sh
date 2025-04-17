# SH SH SH BASH
# SH SH SH BASH
# SH SH SH BASH
# SH SH SH BASH
# SH SH SH BASH

#!/bin/bash

# Obtener timestamp AAAAMMDDhhmmss
timestamp=$(date +"%Y%m%d%H%M%S")

# Nombre base para la migración (puedes cambiarlo)
migrationName="AutoMigration_LicitARIdentity"

# Ruta relativa del proyecto donde está el DbContext
project="LicitAR.Core"

# Ruta relativa del proyecto startup (donde está Program.cs)
startup="LicitAR.Web"

# Directorio donde se guardan las migraciones
outputDir="Migrations/Identidad"

echo "=== Generando migraciones para todos los DbContexts ==="

dotnet ef migrations add AutoMigration_LicitARIdentity_$timestamp --context LicitARIdentityDbContext --output-dir Migrations/Identidad --project $project --startup-project $startup

dotnet ef migrations add AutoMigration_Licitaciones_$timestamp --context LicitacionesDbContext --output-dir Migrations/Licitaciones --project $project --startup-project $startup

dotnet ef migrations add AutoMigration_Actores_$timestamp --context ActoresDbContext --output-dir Migrations/Actores --project $project --startup-project $startup

dotnet ef migrations add AutoMigration_Parametros_$timestamp --context ParametrosDbContext --output-dir Migrations/Parametros --project $project --startup-project $startup

echo "=== Aplicando migraciones ==="

dotnet ef database update --context LicitARIdentityDbContext --project $project --startup-project $startup
dotnet ef database update --context LicitacionesDbContext --project $project --startup-project $startup
dotnet ef database update --context ActoresDbContext --project $project --startup-project $startup
dotnet ef database update --context ParametrosDbContext --project $project --startup-project $startup

echo "=== Listo! ==="