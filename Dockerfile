FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy project files and restore as distinct layers
COPY *.sln .
COPY Havn.Api/*.csproj ./Havn.Api/
COPY Havn.Calculations/*.csproj ./Havn.Calculations/
COPY Havn.DataProviders/*.csproj ./Havn.DataProviders/
COPY Havn.Models/*.csproj ./Havn.Models/
COPY Havn.UnitTests/*.csproj ./Havn.UnitTests/
RUN dotnet restore

# Copy everything else and build
COPY . ./
COPY Havn.Api/. ./Havn.Api/
COPY Havn.Calculations/. ./Havn.Calculations/
COPY Havn.DataProviders/. ./Havn.DataProviders/
COPY Havn.Models/. ./Havn.Models/

WORKDIR /app/Havn.Api
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/Havn.Api/out ./
ENTRYPOINT ["dotnet", "Havn.Api.dll"]