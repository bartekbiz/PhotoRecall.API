﻿FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PhotoRecall.API/PhotoRecall.API.csproj", "PhotoRecall.API/"]
RUN dotnet restore "PhotoRecall.API/PhotoRecall.API.csproj"
COPY . .
WORKDIR "/src/PhotoRecall.API"
RUN dotnet build "PhotoRecall.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PhotoRecall.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir -p wwwroot
ENTRYPOINT ["dotnet", "PhotoRecall.API.dll"]
