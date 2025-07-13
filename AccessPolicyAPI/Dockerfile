# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["AccessPolicyAPI/AccessPolicyAPI.csproj", "AccessPolicyAPI/"]
RUN dotnet restore "./AccessPolicyAPI/AccessPolicyAPI.csproj"
COPY . .
WORKDIR "/src/AccessPolicyAPI"
RUN dotnet build "./AccessPolicyAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./AccessPolicyAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENV ASPNETCORE_URLS="http://0.0.0.0:8080;https://0.0.0.0:8081"

# Ports:
# Inside the container, Kestrel listens on ports 8080 (HTTP) and 8081 (HTTPS), due to EXPOSE directives and ASPNETCORE_URLS setting.
# Outside the container, on the Docker host (where the container runs), access the API via http://localhost:5000 and https://localhost:6000).
# On a different machine on the same network as the Docker host, use http://<docker-host-ip>:5000 and https://<docker-host-ip>:6000.

# Network Interface:
# By default, Kestrel binds to the loopback interface localhost (127.0.0.1).
# This works when running (and accessing) the app on the same machine as the Docker host, 
# but doesn't when running the app in a Docker container and accessing it from outside the container.

# Loopback Interface:
# The loopback network interface (typically 127.0.0.1 for IPv4 or ::1 for IPv6) is isolated to the local machine. 
# It's a virtual interface that allows a device to communicate with itself, but it's not reachable from any other machine, even on the same network.

# 0.0.0.0 Interface:
# Special windcard address that means "any" IPv4 network interface.
# Using 0.0.0.0 makes Kestrel listen on all network interfaces, making the app reachable from outside the container.

# docker run -p 5000:8080 -p 6000:8081 AccessPolicyAPI
# Maps Docker host's port 5000 to hosted container's 8080 (similarly for 6000 and 8081).

ENTRYPOINT ["dotnet", "AccessPolicyAPI.dll"]