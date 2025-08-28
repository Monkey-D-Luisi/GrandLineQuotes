FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5002

COPY DevOps/Docker-Linux/Api/certs/grandlinequotes-api-signed.pfx /https-certs/grandlinequotes-api.pfx

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Front/Api.Public/Api.Public.csproj", "Front/Api.Public/"]
COPY ["Back/Client.Abstractions/Client.Abstractions.csproj", "Back/Client.Abstractions/"]
COPY ["Front/Client/Client.csproj", "Front/Client/"]

RUN dotnet restore "./Front/Api.Public/Api.Public.csproj"

COPY Front/Api.Public/ ./Front/Api.Public/
COPY Back/Client.Abstractions/ ./Back/Client.Abstractions/
COPY Front/Client/ ./Front/Client/

WORKDIR "/src/Front/Api.Public"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.Public.dll"]
