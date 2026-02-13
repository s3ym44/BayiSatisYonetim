namespace BayiSatisYonetim.Services
{
    public interface IPdfService
    {
        byte[] GenerateApplicationPdf(string applicationNumber, string customerName, string productName, string address, DateTime date);
    }
}
