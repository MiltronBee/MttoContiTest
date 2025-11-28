import httpClient from './httpClient';
import type {
  Notificacion,
  NotificacionesResumen,
  ApiResponse
} from '@/interfaces';

export const notificacionesService = {
  async getResumen(): Promise<ApiResponse<NotificacionesResumen>> {
    return await httpClient.get<NotificacionesResumen>('/notificaciones/resumen');
  },

  async getAll(soloNoLeidas: boolean = false): Promise<ApiResponse<Notificacion[]>> {
    const endpoint = soloNoLeidas
      ? '/notificaciones?soloNoLeidas=true'
      : '/notificaciones';
    return await httpClient.get<Notificacion[]>(endpoint);
  },

  async marcarLeida(id: number): Promise<ApiResponse<void>> {
    return await httpClient.post<void>(`/notificaciones/${id}/leer`);
  },

  async marcarTodasLeidas(): Promise<ApiResponse<void>> {
    return await httpClient.post<void>('/notificaciones/leer-todas');
  },

  async eliminar(id: number): Promise<ApiResponse<void>> {
    return await httpClient.delete<void>(`/notificaciones/${id}`);
  }
};

export default notificacionesService;
