namespace BayiSatisYonetim.Services
{
    public class CommissionService : ICommissionService
    {
        public decimal CalculateCommission(decimal saleAmount, decimal commissionRate)
        {
            return Math.Round(saleAmount * commissionRate / 100, 2);
        }
    }
}
