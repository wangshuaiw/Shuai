#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Shuai.IdentityServer.V1.0/Shuai.IdentityServer.V1.0.csproj", "Shuai.IdentityServer.V1.0/"]
RUN dotnet restore "Shuai.IdentityServer.V1.0/Shuai.IdentityServer.V1.0.csproj"
COPY . .
WORKDIR "/src/Shuai.IdentityServer.V1.0"
RUN dotnet build "Shuai.IdentityServer.V1.0.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Shuai.IdentityServer.V1.0.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Shuai.IdentityServer.V1.0.dll"]