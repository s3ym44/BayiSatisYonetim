using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace BayiSatisYonetim.Services
{
    public class PdfService : IPdfService
    {
        public byte[] GenerateApplicationPdf(string applicationNumber, string customerName, string productName, string address, DateTime date)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Text("Bayi Satış Yönetim Sistemi - Başvuru Formu")
                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken2);

                    page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                    {
                        col.Spacing(10);

                        col.Item().Text($"Başvuru No: {applicationNumber}").SemiBold();
                        col.Item().Text($"Tarih: {date:dd.MM.yyyy}");
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().Text($"Müşteri: {customerName}");
                        col.Item().Text($"Ürün/Tarife: {productName}");
                        col.Item().Text($"Kurulum Adresi: {address}");
                        col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                        col.Item().PaddingTop(20).Text("Bu belge otomatik olarak oluşturulmuştur.")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    page.Footer().AlignCenter().Text(x =>
                    {
                        x.Span("Sayfa ");
                        x.CurrentPageNumber();
                        x.Span(" / ");
                        x.TotalPages();
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
