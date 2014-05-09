using System.Web.Mvc;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Service;
using Omu.ProDinner.WebUI.Dto;
using Omu.ProDinner.WebUI.Mappers;

namespace Omu.ProDinner.WebUI.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserController : Crudere<User, UserCreateInput, UserEditInput>
    {
        private new readonly IUserService service;

        public UserController(IMapper<User, UserCreateInput> v, IMapper<User, UserEditInput> ve, IUserService service) : base(service, v, ve)
        {
            this.service = service;
        }

        public ActionResult ChangePassword(int id)
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordInput input)
        {
            if (!ModelState.IsValid) return View(input);
            service.ChangePassword(input.Id, input.Password);
            return Json(new{ Login = service.Get(input.Id).Login});
        }

        protected override string RowViewName
        {
            get { return "ListItems/User"; }
        }
    }
}