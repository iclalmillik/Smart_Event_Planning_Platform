using System.ComponentModel.DataAnnotations;
using System.Globalization;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AkilliEtkinlikPlanlamaApp.Models.Entities
{
    public class Etkinlikler
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Etkinlik adı gereklidir.")]
        [StringLength(100, ErrorMessage = "Etkinlik adı en fazla 100 karakter olabilir.")]
        public string? EtkinlikAdi { get; set; }

        [Required(ErrorMessage = "Açıklama gereklidir.")]
        public string? Aciklama { get; set; }

        [Required(ErrorMessage = "Tarih gereklidir.")]
        public DateTime Tarih { get; set; }

        [Required(ErrorMessage = "Saat gereklidir.")]
        public TimeSpan Saat { get; set; } 

        [Required(ErrorMessage = "Konum gereklidir.")]
        public string? Konum { get; set; }

        [StringLength(100, ErrorMessage = "Kategori en fazla 100 karakter olabilir.")]
        public string? Kategori { get; set; }
        [NotMapped]

        public IFormFile? EtkinlikFoto { get; set; }
        [StringLength(200)]
        public string? EtkinlikFotoYolu { get; set; }

     
        public int? KullaniciID { get; set; }
        public Kullanici? Kullanici { get; set; }
        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public ICollection<Katilimci>? Katilimcilar { get; set; }
        public ICollection<Mesaj>? Mesajlar {  get; set; } 
    }
}

