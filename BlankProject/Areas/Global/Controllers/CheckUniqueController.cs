using BLL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Etka.Areas.Global.Controllers
{

    /// <summary>
    /// برای پروپرتی هایی که باید یکتا بودن آنها چک شود 
    /// از این کنترلر استفاده میشود.
    /// مثلا تلفن کاربر
    /// </summary>
    [Area("Global")]
    public class CheckUniqueController : Controller
    {
        private readonly IUserManager UserManager;
        public CheckUniqueController(IUserManager _UserManager)
        {
            UserManager = _UserManager;
        }



        #region user

        #region بررسی یکتا بودن تلفن کاربر
        public IActionResult MobileIsUnique(string Mobile, long? Id)
        {
            var IsValid = UserManager.MobileIsUnique(Mobile, Id);
            return Json(IsValid);
        }
        #endregion


        #region بررسی یکتا بودن نام کاربری
        public IActionResult UsernameIsUnique(string Username, long? Id)
        {
            var IsValid = UserManager.UsernameIsUnique(Username, Id);
            return Json(IsValid);
        }
        #endregion

        #endregion





    }
}