#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_HTTP_PORTS=80
USER app
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ENV ASPNETCORE_HTTP_PORTS=80
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["MS.CenterApp.Api/MS.CenterApp.Api.csproj", "MS.CenterApp.Api/"]
COPY ["MS.Core/MS.Core.csproj", "MS.Core/"]
COPY ["MS.Infrastructure/MS.Infrastructure.csproj", "MS.Infrastructure/"]
RUN dotnet restore "./MS.CenterApp.Api/MS.CenterApp.Api.csproj"
COPY . .
WORKDIR "/src/MS.CenterApp.Api"
RUN dotnet build "./MS.CenterApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MS.CenterApp.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MS.CenterApp.Api.dll"]
USER root
RUN chmod -R 755 /app/FileTemplate