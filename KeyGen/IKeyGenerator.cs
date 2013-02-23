using System;
using System.Collections.Generic;
using System.Text;

namespace TAlex.PowerCalc.KeyGenerator
{
    public interface IKeyGenerator
    {
        object Generate(IDictionary<string, string> inputs);
    }
}
