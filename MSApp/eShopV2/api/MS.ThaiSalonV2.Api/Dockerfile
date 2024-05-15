#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MS.ThaiSalonV2.Api/MS.ThaiSalonV2.Api.csproj", "MS.ThaiSalonV2.Api/"]
COPY ["MS.ApplicationCore/MS.ApplicationCore.csproj", "MS.ApplicationCore/"]
COPY ["MS.Infrastructure/MS.Infrastructure.csproj", "MS.Infrastructure/"]
RUN dotnet restore "MS.ThaiSalonV2.Api/MS.ThaiSalonV2.Api.csproj"
COPY . .
WORKDIR "/src/MS.ThaiSalonV2.Api"
RUN dotnet build "MS.ThaiSalonV2.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MS.ThaiSalonV2.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MS.ThaiSalonV2.Api.dll"]