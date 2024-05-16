using NLog;
using Final_project.Model;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

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
            case "2":
                AddCategory(db, logger);
                break;
            case "3":
                EditCategory(db, logger);
                break;
            case "4":
                DisplayCategoryAndActiveProducts(db, logger);
                break;
            case "5":
                DisplayAllCategoriesAndActiveProducts(db, logger);
                break;
            case "6":
                AddProduct(db, logger);
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

static void AddCategory(NWContext db, Logger logger)
{
    Category category = new Category();
    Console.WriteLine("Enter Category Name:");
    category.CategoryName = Console.ReadLine();
    Console.WriteLine("Enter the Category Description:");
    category.Description = Console.ReadLine();
    ValidationContext context = new ValidationContext(category, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(category, context, results, true);
    if (isValid)
    {
        // check for unique name
        if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
        {
            // generate validation error
            isValid = false;
            results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
        }
        else
        {
            logger.Info("Validation passed");
            db.Categories.Add(category);
            db.SaveChanges();
            logger.Info("Category added to database");
        }
    }
    if (!isValid)
    {
        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
    }
}

static void EditCategory(NWContext db, Logger logger)
{
    Console.WriteLine("Enter the Category ID to edit:");
    int id = int.Parse(Console.ReadLine());
    var category = db.Categories.Find(id);
    if (category != null)
    {
        Console.WriteLine("Enter Category Name:");
        category.CategoryName = Console.ReadLine();
        Console.WriteLine("Enter the Category Description:");
        category.Description = Console.ReadLine();

        ValidationContext context = new ValidationContext(category, null, null);
        List<ValidationResult> results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(category, context, results, true);
        if (isValid)
        {
            db.SaveChanges();
            logger.Info($"Category with ID {id} updated");
        }
        else
        {
            foreach (var result in results)
            {
                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
            }
        }
    }
    else
    {
        Console.WriteLine("Category not found");
        logger.Warn($"Category with ID {id} not found");
    }
}

static void DisplayCategoryAndActiveProducts(NWContext db, Logger logger)
{
    var query = db.Categories.OrderBy(p => p.CategoryId);

    Console.WriteLine("Select the category whose active products you want to display:");
    Console.ForegroundColor = ConsoleColor.DarkRed;
    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
    }
    Console.ForegroundColor = ConsoleColor.White;
    int id = int.Parse(Console.ReadLine());
    Console.Clear();
    logger.Info($"CategoryId {id} selected");
    Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
    Console.WriteLine($"{category.CategoryName} - {category.Description}");
    foreach (Product p in category.Products.Where(p => !p.Discontinued))
    {
        Console.WriteLine($"\t{p.ProductName}");
    }
}

static void DisplayAllCategoriesAndActiveProducts(NWContext db, Logger logger)
{
    var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
    foreach (var item in query)
    {
        Console.WriteLine($"{item.CategoryName}");
        foreach (Product p in item.Products.Where(p => !p.Discontinued))
        {
            Console.WriteLine($"\t{p.ProductName}");
        }
    }
}

static void AddProduct(NWContext db, Logger logger)
{
    Product product = new Product();
    Console.WriteLine("Enter Product Name:");
    product.ProductName = Console.ReadLine();
    Console.WriteLine("Enter the Supplier ID:");
    product.SupplierId = int.Parse(Console.ReadLine());
    Console.WriteLine("Enter the Category ID:");
    product.CategoryId = int.Parse(Console.ReadLine());
    Console.WriteLine("Enter the Quantity Per Unit:");
    product.QuantityPerUnit = Console.ReadLine();
    Console.WriteLine("Enter the Unit Price:");
    product.UnitPrice = decimal.Parse(Console.ReadLine());
    Console.WriteLine("Enter the Units In Stock:");
    product.UnitsInStock = short.Parse(Console.ReadLine());
    Console.WriteLine("Enter the Units On Order:");
    product.UnitsOnOrder = short.Parse(Console.ReadLine());
    Console.WriteLine("Enter the Reorder Level:");
    product.ReorderLevel = short.Parse(Console.ReadLine());
    Console.WriteLine("Is the product discontinued? (true/false):");
    product.Discontinued = bool.Parse(Console.ReadLine());

    db.Products.Add(product);
    db.SaveChanges();
    logger.Info("Product added to database");
}