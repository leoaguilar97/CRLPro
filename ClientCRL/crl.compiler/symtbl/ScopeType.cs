using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientCRL.crl.compiler.symtbl
{
    enum ScopeType
    {
        GLOBAL,
        FUNCTION,
        LOOP,
        SWITCH,
        NORMAL

        /*

        public bool GlobalCheck { get => globalCheck; set => globalCheck = value; }
    public bool CanBreak { get; internal set; }
    public bool Blocked { get; internal set; }
    public bool HasReturned { get; internal set; }
    public bool Switching { get; internal set; }
    public object ReturnedValue { get; internal set; }
    public bool Continued { get; internal set; }
    public bool Breaked { get; internal set; }
    public bool CanContinue { get; internal set; }
        */

    }
}
