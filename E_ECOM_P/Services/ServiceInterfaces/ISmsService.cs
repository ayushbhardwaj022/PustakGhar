namespace E_ECOM_P.Services.ServiceInterfaces
{
    public interface ISmsService
    {
        Task SendSms(string phone, string message);
    }
}
