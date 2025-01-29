namespace AkilliEtkinlikPlanlamaApp.Models.Entities
{
    public class KullaniciIlgiAlani
    {
        public int KullanicilarID { get; set; }
        public int IlgiAlanlariID { get; set; }

        public Kullanici? Kullanicilar { get; set; }
        public IlgiAlani? IlgiAlanlari { get; set; }
    }

}
