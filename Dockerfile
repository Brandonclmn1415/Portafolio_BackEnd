# ---------- Etapa de build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos el archivo del proyecto y config
COPY Backend.csproj ./
COPY appsettings.json ./

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

# Copiamos el resultado de la build
COPY --from=build /app/out ./
COPY appsettings.json ./  # <- muy importante

EXPOSE 80

ENTRYPOINT ["dotnet", "Backend.dll"]