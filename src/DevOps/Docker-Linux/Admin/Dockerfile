FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

COPY DevOps/Docker-Linux/Api/certs/grandlinequotes-api-signed.pfx /https-certs/grandlinequotes-api.pfx

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["Front/Admin/Admin.csproj", "Front/Admin/"]
COPY ["Back/Application/Application.csproj", "Back/Application/"]
COPY ["Back/Infrastructure/Infrastructure.csproj", "Back/Infrastructure/"]
COPY ["Back/Domain/Domain.csproj", "Back/Domain/"]
COPY ["Back/Client.Abstractions/Client.Abstractions.csproj", "Back/Client.Abstractions/"]

RUN dotnet restore "./Front/Admin/Admin.csproj"

COPY Front/Admin/ ./Front/Admin/
COPY Back/Application/ ./Back/Application/
COPY Back/Infrastructure/ ./Back/Infrastructure/
COPY Back/Domain/ ./Back/Domain/
COPY Back/Client.Abstractions/ ./Back/Client.Abstractions/

WORKDIR "/src/Front/Admin"
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Admin.dll"]