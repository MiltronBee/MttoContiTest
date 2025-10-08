# Continental Vacation App

Enterprise vacation management system for Continental manufacturing plant.

## Features

- 🔐 **User Authentication** - BCrypt password hashing, JWT tokens
- 📅 **Block Reservation System** - Vacation blocks starting at 9:00 AM with 24-hour duration
- 👥 **User Management** - 111 users with roles (SuperUsuario, IE, JefeArea, LiderGrupo, ComiteSindical)
- 📊 **Employee Calendar** - Automated calendar generation (skips weekends, starts day 8)
- 👷 **1,057 Employees** - Pre-imported employee database
- 🎯 **Manning Management** - Track workforce availability
- 📱 **Modern UI** - React + Vite frontend with responsive design

## Tech Stack

### Backend
- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- BCrypt.Net for password hashing

### Frontend
- React 18
- Vite
- TailwindCSS
- Radix UI Components
- React Router
- Axios

## Quick Deployment from GitHub

### On New Server:

```bash
# 1. Clone the repository
git clone https://github.com/MiltronBee/ContiTest.git
cd ContiTest

# 2. Restore the database
sqlcmd -S localhost -U sa -P "YourPassword" -C -Q "RESTORE DATABASE Vacaciones FROM DISK = 'Database back\Vacaciones.bak' WITH REPLACE"

# 3. Start Backend
cd FreeTimeApp/tiempo-libre.app
dotnet restore
dotnet run

# 4. Start Frontend (in a new terminal)
cd continental-frontend
npm install
npm run dev
```

### Updating Existing Deployment:

```bash
# Pull latest changes
git pull origin master

# Restart backend and frontend
```

## Configuration

### Backend (`FreeTimeApp/tiempo-libre.app/appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Vacaciones;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

### Frontend (`continental-frontend/.env`)

```env
VITE_API_URL=http://localhost:5050
```

## Default Credentials

New users imported with default password: **Continental2024!**

## Database

- **Server**: localhost
- **Database**: Vacaciones
- **Total Users**: 1,057 employees + 111 management users
- **Backup Location**: `Database back/Vacaciones.bak`

## Endpoints

- **Backend API**: http://localhost:5050
- **Frontend**: http://localhost:5173
- **Swagger Docs**: http://localhost:5050/swagger

## Deployment Scripts

Two deployment scripts are included:

1. **deploy.ps1** - Automated deployment from zip package
2. **create-deployment-package.ps1** - Creates deployment zip file

## Project Structure

```
ProyectoConti/
├── FreeTimeApp/                  # Backend (.NET 9.0)
│   ├── tiempo-libre.app/         # Main API
│   ├── tiempo-libre.Tests/       # Unit tests
│   ├── EmpleadosImporter/        # Employee importer tool
│   └── UserListImporter/         # User importer tool
├── continental-frontend/         # Frontend (React + Vite)
├── Database back/                # Database backups
└── deploy.ps1                    # Deployment script
```

## Development

### Backend Development
```bash
cd FreeTimeApp/tiempo-libre.app
dotnet watch run
```

### Frontend Development
```bash
cd continental-frontend
npm run dev
```

### Run Tests
```bash
cd FreeTimeApp/tiempo-libre.Tests
dotnet test
```

## Key Features

### Vacation Block Management
- Blocks start at 9:00 AM
- 24-hour duration
- Automatic state updates every 6 hours
- Weekend skipping in calendar generation

### User Roles
- **SuperUsuario** - Full system access
- **Ingeniero (IE)** - Engineer access
- **JefeArea** - Area manager
- **LiderGrupo** - Shift leader
- **ComiteSindical** - Union committee

### Calendar Features
- Automatic employee calendar generation
- Weekends are skipped
- Starts on day 8 of each period
- Manning percentage tracking
- Exception handling for holidays

## API Documentation

Full API documentation available at: `/swagger` when backend is running

## Contributing

This is an internal Continental project. For changes:

1. Create a feature branch
2. Make your changes
3. Test thoroughly
4. Create a pull request
5. Get approval from team lead

## License

Internal Continental AG project - All rights reserved

## Support

For issues or questions, contact the development team.

---

🤖 Generated with [Claude Code](https://claude.com/claude-code)
