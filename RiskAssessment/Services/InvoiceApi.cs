using Refit;

namespace RiskAssessment.Services;

public interface IInvoiceApi
{
    [Get("/Invoice")]
    Task<List<Invoice>> GetInvoiceList(DateTime start, [Authorize("Bearer")] string token);
}

public class Invoice
{
    public string Id { get; set; }
    public string Title { get; set; }
    public DateTime Creation { get; set; }
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
}

