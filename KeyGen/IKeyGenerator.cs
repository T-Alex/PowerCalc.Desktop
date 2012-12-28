using System;
using System.Collections.Generic;
using System.Text;

namespace TAlex.PowerCalc.KeyGenerator
{
    public interface IKeyGenerator
    {
        string Generate(IDictionary<string, string> inputs);
    }
}
