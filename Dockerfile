# -----------------
# Build and runtime using SDK image
# -----------------
FROM mcr.microsoft.com/dotnet/sdk:10.0
WORKDIR /app
COPY Genny/Genny.csproj .
RUN dotnet restore
COPY Genny/ .
RUN dotnet publish -c Release -o /app/publish --no-restore

WORKDIR /app/publish
ENTRYPOINT ["/app/publish/Genny"]
CMD ["--help"]