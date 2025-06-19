# ---------- Etapa de build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos el archivo del proyecto
COPY Backend.csproj ./

# Restauramos paquetes
RUN dotnet restore

# Copiamos el resto del proyecto
COPY . ./

# Publicamos el proyecto
RUN dotnet publish Backend.csproj -c Release -o /app/out

# ---------- Etapa de runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiamos el resultado de la publicación
COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "Backend.dll"]
