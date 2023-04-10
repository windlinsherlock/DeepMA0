#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["PACS.WebAPI/PACS.WebAPI.csproj", "PACS.WebAPI/"]
COPY ["PACS.Shared/PACS.Shared.csproj", "PACS.Shared/"]
RUN dotnet restore "PACS.WebAPI/PACS.WebAPI.csproj"
COPY . .
WORKDIR "/src/PACS.WebAPI"
RUN dotnet build "PACS.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PACS.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PACS.WebAPI.dll"]