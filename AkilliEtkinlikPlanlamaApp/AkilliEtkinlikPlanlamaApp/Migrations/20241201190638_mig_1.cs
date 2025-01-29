using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AkilliEtkinlikPlanlamaApp.Migrations
{
    /// <inheritdoc />
    public partial class mig_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IlgiAlanlari",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IlgiAlaniAdi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IlgiAlanlari", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Roller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Sifre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SifreOnay = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Konum = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    IlgiAlanlariID = table.Column<int>(type: "int", nullable: false),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Cinsiyet = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TelefonNumarasi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProfilFotografiYolu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RollerID = table.Column<int>(type: "int", nullable: true),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    VerificationToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConfirmCode = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Roller_RollerID",
                        column: x => x.RollerID,
                        principalTable: "Roller",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Etkinlikler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EtkinlikAdi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Saat = table.Column<TimeSpan>(type: "time", nullable: false),
                    Konum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kategori = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EtkinlikFotoYolu = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    KullaniciID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Etkinlikler", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Etkinlikler_Kullanicilar_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciIlgiAlani",
                columns: table => new
                {
                    IlgiAlanlariID = table.Column<int>(type: "int", nullable: false),
                    KullanicilarID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciIlgiAlani", x => new { x.IlgiAlanlariID, x.KullanicilarID });
                    table.ForeignKey(
                        name: "FK_KullaniciIlgiAlani_IlgiAlanlari_IlgiAlanlariID",
                        column: x => x.IlgiAlanlariID,
                        principalTable: "IlgiAlanlari",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciIlgiAlani_Kullanicilar_KullanicilarID",
                        column: x => x.KullanicilarID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KullaniciIlgiAlanlari",
                columns: table => new
                {
                    KullanicilarID = table.Column<int>(type: "int", nullable: false),
                    IlgiAlanlariID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KullaniciIlgiAlanlari", x => new { x.KullanicilarID, x.IlgiAlanlariID });
                    table.ForeignKey(
                        name: "FK_KullaniciIlgiAlanlari_IlgiAlanlari_IlgiAlanlariID",
                        column: x => x.IlgiAlanlariID,
                        principalTable: "IlgiAlanlari",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_KullaniciIlgiAlanlari_Kullanicilar_KullanicilarID",
                        column: x => x.KullanicilarID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Puanlar",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KullaniciID = table.Column<int>(type: "int", nullable: false),
                    PuanDegeri = table.Column<int>(type: "int", nullable: false),
                    KazanilanTarih = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Puanlar", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Puanlar_Kullanicilar_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Katilimcilar",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "int", nullable: false),
                    EtkinlikID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Katilimcilar", x => new { x.KullaniciID, x.EtkinlikID });
                    table.ForeignKey(
                        name: "FK_Katilimcilar_Etkinlikler_EtkinlikID",
                        column: x => x.EtkinlikID,
                        principalTable: "Etkinlikler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Katilimcilar_Kullanicilar_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Mesajlar",
                columns: table => new
                {
                    MesajID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GöndericiID = table.Column<int>(type: "int", nullable: true),
                    AliciID = table.Column<int>(type: "int", nullable: true),
                    MesajMetni = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GonderimZamani = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EtkinlikID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mesajlar", x => x.MesajID);
                    table.ForeignKey(
                        name: "FK_Mesajlar_Etkinlikler_EtkinlikID",
                        column: x => x.EtkinlikID,
                        principalTable: "Etkinlikler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Mesajlar_Kullanicilar_AliciID",
                        column: x => x.AliciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Mesajlar_Kullanicilar_GöndericiID",
                        column: x => x.GöndericiID,
                        principalTable: "Kullanicilar",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Etkinlikler_KullaniciID",
                table: "Etkinlikler",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Katilimcilar_EtkinlikID",
                table: "Katilimcilar",
                column: "EtkinlikID");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciIlgiAlani_KullanicilarID",
                table: "KullaniciIlgiAlani",
                column: "KullanicilarID");

            migrationBuilder.CreateIndex(
                name: "IX_KullaniciIlgiAlanlari_IlgiAlanlariID",
                table: "KullaniciIlgiAlanlari",
                column: "IlgiAlanlariID");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_RollerID",
                table: "Kullanicilar",
                column: "RollerID");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_AliciID",
                table: "Mesajlar",
                column: "AliciID");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_EtkinlikID",
                table: "Mesajlar",
                column: "EtkinlikID");

            migrationBuilder.CreateIndex(
                name: "IX_Mesajlar_GöndericiID",
                table: "Mesajlar",
                column: "GöndericiID");

            migrationBuilder.CreateIndex(
                name: "IX_Puanlar_KullaniciID",
                table: "Puanlar",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Katilimcilar");

            migrationBuilder.DropTable(
                name: "KullaniciIlgiAlani");

            migrationBuilder.DropTable(
                name: "KullaniciIlgiAlanlari");

            migrationBuilder.DropTable(
                name: "Mesajlar");

            migrationBuilder.DropTable(
                name: "Puanlar");

            migrationBuilder.DropTable(
                name: "IlgiAlanlari");

            migrationBuilder.DropTable(
                name: "Etkinlikler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "Roller");
        }
    }
}
