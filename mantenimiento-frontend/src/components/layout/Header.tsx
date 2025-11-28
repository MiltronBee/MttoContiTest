import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { Button } from '@/components/ui';
import { Bell, Menu, LogOut, User, ChevronDown } from 'lucide-react';
import { getInitials } from '@/lib/utils';

interface HeaderProps {
  onToggleSidebar: () => void;
}

export function Header({ onToggleSidebar }: HeaderProps) {
  const { user, logout } = useAuth();
  const [showUserMenu, setShowUserMenu] = useState(false);

  const handleLogout = async () => {
    await logout();
  };

  return (
    <header className="fixed top-0 left-0 right-0 z-50 h-16 bg-white border-b shadow-sm">
      <div className="flex items-center justify-between h-full px-4">
        {/* Left side */}
        <div className="flex items-center gap-4">
          <Button
            variant="ghost"
            size="icon"
            onClick={onToggleSidebar}
            className="lg:hidden"
          >
            <Menu className="h-5 w-5" />
          </Button>

          <Link to="/dashboard" className="flex items-center gap-2">
            <div className="w-8 h-8 bg-primary rounded-md flex items-center justify-center">
              <span className="text-white font-bold text-sm">CM</span>
            </div>
            <span className="font-semibold text-lg hidden sm:block">
              Continental Mantenimiento
            </span>
          </Link>
        </div>

        {/* Right side */}
        <div className="flex items-center gap-2">
          {/* Notificaciones */}
          <Link to="/notificaciones">
            <Button variant="ghost" size="icon" className="relative">
              <Bell className="h-5 w-5" />
              <span className="absolute -top-1 -right-1 h-4 w-4 bg-red-500 text-white text-xs rounded-full flex items-center justify-center">
                3
              </span>
            </Button>
          </Link>

          {/* User Menu */}
          <div className="relative">
            <button
              onClick={() => setShowUserMenu(!showUserMenu)}
              className="flex items-center gap-2 p-2 rounded-md hover:bg-gray-100 transition-colors"
            >
              <div className="w-8 h-8 bg-primary/10 text-primary rounded-full flex items-center justify-center">
                <span className="text-sm font-medium">
                  {user ? getInitials(user.nombreCompleto) : 'U'}
                </span>
              </div>
              <div className="hidden md:block text-left">
                <p className="text-sm font-medium text-gray-900">
                  {user?.nombreCompleto}
                </p>
                <p className="text-xs text-gray-500">{user?.rolNombre}</p>
              </div>
              <ChevronDown className="h-4 w-4 text-gray-500" />
            </button>

            {/* Dropdown Menu */}
            {showUserMenu && (
              <>
                <div
                  className="fixed inset-0 z-40"
                  onClick={() => setShowUserMenu(false)}
                />
                <div className="absolute right-0 mt-2 w-48 bg-white rounded-md shadow-lg border z-50">
                  <div className="p-2">
                    <Link
                      to="/perfil"
                      className="flex items-center gap-2 px-3 py-2 text-sm text-gray-700 rounded-md hover:bg-gray-100"
                      onClick={() => setShowUserMenu(false)}
                    >
                      <User className="h-4 w-4" />
                      Mi Perfil
                    </Link>
                    <button
                      onClick={handleLogout}
                      className="flex items-center gap-2 w-full px-3 py-2 text-sm text-red-600 rounded-md hover:bg-red-50"
                    >
                      <LogOut className="h-4 w-4" />
                      Cerrar Sesión
                    </button>
                  </div>
                </div>
              </>
            )}
          </div>
        </div>
      </div>
    </header>
  );
}
