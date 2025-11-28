import httpClient from './httpClient';
import type { CategoriaFalla, Area, ApiResponse } from '@/interfaces';

export interface TipoVehiculoItem {
  id: number;
  nombre: string;
}

export interface EstadoItem {
  id: number;
  nombre: string;
}

export interface PrioridadItem {
  id: number;
  nombre: string;
}

export interface RolItem {
  id: number;
  nombre: string;
}

export interface TecnicoItem {
  id: number;
  nombre: string;
  tipo: string;
  especialidad?: string;
  activo: boolean;
}

export const catalogosService = {
  // Tipos de Vehículo
  async getTiposVehiculo(): Promise<ApiResponse<TipoVehiculoItem[]>> {
    return await httpClient.get<TipoVehiculoItem[]>('/catalogos/tipos-vehiculo');
  },

  // Estados de Vehículo
  async getEstadosVehiculo(): Promise<ApiResponse<EstadoItem[]>> {
    return await httpClient.get<EstadoItem[]>('/catalogos/estados-vehiculo');
  },

  // Estados de Orden de Trabajo
  async getEstadosOrden(): Promise<ApiResponse<EstadoItem[]>> {
    return await httpClient.get<EstadoItem[]>('/catalogos/estados-orden');
  },

  // Prioridades
  async getPrioridades(): Promise<ApiResponse<PrioridadItem[]>> {
    return await httpClient.get<PrioridadItem[]>('/catalogos/prioridades');
  },

  // Categorías de Falla
  async getCategoriasFalla(): Promise<ApiResponse<CategoriaFalla[]>> {
    return await httpClient.get<CategoriaFalla[]>('/catalogos/categorias-falla');
  },

  // Áreas
  async getAreas(): Promise<ApiResponse<Area[]>> {
    return await httpClient.get<Area[]>('/catalogos/areas');
  },

  async getAreaById(id: number): Promise<ApiResponse<Area>> {
    return await httpClient.get<Area>(`/catalogos/areas/${id}`);
  },

  async createArea(area: { nombre: string; codigo?: string; descripcion?: string; supervisorId?: number }): Promise<ApiResponse<Area>> {
    return await httpClient.post<Area>('/catalogos/areas', area);
  },

  async updateArea(id: number, area: { nombre?: string; codigo?: string; descripcion?: string; supervisorId?: number; activa?: boolean }): Promise<ApiResponse<Area>> {
    return await httpClient.put<Area>(`/catalogos/areas/${id}`, area);
  },

  // Roles
  async getRoles(): Promise<ApiResponse<RolItem[]>> {
    return await httpClient.get<RolItem[]>('/catalogos/roles');
  },

  // Técnicos disponibles
  async getTecnicos(soloActivos: boolean = true): Promise<ApiResponse<TecnicoItem[]>> {
    const endpoint = soloActivos
      ? '/catalogos/tecnicos?soloActivos=true'
      : '/catalogos/tecnicos';
    return await httpClient.get<TecnicoItem[]>(endpoint);
  },

  // Tipos de Mantenimiento
  async getTiposMantenimiento(): Promise<ApiResponse<string[]>> {
    return await httpClient.get<string[]>('/catalogos/tipos-mantenimiento');
  }
};

export default catalogosService;
