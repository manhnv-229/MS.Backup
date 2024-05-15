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
COPY ["MS.Api/MS.Api.csproj", "MS.Api/"]
COPY ["MS.Core/MS.Core.csproj", "MS.Core/"]
COPY ["MS.Infrastructure/MS.Infrastructure.csproj", "MS.Infrastructure/"]
RUN dotnet restore "./MS.Api/MS.Api.csproj"
COPY . .
WORKDIR "/src/MS.Api"
RUN dotnet build "./MS.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./MS.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MS.Api.dll"]