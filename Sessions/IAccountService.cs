using FileManager.Models;

namespace FileManager.Sessions
{
    public interface IAccountService
    {
        public Account Login(string username, string password);
 
    }
}
