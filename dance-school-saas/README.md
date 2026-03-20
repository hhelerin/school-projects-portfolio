# TalTech's ICD0024 Web Applications with C# 2025-26 Spring 

**Dance School Management App - SaaS edition.**

Author Helerin Heinsalu | Uni-ID: helehe | Student code: 206451IADB

---

## 📋 Project Overview

A comprehensive multi-tenant SaaS platform for managing dance schools. This application enables dance school administrators to manage their studios, instructors, students, classes, and schedules in a unified system.

## ✨ Features

### Multi-Tenancy
- **Company-based isolation**: Each dance school operates in its own tenant space
- **Tenant-aware authorization**: Role-based access control per company
- **Shared infrastructure**: Single deployment serves multiple dance schools

### Studio Management
- Create and manage multiple studios
- Studio rooms with feature tracking
- Room availability management

### Class Scheduling
- Create dance classes with customizable schedules
- Assign instructors to classes
- Set dance styles and skill levels
- Recurring class schedules

### Student Management
- Student registration and profiles
- Class enrollment
- Attendance tracking
- Student history

### Instructor Management
- Instructor profiles linked to company users
- Schedule management
- Class assignments

### School Configuration
- Dance styles management
- Skill levels configuration
- Studio features (mirrors, barre, sound system, etc.)

## 🛠️ Tech Stack

| Component | Technology |
|-----------|------------|
| **Framework** | ASP.NET Core 10.0 MVC |
| **Database** | SQLite with Entity Framework Core |
| **Authentication** | ASP.NET Core Identity + JWT |
| **Frontend** | Razor Pages + HTMX + Alpine.js |
| **Architecture** | N-Tier (Domain, DAL, BLL, UI) |
| **Patterns** | SOLID, CQRS, Repository Pattern |
| **Testing** | xUnit |

## 📁 Project Structure

```
helehe-dance-school-saas/
├── App.Domain/           # Domain entities and interfaces
├── App.DAL.EF/           # Entity Framework data access layer
├── App.BLL/              # Business logic layer (Commands & Queries)
├── App.DTO/              # Data Transfer Objects
├── App.Helpers/          # Helper services (JWT, Auth, etc.)
├── App.Resources/        # Localization resources
├── Base.Resources/       # Shared localization
├── WebApp/               # ASP.NET Core MVC application
├── WebApp.Tests/         # Unit tests
└── openspec/             # OpenSpec change documentation
```

## 🚀 Getting Started

### Prerequisites

- .NET 10.0 SDK
- SQLite
- Docker (optional, for containerized deployment)

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd helehe-dance-school-saas
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database --project App.DAL.EF --startup-project WebApp update
   ```

4. **Run the application**
   ```bash
   dotnet run --project WebApp
   ```

5. **Access the application**
   - Open browser at `https://localhost:5001` (or the port shown in terminal)
   - Default port: 5001 (HTTP), 5002 (HTTPS)

### Docker Setup

```bash
# Using docker-compose (if available)
docker-compose up -d
```

## 🔧 Development Commands

### Database Migrations
```bash
# Add a new migration
dotnet ef migrations --project App.DAL.EF --startup-project WebApp add <MigrationName>

# Update database
dotnet ef database --project App.DAL.EF --startup-project WebApp update

# Drop database
dotnet ef database --project App.DAL.EF --startup-project WebApp drop

# Remove last migration
dotnet ef migrations --project App.DAL.EF --startup-project WebApp remove
```

### Code Generation
```bash
# Generate MVC controller
dotnet aspnet-codegenerator controller -name <ControllerName> -m <ModelName> -actions -dc AppDbContext -outDir Controllers --useDefaultLayout --useAsyncActions --referenceScriptLibraries -f

# Generate API controller
dotnet aspnet-codegenerator controller -name <ControllerName> -m <ModelName> -actions -dc AppDbContext -outDir ApiControllers -api --useAsyncActions -f

# Generate Identity UI
dotnet aspnet-codegenerator identity -dc DAL.App.EF.AppDbContext -f
```

### Install/Update Tools
```bash
dotnet tool update -g dotnet-ef
dotnet tool update -g dotnet-aspnet-codegenerator
```

### Library Manager (JS/CSS)
```bash
# Install HTMX
libman install htmx.org --files dist/htmx.min.js 

# Install Alpine.js
libman install alpinejs --files dist/cdn.min.js
```

## 🔐 Authentication & Authorization

- **JWT-based authentication** with refresh tokens
- **Role-based access control**: Admin, Instructor, Staff
- **Company-level authorization**: Users can only access their company's data
- **Multi-tenant security**: Tenant ID validation on all data operations

### Default Roles
- **School Admin**: Full access to all features within their company
- **Instructor**: Manage classes and view students
- **Staff**: View-only access to school data

## 📊 API Endpoints

The application exposes both MVC controllers and API endpoints. Key areas:

- `/Identity/*` - Authentication endpoints
- `/Studio` - Studio management
- `/StudioRoom` - Room management
- `/Instructor` - Instructor management
- `/Student` - Student management
- `/Class` - Class management
- `/DanceStyle` - Dance style configuration
- `/Level` - Skill level configuration

## 🔄 Data Model

### Core Entities
- **Company**: Represents a dance school/tenant
- **Studio**: Physical dance studio location
- **StudioRoom**: Individual rooms within a studio
- **StudioFeature**: Features available in rooms (mirrors, barre, etc.)
- **Instructor**: Dance instructors linked to company users
- **Student**: Dance students enrolled in classes
- **Class**: Dance classes with schedule information
- **ClassSchedule**: Recurring schedule for classes
- **DanceStyle**: Types of dance (Hip-hop, Ballet, etc.)
- **Level**: Skill levels (Beginner, Intermediate, Advanced, etc.)

## 📝 License

This project is for educational purposes as part of TalTech's ICD0024 course.

## 📬 Support

For issues or questions, please refer to the course materials or contact the instructor.

---

*Built with ASP.NET Core & ❤️ for dance education*
