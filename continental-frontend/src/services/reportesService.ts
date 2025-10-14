import { env } from '@/config/env';

export const reportesService = {
  /**
   * Descarga el reporte de vacaciones programadas agrupadas por área en formato Excel
   * @param year - Año opcional para filtrar las vacaciones
   */
  async exportarVacacionesPorArea(year?: number): Promise<void> {
    try {
      // Obtener el token de autenticación
      const token = localStorage.getItem('auth_token') ||
        (() => {
          try {
            const user = localStorage.getItem('user');
            if (user) {
              const userData = JSON.parse(user);
              return userData.token || null;
            }
          } catch (e) {
            return null;
          }
        })();

      if (!token) {
        throw new Error('No se encontró token de autenticación');
      }

      const params = year ? `?year=${year}` : '';
      const url = `${env.API_BASE_URL}/api/reportes/vacaciones-por-area${params}`;

      // Realizar la petición con responseType blob para archivos
      const response = await fetch(url, {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error(`Error al descargar el reporte: ${response.statusText}`);
      }

      // Obtener el blob del archivo
      const blob = await response.blob();

      // Extraer el nombre del archivo desde el header Content-Disposition
      const contentDisposition = response.headers.get('Content-Disposition');
      let fileName = year
        ? `VacacionesProgramadas_${year}_${new Date().toISOString().split('T')[0]}.xlsx`
        : `VacacionesProgramadas_Todas_${new Date().toISOString().split('T')[0]}.xlsx`;

      if (contentDisposition) {
        const fileNameMatch = contentDisposition.match(/filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/);
        if (fileNameMatch && fileNameMatch[1]) {
          fileName = fileNameMatch[1].replace(/['"]/g, '');
        }
      }

      // Crear un enlace temporal y simular un click para descargar
      const downloadUrl = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = downloadUrl;
      link.download = fileName;
      document.body.appendChild(link);
      link.click();

      // Limpiar
      document.body.removeChild(link);
      window.URL.revokeObjectURL(downloadUrl);
    } catch (error) {
      console.error('Error al exportar vacaciones por área:', error);
      throw error;
    }
  }
};
