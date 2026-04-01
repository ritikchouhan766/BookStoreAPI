# ── Stage 1: Build ──────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy everything so restore can find all projects
COPY . .

# Restore using solution file
RUN dotnet restore BookStore.sln

# Run tests — fails here if any test fails
RUN dotnet test tests/BookStore.Tests/BookStore.Tests.csproj \
    --no-restore --configuration Release \
    --logger "console;verbosity=minimal"

# Publish
RUN dotnet publish src/BookStore.API/BookStore.API.csproj \
    --no-restore --configuration Release \
    --output /app/publish

# ── Stage 2: Runtime ────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser
USER appuser
COPY --from=build /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENTRYPOINT ["dotnet", "BookStore.API.dll"]