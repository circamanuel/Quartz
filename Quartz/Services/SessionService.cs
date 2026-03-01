using Quartz.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Text.Json;

namespace Quartz.Services
{
    internal class SessionService
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string directoryPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\Quartz";
        string jsonFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}" +
               $"\\Quartz\\TimeLineLog.json";

        public async Task LogToJson(int focusTime, DateTime startDate)
        {
            var pomodoroSession = new PomodoroSessions
            {
                StartDate = startDate,
                EndDate = DateTime.Now, 
                FocusTimeInMinutes = focusTime
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            string content = JsonSerializer.Serialize(pomodoroSession, options);

            if (!File.Exists(jsonFilePath))
            {
                Directory.CreateDirectory(directoryPath);
                File.WriteAllText(jsonFilePath, content);
            } else
            {
                using FileStream openStream = File.OpenRead(jsonFilePath);
                PomodoroSessions pomodoroSessions = JsonSerializer.Deserialize<PomodoroSessions>(openStream);
            }
           


            Console.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
        }

    }
}
