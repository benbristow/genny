# -----------------
# 1. Build stage
# -----------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src/Genny
COPY Genny/Genny.csproj .
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore --self-contained false

# -----------------
# 2. Runtime stage
# -----------------
# Use the lighter runtime image
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
ENTRYPOINT ["dotnet", "Genny.dll"]