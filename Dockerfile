#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
LABEL Nvisia.Profile.Service=true
LABEL UnitTest=true
WORKDIR /app
COPY Nvisia.Profile.Service.sln ./
COPY ./src ./src
COPY ./test ./test
RUN dotnet restore Nvisia.Profile.Service.sln

FROM build AS test
LABEL UnitTest=true
WORKDIR /app/test/Nvisia.Profile.Service.UnitTests
RUN dotnet build --framework:net8.0 --no-restore
RUN dotnet test

FROM build AS publish
LABEL Nvisia.Profile.Service=true
WORKDIR /app/src/Nvisia.Profile.Service.Api
RUN dotnet publish --no-restore -c Release -o /app/publish

FROM base AS final
LABEL Nvisia.Profile.Service=true
WORKDIR /app
COPY --from=publish /app/publish .

# Install curl
RUN apt-get update && apt-get install -y curl

ENTRYPOINT ["dotnet", "Nvisia.Profile.Service.Api.dll"]