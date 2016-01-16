using System;
using System.Collections.Generic;

namespace ITCO.TransferCost.Main
{
    public class FormArgs : EventArgs
    {
        private Dictionary<object, object> _parameters = new Dictionary<object, object>();

        public Dictionary<object, object> Parameters
        {
            get { return _parameters; }
        }
        public FormArgs(Dictionary<object, object> parameters)
        {
            _parameters = parameters;
        }
    }
}
