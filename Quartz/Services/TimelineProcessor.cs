using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Quartz.Models;
using Quartz.Persistence;
using Quartz.Services;
using Spectre.Console;

namespace Quartz.Services
{
    internal class TimelineProcessor
    {


        /* TODO:
         * Anzeige
         *  Datum von bis
         *  Zyklus zeit anzeigen
         *  Totale zeit
         *  mit Spectre.Console die json auslesen und ??Tabellarisch?? anzeigen
         *  beendigung und return zum menue
         *  Navigation mit pfeiltasten oder vim motions
         *  
         *  Welche Klassen brauchen wir:
         *      Json auslesen
         *      Eine klasse die jede Session einzeln tabellarisch ausgibt
         *          - Nice to have im git style mit den linien ganz links
         *      
         */
        public TimelineProcessor()
        {
        }

        public async Task TimelineJsonRader()
        {
            List<SessionModel> session = new List<SessionModel>(); 
            try
            {
                using FileStream openStream = File.OpenRead(AppPaths.TimelineFile);
                //List<SessionModel> session =  
                session = JsonSerializer.Deserialize<List<SessionModel>>(openStream);  
            } 
            catch 
            {
                var panel = new Panel("No dataset")
                    .RoundedBorder()
                    .BorderColor(Color.Orange1);

                AnsiConsole.Write(panel);

                return;
            }


            TimelineViewer(session);
        }

        private void TimelineViewer(List<SessionModel> sessions)
        {
            string currentMonth = "";
            DateTime currentDate = new DateTime();

            var tree = new Tree("Timeline");
            tree.Guide(TreeGuide.Line);

             var sessionsByMonth = sessions
                    .GroupBy(s => s.StartDate.ToString("MMMM"))
                    .ToList();

            foreach (var n in sessionsByMonth)
            {
                var node = tree.AddNode($"[Yellow]{n.Key}[/]");

                foreach (SessionModel s in n)
                {
                    // Create table
                    var table = new Table();
                    table.Border(TableBorder.Square)
                        .Title($"{s.StartDate.ToString("dddd")}/{s.StartDate.ToString("MMM dd")}");

                    table.AddColumn($"[Blue]Period[/]");
                    table.AddColumn($"[Blue]Duration[/]");
                    table.AddColumn($"[Blue]Focus[/]");

                    int difference = (int)s.EndDate.Subtract(s.StartDate).TotalMinutes;
                    table.AddRow($"{s.StartDate.ToString("H:mm")} - {s.EndDate.ToString("H:mm")}", $"{difference} Min", $"{s.FocusTimeInMinutes} Min");

                    node.AddNode(table);
                }
            }
                AnsiConsole.Write(tree);
                var panel = new Panel("[bold]To exit press ESC[/]")
                    .Header("[White]Information[/]", Justify.Center)
                    .RoundedBorder()
                    .BorderColor(Color.CornflowerBlue)
                    .Padding(2, 1)
                    .Expand();

            AnsiConsole.Write(panel);

                OptionExecutor();
        }

        /*TODO: Create Spectre.Console Layout
         *      - Left Tabular data from json 
         *      - Right Options / maybe selection
         *      https://spectreconsole.net/console/widgets/layout
         */

        private void OptionExecutor()
        {

            while (true)
            {
                if (Console.KeyAvailable) 
                {
                    var key = Console.ReadKey();   

                    if (key.Key == ConsoleKey.Escape)
                    {
                        Console.Clear();
                        return;
                    }
                }
            }
        }
    }
}
