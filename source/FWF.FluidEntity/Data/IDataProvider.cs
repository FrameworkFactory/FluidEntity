using System;
using System.Data;
using System.Data.Common;

namespace FWF.FluidEntity.Data
{
    public interface IDataProvider
    {
        DbProviderFactory GetProviderFactory();

        object MapParameterValue(object value);

    }
}
