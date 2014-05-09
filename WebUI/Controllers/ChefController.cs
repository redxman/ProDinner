using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Service;
using Omu.ProDinner.WebUI.Dto;
using Omu.ProDinner.WebUI.Mappers;

namespace Omu.ProDinner.WebUI.Controllers
{
    public class ChefController : Cruder<Chef, ChefInput>
    {
        public ChefController(ICrudService<Chef> service, IMapper<Chef, ChefInput> v)
            : base(service, v)
        {
        }

        protected override string RowViewName
        {
            get { return "ListItems/Chef"; }
        }
    }
}