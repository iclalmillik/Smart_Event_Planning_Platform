namespace AkilliEtkinlikPlanlamaApp.Models.Entities
{
    public class Katilimci
    {
        public int? KullaniciID { get; set; }
        public Kullanici? Kullanici { get; set; } 

        public int? EtkinlikID { get; set; }
        public Etkinlikler? Etkinlik { get; set; } 
    }
}
