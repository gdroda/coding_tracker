using Spectre.Console;
using Microsoft.Extensions.Configuration;
using Dapper;
using Microsoft.Data.Sqlite;


namespace coding_tracker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
            CreateDatabase(builder);

            bool loadStatus = false;
            UserInput.PrintMenu(ref loadStatus);
        }

        static void CreateDatabase(IConfigurationRoot builder)
        {
            try
            {
                using var connection = new SqliteConnection(builder["ConnectionStrings:DatabaseLocation"]);
                connection.Open();
                var tableCommand = @"CREATE TABLE IF NOT EXISTS CodingSessions (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    StartTime TEXT NOT NULL,
                                    EndTime TEXT NOT NULL,
                                    Duration INTEGER
                                );";
                connection.Execute(tableCommand);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error connecting to database: " + ex.Message);
                return;
            }
        }
        
    }
}

