# 🛒 ECommerce API

Modern ve ölçeklenebilir bir e-commerce uygulaması için geliştirilmiş RESTful API. Ürün yönetimi, sipariş işleme ve kimlik doğrulama özelliklerine sahiptir.

**📍 Live API URL:** https://ecommerce-bi4w.onrender.com/

---

## 📋 İçindekiler

- [Proje Özellikleri](#proje-özellikleri)
- [Teknoloji Stack'i](#teknoloji-stacki)
- [Proje Mimarisi](#proje-mimarisi)
- [Kurulum ve Çalıştırma](#kurulum-ve-çalıştırma)
- [API Endpoints](#api-endpoints)
- [Veritabanı Şeması](#veritabanı-şeması)
- [Design Patterns](#design-patterns)

---

## 🎯 Proje Özellikleri

✨ **Kullanıcı Yönetimi**
- Kayıt ve giriş işlemleri
- JWT tabanlı kimlik doğrulama
- Rol tabanlı erişim kontrolü (Role-Based Authorization)
- Güvenli şifre saklama

📦 **Ürün Yönetimi**
- Ürün oluşturma, güncelleme ve silme (Hard/Soft delete)
- Kategori desteği
- Sayfalanmış ürün listeleme
- Düşük stok kontrol
- Ürün filtreleme

🛍️ **Sipariş Yönetimi**
- Sipariş oluşturma ve takip
- Sipariş durumu değiştirme
- Kullanıcı özel siparişleri görüntüleme
- Admin paneli için tüm siparişleri görüntüleme

🔒 **Güvenlik**
- CORS (Cross-Origin Resource Sharing) desteği
- Giriş doğrulaması (FluentValidation)
- Global hata yönetimi (Exception Handling Middleware)
- JWT Bearer Token Authentication

---

## 🛠️ Teknoloji Stack'i

### Backend Framework
- **ASP.NET Core 10.0** - Modern web framework
- **C# 13** - Programlama dili

### Veritabanı
- **PostgreSQL 16** - İlişkisel veritabanı
- **Entity Framework Core 10.0** - ORM
- **Database Migrations** - Veritabanı sürüm yönetimi

### Kimlik Doğrulama & Yetkilendirme
- **JWT (JSON Web Tokens)** - Stateless authentication
- **ASP.NET Core Identity** - Kullanıcı ve rol yönetimi

### Validation & Mapping
- **FluentValidation 11.3.1** - Giriş doğrulaması
- **AutoMapper 15.1.0** - DTO mapping

### Containerization
- **Docker** - Container'ization
- **Docker Compose** - Multi-container orchestration

### API Documentation
- **OpenAPI/Swagger** - API dokümantasyonu

---

## 🏗️ Proje Mimarisi

Proje **Katmanlı Mimari (Layered Architecture)** kullanmaktadır:

```
ECommerce/
├── ECommerce.API/           # Presentation Layer (Controller, Filters, Middleware)
├── ECommerce.Business/      # Business Logic Layer (Services, Validators, DTOs)
├── ECommerce.Data/          # Data Access Layer (Repository, DbContext)
└── ECommerce.Entity/        # Domain Model Layer (Entities)
```

### Katman Açıklamaları

**1. ECommerce.API (Sunum Katmanı)**
- Controllers: HTTP request'leri karşılayan ve yanıt veren sınıflar
- Middleware: Global istek/yanıt işleme
- Filters: Otomatik doğrulama filtreleri

**2. ECommerce.Business (İş Mantığı Katmanı)**
- Services: İş kurallarının uygulandığı yer
- DTOs: Veri transfer nesneleri
- Validators: FluentValidation kuralları
- Exceptions: Özel hata sınıfları

**3. ECommerce.Data (Veri Erişim Katmanı)**
- Repository Pattern: Veritabanı işlemlerinin soyutlanması
- Unit of Work: İşlem yönetimi
- DbContext: Entity Framework Core yapılandırması

**4. ECommerce.Entity (Alan Modeli)**
- Domain entities: Veritabanı tablolarının temsili

---

## 🚀 Kurulum ve Çalıştırma

### Ön Koşullar
- .NET 10.0 SDK
- PostgreSQL 16
- Docker & Docker Compose (opsiyonel)
- Git

### Adım 1: Repository'i Klonlayın
```bash
git clone <repository-url>
cd ECommerce
```

### Adım 2: Veritabanı Bağlantısını Yapılandırın

`ECommerce.API/appsettings.json` dosyasını düzenleyin:

```json
"ConnectionStrings": {
  "PostgreSqlConnection": "Host=localhost;Port=5432;Database=ecommerce;Username=your_user;Password=your_password"
}
```

### Adım 3: Veritabanı Migrasyonlarını Çalıştırın
```bash
cd ECommerce.API
dotnet ef database update
```

### Adım 4: Uygulamayı Çalıştırın

**Manuel:**
```bash
cd ECommerce.API
dotnet run
```

**Docker ile:**
```bash
docker-compose up -d
```

API şu adreste çalışacak: `http://localhost:5070`

---

## 📡 API Endpoints

### 🔐 Authentication (`/api/auth`)

| Method | Endpoint | Açıklama | Auth Gerekli |
|--------|----------|----------|--------------|
| POST | `/api/auth/login` | Kullanıcı girişi | ❌ |
| POST | `/api/auth/register` | Yeni kullanıcı kaydı | ❌ |

**Login Request:**
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Register Request:**
```json
{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "Password123!"
}
```

---

### 📦 Products (`/api/products`)

| Method | Endpoint | Açıklama | Auth Gerekli | Rol |
|--------|----------|----------|--------------|-----|
| GET | `/api/products` | Tüm ürünleri getir | ❌ | - |
| GET | `/api/products/paged` | Sayfalanmış ürünleri getir | ❌ | - |
| GET | `/api/products/{id}` | Belirli ürünü getir | ❌ | - |
| GET | `/api/products/low?threshold=10` | Düşük stok ürünleri getir | ❌ | - |
| POST | `/api/products` | Yeni ürün oluştur | ✅ | Admin |
| PUT | `/api/products/{id}` | Ürünü güncelle | ✅ | Admin |
| DELETE | `/api/products/{id}` | Ürünü soft delete et | ✅ | Admin |
| DELETE | `/api/products?id={id}` | Ürünü hard delete et | ✅ | Admin |

**Sayfalanmış Ürün Query Parametreleri:**
```
GET /api/products/paged?pageNumber=1&pageSize=10&categoryId=1
```

**Create Product Request:**
```json
{
  "name": "iPhone 15",
  "description": "Apple's latest smartphone",
  "price": 999.99,
  "stock": 50,
  "categoryIds": [1, 2]
}
```

---

### 🛍️ Orders (`/api/orders`)

| Method | Endpoint | Açıklama | Auth Gerekli | Rol |
|--------|----------|----------|--------------|-----|
| POST | `/api/orders` | Yeni sipariş oluştur | ✅ | User |
| GET | `/api/orders` | Tüm siparişleri getir | ✅ | Admin |
| GET | `/api/orders/my-orders` | Kendi siparişlerimi getir | ✅ | User |

**Order Now Request:**
```json
{
  "orderItems": [
    {
      "productId": 1,
      "quantity": 2
    }
  ]
}
```

---

## 💾 Veritabanı Şeması

### Ana Tablolar

**Users (AppUser)**
- Id (Primary Key)
- Email
- FirstName, LastName
- PasswordHash
- CreatedAt, UpdatedAt
- IsDeleted (Soft Delete)

**Products**
- Id (Primary Key)
- Name
- Description
- Price
- Stock
- CreatedAt, UpdatedAt
- IsDeleted (Soft Delete)

**Categories**
- Id (Primary Key)
- Name
- Description

**ProductCategories** (Many-to-Many)
- ProductId (Foreign Key)
- CategoryId (Foreign Key)

**Orders**
- Id (Primary Key)
- AppUserId (Foreign Key)
- Status (Pending, Processing, Shipped, Completed, Cancelled)
- TotalPrice
- CreatedAt, UpdatedAt

**OrderItems**
- Id (Primary Key)
- OrderId (Foreign Key)
- ProductId (Foreign Key)
- Quantity
- UnitPrice

---

## 🎨 Design Patterns

### 1. **Repository Pattern**
Veri erişimini soyutlayan ve merkezileştiren pattern.

```csharp
IRepository<Product> repository = unitOfWork.GetRepository<Product>();
var products = await repository.GetAllAsync();
```

**Avantajları:**
- Veritabanı sorguları merkezi bir yerden yönetilir
- Test etmesi kolaydır (Mock'lanabilir)
- Veritabanı değişimi kolaylaşır

---

### 2. **Unit of Work Pattern**
Birden fazla repository işlemini bir transaction altında yönetme.

```csharp
var unitOfWork = new UnitOfWork(dbContext);
var productRepo = unitOfWork.GetRepository<Product>();
await unitOfWork.SaveChangesAsync();
```

**Avantajları:**
- İşlemler atomik olur
- Hata durumunda rollback edilir
- Konsistent veri sağlanır

---

### 3. **Dependency Injection (DI)**
ASP.NET Core'un built-in DI container'ı kullanılır.

```csharp
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```

**Avantajları:**
- Loosely coupled kod
- Test etmesi kolay (Mock'lanabilir)
- Bakımı ve genişletmesi basit

---

### 4. **DTO (Data Transfer Object) Pattern**
Entity'leri doğrudan API yanıtında göndermeyen pattern.

```csharp
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

**Avantajları:**
- Güvenlik (hassas alanlar gönderilmez)
- API versiyonu yönetiminde esneklik
- Frontend için optimal veri

---

### 5. **Factory Pattern**
ResponseDto sınıfında kullanılır.

```csharp
public static ResponseDto<T> Success(T data, int statusCode)
{
    return new ResponseDto<T> { ... };
}
```

**Avantajları:**
- Nesne oluşturma mantığı merkezi
- Konsistent response yapısı

---

### 6. **Strategy Pattern**
Farklı filtreleme stratejileri için kullanılabilir.

```csharp
var response = await _productService.GetAllPagedAsync(
    categoryId: 1,
    orderBy: x => x.OrderByDescending(y => y.CreatedAt)
);
```

---

### 7. **Middleware Pattern**
Global exception handling ve CORS yönetimi.

```csharp
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("DefaultPolicy");
```

---

## 📊 Yanıt Formatı

Tüm API yanıtları standardize edilmiş bir formattadır:

**Başarılı Yanıt (Success):**
```json
{
  "data": {
    "id": 1,
    "name": "Product Name",
    "price": 99.99
  },
  "error": null,
  "isSucceed": true
}
```

**Hata Yanıtı (Error):**
```json
{
  "data": null,
  "error": "Ürün bulunamadı!",
  "isSucceed": false
}
```

---

## 🔒 Güvenlik Yapılandırması

### JWT Configuration
```json
{
  "JwtConfig": {
    "Secret": "your-secret-key-min-32-chars",
    "Issuer": "ECommerce_Backend",
    "Audience": "ECommerce_Web",
    "AccessTokenExpiration": 30
  }
}
```

### CORS Policy
```json
{
  "CorsSettings": {
    "AllowedOrigins": ["http://localhost:3000"],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowedHeaders": ["Content-Type", "Authorization"]
  }
}
```

---

## 📝 Şifre Politikaları

- Minimum 8 karakter
- En az bir büyük harf
- En az bir küçük harf
- En az bir rakam
- En az bir özel karakter

---

## 🐳 Docker Yapılandırması

### PostgreSQL Container
- Image: `postgres:16-alpine`
- Port: `5420:5432`
- Username: `admin`
- Password: `admin123`
- Database: `ecommerce`

### API Container
- Build: Dockerfile'dan
- Port: `5070:8080`
- Environment: Production

---

## 📚 Kullanılan Kütüphaneler

| Kütüphane | Sürüm | Amaç |
|-----------|-------|------|
| Entity Framework Core | 10.0.0 | ORM |
| AutoMapper | 15.1.0 | DTO Mapping |
| FluentValidation | 11.3.1 | Giriş Doğrulaması |
| JWT Bearer | 10.0.0 | Token Authentication |
| PostgreSQL Provider | Latest | PostgreSQL Desteği |

---

## 🚨 Error Handling

Proje özel exception sınıfları kullanır:

- `BusinessException` - İş mantığı hatası
- `NotFoundException` - Kaynak bulunamadı
- `UnauthorizedException` - Yetkilendirme hatası
- `ValidationException` - Doğrulama hatası

---

## 🤝 Katkıda Bulunma

1. Fork'layın
2. Feature branch'i oluşturun (`git checkout -b feature/AmazingFeature`)
3. Değişiklikleri commit'leyin (`git commit -m 'Add some AmazingFeature'`)
4. Branch'ı push'layın (`git push origin feature/AmazingFeature`)
5. Pull Request açın

---

## 📄 Lisans

Bu proje MIT Lisansı altında yayınlanmıştır.

---

## 👨‍💻 Yazarlar

- **Engin Niyazi** - Backend Developer

---

## 📧 İletişim

Sorularınız için bize ulaşabilirsiniz:
- Email: enginniyazi@example.com
- LinkedIn: [Profil](https://linkedin.com)

---

**⭐ Bu projeyi yararlı buluyorsanız lütfen yıldız verin!**
