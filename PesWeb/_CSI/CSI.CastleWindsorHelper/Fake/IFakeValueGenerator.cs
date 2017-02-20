using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CSI.CastleWindsorHelper.Fake
{
    public interface IFakeValueGenerator
    {
        T GenerateValuesFromType<T>();
        object GenerateValuesFromType(Type objectType);
    }
}
