import type { EstadoOrdenTrabajo, Prioridad } from './Api.interface';
import type { Evidencia } from './Reporte.interface';

export interface OrdenTrabajo {
  id: number;
  folio: string;
  reporteFallaId?: number;
  reporteFallaFolio?: string;
  vehiculoId: number;
  vehiculoCodigo?: string;
  vehiculoTipo?: string;
  tecnicoAsignadoId?: number;
  tecnicoNombre?: string;
  creadoPorId: number;
  creadoPorNombre?: string;
  estado: EstadoOrdenTrabajo;
  estadoNombre?: string;
  prioridad: Prioridad;
  prioridadNombre?: string;
  tipoMantenimiento: string;
  descripcion: string;
  diagnostico?: string;
  trabajoRealizado?: string;
  fechaCreacion: string;
  fechaAsignacion?: string;
  fechaInicio?: string;
  fechaFinalizacion?: string;
  fechaValidacion?: string;
  horasTrabajadas?: number;
  costoTotal?: number;
  validadoPorNombre?: string;
  notas?: string;
  evidencias: Evidencia[];
  respuestasChecklist: ChecklistRespuesta[];
  solicitudesRefaccion: SolicitudRefaccion[];
}

export interface OrdenTrabajoList {
  id: number;
  folio: string;
  vehiculoCodigo: string;
  vehiculoTipo?: string;
  tecnicoNombre?: string;
  estado: EstadoOrdenTrabajo;
  estadoNombre?: string;
  prioridad: Prioridad;
  prioridadNombre?: string;
  tipoMantenimiento: string;
  fechaCreacion: string;
  fechaFinalizacion?: string;
}

export interface OrdenTrabajoCreateRequest {
  reporteFallaId?: number;
  vehiculoId: number;
  tecnicoAsignadoId?: number;
  prioridad?: Prioridad;
  tipoMantenimiento?: string;
  descripcion: string;
  notas?: string;
}

export interface AsignarTecnicoRequest {
  tecnicoId: number;
}

export interface IniciarTrabajoRequest {
  diagnostico?: string;
}

export interface CompletarTrabajoRequest {
  trabajoRealizado: string;
  horasTrabajadas: number;
  notas?: string;
}

export interface ValidarOrdenRequest {
  observaciones?: string;
  aprobado?: boolean;
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

export interface SolicitudRefaccion {
  id: number;
  ordenTrabajoId: number;
  ordenTrabajoFolio?: string;
  nombreRefaccion: string;
  numeroParte?: string;
  cantidad: number;
  justificacion?: string;
  estado: string;
  costoEstimado?: number;
  costoReal?: number;
  solicitadoPorNombre?: string;
  aprobadoPorNombre?: string;
  fechaSolicitud: string;
  fechaAprobacion?: string;
  fechaEntrega?: string;
  motivoRechazo?: string;
}
