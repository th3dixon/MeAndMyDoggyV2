---
inclusion: always
---

# MeAndMyDog Project Overview

## Project Context
MeAndMyDog is a comprehensive UK pet services platform built with ASP.NET Core 9.0, designed to connect pet owners with service providers. The platform features AI-powered health recommendations, real-time messaging, and integrated payment processing.

## Solution Architecture
```
MeAndMyDog.sln
├── src/API/MeAndMyDog.API/          # Backend API (Port 7010)
├── src/Web/MeAndMyDog.WebApp/       # MVC Frontend (Port 56682)
└── src/BuildingBlocks/              # Shared components
    ├── MeAndMyDog.SharedKernel/     # Domain primitives
    └── MeAndMyDog.BlobStorage/      # Storage abstraction
```

## Technology Stack
- **Backend**: ASP.NET Core 9.0, Entity Framework Core 9.0, SQL Server
- **Frontend**: MVC with Razor Pages, Vue.js 3.5.13, Tailwind CSS 3.4.16
- **Real-time**: SignalR
- **Authentication**: ASP.NET Core Identity + JWT
- **Cloud**: Microsoft Azure (Key Vault, Blob Storage, Application Insights)
- **Caching**: Redis (StackExchange.Redis)

## Subscription Model
- **Pet Owners**: Free accounts only (no premium subscription options)
- **Service Providers**: Free and Premium subscription tiers available
  - Free: Basic provider profile and limited features
  - Premium: Advanced business tools, priority listing, enhanced analytics

## Key Features
- Dog profile management with medical records
- Pet services marketplace with booking system
- AI-powered health recommendations (Gemini AI)
- Real-time messaging and video calling
- Multi-provider payment system (Santander, Stripe, PayPal)
- Community features (forums, meetups, lost & found)
- Business tools for service providers
- Tiered subscription system for service providers

## Development Ports
- API: https://localhost:63343
- WebApp: https://localhost:56682
- Swagger: https://localhost:63343/swagger

## Database Connection
- Development: Azure VM SQL Server (senseilive.uksouth.cloudapp.azure.com:1433)
- Production: Connection string stored in Azure Key Vault
- Database: MeAndMyDog

## Authentication & Security
- JWT tokens with refresh token support
- Two-factor authentication with TOTP
- Role-based authorization (User, Admin, PetOwner, ServiceProvider)
- Azure Key Vault for secrets management
- Account lockout and IP blocking for security