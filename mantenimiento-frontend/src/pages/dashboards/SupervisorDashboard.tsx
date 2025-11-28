import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { Badge, Button, LoadingCard } from '@/components/ui';
import { dashboardService, ordenesService, reportesService, vehiculosService, usuariosService } from '@/services';
import type { DashboardStats, OrdenTrabajoList, ReporteFallaList } from '@/interfaces';
import { formatDate, formatNumber, getInitials } from '@/lib/utils';
import { cn } from '@/lib/utils';
import {
  Truck,
  AlertTriangle,
  Wrench,
  CheckCircle,
  Clock,
  Users,
  ArrowRight,
  Check,
  X,
  Info,
  CircleDot,
} from 'lucide-react';

interface TeamMember {
  id: number;
  nombre: string;
  rol: string;
  estado: 'Disponible' | 'Ocupado' | 'Fuera de Servicio';
  tareasActivas: number;
}

interface Equipment {
  id: number;
  codigo: string;
  tipo: string;
  estado: 'Operativo' | 'En Mantenimiento' | 'Fuera de Servicio';
  ubicacion: string;
  ultimoMantenimiento?: string;
  tecnicoAsignado?: string;
}

interface PendingApproval {
  id: number;
  tipo: 'refaccion' | 'reprogramacion' | 'tecnico_externo';
  titulo: string;
  tecnico: string;
  equipo: string;
  descripcion: string;
  costoEstimado?: string;
}

export function SupervisorDashboard() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [team, setTeam] = useState<TeamMember[]>([]);
  const [equipment, setEquipment] = useState<Equipment[]>([]);
  const [pendingApprovals, setPendingApprovals] = useState<PendingApproval[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    loadDashboardData();
  }, []);

  const loadDashboardData = async () => {
    setIsLoading(true);
    try {
      const [statsRes, vehiculosRes, usuariosRes] = await Promise.all([
        dashboardService.getStats(),
        vehiculosService.getAll({ pageSize: 10 }),
        usuariosService.getAll({ pageSize: 20 }),
      ]);

      if (statsRes.success && statsRes.data) {
        setStats(statsRes.data);
      }

      // Map vehicles to equipment
      if (vehiculosRes.data?.items) {
        setEquipment(vehiculosRes.data.items.map((v: any) => ({
          id: v.id,
          codigo: v.codigo,
          tipo: v.tipoNombre,
          estado: v.estadoNombre || 'Operativo',
          ubicacion: v.areaNombre || 'Sin asignar',
          ultimoMantenimiento: v.ultimoMantenimiento,
          tecnicoAsignado: v.tecnicoAsignado,
        })));
      }

      // Map users to team (filter for technicians)
      if (usuariosRes.data?.items) {
        const tecnicos = usuariosRes.data.items.filter((u: any) =>
          u.rolNombre === 'Tecnico' || u.rolNombre === 'Técnico'
        );
        setTeam(tecnicos.map((u: any) => ({
          id: u.id,
          nombre: u.nombreCompleto,
          rol: u.rolNombre,
          estado: 'Disponible', // Would come from real-time status
          tareasActivas: Math.floor(Math.random() * 5), // Demo data
        })));
      }

      // Demo pending approvals
      setPendingApprovals([
        {
          id: 1,
          tipo: 'refaccion',
          titulo: 'Solicitud de Refacción - Filtro Hidráulico',
          tecnico: 'Juan Pérez',
          equipo: 'Montacargas #MTC-045',
          descripcion: 'Filtro hidráulico dañado detectado durante inspección. Requerido para completar reparación.',
          costoEstimado: '$450 MXN',
        },
        {
          id: 2,
          tipo: 'reprogramacion',
          titulo: 'Cambio de Programación - Mantenimiento Preventivo',
          tecnico: 'Luis Hernández',
          equipo: 'Tugger #TGR-018',
          descripcion: 'Solicitud de posponer mantenimiento 1 día por alta demanda de producción.',
        },
        {
          id: 3,
          tipo: 'tecnico_externo',
          titulo: 'Solicitud de Técnico Externo - Reparación Especializada',
          tecnico: 'Carlos Ramírez',
          equipo: 'Montacargas #MTC-051',
          descripcion: 'Falla eléctrica compleja en sistema de control. Se requiere especialista.',
          costoEstimado: '$2,500 MXN + refacciones',
        },
      ]);

    } catch (error) {
      console.error('Error al cargar dashboard:', error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleApprove = (id: number) => {
    setPendingApprovals(prev => prev.filter(a => a.id !== id));
    // In real app, call API to approve
  };

  const handleReject = (id: number) => {
    setPendingApprovals(prev => prev.filter(a => a.id !== id));
    // In real app, call API to reject
  };

  if (isLoading) {
    return <LoadingCard message="Cargando dashboard..." />;
  }

  const operativos = equipment.filter(e => e.estado === 'Operativo').length;
  const enMantenimiento = equipment.filter(e => e.estado === 'En Mantenimiento').length;
  const fueraServicio = equipment.filter(e => e.estado === 'Fuera de Servicio').length;

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-continental-black">Portal de Supervisor</h1>
        <p className="text-continental-gray-1">Gestión de equipos y equipo técnico</p>
      </div>

      {/* Area Overview KPIs */}
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-5">
        <OverviewCard
          icon={<Truck className="h-8 w-8" />}
          value={equipment.length}
          label="Equipos en mi Área"
        />
        <OverviewCard
          icon={<CheckCircle className="h-8 w-8 text-continental-green" />}
          value={operativos}
          label="Operativos"
          variant="green"
        />
        <OverviewCard
          icon={<Wrench className="h-8 w-8 text-continental-yellow" />}
          value={enMantenimiento}
          label="En Mantenimiento"
          variant="yellow"
        />
        <OverviewCard
          icon={<Users className="h-8 w-8 text-continental-blue" />}
          value={team.length}
          label="Técnicos Activos"
          variant="blue"
        />
        <OverviewCard
          icon={<Clock className="h-8 w-8 text-continental-yellow" />}
          value={pendingApprovals.length}
          label="Pendientes Aprobación"
          variant="yellow"
        />
      </div>

      {/* Main Content Grid */}
      <div className="grid gap-6 lg:grid-cols-5">
        {/* Equipment Status - 3 columns */}
        <div className="lg:col-span-3 bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-lg font-semibold text-continental-black flex items-center gap-2">
              <Truck className="h-5 w-5" />
              Equipos en mi Área
            </h2>
            <Link
              to="/vehiculos"
              className="text-sm text-continental-yellow hover:text-continental-yellow-dark flex items-center gap-1"
            >
              Ver todos <ArrowRight className="h-4 w-4" />
            </Link>
          </div>

          <div className="space-y-3 max-h-[400px] overflow-y-auto pr-2">
            {equipment.map(equip => (
              <EquipmentCard key={equip.id} equipment={equip} />
            ))}
          </div>
        </div>

        {/* Team Status - 2 columns */}
        <div className="lg:col-span-2 bg-white rounded-xl shadow-lg p-6">
          <div className="flex items-center justify-between mb-6">
            <h2 className="text-lg font-semibold text-continental-black flex items-center gap-2">
              <Users className="h-5 w-5" />
              Mi Equipo de Técnicos
            </h2>
          </div>

          <div className="space-y-3 max-h-[400px] overflow-y-auto pr-2">
            {team.length === 0 ? (
              <p className="text-center text-continental-gray-1 py-8">
                No hay técnicos asignados
              </p>
            ) : (
              team.map(member => (
                <TeamMemberCard key={member.id} member={member} />
              ))
            )}
          </div>
        </div>
      </div>

      {/* Pending Approvals */}
      <div className="bg-white rounded-xl shadow-lg p-6">
        <h2 className="text-lg font-semibold text-continental-black flex items-center gap-2 mb-6">
          <AlertTriangle className="h-5 w-5 text-continental-yellow" />
          Solicitudes Pendientes de Aprobación
          {pendingApprovals.length > 0 && (
            <span className="ml-2 px-2 py-0.5 bg-continental-yellow text-continental-black text-sm font-bold rounded-full">
              {pendingApprovals.length}
            </span>
          )}
        </h2>

        {pendingApprovals.length === 0 ? (
          <div className="text-center py-8 text-continental-gray-1">
            <CheckCircle className="h-12 w-12 mx-auto mb-2 text-continental-green" />
            <p>No hay solicitudes pendientes</p>
          </div>
        ) : (
          <div className="space-y-4">
            {pendingApprovals.map(approval => (
              <ApprovalCard
                key={approval.id}
                approval={approval}
                onApprove={() => handleApprove(approval.id)}
                onReject={() => handleReject(approval.id)}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
}

function OverviewCard({
  icon,
  value,
  label,
  variant = 'default',
}: {
  icon: React.ReactNode;
  value: number;
  label: string;
  variant?: 'default' | 'green' | 'yellow' | 'blue' | 'red';
}) {
  return (
    <div className="bg-white rounded-xl p-6 shadow-lg text-center transition-all hover:-translate-y-1 hover:shadow-xl">
      <div className="flex justify-center mb-3">{icon}</div>
      <div className="text-3xl font-bold text-continental-black mb-1">{value}</div>
      <div className="text-sm text-continental-gray-1">{label}</div>
    </div>
  );
}

function EquipmentCard({ equipment }: { equipment: Equipment }) {
  const getStatusStyle = (estado: string) => {
    switch (estado) {
      case 'Operativo':
        return { border: 'border-l-continental-green', badge: 'bg-green-100 text-green-800' };
      case 'En Mantenimiento':
        return { border: 'border-l-continental-yellow', badge: 'bg-yellow-100 text-yellow-800' };
      default:
        return { border: 'border-l-continental-red', badge: 'bg-red-100 text-red-800' };
    }
  };

  const style = getStatusStyle(equipment.estado);

  return (
    <Link
      to={`/vehiculos/${equipment.id}`}
      className={cn(
        'block bg-continental-bg rounded-lg p-4 border-l-4 transition-all hover:bg-continental-gray-4 hover:translate-x-1',
        style.border
      )}
    >
      <div className="flex items-center justify-between mb-2">
        <span className="font-semibold text-continental-black">{equipment.codigo}</span>
        <span className={cn('px-3 py-1 rounded-full text-xs font-semibold', style.badge)}>
          {equipment.estado}
        </span>
      </div>
      <div className="text-sm text-continental-gray-1 space-y-1">
        <p>📍 {equipment.ubicacion}</p>
        {equipment.tecnicoAsignado && (
          <p>🔧 Técnico: {equipment.tecnicoAsignado}</p>
        )}
        {equipment.ultimoMantenimiento && (
          <p>⏱️ Último Mtto: {equipment.ultimoMantenimiento}</p>
        )}
      </div>
    </Link>
  );
}

function TeamMemberCard({ member }: { member: TeamMember }) {
  const getStatusColor = (estado: string) => {
    switch (estado) {
      case 'Disponible':
        return 'text-continental-green';
      case 'Ocupado':
        return 'text-continental-yellow';
      default:
        return 'text-continental-red';
    }
  };

  return (
    <div className="bg-continental-bg rounded-lg p-4 flex items-center justify-between">
      <div className="flex items-center gap-4">
        <div className="w-12 h-12 rounded-full bg-gradient-to-br from-continental-yellow to-continental-yellow-dark flex items-center justify-center text-white font-bold">
          {getInitials(member.nombre)}
        </div>
        <div>
          <h4 className="font-semibold text-continental-black">{member.nombre}</h4>
          <p className="text-sm text-continental-gray-1">{member.rol}</p>
          <p className={cn('text-sm font-medium flex items-center gap-1', getStatusColor(member.estado))}>
            <CircleDot className="h-3 w-3" />
            {member.estado}
          </p>
        </div>
      </div>
      <div className="text-right">
        <div className="text-2xl font-bold text-continental-black">{member.tareasActivas}</div>
        <div className="text-xs text-continental-gray-1">Tareas Activas</div>
      </div>
    </div>
  );
}

function ApprovalCard({
  approval,
  onApprove,
  onReject,
}: {
  approval: PendingApproval;
  onApprove: () => void;
  onReject: () => void;
}) {
  return (
    <div className="bg-continental-yellow/10 rounded-lg p-5 border-l-4 border-l-continental-yellow">
      <div className="flex items-start justify-between gap-4 mb-3">
        <h3 className="font-semibold text-continental-black">{approval.titulo}</h3>
      </div>
      <div className="text-sm text-continental-gray-1 space-y-1 mb-4">
        <p><strong>Técnico:</strong> {approval.tecnico}</p>
        <p><strong>Equipo:</strong> {approval.equipo}</p>
        <p><strong>Descripción:</strong> {approval.descripcion}</p>
        {approval.costoEstimado && (
          <p><strong>Costo estimado:</strong> {approval.costoEstimado}</p>
        )}
      </div>
      <div className="flex gap-3">
        <Button
          onClick={onApprove}
          size="sm"
          className="bg-continental-green hover:bg-continental-green/90"
        >
          <Check className="h-4 w-4 mr-1" />
          Aprobar
        </Button>
        <Button
          onClick={onReject}
          size="sm"
          variant="outline"
          className="border-continental-red text-continental-red hover:bg-continental-red/10"
        >
          <X className="h-4 w-4 mr-1" />
          Rechazar
        </Button>
        <Button variant="ghost" size="sm">
          <Info className="h-4 w-4 mr-1" />
          Más Info
        </Button>
      </div>
    </div>
  );
}
