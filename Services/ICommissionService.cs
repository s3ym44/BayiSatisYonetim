namespace BayiSatisYonetim.Services
{
    public interface ICommissionService
    {
        decimal CalculateCommission(decimal saleAmount, decimal commissionRate);
    }
}
