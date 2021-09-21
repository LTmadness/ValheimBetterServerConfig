using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValheimBetterServerConfig.Logger
{
    class Logger
    {
        private static Logger instance;

        private LoggerLevel levelSetting;

        private string loggerLocation;

        public static Logger Instance
        {
            get
            {
                return Logger.instance;
            }
        }

        public Logger(string saveLocation, LoggerLevel loggerSetting)
        {
            instance = this;
            loggerLocation = saveLocation + "/logs/";
            levelSetting = loggerSetting;
            //Make sure location excist no point of logging if we can't save it
            Directory.CreateDirectory(loggerLocation);
        }

        public void addLog(string message, LoggerType type, LoggerLevel level)
        {
            if (levelSetting <= level) 
            {
                DateTime now = DateTime.Now;
                string dayNow = now.ToShortDateString().Replace("/", "-").Replace(" ", "");
                string fileName = $"{type}{dayNow}.logs";

                using (FileStream fileStream = new FileStream(loggerLocation + fileName, FileMode.Append))
                {
                    using (StreamWriter file = new StreamWriter(fileStream))
                    {
                        file.WriteLine($"[{level}][{now.ToShortTimeString()}]{message}");
                    }
                }
            }
        }
    }
}
