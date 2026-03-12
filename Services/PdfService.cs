using CampRide.Data;
using CampRide.Data.Entities;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CampRide.Services
{
    public class PdfService
    {
        private readonly AppDbContext _context;

        public PdfService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<byte[]?> GenerateBookingPdfAsync(int bookingId, string userId)
        {
            var booking = await _context.Bookings
                .Include(b => b.Caravan)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.UserId == userId && b.Status == BookingStatus.Approved);

            if (booking == null)
                return null;

            var days = (booking.ToDate - booking.FromDate).Days + 1;
            var totalPrice = days * (booking.Caravan?.PricePerDay ?? 0);

            var doc = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Element(ComposeHeader);

                    page.Content().Element(content =>
                    {
                        content.Column(col =>
                        {
                            col.Spacing(12);

                            col.Item().Text("FOGLALÁSI VISSZAIGAZOLÓ")
                                .FontSize(20).Bold().FontColor(Color.FromHex("#3a0647"));

                            col.Item().Text($"Foglalás azonosítója: #{booking.Id}")
                                .FontSize(11).FontColor(Colors.Grey.Medium);

                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(c =>
                                {
                                    c.RelativeColumn(1);
                                    c.RelativeColumn(2);
                                });

                                void Row(string label, string value)
                                {
                                    table.Cell().Padding(4).Text(label).SemiBold().FontColor(Colors.Grey.Darken1);
                                    table.Cell().Padding(4).Text(value);
                                }

                                Row("Ügyfél neve:", booking.CustomerName);
                                Row("Email:", booking.Email);
                                Row("Telefonszám:", booking.Phone);
                                Row("Lakóautó:", booking.Caravan?.Name ?? "–");
                                Row("Kezdés:", booking.FromDate.ToString("yyyy. MM. dd."));
                                Row("Vége:", booking.ToDate.ToString("yyyy. MM. dd."));
                                Row("Időtartam:", $"{days} nap");
                                if (totalPrice > 0)
                                    Row("Összeg:", $"{totalPrice:N0} Ft");
                                if (!string.IsNullOrWhiteSpace(booking.Notes))
                                    Row("Megjegyzés:", booking.Notes);
                            });

                            col.Item().LineHorizontal(1).LineColor(Colors.Grey.Lighten2);

                            col.Item().Background(Color.FromHex("#f0faf4"))
                                .Padding(12)
                                .Text("✅ A foglalás visszaigazolva.")
                                .FontColor(Color.FromHex("#0f5132")).SemiBold();

                            col.Item().Text("Köszönjük, hogy a CampRide-ot választotta!")
                                .FontSize(11).FontColor(Colors.Grey.Medium);
                        });
                    });

                    page.Footer().AlignCenter()
                        .Text(x =>
                        {
                            x.Span("CampRide – Foglalási visszaigazoló | Generálva: ");
                            x.Span(DateTime.Now.ToString("yyyy. MM. dd. HH:mm")).SemiBold();
                        });
                });
            });

            return doc.GeneratePdf();
        }

        private void ComposeHeader(IContainer container)
        {
            container.Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("CampRide").FontSize(24).Bold().FontColor(Color.FromHex("#3a0647"));
                    col.Item().Text("Prémium lakóautó bérlés").FontSize(11).FontColor(Colors.Grey.Medium);
                });
            });
        }
    }
}
