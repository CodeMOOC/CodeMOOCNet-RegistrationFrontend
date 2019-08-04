# ---
# First stage (build)
FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /app

# Copy solution as distinct layer
COPY *.sln .
COPY Web/*.csproj ./Web/
RUN dotnet restore

# Copy everything else and build
COPY Web/. ./Web/
WORKDIR /app/Web
RUN dotnet publish -c Release -o out

# ---
# Second stage (execution)
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS runtime
WORKDIR /app
COPY --from=build /app/Web/out ./

# Run on localhost:8779
ENV ASPNETCORE_URLS http://+:5001
EXPOSE 5001

ENTRYPOINT ["dotnet", "CodeMooc.Web.dll"]
