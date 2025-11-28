import { NavLink } from 'react-router-dom';
import { cn } from '@/lib/utils';
import { useAuth } from '@/contexts/AuthContext';
import {
  LayoutDashboard,
  Truck,
  AlertTriangle,
  Wrench,
  Users,
  DollarSign,
  Settings,
  FileText,
  Package,
} from 'lucide-react';

interface NavItem {
  label: string;
  href: string;
  icon: React.ReactNode;
  roles?: string[];
}

const navItems: NavItem[] = [
  {
    label: 'Dashboard',
    href: '/dashboard',
    icon: <LayoutDashboard className="h-5 w-5" />,
  },
  {
    label: 'Vehículos',
    href: '/vehiculos',
    icon: <Truck className="h-5 w-5" />,
    roles: ['Administrador', 'Supervisor'],
  },
  {
    label: 'Reportes de Falla',
    href: '/reportes',
    icon: <AlertTriangle className="h-5 w-5" />,
  },
  {
    label: 'Órdenes de Trabajo',
    href: '/ordenes',
    icon: <Wrench className="h-5 w-5" />,
  },
  {
    label: 'Mis Órdenes',
    href: '/mis-ordenes',
    icon: <FileText className="h-5 w-5" />,
    roles: ['Técnico'],
  },
  {
    label: 'Refacciones',
    href: '/refacciones',
    icon: <Package className="h-5 w-5" />,
    roles: ['Administrador', 'Supervisor'],
  },
  {
    label: 'Pagos',
    href: '/pagos',
    icon: <DollarSign className="h-5 w-5" />,
    roles: ['Administrador'],
  },
  {
    label: 'Usuarios',
    href: '/usuarios',
    icon: <Users className="h-5 w-5" />,
    roles: ['Administrador'],
  },
  {
    label: 'Configuración',
    href: '/configuracion',
    icon: <Settings className="h-5 w-5" />,
    roles: ['Administrador'],
  },
];

interface SidebarProps {
  isOpen: boolean;
}

export function Sidebar({ isOpen }: SidebarProps) {
  const { hasRole } = useAuth();

  const filteredNavItems = navItems.filter((item) => {
    if (!item.roles) return true;
    return hasRole(item.roles);
  });

  return (
    <aside
      className={cn(
        'fixed left-0 top-16 z-40 h-[calc(100vh-4rem)] w-64 bg-white border-r transition-transform duration-300 ease-in-out',
        isOpen ? 'translate-x-0' : '-translate-x-full',
        'lg:translate-x-0'
      )}
    >
      <nav className="flex flex-col gap-1 p-4">
        {filteredNavItems.map((item) => (
          <NavLink
            key={item.href}
            to={item.href}
            className={({ isActive }) =>
              cn(
                'flex items-center gap-3 px-3 py-2 rounded-md text-sm font-medium transition-colors',
                isActive
                  ? 'bg-primary text-primary-foreground'
                  : 'text-gray-700 hover:bg-gray-100'
              )
            }
          >
            {item.icon}
            {item.label}
          </NavLink>
        ))}
      </nav>
    </aside>
  );
}
