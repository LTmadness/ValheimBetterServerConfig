namespace ValheimBetterServerConfig
{
    class Command
    {
        private readonly string commandKey;
        private readonly string hint;
        private readonly Method method;

        public Command(string commandKey, string hint, Method method)
        {
            this.commandKey = commandKey;
            this.hint = hint;
            this.method = method;
        }

        public delegate bool Method(string[] args);
        public string Key { get => commandKey; }
        public string Hint { get => hint; }
        public bool Run(string[] args)
        {
            return (bool)method.DynamicInvoke(new object[] { args });
        }
    }
}
