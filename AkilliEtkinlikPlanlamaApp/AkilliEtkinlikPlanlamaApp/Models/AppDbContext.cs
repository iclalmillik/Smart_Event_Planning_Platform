using AkilliEtkinlikPlanlamaApp.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace AkilliEtkinlikPlanlamaApp.Models
{
    public class AppDbContext:DbContext
    {
        internal readonly object KullaniciIlgiAlani;

        public AppDbContext(DbContextOptions options):base(options)
        {
        
        }
        public DbSet<Kullanici>Kullanicilar {  get; set; }
       
        public DbSet<IlgiAlani>IlgiAlanlari {  get; set; }
        public DbSet<Etkinlikler>Etkinlikler {  get; set; }
       
        public DbSet<Katilimci> Katilimcilar { get; set; }
        public DbSet<Puan> Puanlar { get; set; }
        public DbSet<Mesaj> Mesajlar { get; set; }
        public DbSet<KullaniciIlgiAlani> KullaniciIlgiAlanlari{ get; set; }
        public DbSet<Roller> Roller { get; set; }
     





        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Kullanici ve IlgiAlani arasındaki Many-to-Many ilişkiyi tanımlama
            modelBuilder.Entity<Kullanici>()
                .HasMany(k => k.IlgiAlanlari)
                .WithMany(i => i.Kullanicilar) // IlgiAlani tarafında Kullanicilar koleksiyonu olmalı
                .UsingEntity(j => j.ToTable("KullaniciIlgiAlani")); // Ara tablo ismi

           

            // Many-to-Many ilişki tanımlaması
            modelBuilder.Entity<Katilimci>()
                .HasKey(k => new { k.KullaniciID, k.EtkinlikID }); // Birleşik birincil anahtar

            modelBuilder.Entity<Katilimci>()
              .HasOne(k => k.Kullanici)
              .WithMany(u => u.Katilimcilar)
              .HasForeignKey(k => k.KullaniciID)
              .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Katilimci>()
                .HasOne(k => k.Etkinlik)
                .WithMany(e => e.Katilimcilar)
                .HasForeignKey(k => k.EtkinlikID);

            modelBuilder.Entity<Puan>()
               .HasOne(p => p.Kullanici)
               .WithMany(k => k.Puanlar)  // Kullanıcıya ait puanlar
               .HasForeignKey(p => p.KullaniciID);

            // Mesaj için ilişki
            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Gönderici)
                .WithMany(k => k.GonderdigiMesajlar)  // Kullanıcıya ait gönderilen mesajlar
                .HasForeignKey(m => m.GöndericiID)
                .OnDelete(DeleteBehavior.NoAction); // Yabancı anahtar kısıtlamasında silme işlemi yapılmasın

            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Alici)
                .WithMany(k => k.AldigiMesajlar)  // Kullanıcıya ait alınan mesajlar
                .HasForeignKey(m => m.AliciID)
                .OnDelete(DeleteBehavior.NoAction); // Yabancı anahtar kısıtlamasında silme işlemi yapılmasın

            modelBuilder.Entity<Mesaj>()
                .HasOne(m => m.Etkinlik)
                .WithMany(e => e.Mesajlar) // Bir etkinliğin birden fazla mesajı olabilir
                .HasForeignKey(m => m.EtkinlikID)
                .OnDelete(DeleteBehavior.Cascade); // Etkinlik silindiğinde mesajlar da silinir

            modelBuilder.Entity<KullaniciIlgiAlani>()
        .HasKey(kia => new { kia.KullanicilarID, kia.IlgiAlanlariID });

            modelBuilder.Entity<KullaniciIlgiAlani>()
                .HasOne(kia => kia.Kullanicilar)
                .WithMany(k => k.KullaniciIlgiAlanlari)
                .HasForeignKey(kia => kia.KullanicilarID);

            modelBuilder.Entity<KullaniciIlgiAlani>()
                .HasOne(kia => kia.IlgiAlanlari)
                .WithMany(ia => ia.KullaniciIlgiAlanlari)
                .HasForeignKey(kia => kia.IlgiAlanlariID);



            // Kullanici ile Roller arasında ilişki
            modelBuilder.Entity<Kullanici>()
                .HasOne(k => k.Roller)
                .WithMany(r => r.Kullanicilar)
                .HasForeignKey(k => k.RollerID)
                .OnDelete(DeleteBehavior.NoAction);
            
              modelBuilder.Entity<Etkinlikler>()
                 .HasOne(e => e.Kullanici)
                 .WithMany(k => k.Etkinlikler)
               .HasForeignKey(e => e.KullaniciID)
              .OnDelete(DeleteBehavior.Restrict); 

            base.OnModelCreating(modelBuilder);



        }
    }
   
}
