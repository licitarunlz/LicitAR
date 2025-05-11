# SH SH SH BASH
# SH SH SH BASH
# SH SH SH BASH
# SH SH SH BASH
# SH SH SH BASH

#!/bin/bash

# Obtener timestamp AAAAMMDDhhmmss
timestamp=$(date +"%Y%m%d%H%M%S")

# Nombre base para la migración
migrationName="AutoMigration_LicitAR"

# Ruta relativa del proyecto donde está el DbContext
project="LicitAR.Core"

# Ruta relativa del proyecto startup (donde está Program.cs)
startup="LicitAR.Web"

# Directorio donde se guardan las migraciones
outputDir="Migrations/DbContext"

echo "=== Generando migración para LicitARDbContext ==="

dotnet ef migrations add ${migrationName}_${timestamp} --context LicitARDbContext --output-dir ${outputDir} --project ${project} --startup-project ${startup}

echo "=== Aplicando migración ==="

dotnet ef database update --context LicitARDbContext --project ${project} --startup-project ${startup}

echo "=== Listo! ==="