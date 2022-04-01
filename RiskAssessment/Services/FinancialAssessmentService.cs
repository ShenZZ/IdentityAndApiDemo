using Refit;

namespace RiskAssessment.Services;

public static class InvoiceOption
{
    public static readonly string Scheme = "auth_invoice";
    public static readonly string AuthorizationUrl = "https://localhost:5001";
    public static readonly string ClientId = "assess_client";
    public static readonly string ApiUrl = "https://localhost:5002";
}
public interface IFinancialAssessmentService
{
    /// <summary>获取客户财务数据</summary>
    [Get("/Invoice")]
    Task<List<Invoice>> GetFinancialDatas(DateTime start, DateTime end, [Authorize("Bearer")] string token);
}

public class Invoice
{
    public string Id { get; set; }
    public string Title { get; set; }
    public DateTime Creation { get; set; }
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
}