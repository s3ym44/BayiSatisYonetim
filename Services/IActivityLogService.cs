namespace BayiSatisYonetim.Services
{
    public interface IActivityLogService
    {
        Task LogAsync(string userId, string action, string detail, string? ipAddress = null);
    }
}
