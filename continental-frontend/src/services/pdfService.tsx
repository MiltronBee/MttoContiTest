import { pdf } from '@react-pdf/renderer';
import { ConstanciaAntiguedadPDF } from '../components/PDF/ConstanciaAntiguedadPDF';

export interface EmpleadoVacacionData {
  nomina: string;
  nombre: string;
  fechaIngreso: string;
  antiguedadAnios: number;
  diasVacacionesCorresponden: number;
  diasAdicionales: number;
  diasProgramados: Array<{
    de: string;
    al: string;
    dias: number;
  }>;
  diasGozados: Array<{
    de: string;
    al: string;
    dias: number;
  }>;
  totalProgramados: number;
  porProgramar: number;
  totalGozados: number;
  porGozar: number;
}

export interface ConstanciaAntiguedadData {
  empleados: EmpleadoVacacionData[];
  area: string;
  grupos: string[];
  periodo: {
    inicio: string;
    fin: string;
  };
}

export const generateConstanciaAntiguedadPDF = async (data: ConstanciaAntiguedadData): Promise<Blob> => {
  try {
    const blob = await pdf(<ConstanciaAntiguedadPDF data={data} />).toBlob();
    return blob;
  } catch (error) {
    console.error('Error generating PDF:', error);
    throw new Error('Error al generar el PDF de Constancia de Antigüedad');
  }
};

export const downloadConstanciaAntiguedadPDF = async (data: ConstanciaAntiguedadData): Promise<void> => {
  try {
    const blob = await generateConstanciaAntiguedadPDF(data);
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `Constancia_Antiguedad_${data.area}_${new Date().toISOString().split('T')[0]}.pdf`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  } catch (error) {
    console.error('Error downloading PDF:', error);
    throw error;
  }
};
