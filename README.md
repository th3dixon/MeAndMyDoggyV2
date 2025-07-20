# MeAndMyDoggy - UK Pet Services Platform

## Project Structure

```
MeAndMyDog.sln
├── src/
│   ├── API/MeAndMyDog.API/          # Backend API (Port 7010)
│   ├── Web/MeAndMyDog.WebApp/       # MVC Frontend (Port 56682)
│   └── BuildingBlocks/               # Shared components
│       ├── MeAndMyDog.SharedKernel/  # Domain primitives
│       └── MeAndMyDog.BlobStorage/   # Storage abstraction
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server 2019+ (or SQL Server Express)
- Node.js 18+ (for frontend tooling)
- Visual Studio 2022 or VS Code

### Setup Instructions

1. **Clone the repository**
   ```bash
   cd MeAndMyDoggyV2
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Setup the database**
   - Update the connection string in `src/API/MeAndMyDog.API/appsettings.json`
   - Run Entity Framework migrations:
   ```bash
   cd src/API/MeAndMyDog.API
   dotnet ef database update
   ```

4. **Run the API**
   ```bash
   cd src/API/MeAndMyDog.API
   dotnet run
   ```
   The API will be available at https://localhost:7010

5. **Run the Web App**
   In a new terminal:
   ```bash
   cd src/Web/MeAndMyDog.WebApp
   dotnet run
   ```
   The web app will be available at https://localhost:56682

## Development

### API Endpoints

- `POST /api/v1/auth/register` - User registration
- `POST /api/v1/auth/login` - User login
- `GET /api/v1/providers/search` - Search service providers
- `POST /api/v1/bookings` - Create booking request

### Key Features Implemented

- ✅ User authentication with JWT
- ✅ Service provider search by UK postcode
- ✅ Booking system foundation
- ✅ Responsive UI with Tailwind CSS
- ✅ UK-specific localization (£, miles, postcodes)

### Next Steps

1. Create Entity Framework migrations
2. Implement remaining API endpoints
3. Add email service integration
4. Implement payment processing
5. Add unit and integration tests

## Configuration

### API Settings (appsettings.json)

- **Connection String**: Update for your SQL Server instance
- **JWT Secret**: Generate a secure key for production
- **Email Settings**: Configure SMTP for notifications

### Environment Variables

For production, use Azure Key Vault or environment variables:
- `ConnectionStrings__DefaultConnection`
- `Jwt__SecretKey`
- `Azure__StorageConnectionString`

## License

Copyright 2024 MeAndMyDoggy UK. All rights reserved.