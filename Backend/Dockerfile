# Multi-stage build for ASP.NET Core API
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution and project files
COPY ["FlowingLinks.sln", "."]
COPY ["FlowingLinks.Core/FlowingLinks.Core.csproj", "FlowingLinks.Core/"]
COPY ["FlowingLinks.Api/FlowingLinks.Api.csproj", "FlowingLinks.Api/"]
COPY ["FlowingLinks.Console/FlowingLinks.Console.csproj", "FlowingLinks.Console/"]
COPY ["FlowingLinks.Tests/FlowingLinks.Tests.csproj", "FlowingLinks.Tests/"]

# Restore dependencies
RUN dotnet restore "FlowingLinks.sln"

# Copy source code
COPY . .

# Build the solution
RUN dotnet build "FlowingLinks.sln" -c Release -o /app/build

# Publish the API
FROM build AS publish
RUN dotnet publish "FlowingLinks.Api/FlowingLinks.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

# Copy published application
COPY --from=publish /app/publish .

# Create database directory for SQLite
RUN mkdir -p /app/data

EXPOSE 5000
ENV ASPNETCORE_URLS=http://+:5000

ENTRYPOINT ["dotnet", "FlowingLinks.Api.dll"]
