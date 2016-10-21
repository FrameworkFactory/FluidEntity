using System.Data;

namespace FWF.FluidEntity.Data
{
    public class ProcParameter : IProcParameter
    {
        public string Name { get; set; }

        public string TypeName { get; set; }

        public SqlDbType DbType { get; set; }

        public ParameterDirection Direction { get; set; }

        public object Value { get; set; }
    }
}
