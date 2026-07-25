# Runtime image (small)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Render routes traffic to port 10000 by default. The .NET base image would
# otherwise listen on 8080, so bind Kestrel to 10000 here to match.
ENV ASPNETCORE_URLS=http://0.0.0.0:10000
EXPOSE 10000

# Build image (SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ubuntu-health-api.csproj
RUN dotnet publish ubuntu-health-api.csproj -c Release -o publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=build /src/publish .
ENTRYPOINT ["dotnet", "ubuntu-health-api.dll"]
