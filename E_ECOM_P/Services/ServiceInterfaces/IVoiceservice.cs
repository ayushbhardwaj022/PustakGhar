namespace E_ECOM_P.Services.ServiceInterfaces
{
    public interface IVoiceservice
    {
        Task MakeCall(string phone,string message);
    }
}
