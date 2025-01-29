using System.ComponentModel.DataAnnotations;

namespace AkilliEtkinlikPlanlamaApp.Models.Entities
{
    public class Mesaj
    {
        [Key]
        public int MesajID { get; set; }

        public int? GöndericiID { get; set; }
        public Kullanici? Gönderici { get; set; }  

        public int? AliciID { get; set; }
        public Kullanici? Alici { get; set; }  
        [Required]
        public string? MesajMetni {  get; set; }
        [Required]
        public DateTime? GonderimZamani { get; set; }

        
        public int? EtkinlikID { get; set; } 
        public Etkinlikler? Etkinlik { get; set; } 

     


    }
}
