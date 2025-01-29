using AkilliEtkinlikPlanlamaApp.Models;
using AkilliEtkinlikPlanlamaApp.Models.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
// Oturum hizmetlerini etkinle�tir
builder.Services.AddDistributedMemoryCache(); // Oturum verilerini bellekte saklar
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturumun s�resi (30 dakika)
    options.Cookie.HttpOnly = true; // G�venlik i�in �erezleri HttpOnly yap
    options.Cookie.IsEssential = true; // GDPR uyumlulu�u i�in gerekli
});

// Kimlik do�rulama yap�land�rmas�
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Kullan�c�/Login"; // Giri� sayfas�
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

// Middleware s�ras� �nemlidir
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // Oturumlar� middleware'e ekle
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
