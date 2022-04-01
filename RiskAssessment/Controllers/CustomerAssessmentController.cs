using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using RiskAssessment.Domain;
using RiskAssessment.Services;

namespace RiskAssessment.Controllers;

public class CustomerAssessmentController : Controller
{
    private readonly ILogger<CustomerAssessmentController> _logger;
    private readonly IFinancialAssessmentRepository _financialAssessmentRepository;
    private readonly IFinancialAssessmentService _financialAssessmentService;

    public CustomerAssessmentController(ILogger<CustomerAssessmentController> logger,
        IFinancialAssessmentRepository financialAssessmentRepository,
        IFinancialAssessmentService financialAssessmentService)
    {
        _logger = logger;
        _financialAssessmentRepository = financialAssessmentRepository;
        _financialAssessmentService = financialAssessmentService;
    }

    //客户所有评估信息
    public async Task<IActionResult> IndexAsync()
    {
        var customerId = "test_user";
        var model = new AssessmentViewModel();

        //获取最近的财务数据评估
        var recent = await _financialAssessmentRepository.GetRecent(customerId);
        if (recent != null)
        {
            model.FinancialAssessmentInfo = new FinancialAssessmentModel
            {
                Id = recent.Id.ToString(),
                IsSuccess = recent.IsSuccess,
                Description = recent.Description,
                Creation = recent.AssessmentDate,
                IsExpired = recent.IsExpired(),
            };
        }

        //todo: 如果需要走Email流程 在这里获取并设置类型

        //todo: 获取其他类型的评估结果 这部分处理也可以封装到一个FacadeService中

        return View(model);
    }

    //获取财务数据 并保存评估
    public async Task<IActionResult> Assess()
    {
        var customerId = "test_user";

        //1.获取最近的财务数据评估
        var recent = await _financialAssessmentRepository.GetRecent(customerId);
        if (recent != null && !recent.IsExpired())
        {
            return RedirectToAction("AssessResult", recent.Id);
        }

        //2.认证和获取Token
        var token = await this.HttpContext.GetTokenAsync(InvoiceOption.Scheme, "access_token");
        if (token == null) return this.Challenge(InvoiceOption.Scheme);  //跳转客户端认证
        //todo: token过期问题未处理
        //todo: token获取/过期处理 也可以一起封装到service中

        //3.获取api数据
        var end = DateTime.Now;
        var start = end.AddMonths(-6);
        var invoices = await _financialAssessmentService.GetFinancialDatas(start, end, token);

        //4.评估数据
        var model = new FinancialAssessment(customerId)
        {
            FinancialDatas = invoices?.Select(s => new FinancialDataItem
            {
                Id = Guid.NewGuid(),
                Title = s.Title,
                Amount = s.Amount,
                Balance = s.Balance,
                Creation = s.Creation,
            }).ToList()
        };

        model.Assess();

        //5.保存
        await _financialAssessmentRepository.Add(model);

        return RedirectToAction("AssessResult", recent.Id);
    }

    //财务数据评估信息
    public async Task<IActionResult> AssessResult(string id)
    {
        var recent = await _financialAssessmentRepository.Get(id);
        if (recent == null) return NotFound();

        var model = new FinancialAssessmentModel
        {
            IsSuccess = recent.IsSuccess,
            Description = recent.Description,
            Creation = recent.AssessmentDate,
            Items = recent.FinancialDatas?.OrderByDescending(s => s.Creation).Take(10) //todo: 这里应该放到Repository中处理
                .Select(s => new FinancialAssessmentItemModel
                {
                    Title = s.Title,
                    Amount = s.Amount,
                    Balance = s.Balance,
                    Creation = s.Creation,
                }).ToArray()
        };

        return View(model);
    }


}