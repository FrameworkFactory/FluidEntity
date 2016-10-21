namespace FWF.FluidEntity
{
    public class BuildConfig
    {
        private readonly string _name;

        public BuildConfig(string name)
        {
            _name = name;
        }

        public string Name
        {
            get { return _name; }
        }
    }

    public static class BuildEnvironment
    {
        public static readonly BuildConfig DEBUG = new BuildConfig("DEBUG");
        public static readonly BuildConfig CI = new BuildConfig("CI");
        public static readonly BuildConfig DEV = new BuildConfig("DEV");
        public static readonly BuildConfig QA = new BuildConfig("QA");
        public static readonly BuildConfig STAGE = new BuildConfig("STAGE");
        public static readonly BuildConfig PERF = new BuildConfig("PERF");
        public static readonly BuildConfig PROD = new BuildConfig("PROD");
    }
}