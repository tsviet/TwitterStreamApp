FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TwitterMassagesConsumerApp/TwitterMassagesConsumerApp.csproj", "TwitterMassagesConsumerApp/"]
RUN dotnet restore "TwitterMassagesConsumerApp/TwitterMassagesConsumerApp.csproj"
COPY . .
WORKDIR "/src/TwitterMassagesConsumerApp"
RUN dotnet build "TwitterMassagesConsumerApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TwitterMassagesConsumerApp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TwitterMassagesConsumerApp.dll"]