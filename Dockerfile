# ---------- Etapa de build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copiamos el archivo del proyecto y restauramos dependencias
COPY Backend.csproj ./
RUN dotnet restore

# Copiamos el resto del código y publicamos la app
COPY . ./
RUN dotnet publish -c Release -o /app/out

# ---------- Etapa de runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copiamos los archivos publicados desde la etapa de build
COPY --from=build /app/out .

# Exponemos el puerto usado por ASP.NET Core
EXPOSE 80

# Comando de inicio
ENTRYPOINT ["dotnet", "Backend.dll"]