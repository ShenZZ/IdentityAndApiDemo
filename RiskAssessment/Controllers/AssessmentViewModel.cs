namespace RiskAssessment.Controllers;

public class AssessmentViewModel
{
    public FinancialAssessmentModel FinancialAssessmentInfo { get; set; }
    public string AssessType { get; set; } = "api";  //api|email

    // Other Assessment Info
}

public class FinancialAssessmentModel
{
    public string Id { get; set; }
    public bool IsSuccess { get; set; }
    public string Description { get; set; }
    public DateTime Creation { get; set; }
    public bool IsExpired { get; set; }
    public FinancialAssessmentItemModel[] Items { get; set; }
}
public class FinancialAssessmentItemModel
{
    public string Title { get; set; }
    public decimal Amount { get; set; }
    public decimal Balance { get; set; }
    public DateTime Creation { get; set; }
}
