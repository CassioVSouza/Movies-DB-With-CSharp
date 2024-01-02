
namespace DataBase
{
    /// <summary>
    /// Class for handling file operations such as logging errors and providing a connection string.
    /// </summary>
    public class FileHandling
    {
        /// <summary>
        /// Saves the specified error message in a log file.
        /// </summary>
        /// <param name="error">The error message to be logged.</param>
        public void SaveInLog(string error)
        {
            try
            {
                // Get the file path for the log file
                string filePath = GetFilePath();

                // Write the error message to the log file
                using (StreamWriter file = new StreamWriter(filePath))
                {
                    file.WriteLine(error);
                }
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"The log file was not found!: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while trying to save the log: {ex}");
            }
        }

        /// <summary>
        /// Retrieves the file path for the log file. Creates the log directory if it doesn't exist.
        /// </summary>
        /// <returns>The file path for the log file.</returns>
        private string GetFilePath()
        {
            string directoryPath = "LogMoviesApplication";
            string fileName = "Log.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            // Create the log directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }

        /// <summary>
        /// Retrieves the connection string for the SQLite database file. Creates the database directory if it doesn't exist.
        /// </summary>
        /// <returns>The connection string for the SQLite database file.</returns>
        public string GetConnectionString()
        {
            string directoryPath = "MovieDatabase";
            string fileName = "Database.db";
            string connectionString = Path.Combine("Data Source=" + directoryPath, fileName + ";Version=3;");

            // Create the database directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return connectionString;
        }
    }
}
