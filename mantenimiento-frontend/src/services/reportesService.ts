import httpClient from './httpClient';
import type {
  ReporteFalla,
  ReporteFallaList,
  ReporteFallaCreateRequest,
  CategoriaFalla,
  Evidencia,
  ApiResponse
} from '@/interfaces';
import type { PaginatedResponse } from './vehiculosService';

export interface ReporteFilters {
  vehiculoId?: number;
  categoriaId?: number;
  prioridad?: number;
  tieneOrdenTrabajo?: boolean;
  fechaDesde?: string;
  fechaHasta?: string;
  busqueda?: string;
  page?: number;
  pageSize?: number;
}

export const reportesService = {
  async getAll(filters?: ReporteFilters): Promise<ApiResponse<PaginatedResponse<ReporteFallaList>>> {
    const params = new URLSearchParams();

    if (filters) {
      if (filters.vehiculoId !== undefined) params.append('vehiculoId', filters.vehiculoId.toString());
      if (filters.categoriaId !== undefined) params.append('categoriaId', filters.categoriaId.toString());
      if (filters.prioridad !== undefined) params.append('prioridad', filters.prioridad.toString());
      if (filters.tieneOrdenTrabajo !== undefined) params.append('tieneOrdenTrabajo', filters.tieneOrdenTrabajo.toString());
      if (filters.fechaDesde) params.append('fechaDesde', filters.fechaDesde);
      if (filters.fechaHasta) params.append('fechaHasta', filters.fechaHasta);
      if (filters.busqueda) params.append('busqueda', filters.busqueda);
      if (filters.page !== undefined) params.append('page', filters.page.toString());
      if (filters.pageSize !== undefined) params.append('pageSize', filters.pageSize.toString());
    }

    const queryString = params.toString();
    const endpoint = `/reportes${queryString ? `?${queryString}` : ''}`;

    return await httpClient.get<PaginatedResponse<ReporteFallaList>>(endpoint);
  },

  async getById(id: number): Promise<ApiResponse<ReporteFalla>> {
    return await httpClient.get<ReporteFalla>(`/reportes/${id}`);
  },

  async getByVehiculoCodigo(codigo: string): Promise<ApiResponse<ReporteFallaList[]>> {
    return await httpClient.get<ReporteFallaList[]>(`/reportes/vehiculo/${encodeURIComponent(codigo)}`);
  },

  async getSinAtender(): Promise<ApiResponse<ReporteFallaList[]>> {
    return await httpClient.get<ReporteFallaList[]>('/reportes/sin-atender');
  },

  async create(reporte: ReporteFallaCreateRequest): Promise<ApiResponse<ReporteFalla>> {
    return await httpClient.post<ReporteFalla>('/reportes', reporte);
  },

  async crearOrdenTrabajo(reporteId: number): Promise<ApiResponse<{ ordenTrabajoId: number }>> {
    return await httpClient.post<{ ordenTrabajoId: number }>(`/reportes/${reporteId}/crear-orden`);
  },

  async uploadEvidencia(reporteId: number, file: File, descripcion?: string): Promise<ApiResponse<Evidencia>> {
    const formData = new FormData();
    formData.append('archivo', file);
    if (descripcion) {
      formData.append('descripcion', descripcion);
    }
    return await httpClient.uploadFile<Evidencia>(`/reportes/${reporteId}/evidencias`, formData);
  },

  async deleteEvidencia(reporteId: number, evidenciaId: number): Promise<ApiResponse<void>> {
    return await httpClient.delete<void>(`/reportes/${reporteId}/evidencias/${evidenciaId}`);
  },

  // Categorías de Fallas
  async getCategorias(): Promise<ApiResponse<CategoriaFalla[]>> {
    return await httpClient.get<CategoriaFalla[]>('/catalogos/categorias-falla');
  }
};

export default reportesService;
