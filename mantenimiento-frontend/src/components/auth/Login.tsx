import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext';
import { Button, Input, Alert, AlertDescription } from '@/components/ui';
import continentalLogo from '@/assets/continental_real.png';

export function Login() {
  const navigate = useNavigate();
  const { login, isLoading } = useAuth();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');

    if (!username || !password) {
      setError('Por favor ingresa tu número de empleado y contraseña');
      return;
    }

    const result = await login(username, password);

    if (result.success) {
      navigate('/dashboard');
    } else {
      setError(result.message);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-continental-bg p-4">
      <div className="w-full max-w-md">
        {/* Logo y título */}
        <div className="text-center mb-8">
          <div className="inline-flex items-center justify-center mb-4">
            <img
              src={continentalLogo}
              alt="Continental"
              className="h-16 w-auto"
            />
          </div>
          <h1 className="text-2xl font-bold text-continental-black">
            Sistema de Mantenimiento
          </h1>
          <p className="text-continental-gray-1 mt-2">
            Gestión de Equipos de Transporte
          </p>
        </div>

        {/* Formulario */}
        <div className="bg-white rounded-xl shadow-xl p-8 border-t-4 border-continental-yellow">
          <h2 className="text-xl font-semibold text-continental-black mb-6">
            Iniciar Sesión
          </h2>

          {error && (
            <Alert variant="error" className="mb-4" onClose={() => setError('')}>
              <AlertDescription>{error}</AlertDescription>
            </Alert>
          )}

          <form onSubmit={handleSubmit} className="space-y-4">
            <Input
              label="Número de Empleado"
              type="text"
              placeholder="Ingresa tu número de empleado"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              disabled={isLoading}
              autoComplete="username"
            />

            <Input
              label="Contraseña"
              type="password"
              placeholder="Ingresa tu contraseña"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              disabled={isLoading}
              autoComplete="current-password"
            />

            <Button
              type="submit"
              className="w-full bg-continental-yellow hover:bg-continental-yellow/90 text-continental-black font-semibold"
              isLoading={isLoading}
              disabled={isLoading}
            >
              Iniciar Sesión
            </Button>
          </form>

          <div className="mt-6 text-center">
            <a href="#" className="text-sm text-continental-blue-dark hover:underline">
              ¿Olvidaste tu contraseña?
            </a>
          </div>
        </div>

        {/* Footer */}
        <p className="text-center text-continental-gray-1 text-sm mt-6">
          Continental Tire - San Luis Potosí
        </p>
      </div>
    </div>
  );
}
