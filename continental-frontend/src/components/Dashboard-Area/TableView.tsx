/**
 * =============================================================================
 * TABLE VIEW
 * =============================================================================
 * 
 * @description
 * Vista de tabla del calendario que muestra la información de empleados
 * y sus datos semanales en formato tabular. Incluye navegación por semanas
 * y meses con filtrado por grupos seleccionados.
 * 
 * @inputs (Props del componente)
 * - calendarData: CalendarData | null - Datos del calendario para mostrar en tabla
 * - currentDate: Date - Fecha actual seleccionada
 * - selectedGroups: string[] - Grupos seleccionados para filtrar
 * - weekOffset: number - Desplazamiento de semanas para navegación
 * - monthNames: string[] - Nombres de meses para localización
 * - onNavigateMonth: (direction: 'prev' | 'next') => void - Navegación entre meses
 * - onNavigateWeek: (direction: 'prev' | 'next') => void - Navegación entre semanas
 * 
 * @used_in (Componentes padre)
 * - src/components/Dashboard-Area/CalendarWidget.tsx
 * 
 * @user_roles (Usuarios que acceden)
 * - Jefe de Área
 * 
 * @dependencies
 * - React: Framework base y hooks (useState)
 * - lucide-react: Iconos de navegación (ChevronLeft, ChevronRight, ArrowLeft)
 * - date-fns: Utilidades de fecha (format) y localización (es)
 * - ../../interfaces/Calendar.interface: Tipos de datos del calendario
 * 
 * @author Vulcanics Dev Team
 * @created 2024
 * @last_modified 2025-08-20
 * =============================================================================
 */

import React, { useState } from 'react';
import { ChevronLeft, ChevronRight, ArrowLeft } from 'lucide-react';
import { format } from 'date-fns';
import { es } from 'date-fns/locale';
import type { CalendarData } from '../../interfaces/Calendar.interface';
import type { Grupo } from '@/interfaces/Areas.interface';
import type { AusenciasPorFecha } from '@/interfaces/Api.interface';

interface Employee {
    id: string;
    name: string;
    weekData: string[]; // Valores como "3D2", "1A1", etc.
}

interface GroupData {
    id: string;
    name: string;
    employees: Employee[];
}

interface TableViewProps {
    calendarData: CalendarData | null;
    currentDate: Date;
    selectedGroups: string[];
    weekOffset: number;
    monthNames: string[];
    onNavigateMonth: (direction: 'prev' | 'next') => void;
    onNavigateWeek: (direction: 'prev' | 'next') => void;
    currentAreaGroups?: Grupo[];
    ausenciasData?: AusenciasPorFecha[];
    manningRequerido?: number;
}

const TableView: React.FC<TableViewProps> = ({
    currentDate,
    selectedGroups,
    weekOffset,
    monthNames,
    onNavigateMonth,
    onNavigateWeek,
    currentAreaGroups,
    ausenciasData = [],
}) => {
    const [selectedGroup, setSelectedGroup] = useState<string | null>(null);

    // Función para obtener porcentaje real de ausencias por fecha y grupo
    const getRealPercentage = (date: Date, grupoId: number): number => {
        const fechaString = format(date, 'yyyy-MM-dd');
        const fechaData = ausenciasData.find(data => data.fecha === fechaString);
        
        if (!fechaData) return 0;
        
        const grupoData = fechaData.ausenciasPorGrupo.find(grupo => grupo.grupoId === grupoId);
        return grupoData?.porcentajeAusencia || 0;
    };

    // Usar grupos reales del área actual
    const groupsData: GroupData[] = currentAreaGroups?.map(grupo => ({
        id: grupo.grupoId.toString(),
        name: grupo.rol,
        employees: [] // Por ahora vacío, se puede llenar con datos reales si es necesario
    })) || [];

    // Obtener los días de la semana actual
    const getWeekDays = () => {
        const startOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
        const dates = [];
        for (let i = 0; i < 7; i++) {
            const date = new Date(startOfMonth);
            date.setDate(startOfMonth.getDate() + weekOffset + i);
            dates.push(date);
        }
        return dates;
    };



    const handleGroupClick = (groupId: string) => {
        setSelectedGroup(groupId);
    };

    const handleBackClick = () => {
        setSelectedGroup(null);
    };

    const selectedGroupData = groupsData.find(group => group.id === selectedGroup);

    // Si hay un grupo seleccionado, mostrar la tabla de empleados
    if (selectedGroup && selectedGroupData) {
        return (
            <div className="px-8 py-4">
                {/* Navegación de mes para la tabla */}
                <div className="flex justify-center items-center mb-4">
                    <button onClick={() => onNavigateMonth('prev')} className="p-2 hover:bg-gray-100 rounded-md transition-colors cursor-pointer">
                        <ChevronLeft className="w-5 h-5 text-gray-600" />
                    </button>
                    <span className="text-lg font-semibold text-gray-900 mx-6">
                        {monthNames[currentDate.getMonth()]} {currentDate.getFullYear()}
                    </span>
                    <button onClick={() => onNavigateMonth('next')} className="p-2 hover:bg-gray-100 rounded-md transition-colors cursor-pointer">
                        <ChevronRight className="w-5 h-5 text-gray-600" />
                    </button>
                </div>

                {/* Botón de regreso */}
                <div className="mb-4">
                    <button
                        onClick={handleBackClick}
                        className="flex items-center px-4 py-2 bg-blue-100 hover:bg-blue-200 rounded-md text-sm font-medium text-blue-700 transition-colors cursor-pointer"
                    >
                        <ArrowLeft className="w-4 h-4 mr-2" />
                        Regresar a tabla de porcentajes
                    </button>
                </div>

                <div className="bg-white rounded-lg overflow-hidden shadow-sm border border-gray-200">
                    <table className="w-full">
                        <thead>
                            <tr>
                                <th className="px-4 py-3 text-left text-sm font-semibold text-continental-black border-r border-gray-300 bg-continental-yellow">
                                    {selectedGroupData.name}
                                </th>
                                {getWeekDays().map((date, index) => (
                                    <th key={index} className="px-4 py-3 text-center text-sm font-semibold text-gray-700 border-r border-gray-300 last:border-r-0" style={{ backgroundColor: '#F7D5D7' }}>
                                        {format(date, 'EEEE dd-MM-yyyy', { locale: es })}
                                    </th>
                                ))}
                            </tr>
                        </thead>
                        <tbody className="bg-white">
                            {selectedGroupData.employees.map((employee, employeeIndex) => (
                                <tr key={employee.id} className={employeeIndex % 2 === 0 ? 'bg-white' : 'bg-gray-50'}>
                                    <td className="px-4 py-3 text-left text-sm border-r border-gray-300 bg-gray-50">
                                        <div>
                                            <div className="font-medium text-gray-900">{employee.id}</div>
                                            <div className="text-xs text-gray-600">{employee.name}</div>
                                        </div>
                                    </td>
                                    {employee.weekData.map((value, dayIndex) => (
                                        <td key={dayIndex} className="px-4 py-3 text-center text-sm border-r border-gray-300 last:border-r-0 text-gray-900 font-medium">
                                            {value}
                                        </td>
                                    ))}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>

                {/* Navegación por semanas */}
                <div className="flex justify-between items-center mt-4">
                    <button
                        onClick={() => onNavigateWeek('prev')}
                        className="px-4 py-2 bg-gray-100 hover:bg-gray-200 rounded-md text-sm font-medium text-gray-700 transition-colors cursor-pointer"
                    >
                        ← Semana anterior
                    </button>
                    <span className="text-sm font-medium text-gray-700">
                        {(() => {
                            const startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1 + weekOffset);
                            const endDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 7 + weekOffset);
                            return `${startDate.getDate()}-${String(startDate.getMonth() + 1).padStart(2, '0')} al ${endDate.getDate()}-${String(endDate.getMonth() + 1).padStart(2, '0')} ${endDate.getFullYear()}`;
                        })()}
                    </span>
                    <button
                        onClick={() => onNavigateWeek('next')}
                        className="px-4 py-2 bg-gray-100 hover:bg-gray-200 rounded-md text-sm font-medium text-gray-700 transition-colors cursor-pointer"
                    >
                        Semana siguiente →
                    </button>
                </div>
            </div>
        );
    }

    // Tabla original de porcentajes con grupos clickeables
    return (
        <div className="px-8 py-4">
            {/* Navegación de mes para la tabla */}
            <div className="flex justify-center items-center mb-4">
                <button onClick={() => onNavigateMonth('prev')} className="p-2 hover:bg-gray-100 rounded-md transition-colors cursor-pointer">
                    <ChevronLeft className="w-5 h-5 text-gray-600" />
                </button>
                <span className="text-lg font-semibold text-gray-900 mx-6">
                    {monthNames[currentDate.getMonth()]} {currentDate.getFullYear()}
                </span>
                <button onClick={() => onNavigateMonth('next')} className="p-2 hover:bg-gray-100 rounded-md transition-colors cursor-pointer">
                    <ChevronRight className="w-5 h-5 text-gray-600" />
                </button>
            </div>

            <div className="bg-white rounded-lg overflow-hidden shadow-sm border border-gray-200">
                <table className="w-full">
                    <thead className="bg-gray-100">
                        <tr>
                            <th className="px-4 py-3 text-left text-sm font-semibold text-gray-700 border-r border-gray-300">
                                Grupos
                            </th>
                            {(() => {
                                const startOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
                                const dates = [];
                                for (let i = 0; i < 7; i++) {
                                    const date = new Date(startOfMonth);
                                    date.setDate(startOfMonth.getDate() + weekOffset + i);
                                    dates.push(date);
                                }
                                return dates.map((date, index) => (
                                    <th key={index} className="px-4 py-3 text-center text-sm font-semibold text-gray-700 border-r border-gray-300 last:border-r-0">
                                        {date.getDate()}-{String(date.getMonth() + 1).padStart(2, '0')}-{date.getFullYear()}
                                    </th>
                                ));
                            })()}
                        </tr>
                    </thead>
                    <tbody className="bg-white">
                        {selectedGroups.length > 0 ? (
                            selectedGroups.map((groupId, groupIndex) => {
                                const g = currentAreaGroups?.find(ag => ag.grupoId.toString() === groupId);
                                //const label = g ? `${g.rol.split('_')[0]} ${(currentAreaGroups || []).findIndex(ag => ag.grupoId.toString() === groupId) + 1}` : `Grupo ${groupId}`;
                                const label = g ? `${g.rol}` : `Grupo ${groupId}`;

                                return (
                                    <tr
                                        key={groupId}
                                        className={`cursor-pointer hover:bg-blue-50 transition-colors ${groupIndex % 2 === 0 ? 'bg-white' : 'bg-gray-50'}`}
                                        onClick={() => handleGroupClick(groupId)}
                                    >
                                        <td className="px-4 py-3 text-left text-sm font-medium text-gray-900 border-r border-gray-300 bg-gray-50 hover:bg-blue-100">
                                            {label}
                                        </td>
                                        {(() => {
                                            const startOfMonth = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1);
                                            const weekData = [];
                                            for (let i = 0; i < 7; i++) {
                                                const date = new Date(startOfMonth);
                                                date.setDate(startOfMonth.getDate() + weekOffset + i);
                                                
                                                // Usar datos reales de ausencias en lugar de mock
                                                const percentage = getRealPercentage(date, parseInt(groupId));

                                                const isAlert = percentage > 4.5;
                                                weekData.push({ percentage, isAlert });
                                            }

                                            return weekData.map((data, dayIndex) => (
                                                <td key={dayIndex} className={`px-4 py-3 text-center text-sm border-r border-gray-300 last:border-r-0 ${data.isAlert
                                                    ? 'bg-red-500 text-white font-semibold'
                                                    : 'text-gray-900'
                                                    }`}>
                                                    {data.percentage.toFixed(1)}%
                                                </td>
                                            ));
                                        })()}
                                    </tr>
                                );
                            })
                        ) : (
                            <tr>
                                <td colSpan={8} className="px-4 py-8 text-center text-sm text-gray-500">
                                    Selecciona uno o más grupos para ver los datos en la tabla
                                </td>
                            </tr>
                        )}
                    </tbody>
                </table>
            </div>

            {/* Navegación por semanas */}
            <div className="flex justify-between items-center mt-4">
                <button
                    onClick={() => onNavigateWeek('prev')}
                    className="px-4 py-2 bg-gray-100 hover:bg-gray-200 rounded-md text-sm font-medium text-gray-700 transition-colors cursor-pointer"
                >
                    ← Semana anterior
                </button>
                <span className="text-sm font-medium text-gray-700">
                    {(() => {
                        const startDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 1 + weekOffset);
                        const endDate = new Date(currentDate.getFullYear(), currentDate.getMonth(), 7 + weekOffset);
                        return `${startDate.getDate()}-${String(startDate.getMonth() + 1).padStart(2, '0')} al ${endDate.getDate()}-${String(endDate.getMonth() + 1).padStart(2, '0')} ${endDate.getFullYear()}`;
                    })()}
                </span>
                <button
                    onClick={() => onNavigateWeek('next')}
                    className="px-4 py-2 bg-gray-100 hover:bg-gray-200 rounded-md text-sm font-medium text-gray-700 transition-colors cursor-pointer"
                >
                    Semana siguiente →
                </button>
            </div>
        </div>
    );
};

export default TableView;
