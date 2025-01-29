using System.ComponentModel.DataAnnotations;

namespace AkilliEtkinlikPlanlamaApp.Models.Entities
{
    public class Roller
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? Rol { get; set; }
        
        public ICollection<Kullanici>? Kullanicilar { get; set; }

    }
}
