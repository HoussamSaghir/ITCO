using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCO.TransferCost.Main
{
    public class BatchRow : Item
    {
        public string TotalNeeded { get; set; }
        public string TotalSelected { get; set; }
        public string TotalBatches { get; set; }
        public double AddAmount { get; set; }

        public BatchRow()
        {

        }

        public BatchRow(string itemCode, string whsCode, double quantity, string totalNeeded, string totalSelected, string totalBatches)
        {
            ItemCode = itemCode;
            WhsCode = whsCode;
            Quantity = quantity;
            TotalNeeded = totalNeeded;
            TotalSelected = totalSelected;
            TotalBatches = totalBatches;
        }
    }
}
