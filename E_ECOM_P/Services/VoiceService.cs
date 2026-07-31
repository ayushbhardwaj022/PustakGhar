using E_ECOM_P.Services.ServiceInterfaces;
using E_EOM_Utility;
using Microsoft.Extensions.Options;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace E_ECOM_P.Services
{
    public class VoiceService : IVoiceservice
    {
        private readonly TwilioSettings _settings;
        public VoiceService(IOptions<TwilioSettings> settings)
        {
            _settings = settings.Value;
            TwilioClient.Init(_settings.AccountSid, _settings.AuthToken);
        }

        public Task MakeCall(string number, string message)
        {
            return CallResource.CreateAsync(
                twiml: new Twilio.Types.Twiml($"<Response><Say>{message}</Say></Response>"),
                to: new Twilio.Types.PhoneNumber(number),
                from: new Twilio.Types.PhoneNumber(_settings.PhoneNumber)
            );
        }

        
    }
}
