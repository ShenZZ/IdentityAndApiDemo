namespace RiskAssessment.Domain;

public interface IFinancialAssessmentRepository
{
    Task<FinancialAssessment> Get(string id);
    Task<FinancialAssessment> GetRecent(string customerId);
    Task Add(FinancialAssessment entity);
}
public class FinancialAssessmentRepository : IFinancialAssessmentRepository
{
    private static FinancialAssessment _testdata;

    public Task<FinancialAssessment> Get(string id)
    {
        if (_testdata != null) return Task.FromResult(_testdata);

        //测试数据
        var model = new FinancialAssessment("test_user")
        {
            AssessmentDate = DateTime.Now.AddMonths(-7),
            IsSuccess = true
        };

        return Task.FromResult(model);
    }
    public Task<FinancialAssessment> GetRecent(string customerId)
    {
        if (_testdata != null) return Task.FromResult(_testdata);

        //测试数据
        var model = new FinancialAssessment(customerId)
        {
            AssessmentDate = DateTime.Now.AddMonths(-7),
            IsSuccess = true
        };

        return Task.FromResult(model);
    }
    public Task Add(FinancialAssessment entity)
    {
        _testdata = entity;

        return Task.CompletedTask;
    }
}
