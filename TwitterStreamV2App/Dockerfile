FROM mcr.microsoft.com/dotnet/sdk:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TwitterStreamV2App/TwitterStreamV2App.csproj", "TwitterStreamV2App/"]
RUN dotnet restore "TwitterStreamV2App/TwitterStreamV2App.csproj"
COPY . .
WORKDIR "/src/TwitterStreamV2App"
RUN dotnet build "TwitterStreamV2App.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TwitterStreamV2App.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitterStreamV2App.dll"]
