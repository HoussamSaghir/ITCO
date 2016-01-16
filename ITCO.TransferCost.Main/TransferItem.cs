using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCO.TransferCost.Main
{
    public class TransferItem
    {
        public string IssueItemCode { get; set; }
        public string ReceiptItemCode { get; set; }
        public string FromWhs { get; set; }
        public string ToWhs { get; set; }
        public string Quantity { get; set; }
        public double AddCost { get; set; }
        public double AvgCost { get; set; }
        public TransferItem()
        {

        }
    }
}
