# Clinic Scheduling API

A REST API for managing clinic appointments — patients, doctors, and scheduled visits. Built with ASP.NET Core 8, Entity Framework Core, and PostgreSQL.

## Tech Stack

- **Framework:** ASP.NET Core 8 Web API
- **ORM:** Entity Framework Core 8 with Npgsql (PostgreSQL)
- **Auth:** Stateless JWT (HS256) via `Microsoft.AspNetCore.Authentication.JwtBearer`
- **Password hashing:** BCrypt.Net-Next

## Features

- JWT authentication — register and login return a signed token
- Full CRUD for patients and appointments
- Doctor management
- Global exception handler returning consistent `{ "error": "..." }` JSON
- EF Core migrations applied automatically on startup

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL instance (local or remote)

### Configuration

Copy `appsettings.example.json` to `appsettings.json` and fill in your values:

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Database=clinic_scheduling;Username=postgres;Password=YOUR_PASSWORD"
  },
  "Jwt": {
    "Secret": "your-secret-key-at-least-32-characters-long",
    "ExpirationMs": "3600000"
  }
}
```

### Run

```bash
dotnet run
```

The database schema is created automatically on first startup via `db.Database.Migrate()`.

## API Reference

### Auth

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/auth/register` | No | Create account, returns JWT |
| POST | `/api/auth/login` | No | Validate credentials, returns JWT |

**Request body (both endpoints):**
```json
{ "username": "string", "password": "string" }
```

**Response:**
```json
{ "token": "eyJ..." }
```

---

### Patients

> All endpoints require `Authorization: Bearer <token>`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/patients` | List all patients |
| POST | `/api/patients` | Create a patient |
| GET | `/api/patients/{id}` | Get patient by ID |
| PUT | `/api/patients/{id}` | Update patient |
| DELETE | `/api/patients/{id}` | Delete patient |

**Patient body:**
```json
{
  "fullName": "Jane Smith",
  "dateOfBirth": "1990-04-15",
  "email": "jane@example.com",
  "phone": "555-0100"
}
```

---

### Doctors

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/doctors` | No | List all doctors |
| POST | `/api/doctors` | JWT | Create a doctor |

**Doctor body:**
```json
{ "fullName": "Dr. Alice Chen", "specialty": "Cardiology" }
```

---

### Appointments

> All endpoints require `Authorization: Bearer <token>`

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/appointments` | List all appointments |
| POST | `/api/appointments` | Schedule an appointment |
| GET | `/api/appointments/{id}` | Get appointment by ID |
| PUT | `/api/appointments/{id}` | Update status, notes, or time |
| DELETE | `/api/appointments/{id}` | Cancel/delete appointment |

**Create body:**
```json
{
  "patientId": 1,
  "doctorId": 2,
  "scheduledAt": "2025-09-01T10:00:00",
  "notes": "Follow-up visit"
}
```

**Update body** (all fields optional):
```json
{
  "scheduledAt": "2025-09-02T14:00:00",
  "status": "Completed",
  "notes": "Rescheduled"
}
```

Valid status values: `Scheduled`, `Completed`, `Cancelled`

## Project Structure

```
├── Controllers/        # Route handlers — thin, delegate to services
├── Services/           # Business logic, DTO mapping
├── Models/             # EF Core entity classes
├── DTOs/               # Request/response shapes
├── Data/               # AppDbContext and EF configuration
├── Security/           # JWT token generation
├── Exceptions/         # NotFoundException and GlobalExceptionHandler
├── Migrations/         # EF Core migration files
└── Program.cs          # DI wiring and middleware pipeline
```
