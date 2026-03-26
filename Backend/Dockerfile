FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["semantic-search-app.csproj", "./"]
RUN dotnet restore "semantic-search-app.csproj"

# Copy everything else and build
COPY . .
RUN dotnet build "semantic-search-app.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "semantic-search-app.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
RUN apt-get update && apt-get install -y libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
WORKDIR /app
COPY --from=publish /app/publish .


EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "semantic-search-app.dll"]

