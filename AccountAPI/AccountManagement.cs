// Importing necessary libraries
using DataBase;
using System.Text.RegularExpressions;

namespace AccountAPI
{
    // Class for managing user account-related functionalities
    public class AccountManagement
    {
        // Method to check if the provided name is valid (between 8 and 30 characters)
        public bool CheckName(string Name)
        {
            if (Name.Length >= 8 && Name.Length <= 30)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Insert a valid name between 8 and 30 characters!");
            }
            return false;
        }

        // Method to check if the provided email follows a valid pattern
        public bool CheckPatternEmail(string Email)
        {
            // Regular expression pattern for a valid email
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(emailPattern);

            if (regex.IsMatch(Email))
            {
                return true;
            }
            Console.WriteLine("Insert a valid email! Example: userexample@gmail.com");
            return false;
        }

        // Method to check if the provided password meets the minimum length requirement (8 characters)
        public bool CheckPatternPassword(string Password)
        {
            if (Password.Length >= 8)
            {
                return true;
            }
            Console.WriteLine("Password should have a minimum length of 8 characters.");
            return false;
        }
    }

    // Class for managing password-related functionalities
    public class PasswordManager
    {
        // Creating an instance of FileHandling for logging purposes
        FileHandling log = new FileHandling();

        // Method to hash a user's password using BCrypt
        public string HashPassword(AccountModel Account)
        {
            // Generating a salt for password hashing
            string salt = BCrypt.Net.BCrypt.GenerateSalt();

            // Hashing the password using BCrypt
            string HashPassword = BCrypt.Net.BCrypt.HashPassword(Account.Password, salt);

            return HashPassword;
        }

        // Method to verify a user's password against the hashed password stored in the database
        public bool CheckPassword(string DBPassword, string UserPassword)
        {
            // Verifying the user's password using BCrypt
            return BCrypt.Net.BCrypt.Verify(UserPassword, DBPassword);
        }
    }
}
