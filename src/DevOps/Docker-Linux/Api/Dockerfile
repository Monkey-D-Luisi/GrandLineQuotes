FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

COPY DevOps/Docker-Linux/Api/certs/grandlinequotes-api-signed.pfx /https-certs/grandlinequotes-api.pfx

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Front/Api/Api.csproj", "Front/Api/"]
COPY ["Back/Application/Application.csproj", "Back/Application/"]
COPY ["Back/Infrastructure/Infrastructure.csproj", "Back/Infrastructure/"]
COPY ["Back/Domain/Domain.csproj", "Back/Domain/"]
COPY ["Back/Client.Abstractions/Client.Abstractions.csproj", "Back/Client.Abstractions/"]

RUN dotnet restore "./Front/Api/Api.csproj"

COPY Front/Api/ ./Front/Api/
COPY Back/Application/ ./Back/Application/
COPY Back/Infrastructure/ ./Back/Infrastructure/
COPY Back/Domain/ ./Back/Domain/
COPY Back/Client.Abstractions/ ./Back/Client.Abstractions/

WORKDIR "/src/Front/Api"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Api.dll"]