FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Backend/ACME.Backend.ShopApi/ACME.Backend.ShopApi.csproj", "Backend/ACME.Backend.ShopApi/"]
COPY ["Datalayer/ACME.DataLayer.Interfaces/ACME.DataLayer.Interfaces.csproj", "Datalayer/ACME.DataLayer.Interfaces/"]
COPY ["Datalayer/ACME.DataLayer.Documents/ACME.DataLayer.Documents.csproj", "Datalayer/ACME.DataLayer.Documents/"]
COPY ["Datalayer/ACME.DataLayer.Entities/ACME.DataLayer.Entities.csproj", "Datalayer/ACME.DataLayer.Entities/"]
COPY ["Datalayer/ACME.DataLayer.Repository.SqlServer/ACME.DataLayer.Repository.SqlServer.csproj", "Datalayer/ACME.DataLayer.Repository.SqlServer/"]
COPY ["Backend/ACME.Backend.Models/ACME.Backend.Models.csproj", "Backend/ACME.Backend.Models/"]
COPY ["Backend/ACME.Backend.Tools/ACME.Backend.Tools.csproj", "Backend/ACME.Backend.Tools/"]
RUN dotnet restore "Backend/ACME.Backend.ShopApi/ACME.Backend.ShopApi.csproj"
COPY . .
WORKDIR "/src/Backend/ACME.Backend.ShopApi"
RUN dotnet build "ACME.Backend.ShopApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACME.Backend.ShopApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACME.Backend.ShopApi.dll"]