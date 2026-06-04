# ST10482636_CLDV6211_POE_Part_2
Part 2 of the POE for Cloud Development
​EventEase is an enterprise-grade, full-stack ASP.NET Core MVC web application designed to streamline event scheduling, physical venue allocation, and reservation tracking. Built with rigid backend business validation rules and a fully decoupled cloud infrastructure architecture, the platform features a dynamic lookup tracking system and a fully responsive user dashboard optimized for deployment on Microsoft Azure.
​Core Features
​1. Cloud-Native Asset Management
​Venues, Events, and Bookings: Full CRUD capabilities for processing physical property spaces, upcoming event profiles, and reservation schedules.
​Azure Blob Storage Integration: File upload payloads bypass database storage limits by streaming binary image data directly into public-facing cloud storage containers (venue-images, event-images, and booking-images).
​2. Multi-Criteria Filtering Matrix
​The events system incorporates a high-performance backend query engine that processes four explicit parameters simultaneously:
​Text-Token Keyword Search: Scans event title headers and description fields concurrently.
​Relational Category Isolation: Binds records to lookups populated by database seed categories (Concert/Music Festival, Corporate/Conference, Wedding/Celebration, Exhibition/Expo, and Workshop/Seminar).
​Assigned Venue Matrix: Uses a single-pass SQL EXISTS optimization loop to filter and display only the events explicitly scheduled at a selected location.
​Temporal Boundaries: Restricts displayed records to specific calendar start and end date windows.
​3. Business Logic and Relational Constraints
​Asynchronous Double-Booking Prevention: Intercepts data submissions within the booking lifecycle to block duplicate reservations, ensuring a single venue space cannot be assigned twice on the same calendar date.
​Data Integrity Safeguards: Enforces relational database constraints to block deletion requests on active events if they are tied to a scheduled reservation block.
​4. Responsive Web Design (RWD)
​Fluid Layout Scaling: Utilizes pure CSS3 grid mechanics and flexbox wrappers to wrap and adjust the visual dashboard across desktop displays, tablets, and smartphones.
​Adaptive Visibility Layouts: Uses structural style overrides to hide desktop-specific alignment elements when the application drops below standard mobile screen width tolerances.
​Architecture and Technology Stack
​Web Framework: ASP.NET Core MVC (.NET 8.0 / .NET 9.0)
​Language: C# 12
​Object-Relational Mapper: Entity Framework Core (EF Core) Code-First
​Production Infrastructure: Microsoft Azure Cloud Ecosystem
​Compute Engine: Azure App Service (Windows/Linux Environment)
​Relational Database: Azure SQL Database Server
​Object Storage Tiers: Azure Blob Storage Account
​Local Development and Installation
​Prerequisites
​.NET SDK (v8.0 or higher)
​Visual Studio 2022 (with the "ASP.NET and web development" workload enabled)
​Local SQL Server Instance (Express/LocalDB) or remote Azure SQL Server credentials
