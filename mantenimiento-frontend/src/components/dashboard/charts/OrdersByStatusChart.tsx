import { PieChart, Pie, Cell, ResponsiveContainer, Legend, Tooltip } from 'recharts';

interface OrderStatusData {
  name: string;
  value: number;
  color: string;
}

interface OrdersByStatusChartProps {
  data?: OrderStatusData[];
}

const defaultData: OrderStatusData[] = [
  { name: 'Completadas', value: 45, color: '#2db928' },
  { name: 'En Proceso', value: 25, color: '#ffa500' },
  { name: 'Pendientes', value: 20, color: '#00a5dc' },
  { name: 'Canceladas', value: 10, color: '#ff2d37' },
];

export function OrdersByStatusChart({ data = defaultData }: OrdersByStatusChartProps) {
  const total = data.reduce((sum, item) => sum + item.value, 0);

  return (
    <div className="bg-white rounded-xl p-6 shadow-lg">
      <h3 className="text-lg font-semibold text-continental-black mb-4">
        Órdenes por Estado
      </h3>
      <div className="h-64">
        <ResponsiveContainer width="100%" height="100%">
          <PieChart>
            <Pie
              data={data}
              cx="50%"
              cy="50%"
              innerRadius={60}
              outerRadius={80}
              paddingAngle={2}
              dataKey="value"
              label={({ name, percent }) => `${name} ${(percent * 100).toFixed(0)}%`}
              labelLine={false}
            >
              {data.map((entry, index) => (
                <Cell key={`cell-${index}`} fill={entry.color} />
              ))}
            </Pie>
            <Tooltip
              contentStyle={{
                backgroundColor: '#fff',
                border: '1px solid #e5e5e5',
                borderRadius: '8px',
                boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
              }}
              formatter={(value: number) => [`${value} órdenes`, '']}
            />
            <Legend
              verticalAlign="bottom"
              height={36}
              formatter={(value) => (
                <span className="text-sm text-continental-gray-1">{value}</span>
              )}
            />
          </PieChart>
        </ResponsiveContainer>
      </div>
      <div className="text-center mt-2">
        <p className="text-2xl font-bold text-continental-black">{total}</p>
        <p className="text-sm text-continental-gray-1">Total de Órdenes</p>
      </div>
    </div>
  );
}
