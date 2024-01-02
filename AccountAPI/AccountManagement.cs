using DataBase;
using System.Text.RegularExpressions;

namespace AccountAPI
{
    public class AccountManagement
    {
        public bool CheckName(string Name)
        {
            if(Name.Length >= 8 && Name.Length <= 30)
            {
                return true;
            }
            else
            {
                Console.WriteLine("Insert a valid name beetwen 8 and 30 characters!");
            }
            return false;
        }

        public bool CheckPatternEmail(string Email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(emailPattern);

            if(regex.IsMatch(Email)) 
            {
                return true;
            }
            Console.WriteLine("Insert a valid email! Example: userexample@gmail.com");
            return false;
        }

        public bool CheckPatternPassword(string Password) 
        {
            if(Password.Length >= 2)
            {
                return true;
            }
            Console.WriteLine("Min of 2");
            return false;
        }
    }

    public class PasswordManager
    {
       FileHandling log = new FileHandling();
        public string HashPassword(AccountModel Account)
        {
                string salt = BCrypt.Net.BCrypt.GenerateSalt();

                string HashPassword = BCrypt.Net.BCrypt.HashPassword(Account.Password, salt);

                return HashPassword;
        }

        public bool CheckPassword(string DBPassword, string UserPassword)
        {
                return BCrypt.Net.BCrypt.Verify(UserPassword, DBPassword);
        }
    }
}
