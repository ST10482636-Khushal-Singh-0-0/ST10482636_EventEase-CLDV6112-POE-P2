# EventEase (ST10482636_EventEase)
EventEase is an enterprise-grade, full-stack ASP.NET Core MVC web application designed to streamline event scheduling, physical venue allocation, and reservation tracking. Built with rigid backend business validation rules and a fully decoupled cloud infrastructure architecture, the platform features a dynamic lookup tracking system and a fully responsive user dashboard optimized for deployment on Microsoft Azure.
## Core Features
### 1. Cloud-Native Asset Management
 * **Venues, Events, and Bookings:** Full CRUD capabilities for processing physical property spaces, upcoming event profiles, and reservation schedules.
 * **Azure Blob Storage Integration:** File upload payloads bypass database storage limits by streaming binary image data directly into public-facing cloud storage containers (venue-images, event-images, and booking-images).
### 2. Multi-Criteria Filtering Matrix
The events system incorporates a high-performance backend query engine that processes four explicit parameters simultaneously:
 * **Text-Token Keyword Search:** Scans event title headers and description fields concurrently.
 * **Relational Category Isolation:** Binds records to lookups populated by database seed categories (*Concert/Music Festival, Corporate/Conference, Wedding/Celebration, Exhibition/Expo, and Workshop/Seminar*).
 * **Assigned Venue Matrix:** Uses a single-pass SQL EXISTS optimization loop to filter and display only the events explicitly scheduled at a selected location.
 * **Temporal Boundaries:** Restricts displayed records to specific calendar start and end date windows.
### 3. Business Logic and Relational Constraints
 * **Asynchronous Double-Booking Prevention:** Intercepts data submissions within the booking lifecycle to block duplicate reservations, ensuring a single venue space cannot be assigned twice on the same calendar date.
 * **Data Integrity Safeguards:** Enforces relational database constraints to block deletion requests on active events if they are tied to a scheduled reservation block.
### 4. Responsive Web Design (RWD)
 * **Fluid Layout Scaling:** Utilizes pure CSS3 grid mechanics and flexbox wrappers to wrap and adjust the visual dashboard across desktop displays, tablets, and smartphones.
 * **Adaptive Visibility Layouts:** Uses structural style overrides to hide desktop-specific alignment elements when the application drops below standard mobile screen width tolerances.
## Architecture and Technology Stack
 * **Web Framework:** ASP.NET Core MVC (.NET 8.0 / .NET 9.0)
 * **Language:** C# 12
 * **Object-Relational Mapper:** Entity Framework Core (EF Core) Code-First
 * **Production Infrastructure:** Microsoft Azure Cloud Ecosystem
   * **Compute Engine:** Azure App Service (Windows/Linux Environment)
   * **Relational Database:** Azure SQL Database Server
   * **Object Storage Tiers:** Azure Blob Storage Account
## Local Development and Installation
### Prerequisites
 * .NET SDK (v8.0 or higher)
 * Visual Studio 2022 (with the "ASP.NET and web development" workload enabled)
 * Local SQL Server Instance (Express/LocalDB) or remote Azure SQL Server credentials
### 1. Clone the Repository
```bash
git clone https://github.com/YOUR_GITHUB_USERNAME/ST10482636_EventEase.git
cd ST10482636_EventEase

```
### 2. Configure Local Application Secrets
Configure your local appsettings.json file to establish connection links to your database and local storage emulators:
```json
{
  "ConnectionStrings": {
    "ST10482636_EventEaseContext": "Server=(localdb)\\mssqllocaldb;Database=ST10482636_EventEaseContext;Trusted_Connection=True;MultipleActiveResultSets=true;"
  },
  "AzureStorage": {
    "ConnectionString": "UseDevelopmentStorage=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}

```
### 3. Execute Database Migrations and Seeding
Open your terminal window or Package Manager Console inside the project root folder and execute the database schema configuration scripts:
```bash
dotnet ef migrations add InitialSetup
dotnet ef database update

```
### 4. Run the Platform
```bash
dotnet run

```
Open a browser window and navigate to https://localhost:7190 to view the running application locally.
## Azure Cloud Production Deployment
The entry pipeline inside Program.cs is engineered to search flexibly for connection parameters across Azure App Settings, Connection Strings, or internal system environment flags before defaulting to local files.
### Production Schema Sync
To update and push your database layout maps onto your live remote Azure SQL Server instance, execute the migration command with an explicit connection string override flag:
```bash
dotnet ef database update --connection "Server=tcp:YOUR_SERVER_NAME.database.windows.net,1433;Initial Catalog=EventEaseDB;Persist Security Info=False;User ID=YOUR_ADMIN_USERNAME;Password=YOUR_ACTUAL_PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"

```
### Required Azure Infrastructure Variable Configuration
To ensure seamless operations in the cloud environment, the following configuration toggles must be configured inside your Azure Portal instances:
> **App Service Configuration:** Add ASPNETCORE_ENVIRONMENT with a value of Development under the Environment Variables panel to trace real-time runtime exceptions during cloud validation operations.
> **Storage Account Security:** Change the account-level flag **Allow Blob anonymous access** to **Enabled**.
> **Container Privacy Scopes:** Ensure your venue-images, event-images, and booking-images blob storage containers are explicitly set to **Blob (anonymous read access for blobs only)** so image paths render clearly on public browsers.
> 
