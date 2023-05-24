using API.DTOs;
using Domain;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using Persistence;

namespace API.Services
{
    public class EmailService : IEmailService
    {
        // EmailSettings _emailSettings = null;
        private readonly DataContext _context;
        private readonly IConfiguration _config;
        public EmailService( DataContext context, IConfiguration config)
        {
            this._config = config;
            this._context = context;
            // _emailSettings = options.Value;
        }
        public bool SendEmail(EmailData emailData, int flag=1)
        {
            //fetch the email values from the db
            var type = EmailTypeOptions.AUTHENTICATION;
            if (flag==2){
                type= EmailTypeOptions.NOTIFICATION;
            }else if(flag==3){
                type = EmailTypeOptions.REPLY;
            }
            var email_Db = _context.SmtpConfig.Where(x=>x.email_type == type).ToList();
            if(email_Db.Count==0){
                // Console.WriteLine("SMTP Config details are not found over DB");
                return false;
            }


            //fetch the count of the mail sent today
            var tracker_count = _context.TrackerEmail.Where(x => x.created_at >= DateTime.Today & x.status == TrackerEmailStatus.SENT).ToList().Count;


            //check if the email limit exceeded 
            if(email_Db[0].max_emails_per_day <= tracker_count){
                // Console.WriteLine("Email Limit exceeded");
                return false;
            }
            
            var EmailId = email_Db[0].email_id ?? _config["EmailId"];
            var Password = email_Db[0].password ?? _config["EmailPassword"];
            var Name = _config["EmailName"];
            var Host=_config["EmailHost"];
            var Port=int.Parse(_config["EmailPort"]);
            var UseSSL = true;



            //send the mail using above credentials
            try
            {
                MimeMessage emailMessage = new MimeMessage();
                MailboxAddress emailFrom = new MailboxAddress(Name, EmailId);
                emailMessage.From.Add(emailFrom);
                MailboxAddress emailTo = new MailboxAddress(emailData.EmailToName, emailData.EmailToId);
                emailMessage.To.Add(emailTo);
                emailMessage.Subject = emailData.EmailSubject;
                BodyBuilder emailBodyBuilder = new BodyBuilder();
                emailBodyBuilder.TextBody = emailData.EmailBody;
                emailMessage.Body = emailBodyBuilder.ToMessageBody();
                SmtpClient emailClient = new SmtpClient();
                emailClient.Connect(Host, Port, UseSSL);
                emailClient.Authenticate(EmailId, Password);
                emailClient.Send(emailMessage);
                emailClient.Disconnect(true);
                emailClient.Dispose();
                return true;
            }
            catch(Exception ex)
            {
                //Log Exception Details
                // Console.WriteLine(ex);
                return false;
            }
        }
    }
}