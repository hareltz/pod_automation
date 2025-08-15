using Microsoft.Data.Sqlite;

namespace auto_designs
{
    internal static class DBmanager
    {
        public static Mutex mtx = new Mutex();
        private static SqliteConnection SQL;

        /// <summary>
        /// this function initialize the info to the DB
        /// </summary>
        public static void InitializeInfo() 
        {
            SQL = new SqliteConnection("Data Source=tagsDB.db");
            initialzeTagsTable();
            initialzeNicheTable();
            initialzeExistItemsTable();

            // TODO: if table is empty : fill the table with the tage and niches
        }

        /// <summary>
        /// this table adds the table for the niches
        /// </summary>
        private static void initialzeNicheTable()
        {
            string creatsNicheTableQuery = "CREATE TABLE IF NOT EXISTS Niche (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " NAME NVARCHAR(255),"; 

            SqliteCommand createTracksTableCmd = SQL.CreateCommand();
            createTracksTableCmd.CommandText = creatsNicheTableQuery;
            createTracksTableCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// this function adds the table for the tags
        /// </summary>
        private static void initialzeTagsTable()
        {
            string createTagsTableQuery = "CREATE TABLE IF NOT EXISTS Tags (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " TAG NVARCHAR(255)," +
                        " NICHE_ID INT"; // add foreign key

            SqliteCommand createTracksTableCmd = SQL.CreateCommand();
            createTracksTableCmd.CommandText = createTagsTableQuery;
            createTracksTableCmd.ExecuteNonQuery();
        }

        
        /// <summary>
        /// this table adds the table for the items
        /// </summary>
        private static void initialzeExistItemsTable()
        {
            // ADD THE OPTION TO CHECK IF THERES A VIDEO MADE FOR THIS ITEM LATER ON
            string creatsItemsQuery = "CREATE TABLE IF NOT EXISTS Items (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " NAME NVARCHAR(255)," +
                        " NICHE_ID INT "; // add foreign key
            
            SqliteCommand createTracksTableCmd = SQL.CreateCommand();
            createTracksTableCmd.CommandText = creatsItemsQuery;
            createTracksTableCmd.ExecuteNonQuery();
        }
    }
}
