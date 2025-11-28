import { Prioridad } from './Api.interface';

export interface ReporteFalla {
  id: number;
  folio: string;
  vehiculoId: number;
  vehiculoCodigo?: string;
  vehiculoTipo?: string;
  categoriaFallaId?: number;
  categoriaNombre?: string;
  reportadoPorId: number;
  reportadoPorNombre?: string;
  prioridad: Prioridad;
  prioridadNombre?: string;
  descripcion: string;
  ubicacion?: string;
  puedeOperar: boolean;
  fechaReporte: string;
  tieneOrdenTrabajo: boolean;
  ordenTrabajoId?: number;
  evidencias: Evidencia[];
}

export interface ReporteFallaList {
  id: number;
  folio: string;
  vehiculoCodigo: string;
  vehiculoTipo?: string;
  categoriaNombre?: string;
  prioridad: Prioridad;
  prioridadNombre?: string;
  fechaReporte: string;
  tieneOrdenTrabajo: boolean;
  reportadoPorNombre?: string;
  cantidadEvidencias: number;
}

export interface ReporteFallaCreateRequest {
  codigoVehiculo: string;
  categoriaFallaId?: number;
  prioridad?: Prioridad;
  descripcion: string;
  ubicacion?: string;
  puedeOperar?: boolean;
}

export interface Evidencia {
  id: number;
  urlImagen: string;
  nombreArchivo?: string;
  descripcion?: string;
  tipoEvidencia?: string;
  fechaCaptura: string;
}

export interface CategoriaFalla {
  id: number;
  nombre: string;
  descripcion?: string;
  icono?: string;
  activa: boolean;
}
