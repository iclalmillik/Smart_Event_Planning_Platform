using System.ComponentModel.DataAnnotations;

namespace AkilliEtkinlikPlanlamaApp.Models.Entities
{
    public class ConfirmMail
    {
        [Required(ErrorMessage = "Kullanıcı ID gereklidir.")]
        public int? id { get; set; }

        [Required(ErrorMessage = "Onay kodu gereklidir.")]
        public int? ConfirmCode { get; set; }
    }
}
