using AkilliEtkinlikPlanlamaApp.Models;
using AkilliEtkinlikPlanlamaApp.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using AkilliEtkinlikPlanlamaApp.Models.Dtos;


namespace AkilliEtkinlikPlanlamaApp.Controllers
{
    public class KullanıcıController : Controller
	{
		private readonly AppDbContext _context;

		public KullanıcıController(AppDbContext context)
		{
			_context = context;
		}

		[AllowAnonymous]
		[HttpGet]
		public IActionResult Login()
		{
			return View();
		}

		[AllowAnonymous]
		[HttpPost]
		public async Task<IActionResult> Login(Kullanici k)
		{
			var datavalue = _context.Kullanicilar.FirstOrDefault(x => x.Email == k.Email);

            if (datavalue != null && BCrypt.Net.BCrypt.Verify(k.Sifre, datavalue.Sifre))

            {
               
                if (datavalue.RollerID == 1) 
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, datavalue.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

                    var useridentity = new ClaimsIdentity(claims, "Admin");
                    ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                    await HttpContext.SignInAsync(principal);

                    return RedirectToAction("AdminGoruntule", "Authorization"); 
                }
                else 
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, datavalue.Email),
                new Claim(ClaimTypes.Role, "User") 
            };

                    var useridentity = new ClaimsIdentity(claims, "User");
                    ClaimsPrincipal principal = new ClaimsPrincipal(useridentity);
                    await HttpContext.SignInAsync(principal);

                    return RedirectToAction("Giris", "Kullanıcı");
                }
            }
    else
            {
                ViewBag.ErrorMessage = "Geçersiz email veya şifre! Lütfen tekrar deneyiniz.";
                return View("Login");
            }
        }

		public IActionResult Index()
		{
			return View();
		}
        [HttpGet]
		public IActionResult Register()

		{
            var k = new Kullanici()
            { IlgiAlanlari = _context.IlgiAlanlari.ToList() };

            return View(k);
        }


        [HttpPost]
        public async Task<IActionResult> Register(Kullanici kullanici)
        {

            if (!ModelState.IsValid) 
            {
                
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);
                }

                kullanici.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                return View(kullanici);
            }

            if (kullanici.Sifre != kullanici.SifreOnay)
            {
                ModelState.AddModelError("SifreOnay", "Şifreler eşleşmiyor.");
                kullanici.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                return View(kullanici);
            }

            string profilFotografYolu = null;

            // Profil fotoğrafını kaydet
            if (kullanici.ProfilFotografi != null && kullanici.ProfilFotografi.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder); // Klasör yoksa oluşturur
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(kullanici.ProfilFotografi.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                try
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await kullanici.ProfilFotografi.CopyToAsync(fileStream);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Hata oluştu: {ex.Message}");
                }

                profilFotografYolu = $"/uploads/{uniqueFileName}";
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(kullanici.Sifre);
            var hashedPassword2 = BCrypt.Net.BCrypt.HashPassword(kullanici.SifreOnay);

           Random random = new Random();
            int code;
            code = random.Next(100000, 1000000);

            // Yeni kullanıcı oluştur ve ekle
            var yeniKullanici = new Kullanici
            {
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                Email = kullanici.Email,
                Sifre = hashedPassword, 
                SifreOnay = hashedPassword2, 


                Cinsiyet = kullanici.Cinsiyet,
                DogumTarihi = kullanici.DogumTarihi,
                KullaniciAdi = kullanici.KullaniciAdi,
                TelefonNumarasi = kullanici.TelefonNumarasi,
                Konum = kullanici.Konum,
                IlgiAlanlariID = kullanici.IlgiAlanlariID,
                ProfilFotografiYolu = profilFotografYolu,
                ConfirmCode =code
               
            };

            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            

            if (yeniKullanici.IlgiAlanlariID != 0) 
            {
                var kullaniciIlgiAlani = new KullaniciIlgiAlani
                {
                    KullanicilarID = yeniKullanici.ID, 
                    IlgiAlanlariID = yeniKullanici.IlgiAlanlariID
                };

                _context.KullaniciIlgiAlanlari.Add(kullaniciIlgiAlani);
                await _context.SaveChangesAsync();

                 }
            if(yeniKullanici.ID != 0)
            {
                MimeMessage mimeMessage = new MimeMessage();
                MailboxAddress mailboxAddress = new MailboxAddress("Admin","millikiclal0@gmail.com");
                MailboxAddress mailboxAddressTo = new MailboxAddress("User", yeniKullanici.Email);
                mimeMessage.From.Add(mailboxAddress);
                mimeMessage.To.Add(mailboxAddressTo);

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.TextBody = "Kayıt işlemini gerçekleştirmek için onay kodunuz: " + code;
                mimeMessage.Body = bodyBuilder.ToMessageBody();

                mimeMessage.Subject = "Akıllı Etkinlik Platformu Onay Kodu";


                MailKit.Net.Smtp.SmtpClient client = new MailKit.Net.Smtp.SmtpClient();
                client.Connect("smtp.gmail.com",587, MailKit.Security.SecureSocketOptions.StartTls);
                client.Authenticate("millikiclal0@gmail.com", "arccxrfsjqsgqsmz");
                client.Send(mimeMessage);
                client.Disconnect(true);

                TempData["Mail"] = yeniKullanici.Email;




            }




            return RedirectToAction("ConfirmEmail", "Kullanıcı", new { id = yeniKullanici.ID });
        }


        [Authorize]
        public IActionResult Profil()
        {
          
            var email = HttpContext.User.Identity.Name;

           
            var kullanici = _context.Kullanicilar
                .Include(k => k.KullaniciIlgiAlanlari)
                .FirstOrDefault(k => k.Email == email);

            if (kullanici == null)
            {
                return RedirectToAction("Login", "Kullanıcı");
            }

            // Kullanıcının toplam puanını hesapla
            var toplamPuan = _context.Puanlar
                .Where(p => p.KullaniciID == kullanici.ID)
                .Sum(p => p.PuanDegeri);

        
            ViewBag.Puan = toplamPuan;

       
            var ilgiAlaniAdi = _context.IlgiAlanlari
                .Where(ia => ia.ID == kullanici.IlgiAlanlariID)
                .Select(ia => ia.IlgiAlaniAdi)
                .FirstOrDefault();

           
            ViewBag.IlgiAlaniAdi = ilgiAlaniAdi;

            return View(kullanici);
        }

        public int HesaplaToplamPuan(int kullaniciID)
        {
            return _context.Puanlar.Where(p => p.KullaniciID == kullaniciID).Sum(p => p.PuanDegeri);
        }

        [Authorize]
        public IActionResult PuanDetaylari()
        {
            var email = HttpContext.User.Identity.Name;

            var kullanici = _context.Kullanicilar
                .Include(k => k.Puanlar)
                .FirstOrDefault(k => k.Email == email);

            if (kullanici == null)
            {
                return RedirectToAction("Login", "Kullanıcı");
            }

            return View(kullanici.Puanlar);
        }


        public IActionResult Giris()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult ProfilGuncelle()
        {
            var email = HttpContext.User.Identity.Name;

            var kullanici = _context.Kullanicilar
                .Include(k => k.KullaniciIlgiAlanlari)
                .ThenInclude(kia => kia.IlgiAlanlari)
                .FirstOrDefault(k => k.Email == email);

            if (kullanici == null)
            {
                return RedirectToAction("Login", "Kullanıcı");
            }


          
            var profilDto = new ProfilGuncelleDto
            {
                Ad = kullanici.Ad,
                Soyad = kullanici.Soyad,
                Email = kullanici.Email,
                KullaniciAdi = kullanici.KullaniciAdi,
                TelefonNumarasi = kullanici.TelefonNumarasi,
                Konum = kullanici.Konum,
                IlgiAlanlariID = kullanici.IlgiAlanlariID,
                ProfilFotografiYolu = kullanici.ProfilFotografiYolu
            };

            var ilgiAlanlari = _context.IlgiAlanlari
                .Select(ilgi => new SelectListItem
                {
                    Value = ilgi.ID.ToString(),
                    Text = ilgi.IlgiAlaniAdi
                }).ToList();

            ViewBag.IlgiAlanlari = ilgiAlanlari;

            return View(profilDto);
        }


        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProfilGuncelle(ProfilGuncelleDto dto, IFormFile? ProfilFotografi)
        {
            if (!ModelState.IsValid)
            {
                var ilgiAlanlari = _context.IlgiAlanlari
                    .Select(ilgi => new SelectListItem
                    {
                        Value = ilgi.ID.ToString(),
                        Text = ilgi.IlgiAlaniAdi
                    }).ToList();
                ViewBag.IlgiAlanlari = ilgiAlanlari;

                return View(dto);
            }

            var email = HttpContext.User.Identity.Name;
            var mevcutKullanici = _context.Kullanicilar.FirstOrDefault(k => k.Email == email);

            if (mevcutKullanici == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View(dto);
            }

    
            if (!string.IsNullOrEmpty(dto.Sifre) || !string.IsNullOrEmpty(dto.SifreOnay))
            {
                if (string.IsNullOrEmpty(dto.Sifre) || string.IsNullOrEmpty(dto.SifreOnay))
                {
                    ModelState.AddModelError("Sifre", "Şifre ve Şifre Onay alanlarının her ikisini de doldurun.");
                    return ProfilGuncelle();
                }

                if (dto.Sifre != dto.SifreOnay)
                {
                    ModelState.AddModelError("SifreOnay", "Şifreler uyuşmuyor.");
                    return ProfilGuncelle();
                }

                mevcutKullanici.Sifre = BCrypt.Net.BCrypt.HashPassword(dto.Sifre);
                mevcutKullanici.SifreOnay = BCrypt.Net.BCrypt.HashPassword(dto.Sifre);
            }

            mevcutKullanici.Ad = dto.Ad;
            mevcutKullanici.Soyad = dto.Soyad;
            mevcutKullanici.KullaniciAdi = dto.KullaniciAdi;
            mevcutKullanici.TelefonNumarasi = dto.TelefonNumarasi;
            mevcutKullanici.Konum = dto.Konum;
            mevcutKullanici.IlgiAlanlariID = (int)dto.IlgiAlanlariID;

            if (ProfilFotografi != null && ProfilFotografi.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ProfilFotografi.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilFotografi.CopyToAsync(fileStream);
                }

                mevcutKullanici.ProfilFotografiYolu = $"/uploads/{uniqueFileName}";
            }

            _context.Entry(mevcutKullanici).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Profil başarıyla güncellendi.";
            return RedirectToAction("Profil", "Kullanıcı");
        }




        [HttpGet]
        public IActionResult ConfirmEmail(int id)
        {
            var model = new ConfirmMail { id = id };
            Console.WriteLine($"ID değeri: {id}"); 
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmMail confirmMail)
        {
            if (!ModelState.IsValid)
            {
                return View(confirmMail);
            }

           
            var user = await _context.Kullanicilar
                .FirstOrDefaultAsync(u => u.ID == confirmMail.id);

            if (user == null)
            {
                ModelState.AddModelError("", "Kullanıcı bulunamadı.");
                return View(confirmMail);
            }

          
            if (user.ConfirmCode != confirmMail.ConfirmCode)
            {
                ModelState.AddModelError("ConfirmCode", "Onay kodu eşleşmiyor. Lütfen tekrar deneyin.");
                return View(confirmMail);
            }

          
            user.IsVerified = true;
            _context.Kullanicilar.Update(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Hesabınız başarıyla aktifleştirildi!";
            return RedirectToAction("Index", "Home");
        }


    }






}

