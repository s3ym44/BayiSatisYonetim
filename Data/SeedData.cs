using Microsoft.AspNetCore.Identity;
using BayiSatisYonetim.Models.Entities;
using BayiSatisYonetim.Models.Enums;

namespace BayiSatisYonetim.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Rolleri oluştur
            string[] roles = { "Admin", "Dealer", "Customer" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Admin kullanıcı
            if (await userManager.FindByEmailAsync("admin@bayisatis.com") == null)
            {
                var admin = new AppUser
                {
                    UserName = "admin@bayisatis.com",
                    Email = "admin@bayisatis.com",
                    FullName = "Sistem Yöneticisi",
                    PhoneNumber = "05001112233",
                    Role = UserRole.Admin,
                    IsActive = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(admin, "Admin123!");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Bayi 1
            AppUser? bayi1User = await userManager.FindByEmailAsync("bayi1@bayisatis.com");
            if (bayi1User == null)
            {
                bayi1User = new AppUser
                {
                    UserName = "bayi1@bayisatis.com",
                    Email = "bayi1@bayisatis.com",
                    FullName = "Ahmet Yılmaz",
                    PhoneNumber = "05321112233",
                    Role = UserRole.Dealer,
                    IsActive = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(bayi1User, "Bayi123!");
                await userManager.AddToRoleAsync(bayi1User, "Dealer");
            }

            // Bayi 2
            AppUser? bayi2User = await userManager.FindByEmailAsync("bayi2@bayisatis.com");
            if (bayi2User == null)
            {
                bayi2User = new AppUser
                {
                    UserName = "bayi2@bayisatis.com",
                    Email = "bayi2@bayisatis.com",
                    FullName = "Mehmet Demir",
                    PhoneNumber = "05421112233",
                    Role = UserRole.Dealer,
                    IsActive = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(bayi2User, "Bayi123!");
                await userManager.AddToRoleAsync(bayi2User, "Dealer");
            }

            // Müşteri 1
            AppUser? musteri1User = await userManager.FindByEmailAsync("musteri1@bayisatis.com");
            if (musteri1User == null)
            {
                musteri1User = new AppUser
                {
                    UserName = "musteri1@bayisatis.com",
                    Email = "musteri1@bayisatis.com",
                    FullName = "Ayşe Kaya",
                    PhoneNumber = "05551112233",
                    Role = UserRole.Customer,
                    IsActive = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(musteri1User, "Musteri123!");
                await userManager.AddToRoleAsync(musteri1User, "Customer");
            }

            // Müşteri 2
            AppUser? musteri2User = await userManager.FindByEmailAsync("musteri2@bayisatis.com");
            if (musteri2User == null)
            {
                musteri2User = new AppUser
                {
                    UserName = "musteri2@bayisatis.com",
                    Email = "musteri2@bayisatis.com",
                    FullName = "Fatma Çelik",
                    PhoneNumber = "05551113344",
                    Role = UserRole.Customer,
                    IsActive = true,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(musteri2User, "Musteri123!");
                await userManager.AddToRoleAsync(musteri2User, "Customer");
            }

            // Dealer kayıtları
            if (!context.Dealers.Any())
            {
                var dealer1 = new Dealer
                {
                    UserId = bayi1User!.Id,
                    CompanyName = "Yılmaz Teknoloji Ltd.",
                    TaxNumber = "1234567890",
                    Address = "Atatürk Cad. No:15",
                    City = "İstanbul",
                    BankName = "Ziraat Bankası",
                    IBAN = "TR330006100519786457841326",
                    CommissionRate = 12,
                    Status = DealerStatus.Approved,
                    ApprovalDate = DateTime.UtcNow.AddDays(-30)
                };
                var dealer2 = new Dealer
                {
                    UserId = bayi2User!.Id,
                    CompanyName = "Demir İletişim A.Ş.",
                    TaxNumber = "9876543210",
                    Address = "Cumhuriyet Blv. No:42",
                    City = "Ankara",
                    BankName = "İş Bankası",
                    IBAN = "TR760006400000168736214789",
                    CommissionRate = 10,
                    Status = DealerStatus.Approved,
                    ApprovalDate = DateTime.UtcNow.AddDays(-20)
                };
                context.Dealers.AddRange(dealer1, dealer2);
                await context.SaveChangesAsync();
            }

            // Customer kayıtları
            if (!context.Customers.Any())
            {
                var customer1 = new Customer
                {
                    UserId = musteri1User!.Id,
                    TCKimlik = "12345678901",
                    Address = "Bağdat Cad. No:100",
                    City = "İstanbul",
                    BirthDate = new DateTime(1990, 5, 15)
                };
                var customer2 = new Customer
                {
                    UserId = musteri2User!.Id,
                    TCKimlik = "98765432109",
                    Address = "Kızılay Mah. No:25",
                    City = "Ankara",
                    BirthDate = new DateTime(1985, 8, 22)
                };
                context.Customers.AddRange(customer1, customer2);
                await context.SaveChangesAsync();
            }

            // Kategoriler
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new() { Name = "İnternet Paketleri", Description = "Fiber ve ADSL internet paketleri", IsActive = true, SortOrder = 1 },
                    new() { Name = "Alarm Sistemleri", Description = "Ev ve iş yeri alarm sistemleri", IsActive = true, SortOrder = 2 },
                    new() { Name = "Ek Hizmetler", Description = "Ek hizmetler ve çözümler", IsActive = true, SortOrder = 3 }
                };
                context.Categories.AddRange(categories);
                await context.SaveChangesAsync();
            }

            // Firmalar
            if (!context.Companies.Any())
            {
                var companies = new List<Company>
                {
                    new() { Name = "TurkNet", IsActive = true },
                    new() { Name = "Superonline", IsActive = true },
                    new() { Name = "TTNet", IsActive = true },
                    new() { Name = "Türk Telekom", IsActive = true },
                    new() { Name = "Vodafone Net", IsActive = true }
                };
                context.Companies.AddRange(companies);
                await context.SaveChangesAsync();
            }

            // Ürünler
            if (!context.Products.Any())
            {
                var cat1 = context.Categories.First(c => c.Name == "İnternet Paketleri");
                var cat2 = context.Categories.First(c => c.Name == "Alarm Sistemleri");
                var cat3 = context.Categories.First(c => c.Name == "Ek Hizmetler");
                var turknet = context.Companies.First(c => c.Name == "TurkNet");
                var superonline = context.Companies.First(c => c.Name == "Superonline");
                var ttnet = context.Companies.First(c => c.Name == "TTNet");
                var turktelekom = context.Companies.First(c => c.Name == "Türk Telekom");
                var vodafone = context.Companies.First(c => c.Name == "Vodafone Net");

                var products = new List<Product>
                {
                    new() { CategoryId = cat1.Id, CompanyId = turknet.Id, Name = "TurkNet 50Mbps", Speed = "50 Mbps", Quota = "Limitsiz", Price = 299, ContractDuration = 24, SortOrder = 1 },
                    new() { CategoryId = cat1.Id, CompanyId = turknet.Id, Name = "TurkNet 100Mbps Limitsiz", Speed = "100 Mbps", Quota = "Limitsiz", Price = 399, ContractDuration = 24, SortOrder = 2 },
                    new() { CategoryId = cat1.Id, CompanyId = superonline.Id, Name = "Superonline 100Mbps", Speed = "100 Mbps", Quota = "Limitsiz", Price = 449, ContractDuration = 24, SortOrder = 3 },
                    new() { CategoryId = cat1.Id, CompanyId = superonline.Id, Name = "Superonline 200Mbps", Speed = "200 Mbps", Quota = "Limitsiz", Price = 599, ContractDuration = 24, SortOrder = 4 },
                    new() { CategoryId = cat1.Id, CompanyId = ttnet.Id, Name = "TTNet 50Mbps", Speed = "50 Mbps", Quota = "Limitsiz", Price = 349, ContractDuration = 24, SortOrder = 5 },
                    new() { CategoryId = cat1.Id, CompanyId = turktelekom.Id, Name = "Türk Telekom 100Mbps Fiber", Speed = "100 Mbps", Quota = "Limitsiz", Price = 429, ContractDuration = 24, SortOrder = 6 },
                    new() { CategoryId = cat1.Id, CompanyId = vodafone.Id, Name = "Vodafone Net 100Mbps", Speed = "100 Mbps", Quota = "Limitsiz", Price = 379, ContractDuration = 24, SortOrder = 7 },
                    new() { CategoryId = cat2.Id, CompanyId = turktelekom.Id, Name = "Alarm Paketi Temel", Description = "4 sensör + 1 kontrol paneli", Price = 199, ContractDuration = 12, SortOrder = 1 },
                    new() { CategoryId = cat2.Id, CompanyId = turktelekom.Id, Name = "Alarm Paketi Pro", Description = "8 sensör + 2 kontrol paneli + kamera", Price = 349, ContractDuration = 12, SortOrder = 2 },
                    new() { CategoryId = cat2.Id, CompanyId = vodafone.Id, Name = "Akıllı Ev Güvenlik", Description = "Akıllı sensörler + mobil uygulama", Price = 449, ContractDuration = 12, SortOrder = 3 },
                    new() { CategoryId = cat3.Id, CompanyId = turknet.Id, Name = "Statik IP Hizmeti", Description = "Sabit IP adresi tahsisi", Price = 49, SortOrder = 1 },
                    new() { CategoryId = cat3.Id, CompanyId = superonline.Id, Name = "Modem Kurulum Hizmeti", Description = "Profesyonel kurulum hizmeti", Price = 99, SortOrder = 2 }
                };
                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }

            // Başvurular
            if (!context.Applications.Any())
            {
                var dealer1 = context.Dealers.First();
                var dealer2 = context.Dealers.Skip(1).First();
                var customer1 = context.Customers.First();
                var customer2 = context.Customers.Skip(1).First();
                var products = context.Products.ToList();

                var applications = new List<Application>
                {
                    new()
                    {
                        ApplicationNumber = "BSV-2026-00001",
                        CustomerId = customer1.Id,
                        DealerId = dealer1.Id,
                        ProductId = products[0].Id,
                        Status = ApplicationStatus.Completed,
                        InstallationAddress = "Bağdat Cad. No:100",
                        InstallationCity = "İstanbul",
                        Notes = "Acil kurulum isteniyor",
                        CreatedAt = DateTime.UtcNow.AddDays(-25)
                    },
                    new()
                    {
                        ApplicationNumber = "BSV-2026-00002",
                        CustomerId = customer1.Id,
                        DealerId = dealer1.Id,
                        ProductId = products[7].Id,
                        Status = ApplicationStatus.Approved,
                        InstallationAddress = "Bağdat Cad. No:100",
                        InstallationCity = "İstanbul",
                        CreatedAt = DateTime.UtcNow.AddDays(-20)
                    },
                    new()
                    {
                        ApplicationNumber = "BSV-2026-00003",
                        CustomerId = customer2.Id,
                        DealerId = dealer2.Id,
                        ProductId = products[2].Id,
                        Status = ApplicationStatus.Completed,
                        InstallationAddress = "Kızılay Mah. No:25",
                        InstallationCity = "Ankara",
                        CreatedAt = DateTime.UtcNow.AddDays(-15)
                    },
                    new()
                    {
                        ApplicationNumber = "BSV-2026-00004",
                        CustomerId = customer2.Id,
                        DealerId = dealer2.Id,
                        ProductId = products[4].Id,
                        Status = ApplicationStatus.InReview,
                        InstallationAddress = "Kızılay Mah. No:25",
                        InstallationCity = "Ankara",
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new()
                    {
                        ApplicationNumber = "BSV-2026-00005",
                        CustomerId = customer1.Id,
                        ProductId = products[1].Id,
                        Status = ApplicationStatus.Pending,
                        InstallationAddress = "Kadıköy Mah. No:50",
                        InstallationCity = "İstanbul",
                        Notes = "Bireysel başvuru",
                        CreatedAt = DateTime.UtcNow.AddDays(-2)
                    },
                    new()
                    {
                        ApplicationNumber = "BSV-2026-00006",
                        CustomerId = customer2.Id,
                        DealerId = dealer1.Id,
                        ProductId = products[3].Id,
                        Status = ApplicationStatus.Completed,
                        InstallationAddress = "Çankaya Mah. No:10",
                        InstallationCity = "Ankara",
                        CreatedAt = DateTime.UtcNow.AddDays(-10)
                    }
                };
                context.Applications.AddRange(applications);
                await context.SaveChangesAsync();
            }

            // Satışlar
            if (!context.Sales.Any())
            {
                var completedApps = context.Applications
                    .Where(a => a.Status == ApplicationStatus.Completed && a.DealerId != null)
                    .ToList();

                foreach (var app in completedApps)
                {
                    var dealer = context.Dealers.First(d => d.Id == app.DealerId);
                    var product = context.Products.First(p => p.Id == app.ProductId);
                    var commission = Math.Round(product.Price * dealer.CommissionRate / 100, 2);

                    context.Sales.Add(new Sale
                    {
                        ApplicationId = app.Id,
                        DealerId = dealer.Id,
                        ProductId = product.Id,
                        SaleDate = app.CreatedAt.AddDays(3),
                        Amount = product.Price,
                        CommissionAmount = commission,
                        CommissionStatus = CommissionStatus.Paid,
                        PaymentDate = app.CreatedAt.AddDays(30)
                    });
                }

                // Bir tane de ödenmemiş komisyon
                var approvedApp = context.Applications.FirstOrDefault(a => a.Status == ApplicationStatus.Approved && a.DealerId != null);
                if (approvedApp != null)
                {
                    var dealer = context.Dealers.First(d => d.Id == approvedApp.DealerId);
                    var product = context.Products.First(p => p.Id == approvedApp.ProductId);
                    var commission = Math.Round(product.Price * dealer.CommissionRate / 100, 2);

                    context.Sales.Add(new Sale
                    {
                        ApplicationId = approvedApp.Id,
                        DealerId = dealer.Id,
                        ProductId = product.Id,
                        SaleDate = DateTime.UtcNow.AddDays(-5),
                        Amount = product.Price,
                        CommissionAmount = commission,
                        CommissionStatus = CommissionStatus.Pending
                    });
                }

                await context.SaveChangesAsync();
            }

            // Duyurular
            if (!context.Announcements.Any())
            {
                var announcements = new List<Announcement>
                {
                    new()
                    {
                        Title = "Yeni Komisyon Oranları Güncellendi",
                        Content = "Değerli bayilerimiz, 2026 yılı komisyon oranları güncellenmiştir. Detaylar için profil sayfanızı kontrol ediniz.",
                        IsPopup = true,
                        IsActive = true,
                        TargetRole = AnnouncementTarget.DealersOnly,
                        CreatedAt = DateTime.UtcNow.AddDays(-5)
                    },
                    new()
                    {
                        Title = "Sistem Bakımı - 15 Şubat",
                        Content = "15 Şubat 2026 tarihinde 02:00-04:00 saatleri arasında sistem bakımı yapılacaktır.",
                        IsPopup = false,
                        IsActive = true,
                        TargetRole = AnnouncementTarget.All,
                        CreatedAt = DateTime.UtcNow.AddDays(-3)
                    },
                    new()
                    {
                        Title = "Yeni Ürünler Eklendi",
                        Content = "Alarm sistemleri kategorisine yeni ürünler eklenmiştir. Hemen inceleyin!",
                        IsPopup = true,
                        IsActive = true,
                        TargetRole = AnnouncementTarget.CustomersOnly,
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                };
                context.Announcements.AddRange(announcements);
                await context.SaveChangesAsync();
            }

            // Activity Logs
            if (!context.ActivityLogs.Any())
            {
                var adminUser = await userManager.FindByEmailAsync("admin@bayisatis.com");
                var logs = new List<ActivityLog>
                {
                    new() { UserId = adminUser!.Id, Action = "Giriş", Detail = "Admin sisteme giriş yaptı", CreatedAt = DateTime.UtcNow.AddDays(-5) },
                    new() { UserId = bayi1User!.Id, Action = "Giriş", Detail = "Bayi sisteme giriş yaptı", CreatedAt = DateTime.UtcNow.AddDays(-4) },
                    new() { UserId = bayi1User.Id, Action = "Başvuru Oluşturma", Detail = "BSV-2026-00001 numaralı başvuru oluşturuldu", CreatedAt = DateTime.UtcNow.AddDays(-3) },
                    new() { UserId = adminUser.Id, Action = "Başvuru Onay", Detail = "BSV-2026-00001 numaralı başvuru onaylandı", CreatedAt = DateTime.UtcNow.AddDays(-2) },
                    new() { UserId = musteri1User!.Id, Action = "Giriş", Detail = "Müşteri sisteme giriş yaptı", CreatedAt = DateTime.UtcNow.AddDays(-1) }
                };
                context.ActivityLogs.AddRange(logs);
                await context.SaveChangesAsync();
            }
        }
    }
}
