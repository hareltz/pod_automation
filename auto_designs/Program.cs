using auto_designs;
using System;

class Program
{
    static void Main(string[] args)
    {
        SQLitePCL.Batteries.Init();

        // Start the database connection
        DBmanager.startConnection();

        // Add a test niche and tags
        //DBmanager.addNiche("TestNiche", new List<string> { "Tag1", "Tag2", "Tag3" });

        // Print all niches and their tags
/*        Console.WriteLine("Niches and Tags in DB:");
        DBmanager.printNichesAndTags();*/

        // Optionally, initialize data from keywords.json
         DBmanager.initData();

        Console.WriteLine("Niches and Tags in DB:");
        DBmanager.printNichesAndTags();

        // Close the database connection
        DBmanager.CloseConnection();
    }
}
