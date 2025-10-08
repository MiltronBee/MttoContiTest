using System.ComponentModel.DataAnnotations;

namespace tiempo_libre.DTOs
{
    public class UpdateSuplenteRequest
    {
        [Required]
        public string Rol { get; set; } = null!;
        
        public int? GrupoId { get; set; }
        
        public int? AreaId { get; set; }
        
        public int? SuplenteId { get; set; }
    }
}
