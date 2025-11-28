import httpClient from './httpClient';
import type { ApiResponse } from '@/interfaces';

export interface ChecklistTemplate {
  id: number;
  nombre: string;
  descripcion?: string;
  tipoVehiculo?: number;
  tipoVehiculoNombre?: string;
  tipoMantenimiento?: string;
  activo: boolean;
  items: ChecklistItem[];
}

export interface ChecklistItem {
  id: number;
  orden: number;
  pregunta: string;
  tipoRespuesta: number;
  tipoRespuestaNombre?: string;
  opciones?: string;
  obligatorio: boolean;
  requiereFoto: boolean;
}

export interface ChecklistRespuesta {
  id: number;
  checklistItemId: number;
  pregunta?: string;
  valor?: string;
  fotoUrl?: string;
  notas?: string;
  fechaRespuesta: string;
}

export interface RespuestaItemRequest {
  checklistItemId: number;
  valor?: string;
  fotoUrl?: string;
  notas?: string;
}

export interface GuardarRespuestasRequest {
  ordenTrabajoId: number;
  respuestas: RespuestaItemRequest[];
}

export interface InspeccionRapidaRequest {
  vehiculoId: number;
  checklistTemplateId: number;
  respuestas: RespuestaItemRequest[];
}

export const checklistService = {
  // ===== TEMPLATES =====

  async getTemplates(tipoVehiculo?: number, tipoMantenimiento?: string): Promise<ApiResponse<ChecklistTemplate[]>> {
    const params = new URLSearchParams();
    if (tipoVehiculo !== undefined) params.append('tipoVehiculo', tipoVehiculo.toString());
    if (tipoMantenimiento) params.append('tipoMantenimiento', tipoMantenimiento);

    const queryString = params.toString();
    return await httpClient.get<ChecklistTemplate[]>(`/checklists/templates${queryString ? `?${queryString}` : ''}`);
  },

  async getTemplateById(id: number): Promise<ApiResponse<ChecklistTemplate>> {
    return await httpClient.get<ChecklistTemplate>(`/checklists/templates/${id}`);
  },

  async createTemplate(request: {
    nombre: string;
    descripcion?: string;
    tipoVehiculo?: number;
    tipoMantenimiento?: string;
    items: Array<{
      orden: number;
      pregunta: string;
      tipoRespuesta: number;
      opciones?: string;
      obligatorio?: boolean;
      requiereFoto?: boolean;
    }>;
  }): Promise<ApiResponse<{ id: number; nombre: string }>> {
    return await httpClient.post<{ id: number; nombre: string }>('/checklists/templates', request);
  },

  // ===== RESPUESTAS =====

  async getRespuestasByOrden(ordenTrabajoId: number): Promise<ApiResponse<ChecklistRespuesta[]>> {
    return await httpClient.get<ChecklistRespuesta[]>(`/checklists/respuestas/${ordenTrabajoId}`);
  },

  async guardarRespuestas(request: GuardarRespuestasRequest): Promise<ApiResponse<string>> {
    return await httpClient.post<string>('/checklists/respuestas', request);
  },

  async crearInspeccionRapida(request: InspeccionRapidaRequest): Promise<ApiResponse<{ ordenId: number }>> {
    return await httpClient.post<{ ordenId: number }>('/checklists/inspeccion-rapida', request);
  },

  // ===== CHECKLIST DEFAULT PARA INSPECCIÓN =====

  /**
   * Obtiene un checklist de inspección por defecto (usado cuando no hay templates)
   */
  getDefaultInspectionChecklist(): ChecklistItem[] {
    return [
      { id: 1, orden: 1, pregunta: 'Estado de neumáticos', tipoRespuesta: 0, obligatorio: true, requiereFoto: false },
      { id: 2, orden: 2, pregunta: 'Nivel de fluidos (aceite, hidráulico, frenos)', tipoRespuesta: 0, obligatorio: true, requiereFoto: false },
      { id: 3, orden: 3, pregunta: 'Luces y señales', tipoRespuesta: 0, obligatorio: true, requiereFoto: false },
      { id: 4, orden: 4, pregunta: 'Frenos operativos', tipoRespuesta: 0, obligatorio: true, requiereFoto: false },
      { id: 5, orden: 5, pregunta: 'Sistema hidráulico (sin fugas)', tipoRespuesta: 0, obligatorio: true, requiereFoto: false },
      { id: 6, orden: 6, pregunta: 'Batería y conexiones', tipoRespuesta: 0, obligatorio: true, requiereFoto: false },
      { id: 7, orden: 7, pregunta: 'Bocina/Alarma de reversa', tipoRespuesta: 0, obligatorio: true, requiereFoto: false },
      { id: 8, orden: 8, pregunta: 'Cinturón de seguridad', tipoRespuesta: 0, obligatorio: true, requiereFoto: false },
    ];
  }
};

export default checklistService;
