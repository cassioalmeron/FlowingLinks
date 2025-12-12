# ========================================
# Stage 1: Build - Restore and Build
# ========================================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and all project files for solution-wide restore
COPY FlowingLinks.sln ./
COPY FlowingLinks.Api/FlowingLinks.Api.csproj ./FlowingLinks.Api/
COPY FlowingLinks.Core/FlowingLinks.Core.csproj ./FlowingLinks.Core/
COPY FlowingLinks.Console/FlowingLinks.Console.csproj ./FlowingLinks.Console/
COPY FlowingLinks.Tests/FlowingLinks.Tests.csproj ./FlowingLinks.Tests/

# Restore dependencies for the entire solution
RUN dotnet restore FlowingLinks.sln

# Copy all source code
COPY . .

# Build the API project
WORKDIR /src/FlowingLinks.Api
RUN dotnet build FlowingLinks.Api.csproj -c Release -o /app/build

# ========================================
# Stage 2: Publish
# ========================================
FROM build AS publish
WORKDIR /src/FlowingLinks.Api
RUN dotnet publish FlowingLinks.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ========================================
# Stage 3: Runtime - Final production image
# ========================================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Install curl for health checks and clean apt cache to minimize image size
RUN apt-get update && \
    apt-get install -y curl && \
    rm -rf /var/lib/apt/lists/*

# Create data directory for SQLite with proper permissions
RUN mkdir -p /app/data && chmod 755 /app/data

# Copy published output from publish stage
COPY --from=publish /app/publish .

# Expose port (configured via environment variable)
EXPOSE 5000

# Set default environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check endpoint
HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
    CMD curl -f http://localhost:5000/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "FlowingLinks.Api.dll"]
