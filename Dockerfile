# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
# COPY *.sln .
COPY src/*.csproj ./src/
RUN dotnet restore src

# copy everything else and build app
COPY src/. ./src/
WORKDIR /source/src
RUN dotnet publish -c release -o /app 

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "ShortedFaaS.dll"]
EXPOSE 80