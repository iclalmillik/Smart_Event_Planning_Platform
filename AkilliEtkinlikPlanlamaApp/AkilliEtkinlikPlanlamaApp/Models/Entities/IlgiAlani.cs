using System.ComponentModel.DataAnnotations;

namespace AkilliEtkinlikPlanlamaApp.Models.Entities
{
    public class IlgiAlani
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string? IlgiAlaniAdi { get; set; }
        public ICollection<Kullanici>? Kullanicilar { get; set; }
        public ICollection<KullaniciIlgiAlani>? KullaniciIlgiAlanlari { get; set; }
    }
}
