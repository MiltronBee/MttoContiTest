import type { TipoVehiculo, EstadoOrdenTrabajo } from './Api.interface';
import type { OrdenTrabajoList } from './OrdenTrabajo.interface';

export interface DashboardStats {
  totalVehiculos: number;
  vehiculosOperativos: number;
  vehiculosEnReparacion: number;
  vehiculosFueraServicio: number;
  ordenesPendientes: number;
  ordenesEnProceso: number;
  ordenesCompletadasHoy: number;
  ordenesCompletadasSemana: number;
  reportesNuevosHoy: number;
  reportesSinAtender: number;
  pagosPendientes: number;
  montoPagosPendientes: number;
}

export interface KPIs {
  tiempoPromedioResolucion: number;
  porcentajeDisponibilidad: number;
  fallasPorTipo: FallasPorTipo[];
  ordenesPorEstado: OrdenesPorEstado[];
  costoTotalPeriodo: number;
  costoManoObraPeriodo: number;
  costoRefaccionesPeriodo: number;
}

export interface FallasPorTipo {
  tipoVehiculo: TipoVehiculo;
  tipoNombre?: string;
  cantidadFallas: number;
}

export interface OrdenesPorEstado {
  estado: EstadoOrdenTrabajo;
  estadoNombre?: string;
  cantidad: number;
}

export interface DashboardTecnico {
  ordenesAsignadas: number;
  ordenesEnProceso: number;
  ordenesCompletadasHoy: number;
  ordenesCompletadasSemana: number;
  ordenesActivas: OrdenTrabajoList[];
}

export interface Notificacion {
  id: number;
  tipo: number;
  tipoNombre?: string;
  titulo: string;
  mensaje: string;
  urlDestino?: string;
  referenciaId?: number;
  tipoReferencia?: string;
  leida: boolean;
  fechaLectura?: string;
  fechaCreacion: string;
}

export interface NotificacionesResumen {
  totalNoLeidas: number;
  notificacionesRecientes: Notificacion[];
}

export interface Area {
  id: number;
  nombre: string;
  codigo?: string;
  descripcion?: string;
  supervisorId?: number;
  supervisorNombre?: string;
  activa: boolean;
}
