using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCO.TransferCost.Main
{
    public class SelectedBatchDataSources : Item
    {
        public System.Data.DataTable SelectedBatches { get; set; }
        public int Index { get; set; }
        public double AddAmount { get; set; }
        public double TotalNeeded { get; set; }
        public double TotalSelected { get; set; }
    }
}
