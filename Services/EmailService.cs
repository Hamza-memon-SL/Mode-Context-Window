using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Azure.Identity;
using Microsoft.Graph;
using Microsoft.Graph.Users.Item.SendMail;
using Microsoft.Graph.Models;
using GenAiPoc.Core.Interfaces.IService;

namespace GenAiPoc.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<CalculateCostService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<CalculateCostService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            bool emailSent = false;
            try
            {
                //string? from = this._configuration.GetSection("EmailConfiguration").GetSection("From").Value;
                //string? host = this._configuration.GetSection("EmailConfiguration").GetSection("SmtpServer").Value;
                //string? port = this._configuration.GetSection("EmailConfiguration").GetSection("Port").Value;
                //string? user = this._configuration.GetSection("EmailConfiguration").GetSection("Username").Value;
                //string? password = this._configuration.GetSection("EmailConfiguration").GetSection("Password").Value;

                //System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
                //message.To.Add(to);
                //message.From = new System.Net.Mail.MailAddress(from);
                //message.Subject = subject;
                //message.Body = body;
                //message.IsBodyHtml = true;

                //using (SmtpClient client = new SmtpClient(host, Convert.ToInt32(port)))
                //{
                //    client.EnableSsl = true;
                //    //client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                //    //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                //    //client.UseDefaultCredentials = false;
                //    client.Credentials = new NetworkCredential(user, password);

                //    client.Send(message);
                //};



                //Using Graph

                string? tenantId = this._configuration.GetSection("EmailConfiguration").GetSection("TenantID").Value;
                string? clientId = this._configuration.GetSection("EmailConfiguration").GetSection("ClientID").Value;
                string? clientSecret = this._configuration.GetSection("EmailConfiguration").GetSection("ClientSecret").Value;
                string? senderEmail = this._configuration.GetSection("EmailConfiguration").GetSection("SenderMail").Value;

                ClientSecretCredential credential = new(tenantId, clientId, clientSecret);
                GraphServiceClient graphClient = new(credential);


                var requestBody = new SendMailPostRequestBody
                {
                    Message = new Message
                    {
                        Subject = subject,
                        Body = new ItemBody
                        {
                            ContentType = BodyType.Html,
                            Content = body
                        },

                        ToRecipients = new List<Recipient>
                        {
                            new Recipient
                            {
                                EmailAddress = new EmailAddress
                                {
                                    Address = to
                                }
                            }
                        }
                    },
                    SaveToSentItems = false
                };
                emailSent = true;
                await graphClient.Users[senderEmail].SendMail.PostAsync(requestBody);

                return await Task.FromResult(emailSent);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(false);
            }
        }

    }
}
