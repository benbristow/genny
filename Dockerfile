# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files
COPY Genny/Genny.csproj Genny/
RUN dotnet restore Genny/Genny.csproj

# Copy source code
COPY . .

# Build and publish
WORKDIR /src/Genny
RUN dotnet publish -c Release -o /app/publish --self-contained false

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish .

# Set entrypoint
ENTRYPOINT ["dotnet", "Genny.dll"]
