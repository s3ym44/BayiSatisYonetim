namespace BayiSatisYonetim.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadFileAsync(IFormFile file, string folder = "uploads");
    }
}
