using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Procons.TransferCost.Main
{
    enum QueryType{
        SELECT,DELETE
    }
    public class QueryBuilder
    {
        string query = string.Empty;

        public QueryBuilder(QueryType qType)
        {
            query += qType.ToString() + " ";
        }

        public void AddColumns(params string columns)
        {

        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
