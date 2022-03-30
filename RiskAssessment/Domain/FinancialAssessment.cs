using System.ComponentModel.DataAnnotations;

namespace RiskAssessment.Domain;

/// <summary>
/// 财务数据评估
/// </summary>
public class FinancialAssessment
{
    [Required]
    public Guid Id { get; private set; }
    [Required]
    public string CustomerId { get; private set; }
    public bool IsSuccess { get; internal set; }
    public string Description { get; private set; }
    [Required]
    public DateTime AssessmentDate { get; internal set; }

    public IList<FinancialDataItem> FinancialDatas { get; set; }

    public FinancialAssessment() { }
    public FinancialAssessment(string customerId)
    {
        if (string.IsNullOrWhiteSpace(customerId))
            throw new ArgumentNullException(nameof(customerId));

        this.Id = Guid.NewGuid();
        this.CustomerId = customerId;
        this.IsSuccess = false;
    }

    /// <summary>评估</summary>
    public bool Assess()
    {
        if (this.FinancialDatas == null && this.FinancialDatas.Count == 0)
        {
            //todo:为空处理？
            this.IsSuccess = false;
            this.Description = "总金额不低于100万;客户数量不低于10个;";
            this.AssessmentDate = DateTime.Now;
            return false;
        }

        string error = "";

        var amount = this.FinancialDatas.Sum(x => x.Amount);
        if (amount > 1000000)
        {
            error += "总金额不低于100万;";
        }

        var count = this.FinancialDatas.DistinctBy(i => i.Title).Count();
        if (count < 10)
        {
            error += "客户数量不低于10个;";
        }

        var date = DateTime.Now.Date.AddDays(-90);
        var isNopay = this.FinancialDatas.Any(i => i.Balance > 0 && i.Creation < date);
        if (isNopay)
        {
            error += "没有超过90天未完全付款的发票;";
        }

        this.IsSuccess = error.Length == 0;
        this.Description = error;
        this.AssessmentDate = DateTime.Now;

        return this.IsSuccess;
    }

    /// <summary>是否过期</summary>
    public bool IsExpired()
    {
        return this.AssessmentDate < DateTime.Now.AddMonths(-6);
    }
}

/// <summary>
/// 财务数据
/// </summary>
public class FinancialDataItem
{
    [Required]
    public Guid Id { get; set; }
    [Required]
    public FinancialAssessment Assessment { get; set; }
    /// <summary>发票对象名字</summary>
    public string Title { get; set; }
    /// <summary>发票金额</summary>
    public decimal Amount { get; set; }
    /// <summary>未结算金额</summary>
    public decimal Balance { get; set; }
    /// <summary>发票日期</summary>
    public DateTime Creation { get; set; }
}
