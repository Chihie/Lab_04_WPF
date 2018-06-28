using System;
using System.Net.Mail;
using System.Text;
using System.Windows;

namespace Login
{
    //TODO: Handle generating and sending emails in a serious, professional way
    //TODO: At least don't pass password to the sender's email account in this insecure way
    public class GenerateEmail
    {
        private SmtpClient client()
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("appnoreplytestapp@gmail.com","NoReply4$");
            return client;
        }
        
        public void SendEmailRegister(string email, string login)
        {
            MailMessage msg = new MailMessage("donotreply@domain.com", email, "Thank you for your registration!",
                "This is automatically generated email for new user, " + login + ".") {
                BodyEncoding = Encoding.UTF8, DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            try { client().Send(msg); } catch (Exception e) { MessageBox.Show(e.Message); }
        }
        
        public void SendEmailAddUser(string email, string login, string password)
        {
            MailMessage msg = new MailMessage("donotreply@domain.com", email, "You have been registered by admin!",
                "This is automatically generated email for new user, " + login + 
                ".\nYour automatically generated password is '" + password + "'.") {
                BodyEncoding = Encoding.UTF8, DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            try { client().Send(msg); } catch (Exception e) { MessageBox.Show(e.Message); }
        }
        
        public void SendEmailResetPassword(string email, string login, string password)
        {
            MailMessage msg = new MailMessage("donotreply@domain.com", email, "Your password have been changed!",
                "This is automatically generated email for our user, " + login +
                ".\nYour automatically generated password is '" + password + "'.") {
                BodyEncoding = Encoding.UTF8,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            try { client().Send(msg); } catch (Exception e) { MessageBox.Show(e.Message); }
        }

        public void SendEmailPartcipantInfo(string email, string login, string eventName, string attendantType, string approved)
        {
            approved = approved == "true" ? "approved" : "rejected";
            MailMessage msg = new MailMessage("donotreply@domain.com", email, "Administrator " + approved + 
                " your attendace at event.", "This is automatically generated email for our user, " + login +
                ".\nYour attendance as " + attendantType + " at the '" + eventName + "' event has been " + approved +
                " by our administrator.\nYou won't be able to register at this event as this attendant type, " +
                "but you can try to attend as other attendant type or try other event.") {
                BodyEncoding = Encoding.UTF8,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            try { client().Send(msg); } catch (Exception e) { MessageBox.Show(e.Message); }
        }
    }
}