using Quartz.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;
using System.Text.Json;

namespace Quartz.Persistence
{
    internal class TimelineLogRepository
    {

        private readonly string _directoryPath;
        private readonly string _jsonFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public TimelineLogRepository()
        {
            _directoryPath = AppPaths.QuartzDirectory;
            _jsonFilePath = AppPaths.TimelineFile;

            _jsonOptions = new JsonSerializerOptions { WriteIndented = true};
             
            Directory.CreateDirectory(_directoryPath);



        }

        public void Append(SessionModel session)
        {
            var sessions = LoadAll();

            sessions.Add(session);

            var json = JsonSerializer.Serialize(sessions, _jsonOptions);
            File.WriteAllText(_jsonFilePath, json);

        }

        private List<SessionModel> LoadAll()
        {
            if (!File.Exists(_jsonFilePath))
            {
                return new List<SessionModel>();
            }

            var json = File.ReadAllText(_jsonFilePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<SessionModel>();
            }

            try
            {
                List<SessionModel>? sessions =
                    JsonSerializer.Deserialize<List<SessionModel>>(json);

                if (sessions == null)
                {
                    return new List<SessionModel>();
                }

                return sessions;
            }
            catch (JsonException)
            {

                SessionModel? singleSession =
                    JsonSerializer.Deserialize<SessionModel>(json);

                if (singleSession != null)
                { 
                    return new List<SessionModel> { singleSession };
                }

                return new List<SessionModel>();
            }
        }
    }
}
