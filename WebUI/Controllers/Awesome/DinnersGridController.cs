using System.Linq;
using System.Web.Mvc;
using Omu.AwesomeMvc;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Repository;

namespace Omu.ProDinner.WebUI.Controllers.Awesome
{
    public class DinnerGridController : Controller
    {
        private readonly IRepo<Dinner> repo;

        public DinnerGridController(IRepo<Dinner> repo)
        {
            this.repo = repo;
        }

        public ActionResult GetItems(GridParams g, string search, int? chef, int[] meals)
        {
            g.PageSize = 10;
            var list = repo.Where(o => o.Name.Contains(search), User.IsInRole("admin"));

            if (chef.HasValue) list = list.Where(o => o.ChefId == chef.Value);
            if (meals != null) list = list.Where(o => meals.All(m => o.Meals.Select(meal => meal.Id).Contains(m)));

            //by default ordering by id
            list = list.OrderByDescending(o => o.Id);

            return Json(new GridModelBuilder<Dinner>(list.AsQueryable(), g)
                {
                    // Key = "Id", this is needed for EF to always sort by something, but we already do order by Id
                    Map = o => new
                        {
                            o.Id,
                            o.IsDeleted,
                            o.Name,
                            CountryName = o.Country.Name,
                            o.Address,
                            MealsCount = o.Meals.Count,
                            ChefName = o.Chef.FirstName + " " + o.Chef.LastName,
                        }
                }.Build());
        }
    }
}