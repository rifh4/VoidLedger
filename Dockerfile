FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["VoidLedger.Api/VoidLedger.Api.csproj", "VoidLedger.Api/"]
COPY ["VoidLedger.Core/VoidLedger.Core.csproj", "VoidLedger.Core/"]
RUN dotnet restore "VoidLedger.Api/VoidLedger.Api.csproj"

COPY . .
WORKDIR /src/VoidLedger.Api
RUN dotnet publish "VoidLedger.Api.csproj" -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "VoidLedger.Api.dll"]