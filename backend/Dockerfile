# ================
# BASE ASP.NET
# ================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

# Railway passa a porta via variável PORT
ENV ASPNETCORE_URLS=http://+:${PORT}

EXPOSE ${PORT}

# ================
# BUILD
# ================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# copia tudo
COPY . .

# RESTAURE O PROJETO
RUN dotnet restore "./backend.csproj"

# PUBLICA EM RELEASE
RUN dotnet publish "./backend.csproj" -c Release -o /app/publish

# ================
# FINAL
# ================
FROM base AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "backend.dll"]
