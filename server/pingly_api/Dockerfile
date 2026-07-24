FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY pingly_api.csproj ./

RUN dotnet restore

COPY . .

RUN dotnet publish -c Release -o /app/publish

# ---- Stage 2: runtime ----
    FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
    WORKDIR /app
    COPY --from=build /app/publish .
    
    # Tell ASP.NET Core which port to listen on inside the container
    ENV ASPNETCORE_URLS=http://+:8080
    EXPOSE 8080
    
    ENTRYPOINT ["dotnet", "pingly_api.dll"]