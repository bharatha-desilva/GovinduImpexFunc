using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using SendGrid;
using SendGrid.Helpers.Mail;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace GovinduImpexFunc
{
    public class Function
    {
        string SendGridAPIKey = "--API-KEY--";
        HttpClient HttpClient;
        SendGridClient SendGridClient;

        public Function()
        {
            HttpClient = new HttpClient();
            SendGridClient = new SendGridClient(HttpClient, SendGridAPIKey);
        }



        /// <summary>
        /// function which takes Contact Us form data and send acknoledgment email to sender and cc to info
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public string FunctionHandler(ContactUsInputFormData contactUsFormData, ILambdaContext context)
        {
            
            /*string htmlTemplate = Base64Decode(base64TemplateHtml);
            htmlTemplate = htmlTemplate.Replace("{{firstname}}", $" {contactUsFormData.firstname}");
            htmlTemplate = htmlTemplate.Replace("{{lastname}}", $" {contactUsFormData.lastname}");
            htmlTemplate = htmlTemplate.Replace("{{message}}", contactUsFormData.subject);*/
            //return htmlTemplate;


             var msg = $"Hi {contactUsFormData.firstname} {contactUsFormData.lastname} <{contactUsFormData.email}>,\n We noted your message.\b " +
                $"Your message : '{contactUsFormData.subject}'";
            var from = new EmailAddress("info@govinduimpex.com", "Govindu Impex");
            var to = new EmailAddress(contactUsFormData.email, $"{contactUsFormData.firstname} {contactUsFormData.lastname}");
            SendGridMessage sendGridMessage = MailHelper.CreateSingleEmail(
                from, 
                to, 
                $"Thank you {contactUsFormData.firstname} {contactUsFormData.lastname} for contacting us.", msg, msg);
            sendGridMessage.AddBcc(new EmailAddress("info@govinduimpex.com", "Govindu Impex"));
            sendGridMessage.SetTemplateId("d-f25db49cab544c7bb3c81bf730354984");
            sendGridMessage.SetTemplateData(new {
                sender = new
                {
                    firstname = contactUsFormData.firstname,
                    lastname = contactUsFormData.lastname,
                    message = contactUsFormData.subject,
                }
            });
            SendGridClient.SendEmailAsync(sendGridMessage).Wait();
            
            return msg;
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }

    public class ContactUsInputFormData
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string country { get; set; }
        public string subject { get; set; }
    }
}
