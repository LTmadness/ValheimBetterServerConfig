using System;

namespace ValheimBetterServerConfig
{
    class Command
    {
        private string commandKey;
        private string hint;
        private Method method;

        public Command(string commandKey, string hint, Method method)
        {
            this.commandKey = commandKey;
            this.hint = hint;
            this.method = method;
        }

        private Command(string commandKey)
        {
            this.commandKey = commandKey;
        }

        public delegate bool Method(string[] args);

        public string Key { get => commandKey; }
        public string Hint { get => hint; }
        public bool Run (string[] args)
        {
            return (bool) method.DynamicInvoke(new object[] { args });
        }
    }
}
