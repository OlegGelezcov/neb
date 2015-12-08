using System;
using System.Net;
using System.Net.Mail;

namespace ServerClientCommon {
    public class EmailSender {

        private string mSmtpServer;
        private int mSmtpPort;
        private string mSourceUserMail;
        private string mSourceUserPassword;

        public EmailSender(string smtpServer, int smtpServerPort, string sourceUserMail, string sourceUserPassword) {
            mSmtpServer = smtpServer;
            mSmtpPort = smtpServerPort;
            mSourceUserMail = sourceUserMail;
            mSourceUserPassword = sourceUserPassword;
        }

        public bool SendMessage(string email, string subject, string body) {
            bool success = true;
            try {
                SmtpClient server = new SmtpClient(mSmtpServer, mSmtpPort);
                server.Credentials = new NetworkCredential(mSourceUserMail, mSourceUserPassword);
                server.EnableSsl = true;

                MailMessage message = new MailMessage {
                    From = new MailAddress(mSourceUserMail),
                    Subject = subject,
                    Body = body
                };
                message.To.Add(new MailAddress(email));
                server.Send(message);
                success = true;
            } catch (Exception exception) {
                success = false;
            }
            return success;
        }
    }
}
