using DataAccess.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using PetaPoco;
using PetaPoco.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Omu.ValueInjecter;
using System.Net.Mail;

namespace MCSWebApp.Workers
{
    public partial class EmailWorker : BackgroundService
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly IConfiguration configuration;
        private readonly string connectionString;        

        public static int MAX_ATTEMPT = 3;

        public EmailWorker(IConfiguration configuration)
        {
            this.configuration = configuration;
            connectionString = configuration.GetConnectionString("MCS");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            logger.Info($"EmailWorker is running at: {DateTime.Now}");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var db = DatabaseConfiguration
                        .Build()
                        .UsingConnectionString(connectionString)
                        .UsingProvider<PostgreSQLDatabaseProvider>()
                        .Create())
                    {
                        using (var smtpClient = new SmtpClient()
                        {
                            Host = configuration.GetValue<string>("Email:Smtp:Host"),
                            Port = configuration.GetValue<int>("Email:Smtp:Port"),
                            Credentials = new NetworkCredential
                                (
                                    configuration.GetValue<string>("Email:Smtp:Username"),
                                    configuration.GetValue<string>("Email:Smtp:Password")
                                ),
                            EnableSsl = configuration.GetValue<bool>("Email:Smtp:EnableSsl")
                        })
                        {
                            try
                            {
                                var sql = Sql.Builder.Append("SELECT * FROM email_notification");
                                sql.Append("WHERE delivered_on IS NULL");
                                sql.Append("AND COALESCE(delivery_schedule, created_on) > @0", DateTime.Now.AddMonths(-1));
                                sql.Append("AND COALESCE(delivery_schedule, created_on) <= @0", DateTime.Now);
                                sql.Append("AND COALESCE(attempt_count, 0) < @0", MAX_ATTEMPT);
                                sql.Append("AND recipients IS NOT NULL");
                                var emailNotifications = await db.FetchAsync<email_notification>(stoppingToken, sql);

                                if (emailNotifications != null && emailNotifications.Count > 0)
                                {
                                    foreach (var emailNotification in emailNotifications)
                                    {
                                        if (stoppingToken.IsCancellationRequested)
                                            break;

                                        string var1 = "", var2 = "", var3 = "", var4 = "", var5 = "", var6 = "", var7 = "", var8 = "";

                                        var table_name = emailNotification.table_name;
                                        var fields_name = emailNotification.fields;
                                        var criteria = emailNotification.criteria;

                                        var afields = fields_name.Split(',');
                                        fields_name = "";
                                        int num = 1;
                                        foreach (string f in afields)
                                        {
                                            fields_name += f.Trim() + $" as var{num},";
                                            num++;
                                        }
                                        fields_name = fields_name.Substring(0, fields_name.Length - 1);

                                        var qry = $"select id, {fields_name} from {table_name} where {criteria}";
                                        var anytbl = await db.FetchAsync<dynamic>(qry);
                                        foreach (var brs in anytbl)
                                        {
                                            try
                                            {
                                                var1 = Convert.ToString(brs.var1);
                                                if (num > 2) var2 = Convert.ToString(brs.var2);
                                                if (num > 3) var3 = Convert.ToString(brs.var3);
                                                if (num > 4) var4 = Convert.ToString(brs.var4);
                                                if (num > 5) var5 = Convert.ToString(brs.var5);
                                                if (num > 6) var6 = Convert.ToString(brs.var6);
                                                if (num > 7) var7 = Convert.ToString(brs.var7);
                                                if (num > 8) var8 = Convert.ToString(brs.var8);
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error(ex.ToString());
                                            }
                                        }

                                        var email_body = emailNotification.email_content.Replace("{var1}", var1);
                                        if (!string.IsNullOrEmpty(var2)) email_body = email_body.Replace("{var2}", var2);
                                        if (!string.IsNullOrEmpty(var3)) email_body = email_body.Replace("{var3}", var3);
                                        if (!string.IsNullOrEmpty(var4)) email_body = email_body.Replace("{var4}", var4);
                                        if (!string.IsNullOrEmpty(var5)) email_body = email_body.Replace("{var5}", var5);
                                        if (!string.IsNullOrEmpty(var6)) email_body = email_body.Replace("{var6}", var6);
                                        if (!string.IsNullOrEmpty(var7)) email_body = email_body.Replace("{var7}", var7);
                                        if (!string.IsNullOrEmpty(var8)) email_body = email_body.Replace("{var8}", var8);

                                        using (var tx = db.GetTransaction())
                                        {
                                            #region Send email

                                            DateTime? deliveredOn = null;
                                            using (var mailMessage = new MailMessage())
                                            {
                                                try
                                                {
                                                    mailMessage.From = new MailAddress(configuration["Email:EmailAddress:Default"]);
                                                    var recipients = emailNotification.recipients.Replace(" ", "");
                                                    recipients = recipients.Replace(';', ',');
                                                    recipients = recipients.Replace('|', ',');

                                                    if (recipients.EndsWith(','))
                                                    {
                                                        recipients = recipients.Substring(0, recipients.Length - 1);
                                                    }
                                                    mailMessage.To.Add(recipients);
                                                    mailMessage.Subject = emailNotification.email_subject;
                                                    mailMessage.Body = email_body;
                                                    mailMessage.IsBodyHtml = true;

                                                    //var uploadPath = configuration["Path:UploadPath"].ToString();
                                                    //include the attachment if presents...
                                                    if (emailNotification.attachment_file != "")
                                                    {
                                                        string fn = emailNotification.attachment_file;
                                                        mailMessage.Attachments.Add(new Attachment(fn));
                                                    }

                                                    logger.Debug($"Send email to {recipients}");
                                                    await smtpClient.SendMailAsync(mailMessage);

                                                    deliveredOn = DateTime.Now;
                                                }
                                                catch (Exception ex)
                                                {
                                                    logger.Debug(db.LastCommand);
                                                    logger.Error("Error send email: " + ex.ToString());
                                                }
                                            }

                                            emailNotification.attempt_count = (emailNotification.attempt_count ?? 0) + 1;
                                            if (deliveredOn != null)
                                            {
                                                emailNotification.delivered_on = deliveredOn;
                                            }
                                            else
                                            {
                                                //emailNotification.delivery_schedule =
                                                //    DateTime.Now.AddHours(emailNotification.attempt_count ?? 1);
                                            }

                                            await db.UpdateAsync(stoppingToken, emailNotification);
                                            tx.Complete();

                                            #endregion
                                        }
                                    }
                                }
                            }
                            catch (TaskCanceledException tce)
                            {
                                logger.Debug(tce.Message);
                            }
                            catch (OperationCanceledException oce)
                            {
                                logger.Debug(oce.Message);
                            }
                            catch (Exception ex)
                            {
                                logger.Debug(db.LastCommand);
                                logger.Error(ex.ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex.ToString());
                }

                //await Task.Delay(5 * 60 * 1000, stoppingToken);
                await Task.Delay(10000, stoppingToken);     //for test
            };
        }
    }
}
