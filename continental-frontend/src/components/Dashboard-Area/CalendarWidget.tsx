/**
 * =============================================================================
 * CALENDAR WIDGET
 * =============================================================================
 * 
 * @description
 * Widget principal del calendario que integra todas las funcionalidades:
 * visualización de calendario/tabla, filtros, sidebar, navegación y gestión
 * de datos. Es el componente central que coordina toda la lógica del calendario.
 * 
 * @inputs (Props del componente)
 * - showTabs?: boolean - Mostrar pestañas de cambio entre calendar/table
 * - defaultView?: 'calendar' | 'table' - Vista inicial por defecto
 * - showHeader?: boolean - Mostrar header con filtros y controles
 * - showSidebar?: boolean - Mostrar sidebar con leyenda y configuraciones
 * - showManning?: boolean - Mostrar configuración de manning requerido
 * - areas?: { id: string; name: string }[] - Lista de áreas disponibles
 * - selectedArea?: string - Área actualmente seleccionada
 * - onAreaChange?: (areaId: string) => void - Callback cambio de área
 * - className?: string - Clases CSS adicionales
 * 
 * @used_in (Componentes padre)
 * - src/components/Dashboard-Area/CalendarComponent.tsx
 * - src/components/Dashboard-Area/SolicitudDetallePage.tsx
 * 
 * @user_roles (Usuarios que acceden)
 * - Jefe de Área
 * 
 * @dependencies
 * - React: Framework base y hooks (useState, useEffect, useMemo)
 * - lucide-react: Iconos (Calendar, Grid3X3)
 * - ../../interfaces/Calendar.interface: Tipos de datos del calendario
 * - ../../services/calendarService: Servicios de datos del calendario
 * - ./CalendarView: Vista del calendario
 * - ./TableView: Vista de tabla
 * - ./Header: Header con controles
 * - ./Sidebar: Sidebar con configuraciones
 * 
 * @author Vulcanics Dev Team
 * @created 2024
 * @last_modified 2025-08-20
 * =============================================================================
 */

import React, { useState, useEffect, useMemo, useRef } from 'react';
import { Calendar, Grid3X3 } from 'lucide-react';
import type { CalendarData, CalendarFilters } from '../../interfaces/Calendar.interface';
import type { AusenciasPorFecha, AusenciasFilters } from '../../interfaces/Ausencias.interface';
import { CalendarService } from '../../services/calendarService';
import { ausenciasService } from '../../services/ausenciasService';
import CalendarView from './CalendarView';
import TableView from './TableView';
import Header from './Header';
import Sidebar from './Sidebar';
import './Calendar.css';
import { userService } from '@/services/userService';

interface CalendarWidgetProps {
    showTabs?: boolean;
    defaultView?: 'calendar' | 'table';
    showHeader?: boolean;
    showSidebar?: boolean;
    showManning?: boolean;
    areas?: { id: string; name: string; manning?: number }[]; // Lista de áreas disponibles
    selectedArea?: string; // Área seleccionada
    onAreaChange?: (areaId: string) => void; // Callback cuando cambia el área
    currentAreaGroups?: any[]; // Grupos del área actual
    className?: string;
    bossName?: string; // nombre del jefe de área
}

const CalendarWidget: React.FC<CalendarWidgetProps> = ({
    showTabs = true,
    defaultView = 'calendar',
    showHeader = true,
    showSidebar = true,
    showManning = true,
    areas,
    selectedArea,
    onAreaChange,
    currentAreaGroups = [],
    className = '',
    bossName
}) => {
    const [currentDate, setCurrentDate] = useState(new Date());
    const [filters, setFilters] = useState<CalendarFilters>({
        selectedGroups: [],
        view: 'monthly'
    });
    const [calendarData, setCalendarData] = useState<CalendarData | null>(null);
    const [ausenciasData, setAusenciasData] = useState<AusenciasPorFecha[]>([]);
    // Carga interna del calendario; no bloquea la UI para evitar flasheo
    const [_, setLoading] = useState(false);
    const [manningRequerido, setManningRequerido] = useState<number>(48);
    const [activeTab, setActiveTab] = useState<'tabla' | 'calendario'>(defaultView === 'table' ? 'tabla' : 'calendario');
    const [weekOffset, setWeekOffset] = useState<number>(0);
    // Mapa grupoId -> nombre de líder
    const leadersCacheRef = useRef<Map<number, string>>(new Map());
    const [leadersByGroup, setLeadersByGroup] = useState<Record<number, string>>({});

    // Obtener manning base del área seleccionada
    const getManningBase = (): number => {
        if (!selectedArea || !areas) return manningRequerido;
        const selectedAreaData = areas.find(area => area.id === selectedArea);
        return selectedAreaData?.manning || manningRequerido;
    };

    // Sincronizar SIEMPRE todos los grupos del área actual como seleccionados
    // Cuando cambia el área (y por ende sus grupos), marcamos todos por defecto.
    useEffect(() => {
        if (currentAreaGroups && currentAreaGroups.length > 0) {
            const realGroupIds = currentAreaGroups.map((g: any) => g.grupoId?.toString());
            setFilters(prev => ({
                ...prev,
                selectedGroups: realGroupIds
            }));
        } else {
            // Si no hay grupos, limpiar selección
            setFilters(prev => ({ ...prev, selectedGroups: [] }));
        }
    }, [currentAreaGroups]);

    // Generar estructura básica del calendario (solo días del mes)
    useEffect(() => {
        const generateBasicCalendarData = () => {
            setLoading(true);
            try {
                const daysInMonth = new Date(currentDate.getFullYear(), currentDate.getMonth() + 1, 0).getDate();
                const days = [];

                for (let day = 1; day <= daysInMonth; day++) {
                    days.push({
                        date: new Date(currentDate.getFullYear(), currentDate.getMonth(), day),
                        day,
                        manning: [], // Ya no necesitamos datos mock aquí
                        averagePercentage: 0, // Los datos reales vienen de ausenciasData
                        status: 'normal' as const
                    });
                }

                const basicCalendarData = {
                    month: currentDate.getMonth(),
                    year: currentDate.getFullYear(),
                    days,
                    groups: [] // Ya no necesitamos grupos mock
                };

                setCalendarData(basicCalendarData);
            } catch (error) {
                console.error('Error generating calendar structure:', error);
            } finally {
                setLoading(false);
            }
        };

        generateBasicCalendarData();
    }, [currentDate, selectedArea]);

    // Cargar datos reales de ausencias cuando cambien fecha, vista o grupos
    useEffect(() => {
        const loadAusenciasData = async () => {
            if (!currentAreaGroups || currentAreaGroups.length === 0) {
                setAusenciasData([]);
                return;
            }

            try {
                // Para vista diaria, usar la fecha específica del día seleccionado
                // Para vistas mensual/semanal, usar currentDate como base
                let fechaParaAusencias = currentDate;
                if (filters.view === 'daily') {
                    // En vista diaria, currentDate ya debería ser la fecha específica seleccionada
                    fechaParaAusencias = currentDate;
                }

                const ausenciasFilters: AusenciasFilters = {
                    fechaInicio: fechaParaAusencias,
                    grupoIds: currentAreaGroups.map((g: any) => g.grupoId).filter(id => typeof id === 'number'),
                    areaId: selectedArea ? parseInt(selectedArea) : undefined,
                    view: filters.view
                };

                console.log('🔍 Loading ausencias for date:', {
                    originalDate: currentDate.toISOString(),
                    fechaParaAusencias: fechaParaAusencias.toISOString(),
                    view: filters.view,
                    day: fechaParaAusencias.getDate(),
                    month: fechaParaAusencias.getMonth() + 1,
                    year: fechaParaAusencias.getFullYear()
                });

                const data = await ausenciasService.calcularAusenciasParaCalendario(ausenciasFilters);
                setAusenciasData(data);
                
                } catch (error) {
                console.error('❌ Error loading ausencias data:', error);
                setAusenciasData([]);
            }
        };

        loadAusenciasData();
    }, [currentDate, filters.view, selectedArea, JSON.stringify((currentAreaGroups || []).map((g: any) => g.grupoId))]);

    // Cargar nombres de líderes por grupo (usa caché local para evitar llamadas repetidas)
    useEffect(() => {
        const loadLeaders = async () => {
            try {
                const groups = (currentAreaGroups || []) as Array<{ grupoId: number; liderId?: number | null }>;
                const mapping: Record<number, string> = {};

                // Obtener ids únicos de líderes existentes
                const uniqueLeaderIds = Array.from(new Set(groups
                    .map(g => g.liderId)
                    .filter((id): id is number => typeof id === 'number' && id > 0)));

                // Preparar promesas solo para líderes no cacheados
                const fetchPromises = uniqueLeaderIds
                    .filter(lid => !leadersCacheRef.current.has(lid))
                    .map(async lid => {
                        try {
                            const user = await userService.getUserById(lid);
                            if (user?.fullName) {
                                leadersCacheRef.current.set(lid, user.fullName);
                            }
                        } catch {
                            // Silenciar errores por líder individual
                        }
                    });

                if (fetchPromises.length > 0) {
                    await Promise.allSettled(fetchPromises);
                }

                // Construir mapping grupoId -> nombre del líder desde el caché
                for (const g of groups) {
                    if (g.liderId && leadersCacheRef.current.has(g.liderId)) {
                        mapping[g.grupoId] = leadersCacheRef.current.get(g.liderId)!;
                    }
                }

                setLeadersByGroup(mapping);
            } catch {
                setLeadersByGroup({});
            }
        };

        loadLeaders();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [JSON.stringify((currentAreaGroups || []).map((g: any) => ({ gid: g.grupoId, lid: g.liderId })))]);

    const monthNames = [
        'Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio',
        'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'
    ];

    const dayNames = ['Dom', 'Lun', 'Mar', 'Mié', 'Jue', 'Vie', 'Sáb'];

    const navigateMonth = (direction: 'prev' | 'next') => {
        setCurrentDate(prev => {
            const newDate = new Date(prev);
            if (direction === 'prev') {
                newDate.setMonth(prev.getMonth() - 1);
            } else {
                newDate.setMonth(prev.getMonth() + 1);
            }
            return newDate;
        });
        setWeekOffset(0);
    };

    const toggleGroup = (groupId: string) => {
        setFilters(prev => ({
            ...prev,
            selectedGroups: prev.selectedGroups.includes(groupId)
                ? prev.selectedGroups.filter(id => id !== groupId)
                : [...prev.selectedGroups, groupId]
        }));
    };

    const handleExport = () => {
        if (calendarData) {
            CalendarService.exportToCSV(calendarData);
        }
    };

    const changeView = (view: 'monthly' | 'weekly' | 'daily') => {
        setFilters(prev => ({ ...prev, view }));
    };

    const navigateToDate = (date: Date) => {
        setCurrentDate(date);
    };

    const navigateWeek = (direction: 'prev' | 'next') => {
        setWeekOffset(prev => direction === 'prev' ? prev - 7 : prev + 7);
    };

    const handleManningChange = async (newManning: number) => {
        setManningRequerido(newManning);
        console.log(`Manning requerido actualizado a: ${newManning}`);
    };

    // Función para calcular el porcentaje total del día actual
    const calculateTotalPercentage = useMemo(() => {
        if (filters.view !== 'daily' || filters.selectedGroups.length === 0) {
            return 0;
        }

        // Usar datos reales de ausencias si están disponibles
        if (ausenciasData && ausenciasData.length > 0) {
            const todayStr = currentDate.toISOString().split('T')[0];
            const todayData = ausenciasData.find(d => d.fecha === todayStr);
            
            if (todayData) {
                const selectedGroupIds = filters.selectedGroups.map(id => parseInt(id)).filter(id => !isNaN(id));
                const gruposData = todayData.ausenciasPorGrupo.filter(g => 
                    selectedGroupIds.includes(g.grupoId)
                );
                
                if (gruposData.length > 0) {
                    const avg = gruposData.reduce((sum, g) => sum + g.porcentajeAusencia, 0) / gruposData.length;
                    return Math.round(avg * 10) / 10;
                }
            }
        }

        // Fallback a datos del calendario
        if (!calendarData) return 0;

        const today = currentDate.getDate();
        const dayData = calendarData.days.find(d => d.day === today);
        if (!dayData) return 0;

        const selectedManning = dayData.manning.filter(m => filters.selectedGroups.includes(m.group));
        if (selectedManning.length === 0) return dayData.averagePercentage || 0;

        const avg = selectedManning.reduce((sum, m) => sum + m.percentage, 0) / selectedManning.length;
        return Math.round(avg * 10) / 10;
    }, [ausenciasData, calendarData, filters.selectedGroups, currentDate, filters.view]);

    const getActiveViewName = () => {
        switch (filters.view) {
            case 'monthly': return 'Mensual';
            case 'weekly': return 'Semanal';
            case 'daily': return 'Día';
            default: return 'Mensual';
        }
    };

    const monthlyStats = useMemo(() => {
        if (!ausenciasData || ausenciasData.length === 0 || filters.selectedGroups.length === 0) {
            // Fallback a datos del calendario si no hay datos de ausencias
            if (!calendarData || filters.selectedGroups.length === 0) return null;

            const dailyAverages = calendarData.days.map(day => {
                const selectedManning = day.manning.filter(m => filters.selectedGroups.includes(m.group));
                if (selectedManning.length === 0) return day.averagePercentage || 0;
                return selectedManning.reduce((sum, m) => sum + m.percentage, 0) / selectedManning.length;
            });

            const averagePercentage = dailyAverages.reduce((sum, avg) => sum + avg, 0) / (dailyAverages.length || 1);

            return {
                averagePercentage: Math.round(averagePercentage * 10) / 10,
                totalDays: calendarData.days.length,
                selectedGroupsCount: filters.selectedGroups.length
            };
        }

        // Usar datos reales de ausencias
        const selectedGroupIds = filters.selectedGroups.map(id => parseInt(id)).filter(id => !isNaN(id));
        const dailyAverages: number[] = [];

        ausenciasData.forEach(fechaData => {
            const gruposData = fechaData.ausenciasPorGrupo.filter(g => 
                selectedGroupIds.includes(g.grupoId)
            );
            
            if (gruposData.length > 0) {
                const avgForDay = gruposData.reduce((sum, g) => sum + g.porcentajeAusencia, 0) / gruposData.length;
                dailyAverages.push(avgForDay);
            }
        });

        if (dailyAverages.length === 0) return null;

        const averagePercentage = dailyAverages.reduce((sum, avg) => sum + avg, 0) / dailyAverages.length;

        return {
            averagePercentage: Math.round(averagePercentage * 10) / 10,
            totalDays: ausenciasData.length,
            selectedGroupsCount: filters.selectedGroups.length
        };
    }, [ausenciasData, calendarData, filters.selectedGroups]);

    // Nota: mantenemos el contenido visible durante la carga para evitar "flasheo".

    return (
        <div className={`calendar-container ${className}`}>
            {showHeader && (
                <Header
                    selectedGroups={filters.selectedGroups}
                    onToggleGroup={toggleGroup}
                    onExport={handleExport}
                    areas={areas}
                    currentAreaGroups={currentAreaGroups}
                    selectedArea={selectedArea}
                    onAreaChange={onAreaChange}
                    bossName={bossName}
                    leadersByGroup={leadersByGroup}
                />
            )}

            <div className="p-8">
                <div className="flex gap-8">
                    <div className="flex-1 bg-white">
                        {showTabs && (
                            <div className="mb-0">
                                <div className="bg-continental-gray-3 p-1 rounded-md w-full">
                                    <div className="flex w-full">
                                        <button
                                            onClick={() => setActiveTab('tabla')}
                                            className={`flex items-center justify-center gap-2 px-4 py-2 rounded-md transition-colors w-1/2 cursor-pointer ${activeTab === 'tabla'
                                                ? 'bg-white text-continental-black shadow-sm'
                                                : 'bg-transparent text-continental-gray-1 hover:text-continental-black'
                                                }`}
                                        >
                                            <Grid3X3 size={16} />
                                            <span>Tabla</span>
                                        </button>
                                        <button
                                            onClick={() => setActiveTab('calendario')}
                                            className={`flex items-center justify-center gap-2 px-4 py-2 rounded-md transition-colors w-1/2 cursor-pointer ${activeTab === 'calendario'
                                                ? 'bg-white text-continental-black shadow-sm'
                                                : 'bg-transparent text-continental-gray-1 hover:text-continental-black'
                                                }`}
                                        >
                                            <Calendar size={16} />
                                            <span>Calendario</span>
                                        </button>
                                    </div>
                                </div>
                            </div>
                        )}
                        
                        {(activeTab === 'calendario' || (!showTabs && defaultView === 'calendar')) && (
                            <CalendarView
                                calendarData={calendarData}
                                currentDate={currentDate}
                                filters={filters}
                                monthNames={monthNames}
                                dayNames={dayNames}
                                monthlyStats={monthlyStats}
                                onNavigateMonth={navigateMonth}
                                onChangeView={changeView}
                                onNavigateToDate={navigateToDate}
                                currentAreaGroups={currentAreaGroups}
                                ausenciasData={ausenciasData}
                            />
                        )}
                        
                        {(activeTab === 'tabla' || (!showTabs && defaultView === 'table')) && (
                            <TableView
                                calendarData={calendarData}
                                currentDate={currentDate}
                                selectedGroups={filters.selectedGroups}
                                weekOffset={weekOffset}
                                monthNames={monthNames}
                                onNavigateMonth={navigateMonth}
                                onNavigateWeek={navigateWeek}
                                currentAreaGroups={currentAreaGroups}
                                ausenciasData={ausenciasData}
                                manningRequerido={manningRequerido}
                            />
                        )}
                    </div>

                    {showSidebar && (
                        <Sidebar
                            manningRequerido={manningRequerido}
                            onManningChange={handleManningChange}
                            activeView={getActiveViewName()}
                            currentDate={currentDate}
                            selectedGroups={filters.selectedGroups}
                            totalPercentage={calculateTotalPercentage}
                            showManning={showManning}
                            currentAreaGroups={currentAreaGroups}
                            areaId={selectedArea ? parseInt(selectedArea) : undefined}
                            areaNombre={currentAreaGroups.length > 0 ? currentAreaGroups[0]?.areaNombre : undefined}
                            manningBase={getManningBase()}
                            areas={areas}
                        />
                    )}
                </div>
            </div>
        </div>
    );
};

export default CalendarWidget;
