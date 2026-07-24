# Banka RDMS Projesi

## Yerel yapılandırma

Bağlantı bilgisi ve JWT anahtarı repoda tutulmaz. Geliştirme ortamında:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection-string>" --project BankRDMS.API/BankRDMS.API/BankRDMS.API.csproj
dotnet user-secrets set "Jwt:Key" "<en-az-32-byte-rastgele-anahtar>" --project BankRDMS.API/BankRDMS.API/BankRDMS.API.csproj
```

`Jwt:Issuer` ve `Jwt:Audience` hassas olmayan değerler olarak `appsettings.json`
içinde bulunur.

## Kimlik doğrulama

`POST /api/Auth/login` kimlik numarası ve doğum tarihiyle JWT üretir. Login ve
müşteri kayıt endpointi dışında bütün controller endpointleri JWT gerektirir.
Swagger'daki **Authorize** alanına token doğrudan yapıştırılabilir.

Mevcut veritabanı şeması değiştirilmediği için parola saklanmamaktadır. Üretim
ortamında parola/MFA kullanımı için ayrı bir kimlik sağlayıcısı veya kontrollü
bir veritabanı migration'ı gerekir.

## Doğrulama

```powershell
dotnet build BankRDMS.API/BankRDMS.API.sln --configuration Release
dotnet test BankRDMS.API/BankRDMS.Tests/BankRDMS.Tests.csproj --configuration Release
```
