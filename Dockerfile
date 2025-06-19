# ---------- Etapa de build ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY Backend.csproj ./
COPY appsettings.json ./
RUN dotnet restore Backend.csproj

COPY . ./
RUN dotnet publish Backend.csproj -c Release -o /app/out --no-restore

# ---------- Etapa de runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

EXPOSE 80

ENTRYPOINT ["dotnet", "Backend.dll"]