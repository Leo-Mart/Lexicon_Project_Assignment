# Lexicon Project Assignment

A full-stack Learning Management System (LMS) developed as part of the Lexicon project assignment.

The application supports two roles: **Teacher** and **Student**. Teachers can manage courses and learning content, while students can access their assigned course, modules, activities, resources, and submissions.

## Tech Stack

### Backend

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* JWT authentication
* AutoMapper

### Frontend

* React 19
* TypeScript
* Vite
* Tailwind CSS
* React Router

### Testing

* xUnit
* Moq
* EF Core InMemory
* Coverlet

## Main Features

* User authentication and role-based authorization
* Course and student enrollment management
* Modules and activities
* Learning resources
* Student submissions and teacher feedback
* Protection against unauthorized access to course-specific data
* Pagination, searching, and sorting where applicable

## Project Structure

```text
backend/
  LMS.Api/

frontend/
  src/

tests/
  LMS.Api.Tests/
```

## Run the Project

### Backend

```bash
dotnet restore
dotnet run --project backend/LMS.Api
```

### Frontend

```bash
cd frontend
npm install
npm run dev
```

### Tests

```bash
dotnet test
```

## Development

The project uses separate backend and frontend applications. API endpoints are protected using authentication, role-based authorization, and object-level access checks for student-specific data.

