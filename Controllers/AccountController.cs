using FileManager.Sessions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using FileManager.Models;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Routing.Constraints;

namespace FileManager.Controllers
{
    public class AccountController : Controller
    {
        private IAccountService _accountService;
        private readonly IWebHostEnvironment _iweb;
        public AccountController(IAccountService accountService, IWebHostEnvironment iweb)
        {
            _accountService = accountService;
            _iweb = iweb;
        }
   
        private readonly string wwwrootDirectory =
            Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string username, string password)
        {
            var account = _accountService.Login(username, password);
            if (account != null)
            {
                HttpContext.Session.SetString("username", username);
                return RedirectToAction("Welcome", "Account");

            }
            else
            {
                ViewBag.Message = "Invalid Login";
                return View();
            }

            return View();
        }

        public IActionResult Welcome()
        {
            Account ic = new Account();

            // Get the username from the session
            var username = HttpContext.Session.GetString("username");

            if (!string.IsNullOrEmpty(username))
            {
                // Create the folder path based on the username
                var userFolder = Path.Combine(_iweb.WebRootPath, "Folder", username);

                // Ensure the user's folder exists; create it if it doesn't
                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder);
                }

                DirectoryInfo di = new DirectoryInfo(userFolder);
                FileInfo[] fileinfo = di.GetFiles();
                ic.Fileimage = fileinfo;
            }

            return View(ic);
        }


        [HttpPost]
        public async Task<IActionResult> Welcome(IFormFile file)
        {
            // Get the username from the session
            var username = HttpContext.Session.GetString("username");

            if (!string.IsNullOrEmpty(username) && file != null)
            {
                // Create the folder path based on the username
                var userFolder = Path.Combine(_iweb.WebRootPath, "Folder", username);

                // Ensure the user's folder exists; create it if it doesn't
                if (!Directory.Exists(userFolder))
                {
                    Directory.CreateDirectory(userFolder);
                }

                var filesave = Path.Combine(userFolder, file.FileName);
                var stream = new FileStream(filesave, FileMode.Create);
                await file.CopyToAsync(stream);
                stream.Close();
            }

            return RedirectToAction("Welcome");
        }

        public IActionResult Delete(string imgdel)
        {
            var username = HttpContext.Session.GetString("username");

            if (!string.IsNullOrEmpty(username))
            {
                var userFolder = Path.Combine(_iweb.WebRootPath, "Folder", username, imgdel);
                FileInfo fi = new FileInfo(userFolder);

                if (fi != null && fi.Exists)
                {
                    System.IO.File.Delete(userFolder);
                    fi.Delete();
                }
            }

            return RedirectToAction("Welcome");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Remove("username");
            return RedirectToAction("Index", "Account");
        }

        public IActionResult Edit(string imgedit)
        {
            var username = HttpContext.Session.GetString("username");

            if (!string.IsNullOrEmpty(username))
            {
                var userFilePath = Path.Combine(_iweb.WebRootPath, "Folder", username, imgedit);

                if (System.IO.File.Exists(userFilePath))
                {
                    // Read the content of the file
                    string fileContent = System.IO.File.ReadAllText(userFilePath);

                    // Pass the file content to the "EditFile" view
                    return View("EditFile", fileContent);
                }
            }

            return RedirectToAction("Welcome");
        }



        [HttpPost]
        public IActionResult SaveEdit(string fileName, string fileContent)
        {
            var username = HttpContext.Session.GetString("username");

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(fileName))
            {
                // Construct the path to the user's file based on the username and fileName
                var userFilePath = Path.Combine(_iweb.WebRootPath, "Folder", username, fileName);

                try
                {
                    // Write the edited content back to the file
                    System.IO.File.WriteAllText(userFilePath, fileContent);

                    // Redirect to the "Welcome" action after successful save
                    return RedirectToAction("Welcome");
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that occur during the file save
                    // You may want to log the error or display an error message to the user
                    ViewBag.ErrorMessage = "An error occurred while saving the file: " + ex.Message;
                }
            }

            return RedirectToAction("Welcome");
        }





    }
}
