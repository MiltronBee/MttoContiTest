import { useState } from "react";
import { NavbarUser } from "../ui/navbar-user";
import { useNavigate } from "react-router-dom";
import { ChevronLeft } from "lucide-react";
import CalendarWidget from "../Dashboard-Area/CalendarWidget";

export const Plantilla = () => {
  const navigate = useNavigate();
  const [selectedArea, setSelectedArea] = useState('construccion');
    
// Ejemplo de áreas - esto vendrá del backend eventualmente
const areas = [
    { id: 'construccion', name: 'Construcción' },
    { id: 'produccion', name: 'Producción' },
    { id: 'calidad', name: 'Calidad' },
    { id: 'mantenimiento', name: 'MANTENIMIENTO AREA III' }
];

const handleAreaChange = (areaId: string) => {
    setSelectedArea(areaId);
    console.log('Área cambiada a:', areaId);
};

  return (
    <div className="flex flex-col min-h-screen w-full bg-white p-12">
      <header className="flex justify-between items-center pb-4">
        <div className="flex flex-col gap-1">
          <div
            className="flex items-center gap-2 cursor-pointer"
            onClick={() => navigate(-1)}
          >
            <ChevronLeft /> Regresar
          </div>
          <h1 className="text-2xl font-bold  text-slate-800">Plantilla</h1>
          <p className="text-slate-600">
            Revisa el calendario de tu plantilla.
          </p>
        </div>
        <NavbarUser />
      </header>
      <CalendarWidget 
        showTabs={true}
        defaultView="calendar"
        showHeader={true}
        showSidebar={false}
        areas={areas}
        selectedArea={selectedArea}
        onAreaChange={handleAreaChange}
    />
        
    </div>
  );
};

