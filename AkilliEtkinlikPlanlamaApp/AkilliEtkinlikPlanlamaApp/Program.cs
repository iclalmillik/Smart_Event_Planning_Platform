using AkilliEtkinlikPlanlamaApp.Models;
using AkilliEtkinlikPlanlamaApp.Models.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
// Oturum hizmetlerini etkinleþtir
builder.Services.AddDistributedMemoryCache(); // Oturum verilerini bellekte saklar
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturumun süresi (30 dakika)
    options.Cookie.HttpOnly = true; // Güvenlik için çerezleri HttpOnly yap
    options.Cookie.IsEssential = true; // GDPR uyumluluðu için gerekli
});

// Kimlik doðrulama yapýlandýrmasý
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Kullanýcý/Login"; // Giriþ sayfasý
	});

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
	FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Login_v1")),
	RequestPath = "/Login_v1"
});

// Middleware sýrasý önemlidir
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Oturumlarý middleware'e ekle
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
