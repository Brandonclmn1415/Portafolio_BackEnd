# ---------- Etapa de build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiar todos los archivos (y luego restaurar)
COPY . ./

# Restaurar dependencias
RUN dotnet restore

# Compilar y publicar
RUN dotnet publish -c Release -o /app/out

# ---------- Etapa de runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiar artefactos publicados
COPY --from=build /app/out .

# Exponer el puerto
EXPOSE 80

# Ejecutar la app
ENTRYPOINT ["dotnet", "Backend.dll"]