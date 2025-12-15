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

# Create a non-root user for running the application
# This user will be overridden by --user flag when running, but provides a fallback
RUN groupadd -r genny && useradd -r -g genny genny
USER genny

ENTRYPOINT ["/app/publish/Genny"]
CMD ["--help"]