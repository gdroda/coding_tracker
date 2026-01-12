using Spectre.Console;
using Spectre.Console.Rendering;

namespace coding_tracker
{
    internal class UserInput
    {
        public static void PrintMenu(ref bool loadStatus)
        {
            if (!loadStatus)
            FirstLoad(ref loadStatus);

            Console.Clear();

            var fullDate = DateTime.Now;
            var minutes = (int)fullDate.TimeOfDay.TotalMinutes;
            Console.WriteLine(minutes+20);
            var currDate = fullDate.ToString("d");
            var currTime = fullDate.ToString("HH:mm");
            Console.WriteLine($"Date: {currDate}");
            Console.WriteLine($"Time: {currTime}");
            Console.WriteLine(DateTime.Parse($"{currDate} {currTime}"));
            Console.WriteLine(DateTime.Parse("13/01/2026 03:35").Subtract(DateTime.Parse("13/01/2026 01:43")).TotalMinutes);
            //need to enforce input of: 10 chars + space + 5 chars >into> datetime.try{tryparse catch(exception ex){to check if valid.

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold green]~ Welcome to your Code Tracker ~[/]")
                    .WrapAround()
                    .HighlightStyle(new Style(Color.Green, Color.Black, Decoration.Bold))
                    .AddChoices("Add New Session", "Show Previous Sessions", "Empty Session Database", "Exit"));


            switch (choice)
            {
                case "Add New Session":
                    Database_Functions.AddSession();
                    break;
                case "Show Previous Sessions":
                    Database_Functions.ShowDatabase();
                    break;
                case "Empty Session Database":
                    Database_Functions.EmptyDatabase();
                    break;
                case "Exit":
                    Environment.Exit(0);
                    break;
                default:
                    PrintMenu(ref loadStatus);
                    break;
            }
        }

        static void FirstLoad(ref bool loadStatus)
        {
            
            Console.Clear();

            AnsiConsole.Progress()
                .Columns(
                    new SpinnerColumn(),
                    new TaskDescriptionColumn(),
                    new ProgressBarColumn
                    {
                        CompletedStyle = new Style(Color.Green),
                        FinishedStyle = new Style(Color.Lime),
                        RemainingStyle = new Style(Color.Grey)
                    },
                    new PercentageColumn())
                .Start(ctx =>
                {
                    var task = ctx.AddTask("Loading...");

                    while (!ctx.IsFinished)
                    {
                        task.Increment(5);
                        Thread.Sleep(50);
                    }
                });

            AnsiConsole.MarkupLine("[green]\t\t\t\tReady![/]");
            Thread.Sleep(600);
            Console.Clear();
            loadStatus = true;
        }

        
    }
}
