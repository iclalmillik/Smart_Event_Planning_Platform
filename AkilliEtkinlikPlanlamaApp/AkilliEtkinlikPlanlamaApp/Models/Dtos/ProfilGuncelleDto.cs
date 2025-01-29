namespace AkilliEtkinlikPlanlamaApp.Models.Dtos
{
    public class ProfilGuncelleDto
    {
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public string Email { get; set; }
        public string KullaniciAdi { get; set; }
        public string TelefonNumarasi { get; set; }
        public string Konum { get; set; }
        public string? Sifre { get; set; }
        public string? SifreOnay { get; set; }
        public int? IlgiAlanlariID { get; set; }
        public string? ProfilFotografiYolu { get; set; }
        public IFormFile? ProfilFotografi { get; set; }
    }

}
