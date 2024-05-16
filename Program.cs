using NLog;
using Final_project.Model;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    var db = new NWContext();
    string choice;
    do
    {
        Console.WriteLine("1) Display Categories");
        Console.WriteLine("2) Add Category");
        Console.WriteLine("3) Edit a specified record from the Categories table");
        Console.WriteLine("4) Display Category and related active products");
        Console.WriteLine("5) Display all Categories and their related active products");
        Console.WriteLine("6) Add new record to Products table");
        Console.WriteLine("7) Edit a specified record from the Products table");
        Console.WriteLine("8) Display product name from all records in the Products table");
        Console.WriteLine("9) Display a specific Product");
        Console.WriteLine("10) Delete a specified existing record from the Products table");
        Console.WriteLine("11) Delete a specified existing record from the Categories table");
        Console.WriteLine("\"q\" to quit");
        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");
        if (choice.ToLower() == "q")
        {
            // Exit the loop if the user wants to quit
            break;
        }

        switch (choice)
        {
            case "1":
                DisplayCategories(db, logger);
                break;
            default:
                Console.WriteLine("Invalid option. Please try again.");
                logger.Warn($"Invalid option {choice} selected");
                break;
        }

        Console.WriteLine();
    } while (true);
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

logger.Info("Program ended");

static void DisplayCategories(NWContext db, Logger logger)
{
    var query = db.Categories.OrderBy(p => p.CategoryName);

    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.WriteLine($"{query.Count()} records returned");
    Console.ForegroundColor = ConsoleColor.Yellow;
    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryName} - {item.Description}");
    }
    Console.ForegroundColor = ConsoleColor.White;
}