using System.ComponentModel.DataAnnotations;

namespace AkilliEtkinlikPlanlamaApp.Models.Entities
{
    public class Puan
    {
        [Key]
        public int ID { get; set; }  
        public int KullaniciID { get; set; }  
        public Kullanici? Kullanici { get; set; }  

        public int PuanDegeri { get; set; }  

        public DateTime? KazanilanTarih { get; set; }  
    }
}
