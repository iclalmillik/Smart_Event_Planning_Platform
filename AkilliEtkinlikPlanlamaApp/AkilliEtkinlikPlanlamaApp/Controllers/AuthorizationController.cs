using AkilliEtkinlikPlanlamaApp.Models;
using AkilliEtkinlikPlanlamaApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliEtkinlikPlanlamaApp.Controllers
{
    public class AuthorizationController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorizationController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
        public IActionResult AdminGoruntule()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        public IActionResult KullanicilarListe()
        {
            var kullanicilar = _context.Kullanicilar
         .Include(k => k.Roller)
         .Include(k => k.KullaniciIlgiAlanlari)
         .ThenInclude(kia => kia.IlgiAlanlari) 
         .Where(k => k.Roller.Rol != "Admin") 
         .ToList();

            // İlgi alanlarını ViewBag'e aktar
            ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();

            return View(kullanicilar);
        }
        public IActionResult KullaniciSil( )
        {
            return View();
        }


        [HttpPost]
        public IActionResult KullaniciSil(int id)
        {
            Console.WriteLine($"Silinmek istenen kullanıcının ID'si: {id}");

           
            var kullanici = _context.Kullanicilar
                .Include(k => k.Katilimcilar)
                .Include(k => k.GonderdigiMesajlar)
                .Include(k => k.AldigiMesajlar)
                .Include(k => k.Etkinlikler) 
                .FirstOrDefault(k => k.ID == id);

            if (kullanici == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("KullanicilarListe");
            }

            try
            {
                
                _context.Kullanicilar.Remove(kullanici);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Kullanıcı ve ilişkili tüm veriler başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Kullanıcı silinirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("KullanicilarListe");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AdminRolAtama(int id)
        {
            var kullanici = _context.Kullanicilar.Include(k => k.Roller).FirstOrDefault(k => k.ID == id);

            if (kullanici == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("KullanicilarListe");
            }

            try
            {
                var adminRol = _context.Roller.FirstOrDefault(r => r.Rol == "Admin");
                if (adminRol == null)
                {
                    TempData["ErrorMessage"] = "Admin rolü bulunamadı. Lütfen sistem yöneticisi ile iletişime geçin.";
                    return RedirectToAction("KullanicilarListe");
                }

                kullanici.RollerID = adminRol.Id;
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Kullanıcıya Admin rolü başarıyla atandı.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Rol atanırken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("KullanicilarListe");
        }

     

        [Authorize(Roles = "Admin")]
        public IActionResult AdminEtkinlikListele()
        {
            var etkinlikler = _context.Etkinlikler
                .Include(e => e.Kullanici) 
                .ToList();

            return View(etkinlikler);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AdminEtkinlikDuzenle(int id)
        {
            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == id);

            if (etkinlik == null)
            {
                TempData["ErrorMessage"] = "Etkinlik bulunamadı.";
                return RedirectToAction("AdminEtkinlikListele");
            }

            return View(etkinlik);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AdminEtkinlikDuzenle(Etkinlikler etkinlik, IFormFile? EtkinlikFoto)
        {
            if (!ModelState.IsValid)
            {
                return View(etkinlik);
            }

            var mevcutEtkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == etkinlik.ID);
            if (mevcutEtkinlik == null)
            {
                TempData["ErrorMessage"] = "Etkinlik bulunamadı.";
                return RedirectToAction("AdminEtkinlikListele");
            }

            mevcutEtkinlik.EtkinlikAdi = etkinlik.EtkinlikAdi;
            mevcutEtkinlik.Aciklama = etkinlik.Aciklama;
            mevcutEtkinlik.Tarih = etkinlik.Tarih;
            mevcutEtkinlik.Saat = etkinlik.Saat;
            mevcutEtkinlik.Kategori = etkinlik.Kategori;
            mevcutEtkinlik.Konum = etkinlik.Konum;

            if (EtkinlikFoto != null && EtkinlikFoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(EtkinlikFoto.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await EtkinlikFoto.CopyToAsync(fileStream);
                }

                mevcutEtkinlik.EtkinlikFotoYolu = $"/uploads/{uniqueFileName}";
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Etkinlik başarıyla güncellendi.";
            return RedirectToAction("AdminEtkinlikListele");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AdminEtkinlikSil(int id)
        {
            var etkinlik = _context.Etkinlikler
                .Include(e => e.Katilimcilar)
                .Include(e => e.Mesajlar)
                .FirstOrDefault(e => e.ID == id);

            if (etkinlik == null)
            {
                TempData["ErrorMessage"] = "Etkinlik bulunamadı.";
                return RedirectToAction("AdminEtkinlikListele");
            }

            try
            {
                if (etkinlik.Katilimcilar != null)
                    _context.Katilimcilar.RemoveRange(etkinlik.Katilimcilar);

                if (etkinlik.Mesajlar != null)
                    _context.Mesajlar.RemoveRange(etkinlik.Mesajlar);

                _context.Etkinlikler.Remove(etkinlik);
                _context.SaveChanges();

                TempData["SuccessMessage"] = "Etkinlik başarıyla silindi.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Etkinlik silinirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("AdminEtkinlikListele");
        }


    }
}
