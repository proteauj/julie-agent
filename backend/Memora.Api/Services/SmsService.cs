using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace Memora.Api.Services
{
    public class SmsService
    {
        public SmsService(IConfiguration config)
        {
            TwilioClient.Init(
                config["Twilio:AccountSid"],
                config["Twilio:AuthToken"]
            );
        }

        public void SendSms(string toPhoneNumber, string message)
        {
            MessageResource.Create(
                body: message,
                from: "+1YOURTWILIONUMBER",    // à remplacer par ton numéro Twilio validé
                to: toPhoneNumber
            );
        }
    }
}