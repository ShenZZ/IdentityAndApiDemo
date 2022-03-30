namespace RiskAssessment.Domain;

public interface IFinancialAssessmentRepository
{
    Task<FinancialAssessment> GetRecent(string customerId);
    Task Add(FinancialAssessment entity);
}
public class FinancialAssessmentRepository : IFinancialAssessmentRepository
{
    public Task<FinancialAssessment> GetRecent(string customerId)
    {
        //测试数据
        var model = new FinancialAssessment("test_customer")
        {
            AssessmentDate = DateTime.Now.AddMonths(-7),
            IsSuccess = true
        };

        return Task.FromResult(model);
    }
    public Task Add(FinancialAssessment entity)
    {
        return Task.CompletedTask;
    }
}
