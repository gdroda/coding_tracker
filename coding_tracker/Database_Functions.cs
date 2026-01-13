using Spectre.Console;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.Data.Sqlite;

namespace coding_tracker
{
    internal class Database_Functions
    {
        static SqliteConnection EstablishConnection()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

            try
            {
                using var connection = new SqliteConnection(builder["ConnectionStrings:DatabaseLocation"]);
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to database: " + ex.Message);
                throw;
            }
        }

        static void GenericMenu()
        {
            var choice = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("")
                                .WrapAround()
                                .HighlightStyle(new Style(Color.Green, Color.Black, Decoration.Bold))
                                .AddChoices("Return"));
            switch (choice)
            {
                case "Return":
                    ReturnToMenu();
                    break;
            }
        }

        static void ReturnToMenu()
        {
            bool temp_load_status = true;
            UserInput.PrintMenu(ref temp_load_status);
        }

        public static void AddSession()
        {
            AnsiConsole.MarkupLine("Please enter date and time of session start in the following format: DD/MM/YYYY HH:mm");
            AnsiConsole.MarkupLine("Example: 14/09/1993 15:35\n");
            string dateInputStart = AnsiConsole.Ask<string>("Input: ");

            int duration;
            bool succParseStart = DateTime.TryParse(dateInputStart, out DateTime startDate);
            if (!succParseStart)
            {
                Console.WriteLine("Wrong Date format. Session not logged.");
                GenericMenu();
            }

            AnsiConsole.MarkupLine("\nPlease enter date and time of session end in the following format: DD/MM/YYYY HH:mm");
            AnsiConsole.MarkupLine("Example: 14/09/1993 15:35\n");

            string dateInputEnd = AnsiConsole.Ask<string>("Input: ");
            bool succParseEnd = DateTime.TryParse(dateInputEnd, out DateTime endDate);
            if (!succParseEnd)
            {
                Console.WriteLine("\nWrong Date format. Session not logged.");
                GenericMenu();
            }

            duration = (int)endDate.Subtract(startDate).TotalMinutes;
            if (duration <= 0)
            {
                Console.WriteLine("\nSession End Date is before Start Date. Session not logged.");
                GenericMenu();
            }
            var command = "INSERT INTO CodingSessions (StartTime, EndTime, Duration) VALUES (@start_time, @end_time, @durationInMin)";
            EstablishConnection().Execute(command, new { start_time = dateInputStart, end_time = dateInputEnd, durationInMin = duration });

            AnsiConsole.MarkupLine("[green]\nSession successfully logged![/]");

            GenericMenu();
        } 

        public static void ShowDatabase()
        {
            var command = @"SELECT * FROM CodingSessions";
            var sessions = EstablishConnection().Query<CodingSession>(command).ToList();

            var table = new Table();

            table.AddColumn("ID");
            table.AddColumn("Start Date");
            table.AddColumn("End Date");
            table.AddColumn("Duration");

            foreach (var ses in sessions)
            {
                table.AddRow($"{ses.Id}", $"{ses.StartTime}", $"{ses.EndTime}", $"{ses.Duration} min.");
            }

            AnsiConsole.Write(table);



            var choice = AnsiConsole.Prompt(
                            new SelectionPrompt<string>()
                                .Title("")
                                .WrapAround()
                                .HighlightStyle(new Style(Color.Green, Color.Black, Decoration.Bold))
                                .AddChoices("Delete by ID", "Return"));
            switch (choice)
            {
                case "Delete by ID":
                    DeleteLineByID();
                    break;
                case "Return":
                    ReturnToMenu();
                    break;
            }
        }

        static void DeleteLineByID()
        {
            int idToDelete = AnsiConsole.Ask<int>("Ented the ID of the log you wish to delete: ");

            var commanda = $@"DELETE FROM CodingSessions WHERE Id={idToDelete}";
            EstablishConnection().Execute(commanda);
            var commandb = @"REINDEX CodingSessions";
            EstablishConnection().Execute(commandb);

            ShowDatabase();
        }

        public static void EmptyDatabase()
        {
            var commanda = @"DELETE FROM CodingSessions;";
            EstablishConnection().Execute(commanda);
            var commandb = $@"DELETE FROM sqlite_sequence WHERE name='CodingSessions'";
            EstablishConnection().Execute(commandb);

            ReturnToMenu();
        }

    }

    public class CodingSession()
    {
        public int Id { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int Duration { get; set; }
    }
}
