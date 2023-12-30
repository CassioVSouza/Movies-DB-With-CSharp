using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBase
{
    public class FileHandling
    {
        public void SaveInLog(string Error)
        {
            try
            {
                string filePath = GetFilePath();

                using (StreamWriter file = new StreamWriter(filePath))
                {
                    file.WriteLine(Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error happened trying to save the log!: {ex}");
            }
        }

        private string GetFilePath()
        {
            string directoryPath = "LogMoviesApplication";
            string fileName = "Log.txt";
            string filePath = Path.Combine(directoryPath, fileName);

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return filePath;
        }

        public string GetConnectionString()
        {
            string directoryPath = "MovieDatabase";
            string fileName = "Database.db";
            string connectionString = Path.Combine("Data Source=" + directoryPath, fileName + ";Version=3;");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
            return connectionString;
        }
    }
}
