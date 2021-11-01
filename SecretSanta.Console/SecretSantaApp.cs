using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using SecretSanta.Domain;

namespace SecretSanta.Console
{
    public class SecretSantaApp
    {
        private readonly SecretSantaService _secretSantaService;
        private readonly SmtpSettings _smtpSettings;
        private readonly ILogger<SecretSantaApp> _logger;

        public SecretSantaApp (
                SecretSantaService secretSantaService,
                IConfiguration configuration,
                ILogger<SecretSantaApp> logger)
        {
            _secretSantaService = secretSantaService;
            _smtpSettings = configuration.GetSection("Smtp").Get<SmtpSettings> ();
            _logger = logger;
        }


        public async Task Run (string santasFilePath, CancellationToken cancellationToken)
        {
            _logger.LogInformation ($"Reading santas from {santasFilePath}.");
            var santas = await ReadSantas (santasFilePath, cancellationToken);
            _logger.LogInformation ($"Assigning {santas.Length} santas.");
            var assignments = _secretSantaService.AssignSantas (santas);
            _logger.LogInformation ($"Sending emails for {santas.Length} santas.");
            await SendEmails (assignments, cancellationToken);
            _logger.LogInformation ($"Successfully sent emails for {santas.Length} santas.");
        }

        private async Task<Santa[]> ReadSantas (string path, CancellationToken cancellationToken)
        {
            return (await File.ReadAllLinesAsync (path, cancellationToken))
                    .Map (line => line.Split (","))
                    .Map (line => new Santa { Name = line[0], Email = line[1] })
                    .ToArray();
        }

        private async Task SendEmails (List<(Santa, Santa)> assignments, CancellationToken cancellationToken)
        {
            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync (_smtpSettings.Host, _smtpSettings.Port, SecureSocketOptions.StartTls, cancellationToken);
            await smtpClient.AuthenticateAsync (_smtpSettings.User, _smtpSettings.Password, cancellationToken);
            foreach (var assignment in assignments)
            {
                var mailMessage = new MimeMessage();
                mailMessage.From.Add (new MailboxAddress ("Bro Fixe Admin", _smtpSettings.Sender));
                mailMessage.To.Add (new MailboxAddress (assignment.Item1.Name, assignment.Item1.Email));
                mailMessage.Subject = "Bro Fixe Engerl Bengerl 2021";
                mailMessage.Body = new TextPart ("plain")
                                   {
                                           Text = @$"Hallo {assignment.Item1.Name},

dein Bengerl für dieses Jahr ist {assignment.Item2.Name}.

Viel Spaß beim Beschenken!"
                                   };

                _logger.LogDebug (mailMessage.To[0] + ": " + mailMessage.TextBody);
                // await smtpClient.SendAsync (mailMessage, cancellationToken);
            }

            await smtpClient.DisconnectAsync (true, cancellationToken);
        }
    }
}