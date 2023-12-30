using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using BCrypt;

namespace AccountAPI
{
    public class AccountManagement
    {
        public AccountModel RegisterAccount()
        {
            AccountModel Account = new AccountModel();
            Console.WriteLine("Tell us your name: ");
            Account.Name = Console.ReadLine() ?? "";

            Console.WriteLine("Tell us your email: ");
            Account.Email = Console.ReadLine() ?? "";

            Console.WriteLine("Tell us your password: ");
            Account.Password = (Console.ReadLine()) ?? "";

            Console.WriteLine("Tell us your Phone Number: ");
            Account.PhoneNumber = Console.ReadLine() ?? "";

            if (CheckNull(Account))
            {
                Console.WriteLine("Registering Account...");
                Account.SucessfullyCreated = true;
                return Account;
            }
            else
            {
                Console.WriteLine("You can't let fields empty!");
            }
            return Account;
 
        }

        public string LoginAccountEmail()
        {
            Console.Write("Insert your email: ");
            string Email = Console.ReadLine() ?? string.Empty;

            return Email;
        }

        public string LoginAccountPassword()
        {
            Console.Write("Insert your password: ");
            string Password = Console.ReadLine() ?? string.Empty;

            return Password;
        }

        private bool CheckNull(AccountModel Account)
        {
            return !string.IsNullOrWhiteSpace(Account.Name) && !string.IsNullOrWhiteSpace(Account.Email) &&
                   !string.IsNullOrWhiteSpace(Account.PhoneNumber) && !string.IsNullOrWhiteSpace(Account.Password) ? true : false; ;
        }

    }

    public class PasswordManager
    {
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
