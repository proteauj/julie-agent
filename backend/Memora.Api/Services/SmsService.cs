using Twilio;
using Twilio.Exceptions;
using Twilio.Rest.Api.V2010.Account;

namespace Memora.Api.Services
{
    public class SmsService
    {
        private readonly ILogger<SmsService> _logger;
        private readonly string? _accountSid;
        private readonly string? _authToken;
        private readonly string? _fromPhoneNumber;
        private readonly bool _isConfigured;

        public SmsService(IConfiguration config, ILogger<SmsService> logger)
        {
            _logger = logger;

            _accountSid = config["Twilio:AccountSid"]?.Trim();
            _authToken = config["Twilio:AuthToken"]?.Trim();
            _fromPhoneNumber = config["Twilio:FromPhoneNumber"]?.Trim();

            _isConfigured =
                !string.IsNullOrWhiteSpace(_accountSid) &&
                !string.IsNullOrWhiteSpace(_authToken) &&
                !string.IsNullOrWhiteSpace(_fromPhoneNumber);

            if (_isConfigured)
            {
                TwilioClient.Init(_accountSid, _authToken);
            }
            else
            {
                _logger.LogWarning("Twilio is not fully configured. SMS sending will be skipped.");
            }
        }

        public bool SendSms(string toPhoneNumber, string message)
        {
            if (!_isConfigured)
            {
                _logger.LogWarning("SMS skipped because Twilio is not configured.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(toPhoneNumber))
            {
                _logger.LogWarning("SMS skipped because destination phone number is empty.");
                return false;
            }

            try
            {
                MessageResource.Create(
                    body: message,
                    from: _fromPhoneNumber,
                    to: toPhoneNumber
                );

                return true;
            }
            catch (ApiException ex)
            {
                _logger.LogError(ex, "Twilio API error while sending SMS. The app will continue running.");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected SMS error. The app will continue running.");
                return false;
            }
        }
    }
}