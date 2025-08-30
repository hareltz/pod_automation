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
        public static void startConnection()
        {
            FilesManager.createDBFile();
            SQL = new SqliteConnection("Data Source=tags.db");
            // Open the SQL:
            try { SQL.Open(); }
            catch (Exception e)
            { throw new Exception("Faild to connect to the CA Data Base.."); }

            if (SQL.State != System.Data.ConnectionState.Open)
                Console.WriteLine("Faild to connect to the CA Data Base..");

            CreateTagsTable();
            CreateNicheTable();
            CreateItemsTable();

            // TODO: if table is empty : fill the table with the tage and niches
        }  

        /// <summary>
        /// this function closes the connection to the DB
        /// </summary>
        public static void CloseConnection()
        {
            SQL.Close();
        }

        /// <summary>
        /// this table adds the table for the niches
        /// </summary>
        private static void CreateNicheTable()
        {
            string createTableQuery = "CREATE TABLE IF NOT EXISTS Niche (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        "NAME NVARCHAR(255));";

            SqliteCommand createTable = SQL.CreateCommand();
            createTable.CommandText = createTableQuery;
            createTable.ExecuteNonQuery();

        }

        /// <summary>
        /// this function adds the table for the tags
        /// </summary>
        private static void CreateTagsTable()
        {
            string createTagsTableQuery = "CREATE TABLE IF NOT EXISTS Tags (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " TAG NVARCHAR(255)," +
                        " NICHE_ID INT);"; // add foreign key

            SqliteCommand createTracksTableCmd = SQL.CreateCommand();
            createTracksTableCmd.CommandText = createTagsTableQuery;
            createTracksTableCmd.ExecuteNonQuery();
        }


        /// <summary>
        /// this table adds the table for the items
        /// </summary>
        private static void CreateItemsTable()
        {
            // ADD THE OPTION TO CHECK IF THERES A VIDEO MADE FOR THIS ITEM LATER ON
            string creatsItemsQuery = "CREATE TABLE IF NOT EXISTS Items (" +
                        "ID INTEGER PRIMARY KEY AUTOINCREMENT," +
                        " NAME NVARCHAR(255)," +
                        " NICHE_ID INT);"; // add foreign key

            SqliteCommand createTracksTableCmd = SQL.CreateCommand();
            createTracksTableCmd.CommandText = creatsItemsQuery;
            createTracksTableCmd.ExecuteNonQuery();
        }

        /// <summary>
        /// this function adds a niche and its associated tags to the DB
        /// </summary>
        /// <param name="nicheName">the niche name</param>
        /// <param name="tags">the tags</param>
        public static void addNiche(string nicheName, List<string> tags)
        {
            if (cheakNicheExists(nicheName)) { return; } // Niche already exists, do not add again


            // Insert the niche into the Niche table
            mtx.WaitOne();
            string insertNicheQuery = "INSERT INTO Niche (NAME) VALUES (@name);";
            SqliteCommand insertNicheCmd = SQL.CreateCommand();
            insertNicheCmd.CommandText = insertNicheQuery;
            insertNicheCmd.Parameters.AddWithValue("@name", nicheName);
            insertNicheCmd.ExecuteNonQuery();
            mtx.ReleaseMutex();

            // Insert each tag into the Tags table
            foreach (var tag in tags)
            {
                addItem(tag, nicheName);
            }
        }


        /// <summary>
        /// this function adds an item to the DB
        /// </summary>
        /// <param name="itemName">the item namd</param>
        /// <param name="nicheName">the niche name</param>
        public static void addItem(string itemName, string nicheName)
        {
            mtx.WaitOne();
            // Insert the item into the Items table
            string insertItemQuery = "INSERT INTO Items (NAME, NICHE_ID) VALUES (@name, @nicheId);";
            SqliteCommand insertItemCmd = SQL.CreateCommand();
            insertItemCmd.CommandText = insertItemQuery;
            insertItemCmd.Parameters.AddWithValue("@name", itemName);
            insertItemCmd.Parameters.AddWithValue("@nicheId", getNicheId(nicheName));
            insertItemCmd.ExecuteNonQuery();
            mtx.ReleaseMutex();
        }

        /// <summary>
        /// this function prints all the niches and their associated tags from the DB
        /// </summary>
        public static void printNichesAndTags()
        {
            mtx.WaitOne();
            string selectNichesQuery = "SELECT ID, NAME FROM Niche;";
            SqliteCommand selectNichesCmd = SQL.CreateCommand();
            selectNichesCmd.CommandText = selectNichesQuery;
            SqliteDataReader nichesReader = selectNichesCmd.ExecuteReader();
            while (nichesReader.Read())
            {
                long nicheId = nichesReader.GetInt64(0);
                string nicheName = nichesReader.GetString(1);
                Console.WriteLine($"Niche: {nicheName} (ID: {nicheId})");
                string selectTagsQuery = "SELECT TAG FROM Tags WHERE NICHE_ID = @nicheId;";
                SqliteCommand selectTagsCmd = SQL.CreateCommand();
                selectTagsCmd.CommandText = selectTagsQuery;
                selectTagsCmd.Parameters.AddWithValue("@nicheId", nicheId);
                SqliteDataReader tagsReader = selectTagsCmd.ExecuteReader();
                List<string> tags = new List<string>();
                while (tagsReader.Read())
                {
                    tags.Add(tagsReader.GetString(0));
                }
                tagsReader.Close();
                Console.WriteLine("Tags: " + string.Join(", ", tags));
                Console.WriteLine();
            }
            nichesReader.Close();
            mtx.ReleaseMutex();
        }

        /// <summary>
        /// this function initializes the data in the DB from the JSON file
        /// </summary>
        public static void initData()
        {
            FilesManager.deserializeKeywordsData();
            foreach (var niche in FilesManager.GetKeywordsData())
            {
                addNiche(niche.Key, niche.Value);
            }
        }

        /// <summary>
        /// this function gets the ID of a niche given its name
        /// </summary>
        /// <param name="nicheName">the niche name</param>
        /// <returns>the niche id</returns>
        private static long getNicheId(string nicheName)
        {
            mtx.WaitOne();
            string selectNicheQuery = "SELECT ID FROM Niche WHERE NAME = @name;";
            SqliteCommand selectNicheCmd = SQL.CreateCommand();
            selectNicheCmd.CommandText = selectNicheQuery;
            selectNicheCmd.Parameters.AddWithValue("@name", nicheName);
            SqliteDataReader nicheReader = selectNicheCmd.ExecuteReader();
            long nicheId = -1;
            if (nicheReader.Read())
            {
                nicheId = nicheReader.GetInt64(0);
            }
            nicheReader.Close();
            mtx.ReleaseMutex();
            return nicheId;
        }

        /// <summary>
        /// this function checks if a niche exists in the DB given its name
        /// </summary>
        /// <param name="nicheName">the niche name</param>
        /// <returns>true if the niche exists</returns>
        private static bool cheakNicheExists(string nicheName)
        {
            mtx.WaitOne();
            string selectNicheQuery = "SELECT COUNT(1) FROM Niche WHERE NAME = @name;";
            SqliteCommand selectNicheCmd = SQL.CreateCommand();
            selectNicheCmd.CommandText = selectNicheQuery;
            selectNicheCmd.Parameters.AddWithValue("@name", nicheName);
            long count = (long)selectNicheCmd.ExecuteScalar();
            mtx.ReleaseMutex();
            return count > 0;
        }
    }
}
