# ── Build stage ──────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY CampRide.csproj .
RUN dotnet restore

COPY . .
RUN dotnet publish CampRide.csproj -c Release -o /app/publish --no-restore

# ── Runtime stage ─────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Adatkönyvtár a perzisztens volume-nak
RUN mkdir -p /data

COPY --from=build /app/publish .

# Fly.io a PORT env változót állítja be
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "CampRide.dll"]
