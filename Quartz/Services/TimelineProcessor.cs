using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using Quartz.Models;
using Quartz.Persistence;
using Spectre.Console;

namespace Quartz.Services
{
    internal class TimelineProcessor
    {

        private SessionModel session = new SessionModel();
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
            using FileStream openStream = File.OpenRead(AppPaths.TimelineFile);
            List<SessionModel> session =  JsonSerializer.Deserialize<List<SessionModel>>(openStream);

            TimelineViewer(session);
        }

        private void TimelineViewer(List<SessionModel> sessions)
        {
            foreach (SessionModel session in sessions)
            {

                var table = new Table();
                table.Border(TableBorder.Rounded)
                    .Title($"{session.StartDate.ToString("dddd")}/{session.StartDate.ToString("MMM dd")}");

                table.AddColumn($"[Blue]Period[/]");
                table.AddColumn($"[Blue]Duration in min[/]");
                table.AddColumn($"[Blue]Focus in min[/]");

                int difference = (int)session.EndDate.Subtract(session.StartDate).TotalMinutes;
                table.AddRow($"{difference}", $"{session.StartDate.ToString("H:mm")} - {session.EndDate.ToString("H:mm")}", $"{session.FocusTimeInMinutes}");

                AnsiConsole.Write(table);
                Console.WriteLine();

            }
                OptionExecutor();
        }

        /*TODO: Create Spectre.Console Layout
         *      - Left Tabular data from json 
         *      - Right Options / maybe selection
         *      https://spectreconsole.net/console/widgets/layout
         */

        private void OptionExecutor()
        {

    //       AnsiConsole.MarkupLine("[bold red]Error:[/] Something went wrong");
    //        AnsiConsole.MarkupLine("[italic]Emphasis text[/]");
    //        AnsiConsole.MarkupLine("[underline blue]Link text[/]");

    //        // Combining multiple styles
    //        AnsiConsole.MarkupLine("[bold underline]Important[/]");

    //        // Using Style class
    //        var style = new Style(Color.White, decoration: Decoration.Bold | Decoration.Underline);
    //        AnsiConsole.Write("Styled text", style);
    //        var layout = new Layout("Root")
    //.SplitColumns(
    //    new Layout("Left"),
    //    new Layout("Right"));

    //        layout["Left"].Update(
    //            new Panel("Left Panel")
    //                .BorderColor(Color.Green));

    //        layout["Right"].Update(
    //            new Panel("Right Panel")
    //                .BorderColor(Color.Blue));

    //        AnsiConsole.Write(layout);

            // Listener on keu or selector from Spectre console
            while (true)
            {


            }
        }
    }
}
