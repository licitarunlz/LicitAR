

@echo off
setlocal enabledelayedexpansion

:: Obtener timestamp AAAAMMDDhhmmss
for /f %%a in ('wmic os get localdatetime ^| find "."') do set dt=%%a
set timestamp=%dt:~0,4%%dt:~4,2%%dt:~6,2%%dt:~8,2%%dt:~10,2%%dt:~12,2%

:: Nombre base para la migraci�n (pod�s cambiarlo)
set migrationName=AutoMigration_LicitAR

:: Ruta relativa del proyecto donde est� el DbContext
set project=LicitAR.Core

:: Ruta relativa del proyecto startup (donde est� Program.cs)
set startup=LicitAR.Web

:: Directorio donde se guardan las migraciones
set outputDir=Migrations/Identidad



echo === Generando migraciones para todos los DbContexts ===

dotnet ef migrations add AutoMigration_LicitAR_%timestamp% --context LicitARDbContext --output-dir Migrations/DbContext --project Licitar.Core --startup-project Licitar.Web

echo === Aplicando migraciones ===

dotnet ef database update --context LicitARDbContext --project Licitar.Core --startup-project Licitar.Web 

echo === Listo! ===
pause