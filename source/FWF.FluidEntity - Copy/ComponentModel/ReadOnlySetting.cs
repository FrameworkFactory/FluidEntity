namespace FWF.FluidEntity.ComponentModel
{
    public class ReadOnlySetting<T>
    {

        private readonly string _name;
        private readonly T _default;

        public ReadOnlySetting(string name, T @default)
        {
            _name = name;
            _default = @default;
        }

        public string Name
        {
            get { return _name; }
        }

        public T @Default
        {
            get { return _default; }
        }

        
    }
}
