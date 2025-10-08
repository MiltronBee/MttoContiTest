/**
 * Servicio para manejar solicitudes de reprogramación de vacaciones
 */

import { httpClient } from '@/services/httpClient';
import type {
  ApiResponse,
  HistorialReprogramacionResponse,
  SolicitudReprogramacion,
  SolicitarReprogramacionRequest,
  SolicitarReprogramacionResponse
} from '@/interfaces/Api.interface';

export class ReprogramacionService {
  /**
   * Obtiene el historial de solicitudes de reprogramación para un empleado
   * @param empleadoId - ID del empleado
   * @param anio - Año a consultar
   * @returns Historial de solicitudes
   */
  static async obtenerHistorial(
    empleadoId: number,
    anio: number
  ): Promise<HistorialReprogramacionResponse> {
    try {
      console.log('Obteniendo historial de reprogramación:', { empleadoId, anio });

      const response = await httpClient.get<ApiResponse<HistorialReprogramacionResponse>>(
        `/api/reprogramacion/historial/${empleadoId}?anioVacaciones=${anio}`,
        undefined,
        { timeout: 30000 }
      );

      if (!response.success || !response.data) {
        throw new Error(response.errorMsg || 'Error al obtener historial de solicitudes');
      }

      const result = response.data as unknown as HistorialReprogramacionResponse;
      console.log('Historial obtenido exitosamente:', result);
      return result;
    } catch (error) {
      console.error('Error en obtenerHistorial:', error);

      let errorMessage = 'Error al obtener el historial de solicitudes. Por favor intente nuevamente.';

      if (error instanceof Error) {
        if (error.message.includes('Network Error') || error.message.includes('Failed to fetch')) {
          errorMessage = 'Error de conexión. Verifique su conexión a internet e intente nuevamente.';
        } else if (error.message) {
          errorMessage = error.message;
        }
      }

      throw new Error(errorMessage);
    }
  }

  /**
   * Obtiene las últimas N solicitudes de un empleado
   * @param empleadoId - ID del empleado
   * @param anio - Año a consultar
   * @param limite - Número de solicitudes a obtener
   * @returns Array con las últimas solicitudes
   */
  static async obtenerUltimasSolicitudes(
    empleadoId: number,
    anio: number,
    limite: number = 3
  ): Promise<SolicitudReprogramacion[]> {
    try {
      const historial = await this.obtenerHistorial(empleadoId, anio);

      // Ordenar por fecha de solicitud (más recientes primero) y tomar las primeras N
      const solicitudesOrdenadas = historial.solicitudes
        .sort((a, b) => new Date(b.fechaSolicitud).getTime() - new Date(a.fechaSolicitud).getTime())
        .slice(0, limite);

      return solicitudesOrdenadas;
    } catch (error) {
      console.error('Error al obtener últimas solicitudes:', error);
      // Retornar array vacío en caso de error para no romper la UI
      return [];
    }
  }

  /**
   * Solicita una reprogramación de vacación
   * @param request - Datos de la solicitud
   * @returns Respuesta con detalles de la solicitud creada
   */
  static async solicitarReprogramacion(
    request: SolicitarReprogramacionRequest
  ): Promise<SolicitarReprogramacionResponse> {
    try {
      console.log('Solicitando reprogramación:', request);

      const response = await httpClient.post<ApiResponse<SolicitarReprogramacionResponse>>(
        '/api/reprogramacion/solicitar',
        request,
        { timeout: 30000 }
      );

      if (!response.success || !response.data) {
        // Manejar casos de error específicos
        const errorMsg = response.errorMsg || 'Error al procesar la solicitud';

        if (errorMsg.includes('Período no es')) {
          throw new Error('El período actual no permite reprogramaciones');
        } else if (errorMsg.includes('automática')) {
          throw new Error('Las vacaciones automáticas no pueden ser reprogramadas');
        } else if (errorMsg.includes('inhábil')) {
          throw new Error('La fecha seleccionada cae en un día inhábil');
        } else if (errorMsg.includes('Conflicto')) {
          throw new Error('La fecha seleccionada tiene conflicto con otra vacación');
        } else if (errorMsg.includes('permisos')) {
          throw new Error('No tienes permisos para realizar esta acción');
        }

        throw new Error(errorMsg);
      }

      const result = response.data as unknown as SolicitarReprogramacionResponse;
      console.log('Solicitud creada exitosamente:', result);
      return result;
    } catch (error: any) {
      console.error('Error en solicitarReprogramacion:', error);

      // Si el error ya tiene un mensaje personalizado, usarlo
      if (error.message) {
        throw error;
      }

      // Error genérico
      throw new Error('Error al solicitar la reprogramación. Por favor intente nuevamente.');
    }
  }
}