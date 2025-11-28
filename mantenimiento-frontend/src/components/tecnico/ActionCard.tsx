import { cn } from '@/lib/utils';

interface ActionCardProps {
  icon: React.ReactNode;
  title: string;
  description: string;
  onClick: () => void;
  variant?: 'default' | 'yellow' | 'blue' | 'green';
}

const variantStyles = {
  default: 'hover:border-continental-yellow hover:shadow-continental-yellow/20',
  yellow: 'hover:border-continental-yellow hover:shadow-continental-yellow/20',
  blue: 'hover:border-continental-blue hover:shadow-continental-blue/20',
  green: 'hover:border-continental-green hover:shadow-continental-green/20',
};

export function ActionCard({ icon, title, description, onClick, variant = 'default' }: ActionCardProps) {
  return (
    <button
      onClick={onClick}
      className={cn(
        'bg-white rounded-xl p-8 text-center cursor-pointer transition-all duration-300',
        'shadow-lg border-2 border-transparent',
        'hover:-translate-y-1 hover:shadow-xl',
        variantStyles[variant]
      )}
    >
      <div className="text-5xl mb-4">{icon}</div>
      <h3 className="text-xl font-semibold text-continental-black mb-2">{title}</h3>
      <p className="text-continental-gray-1 text-sm">{description}</p>
    </button>
  );
}
