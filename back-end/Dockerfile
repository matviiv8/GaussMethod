FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ["GaussMethod.API/GaussMethod.API.csproj", "GaussMethod.API/"]
COPY ["GaussMethod.Tests/GaussMethod.Tests.csproj", "GaussMethod.Tests/"]

RUN dotnet restore "GaussMethod.API/GaussMethod.API.csproj"

COPY . .

WORKDIR "/src/GaussMethod.API"

RUN dotnet build "GaussMethod.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GaussMethod.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GaussMethod.API.dll"]