# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/GestaoAutonomo.Domain/GestaoAutonomo.Domain.csproj src/GestaoAutonomo.Domain/
COPY src/GestaoAutonomo.Application/GestaoAutonomo.Application.csproj src/GestaoAutonomo.Application/
COPY src/GestaoAutonomo.Infrastructure/GestaoAutonomo.Infrastructure.csproj src/GestaoAutonomo.Infrastructure/
COPY src/GestaoAutonomo.API/GestaoAutonomo.API.csproj src/GestaoAutonomo.API/
RUN dotnet restore src/GestaoAutonomo.API/GestaoAutonomo.API.csproj

COPY src/ src/
RUN dotnet publish src/GestaoAutonomo.API/GestaoAutonomo.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
USER app

COPY --from=build --chown=app:app /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "GestaoAutonomo.API.dll"]
