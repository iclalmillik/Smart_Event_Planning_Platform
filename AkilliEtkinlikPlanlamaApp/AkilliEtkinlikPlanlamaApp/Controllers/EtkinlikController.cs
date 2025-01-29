using AkilliEtkinlikPlanlamaApp.Models;
using AkilliEtkinlikPlanlamaApp.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;


namespace AkilliEtkinlikPlanlamaApp.Controllers
{
    public class EtkinlikController : Controller
    {
        private readonly AppDbContext _context;

        public EtkinlikController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize]
        public IActionResult Index()
        {
            var etkinlikler = _context.Etkinlikler.ToList();
            return View(etkinlikler);
        }
        [HttpGet]
        public IActionResult EtkinlikOlustur()
        {
            var ilgiAlanlari = _context.IlgiAlanlari
                .Select(ia => new SelectListItem
                {
                    Text = ia.IlgiAlaniAdi,
                    Value = ia.IlgiAlaniAdi
                })
                .ToList();

            if (!ilgiAlanlari.Any())
            {
                ViewBag.HataMesaji = "Hiç ilgi alanı bulunmamaktadır. Önce ilgi alanları ekleyin.";
            }

            ViewBag.IlgiAlanlari = ilgiAlanlari;
            return View(new Etkinlikler());
        }

        [HttpPost]
        public async Task<IActionResult> EtkinlikOlustur(Etkinlikler etkinlik, IFormFile? EtkinlikFoto)
        {
            // Giriş yapan kullanıcıyı al
            var email = HttpContext.User.Identity.Name;
            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == email);

            if (mevcutKullanici == null)
            {
                return Unauthorized(); // Kullanıcı giriş yapmamışsa
            }

            if (!ModelState.IsValid)
            {
                // Hataları görmek için
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage); // Konsola hata mesajlarını yaz
                }

                var ilgiAlanlari = _context.IlgiAlanlari
                    .Select(ia => new SelectListItem
                    {
                        Text = ia.IlgiAlaniAdi,
                        Value = ia.IlgiAlaniAdi
                    })
                    .ToList();

                ViewBag.IlgiAlanlari = ilgiAlanlari;
                return View(etkinlik);
            }

            string etkinlikFotoYolu = null;

            // Etkinlik fotoğrafını kaydet
            if (EtkinlikFoto != null && EtkinlikFoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Klasör yoksa oluştur
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(EtkinlikFoto.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await EtkinlikFoto.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata oluştu: {ex.Message}");
                }

                etkinlikFotoYolu = $"/uploads/{uniqueFileName}";
            }

            // Yeni etkinlik oluştur ve giriş yapan kullanıcıyı ilişkilendir
            var yeniEtkinlik = new Etkinlikler
            {
                EtkinlikAdi = etkinlik.EtkinlikAdi,
                Aciklama = etkinlik.Aciklama,
                Tarih = etkinlik.Tarih,
                Saat = etkinlik.Saat,
                Konum = etkinlik.Konum,
                Kategori = etkinlik.Kategori,
                EtkinlikFotoYolu = etkinlikFotoYolu,
                KullaniciID = mevcutKullanici.ID // Kullanıcı ile ilişkilendirme
            };

            try
            {
                _context.Etkinlikler.Add(yeniEtkinlik);

                // Kullanıcıya etkinlik oluşturma puanı ekle
                var yeniPuan = new Puan
                {
                    KullaniciID = mevcutKullanici.ID,
                    PuanDegeri = 15, // Etkinlik oluşturma puanı
                    KazanilanTarih = DateTime.Now
                };
                _context.Puanlar.Add(yeniPuan);

                await _context.SaveChangesAsync();

                TempData["Message"] = "Etkinlik başarıyla oluşturuldu ve 15 puan kazandınız!";
                return RedirectToAction("EtkinlikGoruntule", "Etkinlik");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Etkinlik kaydedilirken bir hata oluştu: " + ex.Message);

                var ilgiAlanlari = _context.IlgiAlanlari
                    .Select(ia => new SelectListItem
                    {
                        Text = ia.IlgiAlaniAdi,
                        Value = ia.IlgiAlaniAdi
                    })
                    .ToList();

                ViewBag.IlgiAlanlari = ilgiAlanlari;
                return View(etkinlik);
            }
        }



        public IActionResult EtkinlikGoruntule()
        {
            // Veritabanı bağlantısı kontrolü
            if (_context.Etkinlikler == null)
            {
                throw new Exception("Etkinlikler tablosu null. Veritabanı bağlantısını kontrol edin.");
            }

            // Etkinlikler ve yorumlarla birlikte kullanıcı bilgilerini dahil ediyoruz
            var etkinlikler = _context.Etkinlikler
               

                .Include(e => e.Kullanici) // Kullanıcı bilgilerini dahil et
    
                .Include(e => e.Mesajlar) // Etkinliğe bağlı mesajlar
                .ThenInclude(m => m.Gönderici) // Yorum yapan kullanıcı bilgisi
                .ToList();

            // Eğer etkinlik tablosunda veri yoksa uyarı
            if (!etkinlikler.Any())
            {
                Console.WriteLine("Etkinlikler tablosunda veri bulunamadı.");
            }

            // Etkinlikler listesini View'e gönder
            return View(etkinlikler);
        }

        [HttpGet]
        public IActionResult EtkinlikDuzenle(int id)
        {
            var email = HttpContext.User.Identity.Name;
            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == email);

            if (mevcutKullanici == null)
            {
                return Unauthorized(); // Kullanıcı giriş yapmamışsa
            }

            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == id && e.KullaniciID == mevcutKullanici.ID);
            if (etkinlik == null)
            {
                return NotFound(); // Etkinlik bulunamadı veya kullanıcıya ait değil
            }

            return View(etkinlik);
        }
        [HttpPost]
        public async Task<IActionResult> EtkinlikDuzenle(Etkinlikler etkinlik, IFormFile? EtkinlikFoto)
        {
            if (!ModelState.IsValid)
            {
                return View(etkinlik);
            }

            // Veritabanındaki mevcut etkinliği bul
            var mevcutEtkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == etkinlik.ID);
            if (mevcutEtkinlik == null)
            {
                return NotFound();
            }

            // Mevcut etkinlik bilgilerini güncelle
            mevcutEtkinlik.EtkinlikAdi = etkinlik.EtkinlikAdi;
            mevcutEtkinlik.Aciklama = etkinlik.Aciklama;
            mevcutEtkinlik.Tarih = etkinlik.Tarih;
            mevcutEtkinlik.Saat = etkinlik.Saat;
            mevcutEtkinlik.Konum = etkinlik.Konum;
            mevcutEtkinlik.Kategori = etkinlik.Kategori;


            // Etkinlik fotoğrafını güncelleme
            if (EtkinlikFoto != null && EtkinlikFoto.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Eğer klasör yoksa oluştur
                }

                // Yeni fotoğraf dosyasını kaydet
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(EtkinlikFoto.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await EtkinlikFoto.CopyToAsync(fileStream);
                }

                // Mevcut fotoğrafı silmek için önce yolunu al
                if (!string.IsNullOrEmpty(mevcutEtkinlik.EtkinlikFotoYolu))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", mevcutEtkinlik.EtkinlikFotoYolu.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                mevcutEtkinlik.EtkinlikFotoYolu = $"/uploads/{uniqueFileName}";
            }

            // Değişiklikleri kaydet
            await _context.SaveChangesAsync();

            return RedirectToAction("EtkinlikGoruntule");
        }
        [HttpGet]
        public IActionResult EtkinlikSil(int id)
        {
            var email = HttpContext.User.Identity.Name;
            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == email);

            if (mevcutKullanici == null)
            {
                return Unauthorized();
            }

            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == id && e.KullaniciID == mevcutKullanici.ID);
            if (etkinlik == null)
            {
                return NotFound();
            }

            return View(etkinlik); // Silme onayı için etkinlik bilgilerini göster
        }

        [HttpPost, ActionName("EtkinlikSil")]
        public IActionResult EtkinlikSilConfirmed(int id)
        {
            var email = HttpContext.User.Identity.Name;
            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == email);

            if (mevcutKullanici == null)
            {
                return Unauthorized();
            }

            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == id && e.KullaniciID == mevcutKullanici.ID);
            if (etkinlik == null)
            {
                return NotFound();
            }

            _context.Etkinlikler.Remove(etkinlik);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Etkinlik başarıyla silindi.";
            return RedirectToAction("EtkinlikGoruntule");
        }

        [HttpPost]
        public async Task<IActionResult> Katil(int etkinlikID)
        {
            // Oturum açan kullanıcıyı al
            var email = User.Identity.Name;
            var mevcutKullanici = _context.Kullanicilar
                .Include(k => k.Katilimcilar)
                .ThenInclude(k => k.Etkinlik)
                .FirstOrDefault(k => k.Email == email);

            if (mevcutKullanici == null)
            {
                return Unauthorized(); // Kullanıcı giriş yapmamış
            }

            // Katılmak istenen etkinliğin bilgilerini al
            var yeniEtkinlik = await _context.Etkinlikler.FirstOrDefaultAsync(e => e.ID == etkinlikID);
            if (yeniEtkinlik == null)
            {
                TempData["Error"] = "Etkinlik bulunamadı.";
                return RedirectToAction("EtkinlikGoruntule");
            }

            // Zaten katılımcıysa tekrar ekleme
            var mevcutKatilim = _context.Katilimcilar
                .FirstOrDefault(k => k.KullaniciID == mevcutKullanici.ID && k.EtkinlikID == etkinlikID);

            if (mevcutKatilim != null)
            {
                TempData["Message"] = "Bu etkinliğe zaten katıldınız.";
                return RedirectToAction("EtkinlikGoruntule");
            }

            // Kullanıcının daha önce katıldığı etkinlikleri al
            var katildigiEtkinlikler = _context.Etkinlikler
                .Where(e => e.Katilimcilar.Any(k => k.KullaniciID == mevcutKullanici.ID))
                .ToList();

            // Çakışmayan etkinlikleri öner
            var cakisMayanEtkinlikler = new List<Etkinlikler>();
            var yeniEtkinlikBaslangic = yeniEtkinlik.Tarih + yeniEtkinlik.Saat;
            var yeniEtkinlikBitis = yeniEtkinlikBaslangic.AddHours(2); // Etkinlik süresi örnek olarak 2 saat

            bool cakisiyor = false;

            foreach (var etkinlik in katildigiEtkinlikler)
            {
                var mevcutEtkinlikBaslangic = etkinlik.Tarih + etkinlik.Saat;
                var mevcutEtkinlikBitis = mevcutEtkinlikBaslangic.AddHours(2);

                if (mevcutEtkinlikBaslangic < yeniEtkinlikBitis && yeniEtkinlikBaslangic < mevcutEtkinlikBitis)
                {
                    cakisiyor = true;
                }
                else
                {
                    cakisMayanEtkinlikler.Add(etkinlik); // Çakışmayan etkinlikleri ekle
                }
            }

            if (cakisiyor)
            {
                // Tüm çakışmayan etkinlikleri öner
                ViewBag.OnerilenEtkinlikler = cakisMayanEtkinlikler;
                ViewBag.Error = "Bu etkinlik başka bir etkinlikle çakışıyor. Çakışmayan etkinlikler önerildi.";
                return View("OnerilenEtkinlik");
            }

            // Zaman çakışması yoksa etkinliğe katıl
            var yeniKatilim = new Katilimci
            {
                KullaniciID = mevcutKullanici.ID,
                EtkinlikID = etkinlikID
            };

            _context.Katilimcilar.Add(yeniKatilim);


            // Kullanıcıya etkinliğe katılım puanı ekle
            var yeniPuan = new Puan
            {
                KullaniciID = mevcutKullanici.ID,
                PuanDegeri = 10, // Etkinliğe katılım puanı
                KazanilanTarih = DateTime.Now
            };
            _context.Puanlar.Add(yeniPuan);

            // Eğer bu kullanıcının ilk katılımıysa ekstra bonus puan ekle
            var katilimSayisi = _context.Katilimcilar.Count(k => k.KullaniciID == mevcutKullanici.ID);
            if (katilimSayisi == 0)
            {
                var bonusPuan = new Puan
                {
                    KullaniciID = mevcutKullanici.ID,
                    PuanDegeri = 20, // İlk katılım bonusu
                    KazanilanTarih = DateTime.Now
                };
                _context.Puanlar.Add(bonusPuan);
                TempData["Message"] = "Bu etkinliğe başarıyla katıldınız ve 30 puan kazandınız! (10 Etkinlik Katılımı + 20 İlk Katılım Bonus)";
            }
            else
            {
                TempData["Message"] = "Bu etkinliğe başarıyla katıldınız ve 10 puan kazandınız!";
            }


            await _context.SaveChangesAsync();
           
            return RedirectToAction("KatildigiEtkinlikler");
        }




        [HttpGet]
        public IActionResult EtkinlikDetay()
        {
            var currentUserEmail = User.Identity.Name;
            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == currentUserEmail);

            if (mevcutKullanici == null)
            {
                return Unauthorized();
            }

            var katildiginizEtkinlikler = _context.Etkinlikler
                .Include(e => e.Mesajlar)
                .ThenInclude(m => m.Gönderici)
                .Where(e => e.Katilimcilar.Any(k => k.KullaniciID == mevcutKullanici.ID))
                .ToList();

            return View(katildiginizEtkinlikler);
        }

        [HttpPost]
        public IActionResult YorumYap(int etkinlikID, string yorumMetni)
        {
            // Giriş yapan kullanıcının bilgilerini al
            var mevcutKullaniciEmail = User.Identity.Name;
            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == mevcutKullaniciEmail);

            if (mevcutKullanici == null)
            {
                return Unauthorized(); // Kullanıcı giriş yapmamış
            }

            // Yorum yapılacak etkinliği veritabanından al
            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == etkinlikID);
            if (etkinlik == null)
            {
                return NotFound(); // Etkinlik bulunamadı
            }

            // Yorum nesnesini oluştur
            var yeniYorum = new Mesaj
            {
                GöndericiID = mevcutKullanici.ID, // Yorumu yapan kullanıcı
                AliciID = etkinlik.KullaniciID, // Etkinliği oluşturan kullanıcı
                EtkinlikID = etkinlik.ID, // Etkinlik ID'si
                MesajMetni = yorumMetni,
                GonderimZamani = DateTime.Now
            };

            // Yorum veritabanına kaydedilir
            _context.Mesajlar.Add(yeniYorum);

            try
            {
                _context.SaveChanges(); // Değişiklikleri kaydet
                TempData["Message"] = "Yorum başarıyla eklendi.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Yorum kaydedilirken bir hata oluştu: {ex.Message}";
            }

            return RedirectToAction("EtkinlikDetay", new { id = etkinlikID });
        }

        public IActionResult EtkinlikYorumlari(int etkinlikID)
        {
            var yorumlar = _context.Mesajlar
        .Where(m => m.EtkinlikID == etkinlikID) // Yalnızca ilgili etkinlik yorumlarını al
        .Include(m => m.Gönderici) // Gönderici bilgileri
        .ToList();

            return View(yorumlar);
        }

        [HttpGet]
        public IActionResult KatildigiEtkinlikler()
        {

            var email = User.Identity.Name;

            var mevcutKullanici = _context.Kullanicilar
                    .Include(k => k.Katilimcilar)
                    .ThenInclude(k => k.Etkinlik)
                    .ThenInclude(e => e.Mesajlar) 
                    .ThenInclude(m => m.Gönderici) 
                .FirstOrDefault(k => k.Email == email);

            if (mevcutKullanici == null)
            {
                TempData["Error"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Login", "Kullanıcı");
            }

            // Kullanıcının katıldığı etkinlikler
            var katildigiEtkinlikler = mevcutKullanici.Katilimcilar
                .Select(k => k.Etkinlik)
                .ToList();

            return View(katildigiEtkinlikler);
        }

        [HttpPost]
        public async Task<IActionResult> EtkinliktenAyril(int etkinlikID)
        {
            // Oturum açan kullanıcıyı al
            var email = User.Identity.Name;
            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == email);

            if (mevcutKullanici == null)
            {
                TempData["Error"] = "Kullanıcı giriş yapmamış.";
                return RedirectToAction("KatildigimEtkinlikler");
            }

            // Katılım kaydını bul
            var katilim = _context.Katilimcilar.FirstOrDefault(k => k.KullaniciID == mevcutKullanici.ID && k.EtkinlikID == etkinlikID);

            if (katilim == null)
            {
                TempData["Error"] = "Bu etkinlikte kayıt bulunamadı.";
                return RedirectToAction("KatildigimEtkinlikler");
            }

            // Katılımı kaldır
            _context.Katilimcilar.Remove(katilim);
            // Kullanıcının puanını güncelle
            var kullaniciPuani = new Puan
            {
                KullaniciID = mevcutKullanici.ID,
                PuanDegeri = -10, // 10 puan silinsin
                KazanilanTarih = DateTime.Now
            };
            _context.Puanlar.Add(kullaniciPuani);

            await _context.SaveChangesAsync();

            TempData["Error"] = "Etkinlikten başarıyla ayrıldınız. 10 puanınız silindi.";
            return RedirectToAction("KatildigiEtkinlikler");
        }
        [HttpGet]
        public IActionResult OnerilenEtkinlik()
        {
            var onerilenEtkinlikler = ViewBag.OnerilenEtkinlikler as List<Etkinlikler>;

            if (onerilenEtkinlikler == null || !onerilenEtkinlikler.Any())
            {
                ViewBag.Message = "Önerilen etkinlik bulunamadı.";
            }

            return View(onerilenEtkinlikler);
        }

    }

}

