using System;
using System.Collections.Generic;

namespace tiempo_libre.Helpers
{
    /// <summary>
    /// Helper centralizado para manejar la lógica de turnos y reglas de calendario
    /// </summary>
    public static class TurnosHelper
    {
        // Reglas definidas basadas en el código TypeScript - ÚNICA FUENTE DE VERDAD
        public static readonly Dictionary<string, string[]> REGLAS = new()
        {
            ["R0144"] = new[] { "1", "1", "1", "1", "1", "D", "D", "D", "3", "3", "3", "3", "3", "D", "2", "2", "D", "D", "2", "2", "3", "3", "D", "2", "2", "D", "1", "1" },
            ["N0439"] = new[] { "1", "1", "1", "1", "1", "D", "D" },
            ["R0135"] = new[] { "1", "1", "1", "1", "1", "D", "D", "D", "1", "1", "1", "1", "1", "D" },
            ["R0229"] = new[] { "D", "1", "1", "1", "1", "1", "D", "1", "1", "D", "D", "1", "1", "1", "1", "D", "1", "1", "D", "1", "1", "2", "2", "2", "2", "2", "D", "D" },
            ["R0154"] = new[] { "2", "2", "2", "2", "2", "D", "D", "D", "1", "1", "1", "1", "1", "D" },
            ["R0267"] = new[] { "2", "2", "D", "2", "2", "2", "D", "1", "1", "1", "1", "1", "D", "D", "D", "3", "3", "3", "D", "1", "1" },
            ["R0130"] = new[] { "1", "1", "1", "1", "1", "D", "D", "D", "3", "3", "D", "2", "2", "2", "2", "2", "D", "3", "3", "3", "3", "3", "D", "2", "2", "D", "1", "1" },
            ["N0440"] = new[] { "2", "2", "2", "2", "2", "D", "D" },
            ["N0A01"] = new[] { "1", "1", "1", "D", "1", "1", "D" },
            ["R0133"] = new[] { "1", "1", "1", "1", "1", "D", "D", "2", "2", "2", "2", "2", "D", "D" },
            ["R0228"] = new[] { "1", "1", "1", "1", "1", "D", "D", "2", "2", "2", "2", "2", "D", "D", "D", "1", "1", "1", "1", "1", "D", "2", "2", "2", "2", "2", "D", "D" }
        };

        /// <summary>
        /// Fecha de referencia para cálculos de calendario (15 de septiembre de 2025)
        /// </summary>
        public static readonly DateTime FECHA_REFERENCIA = new DateTime(2025, 9, 15);

        /// <summary>
        /// Parsear el rol del grupo para extraer regla y número de grupo
        /// </summary>
        /// <param name="rolGrupo">Formato: "R0144_04" o "R0144"</param>
        /// <returns>Tupla con (Regla, NumeroGrupo) o null si es inválido</returns>
        public static (string Regla, int NumeroGrupo)? ParseRolGrupo(string rolGrupo)
        {
            if (string.IsNullOrEmpty(rolGrupo))
                return null;

            var parts = rolGrupo.Split('_');
            
            string regla;
            int numeroGrupo = 1; // Por defecto grupo 1
            
            if (parts.Length == 1)
            {
                // Solo regla, sin número (ej: "R0144")
                regla = parts[0];
            }
            else if (parts.Length == 2)
            {
                // Regla con número (ej: "R0144_04")
                regla = parts[0];
                if (!int.TryParse(parts[1], out numeroGrupo))
                {
                    numeroGrupo = 1;
                }
            }
            else
            {
                return null;
            }
            
            if (!REGLAS.ContainsKey(regla))
                return null;

            return (regla, numeroGrupo);
        }

        /// <summary>
        /// Crear el rol específico para un grupo basado en la regla y número de grupo
        /// </summary>
        /// <param name="reglaRef">Código de regla (ej: "R0144")</param>
        /// <param name="gpoRef">Número de grupo (1-based)</param>
        /// <returns>Array de turnos para el grupo específico</returns>
        public static string[] CrearRol(string reglaRef, int gpoRef)
        {
            if (!REGLAS.ContainsKey(reglaRef))
                return new string[0];

            var regla = REGLAS[reglaRef];
            var cantSemanas = regla.Length / 7;
            var rol = new string[cantSemanas * 7];
            var dia = (gpoRef - 1) * 7;

            for (int i = 0; i < cantSemanas * 7; i++, dia++)
            {
                rol[i] = regla[dia % (cantSemanas * 7)];
            }

            return rol;
        }

        /// <summary>
        /// Obtener el turno de un empleado para una fecha específica
        /// </summary>
        /// <param name="rolGrupo">Rol del grupo del empleado</param>
        /// <param name="fecha">Fecha a consultar</param>
        /// <returns>Código de turno ("1", "2", "3", "D", etc.)</returns>
        public static string ObtenerTurnoParaFecha(string rolGrupo, DateOnly fecha)
        {
            var reglaInfo = ParseRolGrupo(rolGrupo);
            if (reglaInfo == null)
                return "1"; // Fallback

            var rol = CrearRol(reglaInfo.Value.Regla, reglaInfo.Value.NumeroGrupo);
            if (rol.Length == 0)
                return "1"; // Fallback

            var fechaDateTime = fecha.ToDateTime(TimeOnly.MinValue);
            var diasDiferencia = (fechaDateTime - FECHA_REFERENCIA).Days;
            var indice = Math.Abs(diasDiferencia) % rol.Length;
            
            return rol[indice];
        }

        /// <summary>
        /// Verificar si un turno es día de descanso
        /// </summary>
        /// <param name="turno">Código de turno</param>
        /// <returns>True si es descanso</returns>
        public static bool EsDescanso(string turno)
        {
            return turno == "D" || turno == "0";
        }

        /// <summary>
        /// Obtener todas las reglas disponibles
        /// </summary>
        /// <returns>Lista de códigos de reglas</returns>
        public static List<string> ObtenerReglasDisponibles()
        {
            return new List<string>(REGLAS.Keys);
        }

        /// <summary>
        /// Agregar o actualizar una regla de turnos
        /// </summary>
        /// <param name="codigoRegla">Código de la regla</param>
        /// <param name="patron">Patrón de turnos</param>
        public static void ActualizarRegla(string codigoRegla, string[] patron)
        {
            REGLAS[codigoRegla] = patron;
        }
    }
}
