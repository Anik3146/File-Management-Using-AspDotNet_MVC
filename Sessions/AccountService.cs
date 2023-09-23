using FileManager.Models;

namespace FileManager.Sessions
{
    public class AccountService : IAccountService
    {
        private List<Account> accounts;

        public AccountService()
        {
            accounts = new List<Account>()
            {
                new Account()
                {
                    Username = "admin",
                    Password = "admin"
                },

                 new Account()
                {
                    Username = "anik3146",
                    Password = "anik3146"
                },

                  new Account()
                {
                    Username = "gtrbd",
                    Password = "gtrbd"
                }
            };
        }
        public Account Login(string username, string password)
        {
            return accounts.SingleOrDefault(x => x.Username == username && x.Password == password);
        }
    }

}
