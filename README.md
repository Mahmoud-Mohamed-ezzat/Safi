**A modern, intelligent, and comprehensive hospital management system built with ASP.NET Core.**

---

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Architecture](#architecture)
- [AI Integration](#ai-integration)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## Overview

**Safi** is an intelligent Hospital Management System designed to automate and optimize hospital operations. It provides a powerful backend API to manage patients, doctors, nurses, appointments, rooms, billing, staff shifts, and more — with built-in AI capabilities for smarter decision-making.

The system is developed using clean architecture principles and is ready for both development and production use.

---

## ✨ Key Features

### Core Modules
- **Patient Management** – Full patient records and medical history
- **Doctor & Staff Management** – Profiles, shifts, attendance, and assignments
- **Appointment & Reservation System** – Booking, scheduling, and room allocation
- **Department & Facility Management** – Rooms, ICU, Emergency handling
- **Billing & Finance** – Automated invoicing and price management
- **Reporting & Analytics** – Doctor reports, statistics, and insights

### Advanced Capabilities
- Real-time communication using SignalR
- JWT Authentication with Refresh Tokens
- AI-powered analysis and predictions
- Comprehensive RESTful API
- Global exception handling and logging
- Outbox pattern for reliable messaging

---

## 🛠 Tech Stack

- **Framework**: ASP.NET Core Web API
- **Language**: C# 
- **Database**: SQL Server + Entity Framework Core
- **ORM**: Entity Framework Core
- **Object Mapping**: AutoMapper
- **Real-time**: SignalR Hubs
- **Authentication**: JWT + Refresh Tokens
- **Architecture**: Clean Architecture + Repository Pattern
- **Other**: MediatR-style services, Global Middleware

---

## 📁 Project Structure
Safi/
├── Controllers/                 # All API Controllers
├── Models/                      # Domain Entities & DbContext
├── Dto/                         # Data Transfer Objects
├── Repositories/                # Data Access Layer
├── Services/                    # Business Logic Layer
├── Interfaces/                  # Contracts & Abstractions
├── Hubs/                        # SignalR Real-time Hubs
├── Mapper/                      # AutoMapper Profiles
├── Configuration/               # Configuration Classes
├── Exceptions/                  # Custom Exceptions
├── Helpers/                     # Utility Helpers
├── Images/                      # Static Images
├── wwwroot/                     # Static Web Assets
├── Migrations/                  # EF Core Database Migrations
├── Safi.csproj
├── Program.cs
├── appsettings.json
└── Safi.sln
text
---

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- SQL Server (LocalDB or full version)
- Visual Studio 2022 or VS Code + C# Dev Kit

### Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/Mahmoud-Mohamed-ezzat/Safi.git
   cd Safi
   git checkout Test

Restore packages:Bashdotnet restore
Update connection string in Safi/appsettings.json
Apply database migrations:Bash cd Safi
dotnet ef database update
Run the application:Bashdotnet run

The API will be available at https://localhost:7250 (or the port shown in console).

📡 Main API Controllers

AccountsController – User registration & authentication
DoctorController, NurseController, StaffController
Patient & ReservationController
RoomController, ICUController, EmergencyController
BillController, PricesController
AIModelsController – AI features
StatisticsController – Reports & analytics
AttendanceController, ShiftController

Swagger UI available at /swagger when running locally.
