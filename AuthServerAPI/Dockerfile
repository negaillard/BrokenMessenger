FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем ВСЕ файлы решения
COPY . .

# Восстанавливаем и строим из корня решения
RUN dotnet restore "AuthServerAPI/AuthServerAPI.csproj"
RUN dotnet build "AuthServerAPI/AuthServerAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthServerAPI/AuthServerAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthServerAPI.dll"]

