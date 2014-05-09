using Omu.AwesomeMvc;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Service;
using Omu.ProDinner.WebUI.Dto;
using Omu.ProDinner.WebUI.Mappers;

namespace Omu.ProDinner.WebUI.Controllers
{
    public class CountryController : Cruder<Country, CountryInput>
    {
        public CountryController(ICrudService<Country> service, IMapper<Country, CountryInput> v)
            : base(service, v)
        {
        }

        protected override string RowViewName
        {
            get { return "ListItems/Country"; }
        }
    }
}