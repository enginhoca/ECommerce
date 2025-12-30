# 1.STAGE
# Burası "build stage". Burada dotnet 10.0 Sdk'ini ilgili adresten indir(kopyalamak için)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

# src adında bir klasör oluştur ve oraya geç
WORKDIR /src

# Host'taki ECommerce.sln dosyasını, Container'daki /src klasörüne kopyala.
COPY ECommerce.sln .

# Proje dosyalarını kopyala
COPY ECommerce.API/*.csproj ./ECommerce.API/
COPY ECommerce.Business/*.csproj ./ECommerce.Business/
COPY ECommerce.Data/*.csproj ./ECommerce.Data/
COPY ECommerce.Entity/*.csproj ./ECommerce.Entity/



# Tüm kaynak kodları containera kopyala
COPY . .

# API projesine geç
WORKDIR /src/ECommerce.API

# Container içinde projelerin bağımlılıklarını restore et.
RUN dotnet restore

# Uygulamayı production için hazırla
RUN dotnet publish -c Release -o /publish --no-restore

# 2.STAGE
# Burası "runtime stage". Burada Asp.Net Core 10.0 Runtime'ı ilgili adresten indir
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

# Build stage'deki publish edilmiş dosyaları kopyala
COPY --from=build /app/publish .

# Container'ın 8080 portunu dinlemesini sağla
EXPOSE 8080

# Container başlatıldığında çalışacak komut(dotnet ECommerce.API.dll)
ENTRYPOINT ["dotnet","ECommerce.API.dll"]