import { httpClient } from './httpClient'
import type { ApiResponse } from '../interfaces/Api.interface'
import { 
    type Solicitud, 
    type SolicitudesListResponse, 
    type CreateSolicitudRequest, 
    type UpdateSolicitudRequest,
    type SolicitudFilters,
    SolicitudStatus,
} from '../interfaces/Solicitudes.interface'

class SolicitudesService {
    private readonly baseUrl = '/api/reprogramacion'

    /**
     * Obtiene la lista de solicitudes de reprogramación con filtros
     */
    async getSolicitudesList(
        filters?: SolicitudFilters
    ): Promise<SolicitudesListResponse> {
        try {
            console.log('Fetching solicitudes list:', { filters })
            
            const params = new URLSearchParams()
            
            if (filters?.estado) params.append('estado', filters.estado)
            if (filters?.empleadoId) params.append('empleadoId', filters.empleadoId.toString())
            if (filters?.areaId) params.append('areaId', filters.areaId.toString())
            if (filters?.fechaDesde) params.append('fechaDesde', filters.fechaDesde)
            if (filters?.fechaHasta) params.append('fechaHasta', filters.fechaHasta)

            const url = `${this.baseUrl}/solicitudes${params.toString() ? `?${params.toString()}` : ''}`
            
            const response = await httpClient.get<ApiResponse<SolicitudesListResponse>>(url)
            
            console.log('Solicitudes API response:', response)
            
            if (response.success && response.data) {
                return response.data as unknown as SolicitudesListResponse
            }
            
            throw new Error('Invalid API response format')
        } catch (error) {
            console.error('Error fetching solicitudes list:', error)
            throw error
        }
    }

    /**
     * Obtiene una solicitud por ID
     */
    async getSolicitudById(id: number): Promise<Solicitud> {
        try {
            console.log('Fetching solicitud by ID:', id)
            
            const response = await httpClient.get<ApiResponse<Solicitud>>(
                `${this.baseUrl}/solicitud/${id}`
            )
            
            console.log('Solicitud detail API response:', response)
            
            if (response.success && response.data) {
                return response.data as unknown as Solicitud
            }
            
            throw new Error('Solicitud not found')
        } catch (error) {
            console.error(`Error fetching solicitud ${id}:`, error)
            throw error
        }
    }

    /**
     * Crea una nueva solicitud
     */
    async createSolicitud(solicitudData: CreateSolicitudRequest): Promise<Solicitud> {
        try {
            console.log('Creating solicitud:', solicitudData)
            
            const response = await httpClient.post<ApiResponse<Solicitud>>(
                this.baseUrl,
                solicitudData
            )
            
            if (response.success && response.data) {
                return response.data as unknown as Solicitud
            }
            
            throw new Error('Failed to create solicitud')
        } catch (error) {
            console.error('Error creating solicitud:', error)
            throw error
        }
    }

    /**
     * Actualiza el estado de una solicitud
     */
    async updateSolicitud(id: number, updateData: UpdateSolicitudRequest): Promise<Solicitud> {
        try {
            console.log('Updating solicitud:', { id, updateData })
            
            const response = await httpClient.put<ApiResponse<Solicitud>>(
                `${this.baseUrl}/${id}`,
                updateData
            )
            
            if (response.success && response.data) {
                return response.data as unknown as Solicitud
            }
            
            throw new Error('Failed to update solicitud')
        } catch (error) {
            console.error(`Error updating solicitud ${id}:`, error)
            throw error
        }
    }

    /**
     * Aprueba una solicitud
     */
    async aprobarSolicitud(id: number, comentarios?: string): Promise<Solicitud> {
        return this.updateSolicitud(id, {
            status: SolicitudStatus.Aprobada,
            comentarios
        })
    }

    /**
     * Rechaza una solicitud
     */
    async rechazarSolicitud(id: number, comentarios?: string): Promise<Solicitud> {
        return this.updateSolicitud(id, {
            status: SolicitudStatus.Rechazada,
            comentarios
        })
    }
}

export const solicitudesService = new SolicitudesService()
export default solicitudesService
