using System.Collections;

namespace WebClient.Controllers
{
    public class CheckInvoiceModel
    {
        public Invoice[] Invoices { get; set; }
        public IList<string> Messages { get; set; } = new List<string>();
    }
}
