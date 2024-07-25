using BusinessLogic;
using DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using NLog;
using PetaPoco;
using PetaPoco.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace WebApp.Services
{
    public interface IEmailService
    {
        Task<StandardResult> SendForgotPasswordLink(dynamic Data);
    }

    public partial class EmailService : IEmailService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration configuration;
        private readonly string connectionString;
        private readonly SmtpClient smtpClient;

        public EmailService(IConfiguration configuration, SmtpClient smtpClient)
        {
            this.configuration = configuration;
            connectionString = configuration.GetConnectionString("MCS");
            this.smtpClient = smtpClient;
        }

        public async Task<StandardResult> SendForgotPasswordLink(dynamic Data)
        {
            StandardResult result = new StandardResult();

            try
            {
                using (var mailMessage = new MailMessage(
                    from: configuration["Email:EmailAddress:Default"],
                    to: (string)Data.Email))
                {
                    mailMessage.Subject = "Reset Password - SmartMining";

                    var msg = "";
                    var fileTemplate = configuration["Email:ResetPassword:Template"];
                    if (File.Exists(fileTemplate))
                    {
                        msg = File.ReadAllText(fileTemplate);
                        msg = msg?.Replace("{{reset-password-url}}", (string)Data.Url);
                    }
                    else
                    {
                        logger.Error($"File template {fileTemplate} does not exist");
                    }

                    // Send email
                    try
                    {
                        if (!string.IsNullOrEmpty(msg))
                        {
                            mailMessage.Body = msg;
                            mailMessage.IsBodyHtml = true;

                            await smtpClient.SendMailAsync(mailMessage);
                            result.Success = true;
                        }
                        else
                        {
                            result.Message = "Email template is empty";
                            logger.Error(result.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex.ToString());
                        result.Message = ex.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
                result.Message = ex.Message;
            }

            return result;
        }
    }
}
