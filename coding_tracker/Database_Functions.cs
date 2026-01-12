using Spectre.Console;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.Data.Sqlite;
using System.Net.NetworkInformation;

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
            AnsiConsole.MarkupLine("Please enter date and time in the following format: DD/MM/YYYY HH:mm");
            AnsiConsole.MarkupLine("Example: 14/09/1993 15:35\n");
            string dateInput = AnsiConsole.Ask<string>("Input: ");

            DateTime date;
            bool succParse = DateTime.TryParse(dateInput, out date);
            if (succParse)
                Console.WriteLine("Succ!");
            else
            {
                Console.WriteLine("Wrong Date format.");
                GenericMenu();
            }          
                

            //var command = "INSERT INTO CodingSessions (StartTime, EndTime) VALUES (@start_time, @end_time)";
            //EstablishConnection().Execute(command, new { start_time = "test_string", end_time = "test_2" });

            //ReturnToMenu();
        } 

        public static void ShowDatabase()
        {
            var command = @"SELECT * FROM CodingSessions";
            var sessions = EstablishConnection().Query<CodingSession>(command).ToList();

            foreach (var ses in sessions)
            {
                Console.WriteLine($"id = {ses.SessionId}, start time = {ses.StartTime}, end time = {ses.EndTime}, duration = {ses.SessionDuration}");
            }

            GenericMenu();
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
        public int SessionId { get; set; }
        public string? StartTime { get; set; }
        public string? EndTime { get; set; }
        public int SessionDuration { get; set; }
    }
}
