# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first (layer caching)
COPY BookStore.sln .
COPY src/BookStore.Domain/BookStore.Domain.csproj src/BookStore.Domain/
COPY src/BookStore.Application/BookStore.Application.csproj src/BookStore.Application/
COPY src/BookStore.Infrastructure/BookStore.Infrastructure.csproj src/BookStore.Infrastructure/
COPY src/BookStore.API/BookStore.API.csproj src/BookStore.API/
COPY tests/BookStore.Tests/BookStore.Tests.csproj tests/BookStore.Tests/

# Restore dependencies
RUN dotnet restore

# Copy all source code
COPY . .

# Run tests
RUN dotnet test tests/BookStore.Tests/BookStore.Tests.csproj \
    --no-restore \
    --configuration Release \
    --logger "console;verbosity=minimal"

# Publish the API
RUN dotnet publish src/BookStore.API/BookStore.API.csproj \
    --no-restore \
    --configuration Release \
    --output /app/publish

# ── Stage 2: Runtime ───────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Non-root user for security
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser

COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "BookStore.API.dll"]
