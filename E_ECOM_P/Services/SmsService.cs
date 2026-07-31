using E_ECOM_P.Services.ServiceInterfaces;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace E_ECOM_P.Services
{
    public class SmsService : ISmsService
    {
        private readonly E_EOM_Utility.TwilioSettings _settings;

        public SmsService(IOptions<E_EOM_Utility.TwilioSettings> settings)
        {
            _settings = settings.Value;
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
        }

        public async Task SendSms(string phone, string message)
        {
            var accountSid = "ACe788421cc526195685dc9f0031f28b6c";
            var authToken = "a5f7c054a4eebd81ce9f1a69f59dc6b8";
            TwilioClient.Init(accountSid, authToken);

            var result = await MessageResource.CreateAsync(
                to: new PhoneNumber(phone),                  // Customer phone number
                from: new PhoneNumber("+19787554752"),       // Your Twilio phone number
                body: message
            );
        }

    }
}
