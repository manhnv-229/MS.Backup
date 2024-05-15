#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MS.eShop.Api/MS.eShop.Api.csproj", "MS.eShop.Api/"]
COPY ["MS.Core/MS.Core.csproj", "MS.Core/"]
COPY ["MS.Infrastructure/MS.Infrastructure.csproj", "MS.Infrastructure/"]
RUN dotnet restore "MS.eShop.Api/MS.eShop.Api.csproj"
COPY . .
WORKDIR "/src/MS.eShop.Api"
RUN dotnet build "MS.eShop.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MS.eShop.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MS.eShop.Api.dll"]