using BLL;
using BLL.Interface;
using Domain.Entities;
using DTO.Base;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Food.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[UserAuthorize(Area: "admin", Controller: "forbidden", Action: "index")]
    public class ForbiddenController : Controller
    {


        // GET: PodcastSystem/Abouts
        public ActionResult Index()
        {
            return View("ErrorPage", new ErrorPageDTO
            {
                Title = "دسترسی غیر مجاز",
                ErrorCode = "403",
                Description = "شما به این صفحه دسترسی ندارید!"
            });
        }



    }
}
