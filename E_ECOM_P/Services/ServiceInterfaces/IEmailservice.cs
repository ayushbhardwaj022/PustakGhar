namespace E_ECOM_P.Services.ServiceInterfaces
{
    public interface IEmailservice
    {
        Task SendEmail(string to, string subject, string body);
    }
}
