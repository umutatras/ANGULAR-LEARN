using BlogCore.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;

namespace BlogCore.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HelperController : ControllerBase
    {
        private readonly BlogDbContext _context;

        public HelperController(BlogDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public IActionResult SendContactEMail(Contact contact)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                SmtpClient smtpClient = new SmtpClient("umutatras@gmail.com");
                mailMessage.From = new MailAddress("umutatras@gmail.com");
                mailMessage.To.Add("umutatras@gmail.com");
                mailMessage.Subject = contact.Subject;
                mailMessage.Body = contact.Message;
                mailMessage.IsBodyHtml = true;
                smtpClient.Port = 123;
                smtpClient.Credentials = new NetworkCredential("mail", "sifre");
                smtpClient.Send(mailMessage);

                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
       
        }
    }
}
