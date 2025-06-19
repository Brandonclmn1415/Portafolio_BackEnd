# ---------- Etapa de build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia solo el archivo del proyecto
COPY Backend.csproj ./

# Restauramos dependencias
RUN dotnet restore

# Copiamos el resto del proyecto
COPY . ./

# Compilamos y publicamos
RUN dotnet build -c Release --no-restore
RUN dotnet publish -c Release -o /app/out --no-restore

# ---------- Etapa de runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiamos el resultado de la etapa de build
COPY --from=build /app/out .

# Exponemos el puerto por defecto de ASP.NET
EXPOSE 80

# Arranque de la aplicación
ENTRYPOINT ["dotnet", "Backend.dll"]