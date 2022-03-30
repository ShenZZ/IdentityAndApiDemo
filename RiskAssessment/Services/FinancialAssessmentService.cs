using RiskAssessment.Domain;

namespace RiskAssessment.Services;

public interface IFinancialAssessmentService
{
    /// <summary>获取客户财务数据评估的设置</summary>
    Task<AssessmentOption> GetOption(string customerId);
    /// <summary>获取客户当前最新的财务数据评估</summary>
    Task<FinancialAssessment> GetRecentAssessment(string customerId);
    /// <summary>获取客户财务数据</summary>
    Task<Invoice[]> GetFinancialDatas(string customerId, string token);

}
public class FinancialAssessmentService
{
    public FinancialAssessmentService(IFinancialAssessmentRepository assessmentRepository, IInvoiceApi invoiceApi)
    {
        InvoiceApi = invoiceApi;
    }
    public IInvoiceApi InvoiceApi { get; }


}

