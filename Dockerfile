# ---------- Etapa de build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos los archivos necesarios
COPY Backend.csproj ./
COPY appsettings.json ./

# Restauramos dependencias
RUN dotnet restore

# Copiamos el resto del código
COPY . ./

# Compilamos y publicamos
RUN dotnet publish Backend.csproj -c Release -o /app/out --no-restore

# ---------- Etapa de runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiamos la salida del build
COPY --from=build /app/out .

# Exponemos el puerto
EXPOSE 80

# Arranque de la aplicación
ENTRYPOINT ["dotnet", "Backend.dll"]