using System.Data;

namespace FWF.FluidEntity.Data
{
    public interface IProcParameter
    {
        string Name { get; }
        SqlDbType DbType { get; }
        ParameterDirection Direction { get; }
        object Value { get; set; }
        string TypeName { get; }
    }
}
