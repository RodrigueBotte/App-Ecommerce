# ECommerce - Jeux de Société

Application e-commerce mobile dédiée à la vente de jeux de société, 
développée avec .NET MAUI et ASP.NET Core.

## Technologies

| Couche | Technologie |
|--------|-------------|
| Mobile | .NET MAUI (.NET 9) |
| API | ASP.NET Core 8 |
| Base de données | SQL Server + Entity Framework Core |
| Authentification | JWT + ASP.NET Identity |

## Structure du projet

- ECommerce.Api # Backend APS.NET Core avec Entity Framework
- ECommerce.Mobile # Application mobile .NET MAUI

## Fonctionnalités

- Catalogue de jeux de société 
- Authentification JWT (Admin / Utilisateur)
- Upload d'images produits
- Interface d'administration (CRUD produits, éditeurs, auteurs, catégories, thèmes)

## Installation

### Prérequis
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [.NET 8 SDK](https://dotnet.microsoft.com/download) (pour l'API)
- [SQL Server](https://www.microsoft.com/fr-fr/sql-server/sql-server-downloads) (ou SQL Server Express)
- [Visual Studio 2022](https://visualstudio.microsoft.com/fr/) avec les workloads :
  - **ASP.NET et développement web**
  - **Développement mobile .NET MAUI**
- [EF Core CLI](https://learn.microsoft.com/fr-fr/ef/core/cli/dotnet) :
  ```bash
  dotnet tool install --global dotnet-ef
  ```

### 1. Cloner le projet
```bash
git clone https://github.com/ton-profil/ton-repo.git
cd ton-repo
```

### 2. Configurer la base de données
Dans `ECommerce.Api/appsettings.json`, remplace la connection string :
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TON_SERVEUR;Database=ECommerceDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 3. Appliquer les migrations
```bash
cd ECommerce.Api
dotnet ef database update
```
Cela crée automatiquement la base de données et toutes les tables.

### 4. Créer le compte admin
Au premier lancement, crée un compte avec l'endpoint `/api/auth/register` via Swagger :
```json
{
  "email": "admin@ecommerce.com",
  "password": "TonMotDePasse"
}
```
Le rôle Admin est attribué automatiquement à cet email au démarrage de l'API.

### 5. Lancer l'API
```bash
cd ECommerce.Api
dotnet run
```
L'API sera disponible sur `https://localhost:7251` et Swagger sur `https://localhost:7251/swagger`

### 6. Configurer l'URL dans le Mobile
Dans `ECommerce.Mobile/MauiProgram.cs`, vérifie que l'URL pointe bien vers ton API :
```csharp
client.BaseAddress = new Uri("https://localhost:7251/");
```

### 7. Lancer l'application Mobile
Ouvre la solution dans **Visual Studio 2022** et lance `ECommerce.Mobile` sur l'émulateur ou en Windows.

## Auteur
Rodrigue Botte — [GitHub](https://github.com/RodrigueBotte)