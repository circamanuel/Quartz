using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.Persistence
{
    internal class AppPaths
    {
        // %AppData%Quartz
        public static string QuartzDirectory =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Quartz"
                );

        //timeline.json
        public static string TimelineFile => Path.Combine(QuartzDirectory, "timeline.json");

        // Sound file
        public static string SoundPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory, "C:\\Users\\manu_\\source\\repos\\Quartz\\Quartz\\Sounds\\freesound_community-beep-6-96243.wav");

    }
}
