namespace ValheimBetterServerConfig
{
    class Command
    {
        private readonly string commandKey;
        private readonly string hint;
        private readonly Method method;
        private readonly bool modsAllowed;

        public Command(string commandKey, string hint, Method method, bool modsAllowed)
        {
            this.commandKey = commandKey;
            this.hint = hint;
            this.method = method;
            this.modsAllowed = modsAllowed;
        }

        public delegate bool Method(string[] args);
        public string Key { get => commandKey; }
        public string Hint { get => hint; }
        public bool ModsAllowed { get => modsAllowed; }

        public bool Run(string[] args)
        {
            return (bool)method.DynamicInvoke(new object[] { args });
        }
    }
}
