# Use official .NET SDK for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY *.sln ./
COPY API/*.csproj API/
COPY API.Test/*.csproj API.Test/
COPY Application/*.csproj Application/
COPY Application.Test/*.csproj Application.Test/
COPY Domain/*.csproj Domain/
COPY Domain.Test/*.csproj Domain.Test/
COPY Infrastructure/*.csproj Infrastructure/
COPY Infrastructure.Test/*.csproj Infrastructure.Test/

# Restore dependencies
RUN dotnet restore API/API.csproj

# Copy everything else and build
COPY . .
WORKDIR /app/API
RUN dotnet publish -c Release -o /out

# Use lightweight ASP.NET Core runtime for final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .
EXPOSE 80
ENTRYPOINT ["dotnet", "API.dll"]
