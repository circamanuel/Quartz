using Quartz.Persistence;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Media;
using System.Media;
using System.Numerics;
using System.Text;
using System.Windows.Forms;

namespace Quartz.Services
{

    //private string soundPath;
    internal class NotificationService
    {
        private NotifyIcon notifyIcon;
        private SoundPlayer soundPlayer;

        // Notifications deactivated: Windows notification sounds
        public NotificationService()
        {
            notifyIcon = new NotifyIcon();  
            notifyIcon.Icon = SystemIcons.Application;
            notifyIcon.Visible = true;

        }

        public void PlaySound()
        {
            if (File.Exists(AppPaths.SoundPath))
            {

                soundPlayer = new SoundPlayer(AppPaths.SoundPath);

                soundPlayer.Play();

            }
        }


        public void ShowTimerCompleteNotification()
        {
            //PlaySound();
            notifyIcon.ShowBalloonTip(
                5000,
                "🍅 Pomodoro Complete!",
                "Round finished - Time for a break",
                ToolTipIcon.Info
            );
        }

        public void ShowPauseNotification()
        {

                string[] funMessages = new[]
                {
                    "No skipping! Go grab a coffee ☕",
                    "You earned this break - enjoy it! 😎",
                    "Time to stretch those legs! 🧘",
                    "Break time - no cheating! 🚫",
                    "Rest your eyes, they need it! 👀",
                    "Minutes of pure laziness allowed 😴",
                    "Mandatory chill mode activated 🎵"
                };

                Random random = new Random();
                string message = funMessages[random.Next(funMessages.Length)];

                notifyIcon.ShowBalloonTip(
                5000,
                "⏸️ Break Time",
                message,
                ToolTipIcon.Warning
            );
        }

        public void ShowAllCompleteNotification()
        {
            //PlaySound();
            notifyIcon.ShowBalloonTip(
                5,"✅ All Rounds Complete!",
                "Great job 🎉",
                ToolTipIcon.Info
            );
        }

        public void Dispose()
        {
            notifyIcon?.Dispose();
        }

        public async Task ProgressBar(int totalTime, CancellationToken token)
        {
           await AnsiConsole.Progress()
            .Start(async ctx =>
            {
                var task = ctx.AddTask("Processing files", maxValue: totalTime);

                while (!ctx.IsFinished)
                {
                    task.Increment(1);
                    await Task.Delay(1000, token);
                }
            });
        }
    }
}
