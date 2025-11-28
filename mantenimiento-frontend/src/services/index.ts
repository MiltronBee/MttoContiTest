export { httpClient } from './httpClient';
export { authService } from './authService';
export { vehiculosService } from './vehiculosService';
export { reportesService } from './reportesService';
export { ordenesService } from './ordenesService';
export { dashboardService } from './dashboardService';
export { notificacionesService } from './notificacionesService';
export { catalogosService } from './catalogosService';
export { usuariosService } from './usuariosService';
export { pagosService } from './pagosService';
export { refaccionesService } from './refaccionesService';
export { checklistService } from './checklistService';

export type { VehiculoFilters, PaginatedResponse } from './vehiculosService';
export type { ReporteFilters } from './reportesService';
export type { OrdenFilters } from './ordenesService';
export type { DashboardFilters } from './dashboardService';
export type { UserFilters } from './usuariosService';
export type { PagoFilters, RegistroPago, CrearPagoRequest } from './pagosService';
export type { SolicitudRefaccion, SolicitudRefaccionCreateRequest } from './refaccionesService';
export type { ChecklistTemplate, ChecklistItem, ChecklistRespuesta } from './checklistService';
export type {
  TipoVehiculoItem,
  EstadoItem,
  PrioridadItem,
  RolItem,
  TecnicoItem
} from './catalogosService';
