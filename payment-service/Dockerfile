FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["payment-service.csproj", "./"]
RUN dotnet restore "payment-service.csproj"

COPY . .
RUN dotnet publish "payment-service.csproj" --configuration Release --output /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

ENV ASPNETCORE_ENVIRONMENT=Docker \
    Kestrel__Endpoints__gRPC__Url=http://+:5006 \
    Kestrel__Endpoints__gRPC__Protocols=Http2 \
    Kestrel__Endpoints__Http1__Url=http://+:8080 \
    Kestrel__Endpoints__Http1__Protocols=Http1AndHttp2

EXPOSE 8080 5006

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "payment-service.dll"]
