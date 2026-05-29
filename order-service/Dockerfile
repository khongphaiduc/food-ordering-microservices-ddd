# 1. Dùng image SDK để chứa đồ nghề build code
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 2. Copy file .csproj vào trước để tải thư viện (giúp build nhanh hơn)
COPY ["order-service.csproj", "./"]
RUN dotnet restore "./order-service.csproj"

# 3. Copy toàn bộ code còn lại vào và tiến hành Build
COPY . .
RUN dotnet build "order-service.csproj" -c Release -o /app/build

# 4. Publish code ra file chạy (DLL)
FROM build AS publish
RUN dotnet publish "order-service.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 5. Dùng image ASP.NET Runtime (bản siêu nhẹ chỉ để chạy app)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080

# 6. Copy đống file đã publish ở bước 4 sang đây
COPY --from=publish /app/publish .

# 7. Lệnh khởi động app
ENTRYPOINT ["dotnet", "order-service.dll"]