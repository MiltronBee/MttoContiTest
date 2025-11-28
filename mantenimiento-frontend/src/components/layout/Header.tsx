import { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { Button } from '@/components/ui';
import { Bell, Menu, LogOut, User, ChevronDown } from 'lucide-react';
import { getInitials } from '@/lib/utils';
import continentalLogo from '@/assets/continental_real.png';

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
    <header className="fixed top-0 left-0 right-0 z-50 h-16 bg-continental-black border-b border-continental-yellow shadow-sm">
      <div className="flex items-center justify-between h-full px-4">
        {/* Left side */}
        <div className="flex items-center gap-4">
          <Button
            variant="ghost"
            size="icon"
            onClick={onToggleSidebar}
            className="lg:hidden text-continental-white hover:bg-continental-gray-1"
          >
            <Menu className="h-5 w-5" />
          </Button>

          <Link to="/dashboard" className="flex items-center gap-3">
            <img
              src={continentalLogo}
              alt="Continental"
              className="h-10 w-auto"
            />
            <div className="hidden sm:block border-l border-continental-yellow pl-3">
              <span className="font-semibold text-lg text-continental-white">
                Sistema de Mantenimiento
              </span>
            </div>
          </Link>
        </div>

        {/* Right side */}
        <div className="flex items-center gap-2">
          {/* Notificaciones */}
          <Link to="/notificaciones">
            <Button variant="ghost" size="icon" className="relative text-continental-white hover:bg-continental-gray-1">
              <Bell className="h-5 w-5" />
              <span className="absolute -top-1 -right-1 h-4 w-4 bg-continental-red text-white text-xs rounded-full flex items-center justify-center">
                3
              </span>
            </Button>
          </Link>

          {/* User Menu */}
          <div className="relative">
            <button
              onClick={() => setShowUserMenu(!showUserMenu)}
              className="flex items-center gap-2 p-2 rounded-md hover:bg-continental-gray-1 transition-colors"
            >
              <div className="w-8 h-8 bg-continental-yellow text-continental-black rounded-full flex items-center justify-center">
                <span className="text-sm font-bold">
                  {user ? getInitials(user.nombreCompleto) : 'U'}
                </span>
              </div>
              <div className="hidden md:block text-left">
                <p className="text-sm font-medium text-continental-white">
                  {user?.nombreCompleto}
                </p>
                <p className="text-xs text-continental-gray-2">{user?.rolNombre}</p>
              </div>
              <ChevronDown className="h-4 w-4 text-continental-gray-2" />
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
