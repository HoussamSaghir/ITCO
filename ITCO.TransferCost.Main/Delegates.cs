using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCO.TransferCost.Main
{
    public delegate void UserFormClosedDone(object sender);
    public delegate void SubFormCreated<T>(object sender, FormArgs formArgs) where T : new();
}
