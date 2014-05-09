using System.Linq;
using System.Web.Mvc;
using Omu.AwesomeMvc;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Repository;

namespace Omu.ProDinner.WebUI.Controllers.Awesome
{
    public class ChefGridController : Controller
    {
        private readonly IRepo<Chef> repo;

        public ChefGridController(IRepo<Chef> repo)
        {
            this.repo = repo;
        }

        public ActionResult GetItems(GridParams g, string parent)
        {
            var data = repo.Where(o => o.FirstName.StartsWith(parent) || o.LastName.StartsWith(parent), User.IsInRole("admin"))
                .OrderByDescending(o => o.Id);
            
            return Json(new GridModelBuilder<Chef>(data.AsQueryable(), g)
            {
                Map = chef => new
                {
                    chef.FirstName,
                    chef.LastName,
                    Country = chef.Country.Name,
                    Actions = this.RenderView("ChefGridActions", chef) // view in Shared/ChefGridActions.cshtml
                }
            }.Build());
        }
    }
}