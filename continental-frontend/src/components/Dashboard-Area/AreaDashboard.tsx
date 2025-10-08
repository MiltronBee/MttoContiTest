/**
 * =============================================================================
 * AREA DASHBOARD
 * =============================================================================
 * 
 * @description
 * Dashboard principal para jefes de área que proporciona navegación y gestión
 * de calendario, solicitudes, plantilla y reportes. Incluye routing y layout
 * principal para todas las funcionalidades del área.
 * 
 * @inputs (Datos de endpoints)
 * - currentPeriod: Period - Período actual de gestión de vacaciones
 * - location: Location - Ubicación actual de la ruta para navegación activa
 * 
 * @used_in (Componentes padre)
 * - src/App.tsx (como ruta protegida para área)
 * 
 * @user_roles (Usuarios que acceden)
 * - Jefe de Área
 * 
 * @dependencies
 * - React: Framework base y hooks (useState, JSX)
 * - react-router-dom: Navegación y routing (Routes, Route, Link, useLocation)
 * - lucide-react: Iconos de navegación (Calendar, File, FileChartColumn, Users)
 * - ./CalendarComponent: Componente de calendario del área
 * - ./SolicitudesComponent: Gestión de solicitudes
 * - ./Plantilla: Gestión de plantilla de empleados
 * - ./Reportes: Generación de reportes
 * 
 * @author Vulcanics Dev Team
 * @created 2024
 * @last_modified 2025-08-20
 * =============================================================================
 */

import { useState, type JSX } from "react";
import { Routes, Route, Link, useLocation, Navigate } from "react-router-dom";
import { Navbar } from "../Navbar/Navbar";
import { Calendar, File, FileChartColumn, Users } from "lucide-react";
import CalendarComponent from "./CalendarComponent";
import SolicitudesComponent from "./SolicitudesComponent";
import SolicitudDetallePage from "./SolicitudDetallePage";
import { Plantilla } from "./Plantilla";
import { Reportes } from "./Reportes";
import { DetallesEmpleado } from "../Empleado/DetallesEmpleado";
import { PeriodOptions, type Period } from "@/interfaces/Calendar.interface";

// Manager subroute components

const navItems = [
  { to: "/area/calendario", label: "Calendario", icon: <Calendar /> },
  { to: "/area/solicitudes", label: "Solicitudes", icon: <File /> },
  { to: "/area/plantilla", label: "Plantilla", icon: <Users /> },
  { to: "/area/reportes", label: "Reportes", icon: <FileChartColumn /> },
];

const AreaDashboard = (): JSX.Element => {
  const location = useLocation();
  const [currentPeriod] = useState<Period>(PeriodOptions.reprogramming);

  return (
    <div className="flex flex-col h-screen">
      <Navbar>
              <nav className="flex gap-2">
                {navItems.map((item) => {
                  const isActive = location.pathname === item.to || 
                    (location.pathname === '/area' && item.to === '/area/calendario');
                  
                  return (
                    <Link
                      key={item.to}
                      to={item.to}
                      className={`rounded-lg flex items-center gap-2 px-4 py-2 transition-colors ${
                        isActive 
                          ? 'bg-continental-yellow text-continental-black' 
                          : 'hover:bg-continental-yellow hover:text-continental-black'
                      }`}
                    >
                      {item.icon}
                      <span>{item.label}</span>
                    </Link>
                  );
                })}
              </nav>
            </Navbar>

      {/* Main Content */}
      <div className="flex-1 bg-gray-100 w-full overflow-x-hidden">
        <Routes>
          <Route index element={<Navigate to="/area/calendario" replace />} />
          <Route path="calendario" element={<CalendarComponent />} />
          <Route path="solicitudes" element={<SolicitudesComponent />} />
          <Route path="solicitudes/:id" element={<SolicitudDetallePage />} />
          <Route path="plantilla" element={<Plantilla />} />
          <Route path="plantilla/:id" element={<DetallesEmpleado currentPeriod={currentPeriod} />} />
          <Route path="reportes" element={<Reportes />} />
        </Routes>
      </div>
    </div>
  );
};

export default AreaDashboard;
