using System;

namespace ValheimBetterServerConfig
{
    class Validator
    {
        private static Validator m_instance;
        public Validator instance
        {
            get
            {
                return Validator.m_instance;
            }
        }

        public bool isPasswordValid(String password, World world, string serverName)
        {
            return !(world.m_name.Contains(password) || world.m_seedName.Contains(password) || serverName.Contains(password));
        }

    }
}
