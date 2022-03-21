FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["GhostNetwork.Messages.Api/GhostNetwork.Messages.Api.csproj", "GhostNetwork.Messages.Api/"]
RUN dotnet restore "GhostNetwork.Messages.Api/GhostNetwork.Messages.Api.csproj"
COPY . .
WORKDIR "/src/GhostNetwork.Messages.Api"
RUN dotnet build "GhostNetwork.Messages.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GhostNetwork.Messages.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GhostNetwork.Messages.Api.dll"]
