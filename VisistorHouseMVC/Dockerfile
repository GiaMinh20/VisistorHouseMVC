#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["VisistorHouseMVC/VisistorHouseMVC.csproj", "VisistorHouseMVC/"]
RUN dotnet restore "VisistorHouseMVC/VisistorHouseMVC.csproj"
COPY . .
WORKDIR "/src/VisistorHouseMVC"
RUN dotnet build "VisistorHouseMVC.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VisistorHouseMVC.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VisistorHouseMVC.dll"]