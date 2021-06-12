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
        }

        public void addLog(string message, LoggerType type, LoggerLevel level)
        {
            if (levelSetting >= level) 
            {
                DateTime now = DateTime.Now;
                string dayNow = now.ToShortDateString().Replace("/", "-").Replace(" ", "");
                string fileName = $"{type}{dayNow}.logs";
                addLog($"[{level}][{now.ToShortTimeString()}]{message}", loggerLocation + fileName).RunSynchronously();
            }
        }

        private async Task addLog(string message, string fileLocation)
        {
            StreamWriter file = new StreamWriter(fileLocation, append: true);
            await file.WriteLineAsync(message);
        }
    }
}
