using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class InvoiceController : ControllerBase
    {
        private readonly ILogger<InvoiceController> _logger;
        private static readonly string[] _corps = new[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L" };

        public InvoiceController(ILogger<InvoiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Invoice> Get(DateTime start)
        {
            return Enumerable.Range(1, 100).Select(index =>
            {
                var inv = new Invoice
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = _corps[Random.Shared.Next(_corps.Length)],
                    Creation = DateTime.Now.AddDays(-index),
                    Amount = Random.Shared.Next(1, 999999)
                };
                inv.Balance = Random.Shared.NextDouble() > 0.5 ? Random.Shared.Next(1, (int)inv.Amount) : 0;
                return inv;
            })
            .ToArray();
        }
    }

    public class Invoice
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime Creation { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}