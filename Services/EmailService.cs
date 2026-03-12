using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace CampRide.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendBookingApprovedAsync(string toEmail, string customerName,
            string caravanName, DateTime fromDate, DateTime toDate)
        {
            var subject = "✅ Foglalásod visszaigazolva – CampRide";
            var days = (toDate - fromDate).Days + 1;
            var body = $@"
                <div style='font-family:sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:linear-gradient(135deg,#3a0647,#05276e);padding:2rem;border-radius:12px 12px 0 0;text-align:center;'>
                        <h1 style='color:white;margin:0;font-size:1.8rem;'>CampRide</h1>
                    </div>
                    <div style='background:white;padding:2rem;border:1px solid #e0e0e0;border-top:none;'>
                        <div style='text-align:center;margin-bottom:1.5rem;'>
                            <div style='width:64px;height:64px;background:#d1e7dd;border-radius:50%;display:inline-flex;align-items:center;justify-content:center;font-size:2rem;'>✅</div>
                        </div>
                        <h2 style='color:#0f5132;text-align:center;'>Foglalásod jóváhagyva!</h2>
                        <p>Kedves <strong>{customerName}</strong>!</p>
                        <p>Örömmel értesítünk, hogy foglalásod visszaigazolásra került.</p>
                        <div style='background:#f8f9fa;border-radius:8px;padding:1.25rem;margin:1.5rem 0;'>
                            <table style='width:100%;border-collapse:collapse;'>
                                <tr><td style='padding:6px 0;color:#666;'>🚐 Jármű:</td><td style='padding:6px 0;font-weight:600;'>{caravanName}</td></tr>
                                <tr><td style='padding:6px 0;color:#666;'>📅 Kezdés:</td><td style='padding:6px 0;font-weight:600;'>{fromDate:yyyy. MM. dd.}</td></tr>
                                <tr><td style='padding:6px 0;color:#666;'>📅 Vége:</td><td style='padding:6px 0;font-weight:600;'>{toDate:yyyy. MM. dd.}</td></tr>
                                <tr><td style='padding:6px 0;color:#666;'>⏱ Időtartam:</td><td style='padding:6px 0;font-weight:600;color:#0d6efd;'>{days} nap</td></tr>
                            </table>
                        </div>
                        <p>Ha kérdésed van, vedd fel velünk a kapcsolatot!</p>
                        <p style='color:#888;font-size:0.85rem;margin-top:2rem;'>Jó utat kívánunk! 🏕️<br/>A CampRide csapat</p>
                    </div>
                </div>";

            await SendAsync(toEmail, subject, body);
        }

        public async Task SendBookingCancelledAsync(string toEmail, string customerName,
            string caravanName, DateTime fromDate, DateTime toDate)
        {
            var subject = "❌ Foglalásod lemondva – CampRide";
            var body = $@"
                <div style='font-family:sans-serif;max-width:600px;margin:0 auto;'>
                    <div style='background:linear-gradient(135deg,#3a0647,#05276e);padding:2rem;border-radius:12px 12px 0 0;text-align:center;'>
                        <h1 style='color:white;margin:0;font-size:1.8rem;'>CampRide</h1>
                    </div>
                    <div style='background:white;padding:2rem;border:1px solid #e0e0e0;border-top:none;'>
                        <h2 style='color:#842029;text-align:center;'>Foglalásod lemondásra került</h2>
                        <p>Kedves <strong>{customerName}</strong>!</p>
                        <p>Sajnálattal értesítünk, hogy az alábbi foglalásod lemondásra került.</p>
                        <div style='background:#f8f9fa;border-radius:8px;padding:1.25rem;margin:1.5rem 0;'>
                            <table style='width:100%;border-collapse:collapse;'>
                                <tr><td style='padding:6px 0;color:#666;'>🚐 Jármű:</td><td style='padding:6px 0;font-weight:600;'>{caravanName}</td></tr>
                                <tr><td style='padding:6px 0;color:#666;'>📅 Kezdés:</td><td style='padding:6px 0;font-weight:600;'>{fromDate:yyyy. MM. dd.}</td></tr>
                                <tr><td style='padding:6px 0;color:#666;'>📅 Vége:</td><td style='padding:6px 0;font-weight:600;'>{toDate:yyyy. MM. dd.}</td></tr>
                            </table>
                        </div>
                        <p>Ha kérdésed van vagy új foglalást szeretnél indítani, látogass el oldalunkra!</p>
                        <p style='color:#888;font-size:0.85rem;margin-top:2rem;'>Üdvözlettel,<br/>A CampRide csapat</p>
                    </div>
                </div>";

            await SendAsync(toEmail, subject, body);
        }

        private async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            var section = _config.GetSection("Email");
            var host = section["SmtpHost"];
            var port = int.Parse(section["SmtpPort"] ?? "587");
            var useSsl = bool.Parse(section["UseSsl"] ?? "false");
            var username = section["Username"];
            var password = section["Password"];
            var fromAddress = section["FromAddress"];
            var fromName = section["FromName"] ?? "CampRide";

            if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(username) ||
                username == "your-email@gmail.com")
            {
                _logger.LogWarning("Email küldés kihagyva: SMTP nincs konfigurálva.");
                return;
            }

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromAddress));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;
                message.Body = new TextPart("html") { Text = htmlBody };

                using var client = new SmtpClient();
                var socketOption = useSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
                await client.ConnectAsync(host, port, socketOption);
                await client.AuthenticateAsync(username, password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email elküldve: {To} – {Subject}", toEmail, subject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Email küldési hiba: {To}", toEmail);
            }
        }
    }
}
