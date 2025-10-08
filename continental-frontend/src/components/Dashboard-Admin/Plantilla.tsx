import { useState, useMemo, useEffect, useCallback } from 'react';
import { Button } from '@/components/ui/button';
import { DataTable, type Column, type PaginationConfig } from '@/components/ui/data-table';
import { FilterBar, type FilterConfig } from '@/components/ui/filter-bar';
import { ContentContainer } from '@/components/ui/content-container';
import { useNavigate } from 'react-router-dom';
import { useEmpleadosSindicalizados } from '@/hooks/useEmpleadosSindicalizados';
import { useAreas } from '@/hooks/useAreas';
import { useLeaderCache } from '@/hooks/useLeaderCache';
import Fuse from 'fuse.js';
import { Loader2 } from 'lucide-react';

// Interface for transformed employee data
interface TransformedEmployee extends Record<string, unknown> {
  id: string;
  noNomina: string;
  nombre: string;
  area: string;
  grupo: string;
  antiguedad: string;
}

// Componente principal Plantilla
export const Plantilla = () => {
  const navigate = useNavigate();
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedArea, setSelectedArea] = useState('all');
  const [selectedGroup, setSelectedGroup] = useState('all');
  const [sortColumn, setSortColumn] = useState<string | null>(null);
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('asc');
  
  // Hook para manejar empleados sindicalizados
  const {
    empleados,
    loading,
    error,
    currentPage,
    pageSize,
    totalUsers,
    totalPages,
    refetch,
    setPage,
    setPageSize,
    setFilters
  } = useEmpleadosSindicalizados();

  // Hook para manejar áreas (reutilizando la implementación existente)
  const { areas, getAreaById } = useAreas();
  
  // State for groups from selected area
  interface GroupOption {
    value: string;
    label: string;
    liderId?: number;
  }
  const [availableGroups, setAvailableGroups] = useState<GroupOption[]>([]);
  
  // Hook para cache de líderes
  const { getLeadersBatch, formatLeaderName } = useLeaderCache();
  
  // Load groups when area is selected
  const loadGroupsForArea = useCallback(async (areaId: string) => {
    if (areaId === 'all' || !areaId) {
      setAvailableGroups([]);
      return;
    }
    
    try {
      const areaDetails = await getAreaById(parseInt(areaId));
      if (areaDetails.grupos && areaDetails.grupos.length > 0) {
        // Obtener IDs de líderes únicos
        const leaderIds = areaDetails.grupos
          .map(grupo => grupo.liderId)
          .filter((id): id is number => id !== undefined && id !== null);
        
        // Cargar información de líderes en batch (optimizado)
        const leadersMap = leaderIds.length > 0 ? await getLeadersBatch(leaderIds) : new Map();
        
        // Crear opciones de grupo con nombres de líderes
        const groupOptions = areaDetails.grupos.map((grupo) => {
          const groupCode = transformGroupRole(grupo.rol);
          let displayLabel = groupCode;

          if (grupo.liderId && leadersMap.has(grupo.liderId)) {
            const leader = leadersMap.get(grupo.liderId)!;
            const leaderName = formatLeaderName(leader.fullName);
            displayLabel = `${groupCode} - ${leaderName}`;
          }

          return {
            value: grupo.grupoId.toString(),
            label: displayLabel,
            liderId: grupo.liderId
          };
        });
        setAvailableGroups(groupOptions);
      } else {
        setAvailableGroups([]);
      }
    } catch (error) {
      console.error('Error loading groups for area:', error);
      setAvailableGroups([]);
    }
  }, [getAreaById, getLeadersBatch, formatLeaderName]);

  // Effect to load groups when area changes
  useEffect(() => {
    loadGroupsForArea(selectedArea);
  }, [selectedArea, getAreaById, loadGroupsForArea]);

  // Calcular antigüedad desde FechaIngreso
  const calculateAntiguedad = (fechaIngreso: string): string => {
    const ingreso = new Date(fechaIngreso);
    const ahora = new Date();
    const años = ahora.getFullYear() - ingreso.getFullYear();
    const meses = ahora.getMonth() - ingreso.getMonth();
    
    if (años > 0) {
      return `${años} año${años > 1 ? 's' : ''}`;
    } else if (meses > 0) {
      return `${meses} mes${meses > 1 ? 'es' : ''}`;
    } else {
      return 'Menos de 1 mes';
    }
  };
  const transformGroupRole = (role: string) => {
    return role;
  };

  // Transformar datos de API a formato de tabla
  const transformedEmployeeData = useMemo(() => {
    let data = empleados && empleados?.map(empleado => ({
      id: empleado.id.toString(),
      noNomina: empleado.username,
      nombre: empleado.fullName,
      area: empleado.area?.nombreGeneral || 'Sin área',
      grupo: empleado.grupo?.rol ? transformGroupRole(empleado.grupo.rol) : 'Sin grupo',
      antiguedad: calculateAntiguedad(empleado.fechaIngreso),
    })) || [];

    // Aplicar ordenamiento si hay una columna seleccionada
    if (sortColumn && data.length > 0) {
      data = [...data].sort((a, b) => {
        const aValue = a[sortColumn as keyof typeof a];
        const bValue = b[sortColumn as keyof typeof b];
        
        // Manejar valores undefined o null
        if (aValue == null && bValue == null) return 0;
        if (aValue == null) return sortDirection === 'asc' ? 1 : -1;
        if (bValue == null) return sortDirection === 'asc' ? -1 : 1;
        
        let comparison = 0;
        
        // Lógica especial para la columna de antigüedad (ordenamiento numérico)
        if (sortColumn === 'antiguedad') {
          // Extraer números de las cadenas de antigüedad
          const extractNumber = (str: string): number => {
            const match = str.match(/(\d+)/);
            return match ? parseInt(match[1], 10) : 0;
          };
          
          const aNum = extractNumber(String(aValue));
          const bNum = extractNumber(String(bValue));
          
          comparison = aNum - bNum;
        } else {
          // Para otras columnas, usar comparación de strings
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


  // Configuración de Fuse.js para búsqueda fuzzy
  const fuseOptions = useMemo(() => ({
    keys: [
      {
        name: 'nombre',
        weight: 0.7
      },
      {
        name: 'noNomina',
        weight: 0.3
      }
    ],
    threshold: 0.4, // 0.0 = coincidencia exacta, 1.0 = coincide con cualquier cosa
    distance: 100,
    minMatchCharLength: 1,
    includeScore: true
  }), []);

  // Crear instancia de Fuse con datos transformados
  const fuse = useMemo(() => new Fuse(transformedEmployeeData, fuseOptions), [transformedEmployeeData, fuseOptions]);

  // Configuración de filtros
  const filterConfigs: FilterConfig[] = [
    {
      type: 'search',
      key: 'search',
      placeholder: 'Busca por nombre o nómina',
      value: searchTerm,
      onChange: setSearchTerm
    },
    {
      type: 'select',
      key: 'area',
      label: 'Área',
      placeholder: 'Seleccionar área',
      value: selectedArea,
      onChange: (value: string) => {
        setSelectedArea(value);
        setSelectedGroup('all'); // Reset grupo cuando cambia área
        const areaId = value === 'all' ? undefined : parseInt(value);
        setFilters(areaId, undefined);
      },
      options: [
        { value: 'all', label: 'Todas las áreas' },
        ...areas.map(area => ({ value: area.areaId.toString(), label: area.nombreGeneral }))
      ]
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
        setFilters(areaId, grupoId);
      },
      options: [
        { value: 'all', label: 'Todos los grupos' },
        ...availableGroups.map(group => ({ value: group.value, label: group.label }))
      ]
    }
  ];

  // Configuración de columnas de la tabla
  const columns: Column<TransformedEmployee>[] = [
    {
      key: 'noNomina',
      label: 'No nómina',
      sortable: true,
      render: (value) => <span className="font-bold">{String(value)}</span>
    },
    {
      key: 'nombre',
      label: 'Nombre',
      sortable: true
    },
    {
      key: 'area',
      label: 'Área',
      sortable: true
    },
    {
      key: 'grupo',
      label: 'Regla de grupo',
      sortable: true
    },
    {
      key: 'antiguedad',
      label: 'Antigüedad',
      sortable: true
    },
    {
      key: 'acciones',
      label: 'Acciones',
      sortable: false,
      render: (_, row) => (
        <Button
          variant="continental"
          size="sm"
          onClick={() => handleViewEmployee(row.id)}
        >
          Ver empleado
        </Button>
      )
    }
  ];

  // Filtrar datos basado en búsqueda local (los filtros de área y grupo se manejan en el backend)
  const filteredData = useMemo(() => {
    if (!searchTerm.trim()) {
      return transformedEmployeeData;
    }

    // Aplicar búsqueda fuzzy si hay término de búsqueda
    const fuseResults = fuse.search(searchTerm);
    return fuseResults.map(result => result.item);
  }, [transformedEmployeeData, searchTerm, fuse]);

  // Los datos ya vienen paginados del backend, pero aplicamos filtro de búsqueda local
  const displayData = filteredData;

  // Configuración de paginación
  const paginationConfig: PaginationConfig = {
    currentPage,
    totalPages,
    pageSize,
    totalItems: totalUsers,
    onPageChange: setPage,
    onPageSizeChange: setPageSize
  };

  // Handlers
  const handleUpdateData = async () => {
    await refetch();
  };

  const handleViewEmployee = (id: string) => {
    navigate(`/admin/plantilla/${id}`);
  };

  const handleSort = (column: string) => {
    if (sortColumn === column) {
      // Si ya está ordenando por esta columna, cambiar dirección
      setSortDirection(sortDirection === 'asc' ? 'desc' : 'asc');
    } else {
      // Nueva columna, empezar con ascendente
      setSortColumn(column);
      setSortDirection('asc');
    }
  };



  return (
    <div className="p-6 bg-white h-screen">
      <div className="max-w-7xl mx-auto w-full space-y-6">
        
        {/* Botón Actualizar datos */}
        <div className="flex justify-end">
          <Button
            variant="continental"
            className="h-[45px] w-[150px] rounded-lg"
            onClick={handleUpdateData}
          >
            Actualizar datos
          </Button>
        </div>

        {/* Contenedor con filtros y tabla */}
        <ContentContainer>
          {/* Filtros */}
          <FilterBar 
            filters={filterConfigs}
            gridCols={3}
          />

          {/* Loading State */}
          {loading && (
            <div className="flex justify-center items-center py-8">
              <Loader2 className="h-8 w-8 animate-spin text-blue-600" />
              <span className="ml-2 text-gray-600">Cargando empleados...</span>
            </div>
          )}

          {/* Error State */}
          {error && (
            <div className="bg-red-50 border border-red-200 rounded-md p-4 mb-4">
              <p className="text-red-800">Error: {error}</p>
              <Button 
                variant="outline" 
                size="sm" 
                onClick={refetch}
                className="mt-2"
              >
                Reintentar
              </Button>
            </div>
          )}

          {/* Tabla */}
          {!loading && !error && (
            <DataTable<TransformedEmployee>
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