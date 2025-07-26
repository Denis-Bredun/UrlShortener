# UrlShortener

A URL shortening service built with ASP.NET Core 8.0, featuring user authentication, JWT tokens, and Entity Framework Core with SQL Server.

## Features

- **URL Shortening**: Create short URLs with custom or auto-generated short codes
- **User Authentication**: JWT-based authentication with ASP.NET Core Identity
- **User Management**: Register, login, and manage user accounts
- **About Information**: Manage and display about information
- **API Documentation**: Swagger/OpenAPI documentation
- **Logging**: Comprehensive logging with Serilog
- **Validation**: Input validation using FluentValidation
- **Clean Architecture**: Layered architecture with Domain, Application, Infrastructure, and API layers

## Prerequisites

Before running this project, ensure you have the following installed:

- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **ASP.NET and web development** (workload from Visual Studio 2022 Installer)
- **.NET desktop development** (workload from Visual Studio 2022 Installer)
- **SQL Server LocalDB** (an additional component in Visual Studio 2022 Installer)
- **Git** (for cloning the repository)

## Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd UrlShortener
   ```

2. **Open the solution in Visual Studio 2022**
   - Open `UrlShortener.sln` in Visual Studio 2022
   - Wait for the solution to load and restore NuGet packages

3. **Verify dependencies**
   - Ensure all NuGet packages are restored
   - Check that the solution builds successfully (Build → Build Solution)

## Configuration

The project is pre-configured for local development. Key configuration files:

- **`appsettings.json`**: Contains database connection string and JWT settings
- **`Program.cs`**: Contains dependency injection setup and middleware configuration

### Database Configuration

The project uses SQL Server LocalDB with the following connection string:
```
Server=(localdb)\mssqllocaldb;Database=UrlShortenerDb;Trusted_Connection=True;TrustServerCertificate=True;
```

### JWT Configuration

JWT tokens are configured for local development:
- **Issuer**: `https://localhost:7001`
- **Audience**: `https://localhost:7001`
- **Key**: Pre-configured secret key (change for production)

## Running the Application

### Local Development (Required - HTTPS Only)

**Important**: This project must be run locally via HTTPS in Visual Studio 2022. Docker deployment is not supported for this application.

1. **Set the startup project**
   - Right-click on `UrlShortener.API` in Solution Explorer
   - Select "Set as Startup Project"

2. **Run the application**
   - Press `F5` or click the "Start" button in Visual Studio
   - The application will run on `https://localhost:7001`

3. **Access the API**
   - **Swagger UI**: `https://localhost:7001/swagger`
   - **API Base URL**: `https://localhost:7001/api`

### Database Migrations

**Automatic Migration**: The application automatically runs migrations on startup in the `Program.cs` file. The database is recreated each time the application starts.

**Manual Migration** (if needed):
```bash
# Create a new migration
dotnet ef migrations add InitialCreate --project UrlShortener.Infrastructure --startup-project UrlShortener.API

# Apply migrations to database
dotnet ef database update --project UrlShortener.Infrastructure --startup-project UrlShortener.API
```

**Note**: Make sure you have the Entity Framework Core CLI tools installed:
```bash
dotnet tool install --global dotnet-ef
```

### Default Admin Account

The application creates a default admin account on startup:
- **Email**: `adminpanel@gmail.com`
- **Password**: `sUpEr$ecret123`
- **Username**: `Admin`

## API Endpoints

### Authentication (`/api/auth`)
- `POST /api/auth/register` - Register a new user account
- `POST /api/auth/login` - Authenticate user with email and password
- `GET /api/auth/me` - Get current authenticated user information (requires authentication)

### URL Management (`/api/urls`)
- `GET /api/urls` - Get all short URLs (accessible to all users, including anonymous)
- `GET /api/urls/{id}` - Get details of a specific short URL by ID (requires authentication)
- `POST /api/urls` - Create a new short URL (requires authentication)
- `DELETE /api/urls/{id}` - Delete a short URL (users can delete only their own URLs, admins can delete any URL)

### Redirect
- `GET /{shortCode}` - Redirect to original URL based on short code (accessible to everyone, including anonymous)

### About Information (`/api/about`)
- `GET /api/about` - Get URL shortening algorithm description (accessible to all users, including anonymous)
- `PUT /api/about` - Update URL shortening algorithm description (requires admin role)

## Testing

**Note**: Tests were not completed due to time constraints. The test project structure is in place but requires implementation.

## Development Notes

### Architecture & Design Patterns
- **Clean Architecture**: Layered architecture with clear separation of concerns
- **Dependency Injection**: Comprehensive DI setup in `Program.cs` with service registration and decoration
- **Repository Pattern**: Entity Framework Core with custom DbContext
- **Decorator Pattern**: Logging decorators for all services (Auth, ShortUrl, About, Identity, AboutInfo)
- **Exception Handling**: Custom exception middleware and domain-specific exceptions

### Database & Migrations
- **SQL Server LocalDB**: Chosen for Windows development environment
- **Migration History**: 
  - `20250724193345_InitialCreate` - Created initial database schema with ASP.NET Core Identity tables (AspNetUsers, AspNetRoles, AspNetUserRoles, etc.) and ShortUrls table with unique ShortCode index
  - `20250724213545_AboutInfoTable` - Added AboutInfo table
  - `20250725181424_AddedNavigationProperties` - Added navigation properties
- **Auto-Seeding**: Database is automatically seeded with roles, admin user and demo data on startup

### Why No Docker?
The project uses **Microsoft SQL Server LocalDB**, which is primarily designed for Windows development environments. To avoid spending time on complex Docker configuration for SQL Server on different platforms, the decision was made to focus on local development with Visual Studio 2022 and HTTPS.

### Logging & Monitoring
- **Serilog**: Structured logging with console and file outputs
- **Log Files**: Daily rolling logs in `logs/log-.txt`
- **Service Logging**: All services are wrapped with logging decorators for comprehensive audit trails

### Security & Authentication
- **JWT Tokens**: Bearer token authentication with configurable issuer/audience
- **Role-Based Authorization**: Admin and User roles with different permissions
- **Password Validation**: ASP.NET Core Identity with secure password policies

### API Features
- **Swagger Documentation**: Auto-generated API documentation at `/swagger`
- **CORS**: Configured for Angular development client (`http://localhost:4200`)
- **Validation**: FluentValidation with automatic model validation
- **Error Handling**: Consistent error responses with custom exception middleware

### Development Workflow
- **Hot Reload**: Configured for development environment
- **Database Recreation**: Database is recreated on each startup for clean development

## Troubleshooting

### Common Issues

1. **Database Connection Error**
   - Ensure SQL Server LocalDB is installed
   - Check that the connection string in `appsettings.json` is correct

2. **Port Already in Use**
   - The application will automatically use the next available port
   - Check the console output for the actual URL

3. **Build Errors**
   - Clean and rebuild the solution
   - Restore NuGet packages
   - Ensure .NET 8.0 SDK is installed

### Getting Help

If you encounter issues:
1. Check the application logs in the `logs/` directory
2. Review the console output for error messages
3. Ensure all prerequisites are installed correctly

## License

This project is licensed under the terms specified in the LICENSE.txt file.