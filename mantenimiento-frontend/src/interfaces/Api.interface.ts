// Respuesta estándar de la API
export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  message?: string;
  errors?: string[];
}

// Tipos de Enums como constantes
export const TipoVehiculo = {
  Carrito: 1,
  Tugger: 2,
  Montacargas: 3
} as const;
export type TipoVehiculo = typeof TipoVehiculo[keyof typeof TipoVehiculo];

export const EstadoVehiculo = {
  Operativo: 1,
  EnReparacion: 2,
  FueraDeServicio: 3,
  EnEspera: 4
} as const;
export type EstadoVehiculo = typeof EstadoVehiculo[keyof typeof EstadoVehiculo];

export const EstadoOrdenTrabajo = {
  Pendiente: 1,
  Asignada: 2,
  EnProceso: 3,
  EsperandoRefacciones: 4,
  Completada: 5,
  Cancelada: 6,
  Validada: 7
} as const;
export type EstadoOrdenTrabajo = typeof EstadoOrdenTrabajo[keyof typeof EstadoOrdenTrabajo];

export const Prioridad = {
  Baja: 1,
  Media: 2,
  Alta: 3,
  Urgente: 4
} as const;
export type Prioridad = typeof Prioridad[keyof typeof Prioridad];

export const TipoTecnico = {
  Interno: 1,
  Externo: 2
} as const;
export type TipoTecnico = typeof TipoTecnico[keyof typeof TipoTecnico];

export const EstadoPago = {
  Pendiente: 1,
  EnRevision: 2,
  Aprobado: 3,
  Pagado: 4,
  Rechazado: 5
} as const;
export type EstadoPago = typeof EstadoPago[keyof typeof EstadoPago];

// Helper para obtener nombres de enums
export const TipoVehiculoNombres: Record<number, string> = {
  [TipoVehiculo.Carrito]: 'Carrito',
  [TipoVehiculo.Tugger]: 'Tugger',
  [TipoVehiculo.Montacargas]: 'Montacargas'
};

export const EstadoVehiculoNombres: Record<number, string> = {
  [EstadoVehiculo.Operativo]: 'Operativo',
  [EstadoVehiculo.EnReparacion]: 'En Reparación',
  [EstadoVehiculo.FueraDeServicio]: 'Fuera de Servicio',
  [EstadoVehiculo.EnEspera]: 'En Espera'
};

export const EstadoOrdenNombres: Record<number, string> = {
  [EstadoOrdenTrabajo.Pendiente]: 'Pendiente',
  [EstadoOrdenTrabajo.Asignada]: 'Asignada',
  [EstadoOrdenTrabajo.EnProceso]: 'En Proceso',
  [EstadoOrdenTrabajo.EsperandoRefacciones]: 'Esperando Refacciones',
  [EstadoOrdenTrabajo.Completada]: 'Completada',
  [EstadoOrdenTrabajo.Cancelada]: 'Cancelada',
  [EstadoOrdenTrabajo.Validada]: 'Validada'
};

export const PrioridadNombres: Record<number, string> = {
  [Prioridad.Baja]: 'Baja',
  [Prioridad.Media]: 'Media',
  [Prioridad.Alta]: 'Alta',
  [Prioridad.Urgente]: 'Urgente'
};
