/**
 * =============================================================================
 * PLANTILLA (Área)
 * =============================================================================
 * 
 * @description
 * Vista de plantilla para Jefe de Área con datos reales de la API.
 * Permite buscar, ordenar y filtrar por área y grupo (limitado a sus áreas),
 * además de navegar al detalle del empleado.
 * 
 * @used_in (Componentes padre)
 * - src/components/Dashboard-Area/AreaDashboard.tsx
 * 
 * @user_roles (Usuarios que acceden)
 * - Jefe de Área
 * 
 * @dependencies
 * - React: Framework base y hooks (useState, useMemo)
 * - @/components/ui/button: Componente de botón reutilizable
 * - @/components/ui/data-table: Tabla de datos con paginación
 * - @/components/ui/filter-bar: Barra de filtros
 * - @/components/ui/content-container: Contenedor de contenido
 * - react-router-dom: Navegación (useNavigate)
 * - @/interfaces/Sindicalizado: Interfaz de empleado sindicalizado
 * - fuse.js: Librería para búsqueda fuzzy
 * 
 * @author Vulcanics Dev Team
 * @created 2025
 * @last_modified 2025-09-10
 * =============================================================================
 */

import { useEffect, useMemo, useRef, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { DataTable, type Column, type PaginationConfig } from '@/components/ui/data-table';
import { FilterBar, type FilterConfig } from '@/components/ui/filter-bar';
import { ContentContainer } from '@/components/ui/content-container';
import { useEmpleadosSimple } from '@/hooks/useEmpleadosSimple';
import { useAreas } from '@/hooks/useAreas';
import useAuth from '@/hooks/useAuth';
import { UserRole } from '@/interfaces/User.interface';
import type { UsuarioInfoDto } from '@/interfaces/Api.interface';
import { areasService } from '@/services/areasService';
import { debugEmpleados } from '@/utils/empleadosDebugger';
import Fuse from 'fuse.js';
import { Loader2 } from 'lucide-react';
import { userService } from '@/services/userService';

type TableRow = {
  id: number;
  noNomina: string;
  nombre: string;
  areaId?: number;
  area: string;
  grupo: string;
  antiguedad: string;
};

export const Plantilla = () => {
  const navigate = useNavigate();
  const { user, hasRole } = useAuth();

  // Debug navigation
  useEffect(() => {
    debugEmpleados.logComponentMount('Plantilla', { AreaId: user?.area?.areaId });
    return () => {
      debugEmpleados.logComponentUnmount('Plantilla');
    };
  }, [user?.area?.areaId]);

  const [searchTerm, setSearchTerm] = useState('');
  const [selectedArea, setSelectedArea] = useState<string>('all');
  const [selectedGroup, setSelectedGroup] = useState('all');
  const [sortColumn, setSortColumn] = useState<string | null>(null);
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');
  const [isRefreshing, setIsRefreshing] = useState(false);
  // Mantener estado de paginación local para "Todas las áreas"
  const [allAreasPage, setAllAreasPage] = useState(1);
  const [allAreasPageSize, setAllAreasPageSize] = useState(25);

  // Datos desde hooks (paginación y filtros en backend) - usando hook simple con caché integrado
  const {
    empleados,
    loading,
    error,
    currentPage,
    pageSize,
    totalUsers,
    totalPages,
    refetch,
    fetchAllAreas,
    setPage,
    setPageSize,
    setFilters,
    invalidateCache: invalidateCacheHook,
    clearAllCache: clearAllCacheHook
  } = useEmpleadosSimple({ AreaId: user?.area?.areaId });

  // Áreas disponibles (del catálogo general) y helper para detalles
  const { areas, getAreaById } = useAreas();

  // Áreas permitidas según rol del usuario (con grupos opcionales ya incluidos)
  const [allowedAreas, setAllowedAreas] = useState<Array<{ areaId: number; nombreGeneral: string; grupos?: Array<{ grupoId: number; rol: string }> }>>([]);

  // Efecto 1: Roles con endpoints (INDUSTRIAL / LEADER) -> depende solo de user.id
  useEffect(() => {
    let cancelled = false;
    const loadAllowed = async () => {
      if (!user?.id) return;
      try {
        // Ingeniero industrial
        if (hasRole(UserRole.INDUSTRIAL)) {
          const resp = await areasService.getAreasByIngeniero(user.id);
          const data = (resp.success && resp.data) ? resp.data : [];
          const mapped = data.map(a => ({ areaId: a.areaId, nombreGeneral: a.areaNombre }));
          if (!cancelled) setAllowedAreas(mapped);
          return;
        }
        // Líder de grupo
        if (hasRole(UserRole.LEADER)) {
          const resp = await areasService.getAreasByLider(user.id);
          const data = (resp.success && resp.data) ? resp.data : [];
          const mapped = data.map(a => ({ areaId: a.areaId, nombreGeneral: a.nombreGeneral, grupos: a.grupos as any }));
          if (!cancelled) setAllowedAreas(mapped);
          return;
        }
      } catch (e) {
        console.error('Error loading allowed areas (role endpoints):', e);
        if (!cancelled) setAllowedAreas([]);
      }
    };
    loadAllowed();
    return () => { cancelled = true; };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [user?.id]);

  // Efecto para cargar datos cuando las áreas permitidas cambian y estamos en "Todas las áreas"
  useEffect(() => {
    if (selectedArea === 'all' && allowedAreas.length > 0) {
      const areaIds = allowedAreas.map(area => area.areaId);
      const grupoId = selectedGroup === 'all' ? undefined : parseInt(selectedGroup);
      // Usar el estado local para evitar que se pierdan datos al cambiar de página
      fetchAllAreas(areaIds, { Page: allAreasPage, PageSize: allAreasPageSize, GrupoId: grupoId });
    }
  }, [allowedAreas, selectedArea, fetchAllAreas, selectedGroup, allAreasPage, allAreasPageSize]);

  // Efecto 2: Jefe de Área y otros (usa catálogo de áreas) -> depende de areas
  useEffect(() => {
    let cancelled = false;
    const loadFromAreas = () => {
      if (!user?.id) return;
      try {
        if (hasRole(UserRole.INDUSTRIAL) || hasRole(UserRole.LEADER)) {
          // Estos roles se resuelven en el efecto 1; evitar duplicados
          return;
        }
        if (hasRole(UserRole.AREA_ADMIN)) {
          const mine = (areas || [])
            .filter(a => a.jefeId === user.id || a.jefeSuplenteId === user.id)
            .map(a => ({ areaId: a.areaId, nombreGeneral: a.nombreGeneral, grupos: a.grupos as any }));
          if (!cancelled) setAllowedAreas(mine);
          return;
        }
        // Otros roles: todas las áreas
        const all = (areas || []).map(a => ({ areaId: a.areaId, nombreGeneral: a.nombreGeneral, grupos: a.grupos as any }));
        if (!cancelled) setAllowedAreas(all);
      } catch (e) {
        console.error('Error loading allowed areas (from areas):', e);
        if (!cancelled) setAllowedAreas([]);
      }
    };
    loadFromAreas();
    return () => { cancelled = true; };
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [areas, user?.id]);

  const areaOptions = useMemo(() => {
    if (allowedAreas.length > 0) {
      return [
        { value: 'all', label: 'Todas las áreas' },
        ...allowedAreas.map(a => ({ value: String(a.areaId), label: a.nombreGeneral })),
      ];
    }
    if (user?.area?.areaId) {
      return [{ value: String(user.area.areaId), label: user.area.nombreGeneral }];
    }
    return [] as { value: string; label: string }[];
  }, [allowedAreas, user?.area?.areaId, user?.area?.nombreGeneral]);

  // Grupos disponibles de la área seleccionada (id + rol)
  const [availableGroups, setAvailableGroups] = useState<Array<{ id: number; label: string }>>([]);
  // Cache local de líderes para evitar llamadas repetidas
  const leadersCacheRef = useRef<Map<number, string>>(new Map());

  const loadGroupsForArea = async (areaId: string) => {
    if (areaId === 'all' || !areaId) {
      setAvailableGroups([]);
      return;
    }
    try {
      const allowed = allowedAreas.find(a => a.areaId === parseInt(areaId));
      // Determinar fuente de grupos
      let groupsSource: Array<{ grupoId: number; rol: string; liderId?: number | null }> = [];

      if (allowed?.grupos && allowed.grupos.length > 0) {
        groupsSource = allowed.grupos as any;
      } else {
        const areaDetails = await getAreaById(parseInt(areaId));
        if (areaDetails.grupos && areaDetails.grupos.length > 0) {
          groupsSource = areaDetails.grupos as any;
        }
      }

      if (!groupsSource || groupsSource.length === 0) {
        setAvailableGroups([]);
        return;
      }

      // Cargar nombres de líderes faltantes a caché
      const uniqueLeaderIds = Array.from(new Set(
        groupsSource
          .map(g => g.liderId)
          .filter((id): id is number => typeof id === 'number' && id > 0)
      ));

      const missingIds = uniqueLeaderIds.filter(id => !leadersCacheRef.current.has(id));
      if (missingIds.length > 0) {
        await Promise.allSettled(missingIds.map(async (lid) => {
          try {
            const user = await userService.getUserById(lid);
            if (user?.fullName) leadersCacheRef.current.set(lid, user.fullName);
          } catch {
            // Silenciar errores individuales
          }
        }));
      }

      // Formatear etiqueta como en Header: "Rol N — Nombre"
      const formatted = groupsSource.map((g) => {
        //const base = `${g.rol.split('_')[0]} ${index + 1}`;
        const base = `${g.rol}`;
        const leaderFull = g.liderId ? leadersCacheRef.current.get(g.liderId) : undefined;
        const firstName = leaderFull ? leaderFull.split(' ')[0] : undefined;
        const label = firstName ? `${base} — ${firstName}` : base;
        return { id: g.grupoId, label };
      });

      setAvailableGroups(formatted);
    } catch (err) {
      console.error('Error loading groups for area:', err);
      setAvailableGroups([]);
    }
  };

  useEffect(() => {
    loadGroupsForArea(selectedArea);
    // Reset grupo al cambiar de área
    setSelectedGroup('all');
  }, [selectedArea]);

  // Funciones para manejar paginación en modo "Todas las áreas"
  const handlePageChange = (page: number) => {
    if (selectedArea === 'all') {
      // Actualizar estado local y pedir la página combinada
      setAllAreasPage(page);
      const areaIds = allowedAreas.map(area => area.areaId);
      const grupoId = selectedGroup === 'all' ? undefined : parseInt(selectedGroup);
      fetchAllAreas(areaIds, { Page: page, PageSize: allAreasPageSize, GrupoId: grupoId });
    } else {
      setPage(page);
    }
  };

  const handlePageSizeChange = (newPageSize: number) => {
    if (selectedArea === 'all') {
      setAllAreasPage(1);
      setAllAreasPageSize(newPageSize);
      const areaIds = allowedAreas.map(area => area.areaId);
      const grupoId = selectedGroup === 'all' ? undefined : parseInt(selectedGroup);
      fetchAllAreas(areaIds, { Page: 1, PageSize: newPageSize, GrupoId: grupoId });
    } else {
      setPageSize(newPageSize);
    }
  };

  // Utilidades de presentación
  const calculateAntiguedad = (fechaIngreso: string): string => {
    const ingreso = new Date(fechaIngreso);
    const ahora = new Date();
    const años = ahora.getFullYear() - ingreso.getFullYear();
    const meses = ahora.getMonth() - ingreso.getMonth();
    if (años > 0) return `${años} año${años > 1 ? 's' : ''}`;
    if (meses > 0) return `${meses} mes${meses > 1 ? 'es' : ''}`;
    return 'Menos de 1 mes';
  };
  const transformGroupRole = (role: string) => role.split('_')[0];

  // Transformar datos de API a filas de tabla y ordenar
  const transformedEmployeeData: TableRow[] = useMemo(() => {
    let data: TableRow[] = (empleados || []).map((empleado: UsuarioInfoDto) => ({
      id: empleado.id,
      noNomina: empleado.username,
      nombre: empleado.fullName,
  areaId: empleado.area?.areaId,
      area: empleado.area?.nombreGeneral,
      grupo: transformGroupRole(empleado.grupo?.rol || ''),
      antiguedad: calculateAntiguedad(empleado.fechaIngreso),
    }));

    if (sortColumn && data.length > 0) {
      data = [...data].sort((a, b) => {
        const aValue = a[sortColumn as keyof TableRow];
        const bValue = b[sortColumn as keyof TableRow];
        if (aValue == null && bValue == null) return 0;
        if (aValue == null) return sortDirection === 'asc' ? 1 : -1;
        if (bValue == null) return sortDirection === 'asc' ? -1 : 1;
        let comparison = 0;
        if (sortColumn === 'antiguedad') {
          const n = (s: string) => parseInt((s.match(/(\d+)/)?.[1] || '0'), 10);
          comparison = n(String(aValue)) - n(String(bValue));
        } else {
          const aStr = String(aValue).toLowerCase();
          const bStr = String(bValue).toLowerCase();
          if (aStr < bStr) comparison = -1;
          if (aStr > bStr) comparison = 1;
        }
        return sortDirection === 'asc' ? comparison : -comparison;
      });
    }
    return data;
  }, [empleados, sortColumn, sortDirection]);

  // Búsqueda fuzzy local
  const fuseOptions = {
    keys: [
      { name: 'nombre', weight: 0.7 },
      { name: 'noNomina', weight: 0.3 },
    ],
    threshold: 0.4,
    distance: 100,
    minMatchCharLength: 1,
    includeScore: true,
  };

  const filteredData = useMemo(() => {
    // Siempre usar los datos del hook optimizado
    const base = transformedEmployeeData;
    
    // Búsqueda fuzzy
    if (!searchTerm.trim()) return base;
    return new Fuse(base, fuseOptions).search(searchTerm).map(r => r.item);
  }, [transformedEmployeeData, searchTerm]);

  // Usar siempre los datos filtrados del hook (ya paginados por el backend)
  const displayData = filteredData;

  // Filtros UI
  const filterConfigs: FilterConfig[] = [
    {
      type: 'search',
      key: 'search',
      placeholder: 'Busca por nombre o nómina',
      value: searchTerm,
      onChange: setSearchTerm,
    },
    {
      type: 'select',
      key: 'area',
      label: 'Área',
      placeholder: 'Seleccionar área',
      value: selectedArea,
      onChange: (value: string) => {
        setSelectedArea(value);
        setSelectedGroup('all');
        debugEmpleados.logNavigationEvent('Plantilla', 'areaChange', { from: selectedArea, to: value });
        if (value === 'all') {
          // Reiniciar paginación local y cargar "Todas las áreas"
          setAllAreasPage(1);
          setAllAreasPageSize(pageSize);
          const areaIds = allowedAreas.map(area => area.areaId);
          fetchAllAreas(areaIds, { Page: 1, PageSize: pageSize });
          return;
        }
        const areaId = parseInt(value);
        setFilters(areaId, undefined);
      },
      options: areaOptions,
    },
    {
      type: 'select',
      key: 'group',
      label: 'Grupo',
      placeholder: 'Seleccionar grupo',
      value: selectedGroup,
      onChange: (value: string) => {
        setSelectedGroup(value);
        const areaId = selectedArea === 'all' ? undefined : parseInt(selectedArea);
        const grupoId = value === 'all' ? undefined : parseInt(value);
        debugEmpleados.logNavigationEvent('Plantilla', 'groupChange', { areaId, grupoId });
        
        if (selectedArea === 'all') {
          // Si estamos en "Todas las áreas", usar fetchAllAreas con filtro de grupo
          setAllAreasPage(1);
          const areaIds = allowedAreas.map(area => area.areaId);
          fetchAllAreas(areaIds, { Page: 1, PageSize: allAreasPageSize, GrupoId: grupoId });
        } else {
          setFilters(areaId, grupoId);
        }
      },
      options: [
        { value: 'all', label: 'Todos los grupos' },
        ...availableGroups.map(g => ({ value: String(g.id), label: g.label }))
      ],
    },
  ];

  // Columnas de tabla
  const columns: Column<any>[] = [
    { key: 'noNomina', label: 'No nómina', sortable: true, render: (v) => <span className="font-bold">{String(v)}</span> },
    { key: 'nombre', label: 'Nombre', sortable: true },
    { key: 'area', label: 'Área', sortable: true },
    { key: 'grupo', label: 'Grupo', sortable: true },
    { key: 'antiguedad', label: 'Antigüedad', sortable: true },
    {
      key: 'acciones',
      label: 'Acciones',
      sortable: false,
      render: (_: any, row: TableRow) => (
        <Button variant="continental" size="sm" onClick={() => handleViewEmployee(row.id)}>
          Ver empleado
        </Button>
      ),
    },
  ];

  // Paginación del backend (siempre usar la del hook optimizado)
  const paginationConfig: PaginationConfig = {
    currentPage: selectedArea === 'all' ? allAreasPage : currentPage,
    totalPages,
    pageSize: selectedArea === 'all' ? allAreasPageSize : pageSize,
    totalItems: totalUsers,
    onPageChange: handlePageChange,
    onPageSizeChange: handlePageSizeChange,
  };

  // Handlers
  const handleViewEmployee = (id: number) => {
    navigate(`/area/plantilla/${id}`);
  };

  const handleSort = (column: string) => {
    if (sortColumn === column) {
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      setSortColumn(column);
      setSortDirection('asc');
    }
  };

  const handleUpdateData = async () => {
    setIsRefreshing(true);
    try {
      // Si hay un área específica seleccionada, solo invalidar esa área
      if (selectedArea !== 'all') {
        const areaId = parseInt(selectedArea);
        invalidateCacheHook(areaId);
        await refetch();
      } else {
        // Si está en "todas las áreas", limpiar todo el caché y refrescar con fetchAllAreas
        clearAllCacheHook();
        const areaIds = allowedAreas.map(area => area.areaId);
        const grupoId = selectedGroup === 'all' ? undefined : parseInt(selectedGroup);
  await fetchAllAreas(areaIds, { Page: allAreasPage, PageSize: allAreasPageSize, GrupoId: grupoId });
      }
    } catch (error) {
      console.error('Error updating data:', error);
    } finally {
      setIsRefreshing(false);
    }
  };

  // Función de refetch personalizada que maneja tanto áreas específicas como "Todas las áreas"
  const handleRefetch = async () => {
    if (selectedArea === 'all') {
      const areaIds = allowedAreas.map(area => area.areaId);
      const grupoId = selectedGroup === 'all' ? undefined : parseInt(selectedGroup);
  await fetchAllAreas(areaIds, { Page: allAreasPage, PageSize: allAreasPageSize, GrupoId: grupoId });
    } else {
      await refetch();
    }
  };

  return (
    <div className="p-6 bg-white min-h-screen">
      <div className="max-w-7xl mx-auto w-full space-y-6">
        {/* Botón Actualizar datos: solo para SuperUsuario y Administrador */}
        {hasRole(UserRole.SUPER_ADMIN) || hasRole(UserRole.ADMIN) ? (
          <div className="flex justify-end">
            <Button
              variant="continental"
              className="h-[45px] w-[150px] rounded-lg"
              onClick={handleUpdateData}
              disabled={isRefreshing}
            >
              {isRefreshing ? (
                <>
                  <Loader2 className="h-4 w-4 animate-spin mr-2" />
                  Actualizando...
                </>
              ) : (
                'Actualizar datos'
              )}
            </Button>
          </div>
        ) : null}

        <ContentContainer>
          <FilterBar filters={filterConfigs} gridCols={3} />

          {loading && (
            <div className="flex justify-center items-center py-8">
              <Loader2 className="h-8 w-8 animate-spin text-blue-600" />
              <span className="ml-2 text-gray-600">Cargando empleados...</span>
            </div>
          )}

          {error && (
            <div className="bg-red-50 border border-red-200 rounded-md p-4 mb-4">
              <p className="text-red-800">Error: {error}</p>
              <Button variant="outline" size="sm" onClick={handleRefetch} className="mt-2">
                Reintentar
              </Button>
            </div>
          )}

          {!loading && !error && (
            <DataTable<any>
              columns={columns}
              data={displayData}
              keyField="noNomina"
              emptyMessage="No se encontraron empleados que coincidan con los filtros seleccionados."
              onSort={handleSort}
              pagination={paginationConfig}
            />
          )}
        </ContentContainer>
      </div>
    </div>
  );
};