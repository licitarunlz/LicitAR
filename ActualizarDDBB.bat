

@echo off
setlocal enabledelayedexpansion

:: Obtener timestamp AAAAMMDDhhmmss
for /f %%a in ('wmic os get localdatetime ^| find "."') do set dt=%%a
set timestamp=%dt:~0,4%%dt:~4,2%%dt:~6,2%%dt:~8,2%%dt:~10,2%%dt:~12,2%

:: Nombre base para la migración (podés cambiarlo)
set migrationName=AutoMigration_LicitARIdentity

:: Ruta relativa del proyecto donde está el DbContext
set project=LicitAR.Core

:: Ruta relativa del proyecto startup (donde está Program.cs)
set startup=LicitAR.Web

:: Directorio donde se guardan las migraciones
set outputDir=Migrations/Identidad



echo === Generando migraciones para todos los DbContexts ===

dotnet ef migrations add AutoMigration_LicitARIdentity_%timestamp% --context LicitARIdentityDbContext --output-dir Migrations/Identidad --project Licitar.Core --startup-project Licitar.Web

dotnet ef migrations add AutoMigration_Licitaciones_%timestamp% --context LicitacionesDbContext --output-dir Migrations/Licitaciones  --project Licitar.Core --startup-project Licitar.Web

dotnet ef migrations add AutoMigration_Actores_%timestamp% --context ActoresDbContext --output-dir Migrations/Actores --project Licitar.Core --startup-project Licitar.Web

dotnet ef migrations add AutoMigration_Parametros_%timestamp% --context ParametrosDbContext --output-dir Migrations/Parametros --project Licitar.Core --startup-project Licitar.Web


echo === Aplicando migraciones ===

dotnet ef database update --context LicitARIdentityDbContext --project Licitar.Core --startup-project Licitar.Web
dotnet ef database update --context LicitacionesDbContext --project Licitar.Core --startup-project Licitar.Web
dotnet ef database update --context ActoresDbContext --project Licitar.Core --startup-project Licitar.Web
dotnet ef database update --context ParametrosDbContext --project Licitar.Core --startup-project Licitar.Web

echo === Listo! ===
pause