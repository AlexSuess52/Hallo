# === Build Stage ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# copy project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# copy the full source code and publish the application
COPY . ./
RUN dotnet publish -c Release -o /out

# === Runtime Stage ===
# use a smaller ASP.NET Core runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# install additional tools
RUN apt-get update \
    && apt-get install -y sqlite3 postgresql-client \
    && rm -rf /var/lib/apt/lists/*

# copy the published output from the build stage
COPY --from=build /out ./

# expose port 80 for HTTP traffic
EXPOSE 80

# copy the entrypoint script
COPY entrypoint.sh .

# normalize line endings for compatibility (fixes Windows line endings) and make the script executable
RUN sed -i 's/\r$//' entrypoint.sh && chmod +x ./entrypoint.sh

# run the application using the custom entrypoint
ENTRYPOINT ["./entrypoint.sh"]

