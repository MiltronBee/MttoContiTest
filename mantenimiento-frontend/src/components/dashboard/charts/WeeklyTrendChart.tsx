import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  Legend,
} from 'recharts';

interface WeeklyData {
  name: string;
  solicitudes: number;
  completadas: number;
}

interface WeeklyTrendChartProps {
  data?: WeeklyData[];
}

const defaultData: WeeklyData[] = [
  { name: 'Lun', solicitudes: 12, completadas: 10 },
  { name: 'Mar', solicitudes: 15, completadas: 13 },
  { name: 'Mié', solicitudes: 18, completadas: 15 },
  { name: 'Jue', solicitudes: 14, completadas: 14 },
  { name: 'Vie', solicitudes: 20, completadas: 18 },
  { name: 'Sáb', solicitudes: 8, completadas: 8 },
  { name: 'Dom', solicitudes: 5, completadas: 5 },
];

export function WeeklyTrendChart({ data = defaultData }: WeeklyTrendChartProps) {
  return (
    <div className="bg-white rounded-xl p-6 shadow-lg">
      <h3 className="text-lg font-semibold text-continental-black mb-4">
        Tendencia Semanal
      </h3>
      <div className="h-64">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart
            data={data}
            margin={{ top: 10, right: 10, left: 0, bottom: 0 }}
          >
            <CartesianGrid strokeDasharray="3 3" stroke="#e5e5e5" />
            <XAxis
              dataKey="name"
              tick={{ fill: '#666', fontSize: 12 }}
              axisLine={{ stroke: '#e5e5e5' }}
            />
            <YAxis
              tick={{ fill: '#666', fontSize: 12 }}
              axisLine={{ stroke: '#e5e5e5' }}
            />
            <Tooltip
              contentStyle={{
                backgroundColor: '#fff',
                border: '1px solid #e5e5e5',
                borderRadius: '8px',
                boxShadow: '0 4px 6px -1px rgb(0 0 0 / 0.1)',
              }}
              labelStyle={{ color: '#000', fontWeight: 600 }}
            />
            <Legend
              verticalAlign="bottom"
              height={36}
              formatter={(value) => (
                <span className="text-sm text-continental-gray-1">
                  {value === 'solicitudes' ? 'Solicitudes' : 'Completadas'}
                </span>
              )}
            />
            <Line
              type="monotone"
              dataKey="solicitudes"
              stroke="#ffa500"
              strokeWidth={2}
              dot={{ fill: '#ffa500', strokeWidth: 2, r: 4 }}
              activeDot={{ r: 6, fill: '#ffa500' }}
            />
            <Line
              type="monotone"
              dataKey="completadas"
              stroke="#2db928"
              strokeWidth={2}
              dot={{ fill: '#2db928', strokeWidth: 2, r: 4 }}
              activeDot={{ r: 6, fill: '#2db928' }}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}
