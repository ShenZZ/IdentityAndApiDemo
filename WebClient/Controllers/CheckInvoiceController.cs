using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Refit;

namespace WebClient.Controllers
{
    public class CheckInvoiceController : Controller
    {
        private readonly ILogger<CheckInvoiceController> _logger;

        public CheckInvoiceController(ILogger<CheckInvoiceController> logger)
        {
            _logger = logger;
        }

        [Microsoft.AspNetCore.Authorization.Authorize(AuthenticationSchemes = "oidc")]
        public async Task<IActionResult> IndexAsync()
        {
            var token = await this.HttpContext.GetTokenAsync("access_token");

            var invoiceApi = RestService.For<IInvoiceApi>("https://localhost:5002");

            var list = await invoiceApi.GetInvoiceList(DateTime.Now.AddMonths(-6), token);

            //保存数据库
            //invoiceService.save(list);

            var amount = list.Sum(x => x.Amount);
            var date = DateTime.Now.Date.AddDays(-90);
            var isNopay = list.Any(i => i.Balance > 0 && i.Creation < date);
            var count = list.DistinctBy(i => i.Title).Count();

            var model = new CheckInvoiceModel();

            if (amount < 1000000) model.Messages.Add("总金额不低于100万");
            if (isNopay) model.Messages.Add("没有超过90天未完全付款的发票");
            if (count < 10) model.Messages.Add("客户数量不低于10个");

            model.Invoices = list.OrderByDescending(i => i.Creation).Take(10).ToArray();

            return View(model);
        }
        

    }
}