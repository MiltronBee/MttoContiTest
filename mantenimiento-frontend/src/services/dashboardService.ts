import httpClient from './httpClient';
import type {
  DashboardStats,
  KPIs,
  DashboardTecnico,
  ApiResponse
} from '@/interfaces';

export interface DashboardFilters {
  fechaDesde?: string;
  fechaHasta?: string;
  areaId?: number;
}

export const dashboardService = {
  async getStats(): Promise<ApiResponse<DashboardStats>> {
    return await httpClient.get<DashboardStats>('/dashboard/stats');
  },

  async getKPIs(filters?: DashboardFilters): Promise<ApiResponse<KPIs>> {
    const params = new URLSearchParams();

    if (filters) {
      if (filters.fechaDesde) params.append('fechaDesde', filters.fechaDesde);
      if (filters.fechaHasta) params.append('fechaHasta', filters.fechaHasta);
      if (filters.areaId !== undefined) params.append('areaId', filters.areaId.toString());
    }

    const queryString = params.toString();
    const endpoint = `/dashboard/kpis${queryString ? `?${queryString}` : ''}`;

    return await httpClient.get<KPIs>(endpoint);
  },

  async getDashboardTecnico(): Promise<ApiResponse<DashboardTecnico>> {
    return await httpClient.get<DashboardTecnico>('/dashboard/tecnico');
  },

  async getResumenSemanal(areaId?: number): Promise<ApiResponse<{
    ordenesCreadas: number;
    ordenesCompletadas: number;
    tiempoPromedioResolucion: number;
    reportesNuevos: number;
    vehiculosAtendidos: number;
  }>> {
    const endpoint = areaId
      ? `/dashboard/resumen-semanal?areaId=${areaId}`
      : '/dashboard/resumen-semanal';
    return await httpClient.get(endpoint);
  },

  async getGraficoOrdenesPorDia(dias: number = 7): Promise<ApiResponse<{
    fecha: string;
    creadas: number;
    completadas: number;
  }[]>> {
    return await httpClient.get(`/dashboard/ordenes-por-dia?dias=${dias}`);
  },

  async getTopVehiculosConFallas(limit: number = 5): Promise<ApiResponse<{
    vehiculoId: number;
    vehiculoCodigo: string;
    tipoNombre: string;
    cantidadFallas: number;
  }[]>> {
    return await httpClient.get(`/dashboard/top-vehiculos-fallas?limit=${limit}`);
  },

  async getTiempoPromedioPorTipo(): Promise<ApiResponse<{
    tipoMantenimiento: string;
    tiempoPromedio: number;
    cantidadOrdenes: number;
  }[]>> {
    return await httpClient.get('/dashboard/tiempo-promedio-tipo');
  }
};

export default dashboardService;
