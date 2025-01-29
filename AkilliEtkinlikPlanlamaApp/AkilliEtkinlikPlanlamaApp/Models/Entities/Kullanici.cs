using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;





namespace AkilliEtkinlikPlanlamaApp.Models.Entities
{
    public class Kullanici
    {
        internal List<SelectListItem> ?IlgiAlanlariSelectList;

        [Key]
        public int ID { get; set; }

        [StringLength(50)]
        public string? KullaniciAdi { get; set; }

     
       
        [Required(ErrorMessage = "Şifre alanı zorunludur.")]
        [DataType(DataType.Password)]
        public string? Sifre { get; set; }

        [Compare("Sifre", ErrorMessage = "Şifreler eşleşmiyor.")]
        [DataType(DataType.Password)]
        public string? SifreOnay { get; set; }

        [StringLength(100)]
       
        public string? Email { get; set; }

        [StringLength(100)]
        public string? Konum { get; set; }

        public int IlgiAlanlariID { get; set; }

        [StringLength(100)]
        public string? Ad { get; set; }

        [StringLength(100)]
        public string? Soyad { get; set; }

        [Required]
        public DateTime? DogumTarihi { get; set; }

        [StringLength(100)]
        public string? Cinsiyet { get; set; }

        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        public string? TelefonNumarasi { get; set; }



        [NotMapped] 
        public IFormFile? ProfilFotografi { get; set; }

        [StringLength(200)]
        public string? ProfilFotografiYolu { get; set; }

        public int? RollerID { get; set; }
        public Roller? Roller {  get; set; }

        public ICollection<IlgiAlani>? IlgiAlanlari { get; set; }
        public ICollection<Etkinlikler>? Etkinlikler { get; set; }

        public ICollection<Katilimci>? Katilimcilar { get; set; }
        public ICollection<Puan>? Puanlar { get; set; }
        public ICollection<Mesaj>? GonderdigiMesajlar { get; set; }
        public ICollection<Mesaj>? AldigiMesajlar { get; set; }
        public ICollection<KullaniciIlgiAlani>? KullaniciIlgiAlanlari { get; set; }

       
        public bool IsVerified { get; set; } = false;
        public string? VerificationToken { get; set; }
        public int? ConfirmCode { get; set; }


    }

}


