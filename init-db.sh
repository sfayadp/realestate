#!/bin/bash
# Script para inicializar la base de datos en Docker

echo "Esperando a que SQL Server esté listo..."
sleep 30

echo "Ejecutando script de inicialización de la base de datos..."

# Ejecutar el script SQL
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourStrong@Passw0rd" -i /docker-entrypoint-initdb.d/RealEstateDB.sql

echo "Base de datos inicializada correctamente!"
