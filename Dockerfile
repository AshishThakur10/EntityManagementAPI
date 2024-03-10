# Use the official .NET Core SDK image as a base image
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

# Set the working directory in the container
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the remaining files and build the application
COPY . ./
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0

# Set the working directory in the container
WORKDIR /app

# Copy the published application to the container
COPY --from=build-env /app/out .

# Expose the port your application listens on
EXPOSE 80

# Command to run the application
ENTRYPOINT ["dotnet", "EntityManagementAPI.dll"]
