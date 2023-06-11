FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Frontend/ACME.Frontend.Web.Mvc/ACME.Frontend.Web.Mvc.csproj", "Frontend/ACME.Frontend.Web.Mvc/"]
COPY ["Backend/ACME.Backend.Models/ACME.Backend.Models.csproj", "Backend/ACME.Backend.Models/"]
RUN dotnet restore "Frontend/ACME.Frontend.Web.Mvc/ACME.Frontend.Web.Mvc.csproj"
COPY . .
WORKDIR "/src/Frontend/ACME.Frontend.Web.Mvc"
RUN dotnet build "ACME.Frontend.Web.Mvc.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ACME.Frontend.Web.Mvc.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ACME.Frontend.Web.Mvc.dll"]