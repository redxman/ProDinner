using System.Web.Mvc;
using System.Web.UI;
using Omu.AwesomeMvc;
using Omu.ProDinner.Core;
using Omu.ProDinner.Core.Model;
using Omu.ProDinner.Core.Service;
using Omu.ProDinner.WebUI.Dto;
using Omu.ProDinner.WebUI.Mappers;

namespace Omu.ProDinner.WebUI.Controllers
{
    /// <summary>
    /// generic crud controller for entities where there is difference between the edit and create view
    /// </summary>
    /// <typeparam name="TEntity"> the entity</typeparam>
    /// <typeparam name="TCreateInput">create viewmodel</typeparam>
    /// <typeparam name="TEditInput">edit viewmodel</typeparam>
    public abstract class Crudere<TEntity, TCreateInput, TEditInput> : BaseController
        where TCreateInput : new()
        where TEditInput : Input, new()
        where TEntity : DelEntity, new()
    {
        protected readonly ICrudService<TEntity> service;
        private readonly IMapper<TEntity, TCreateInput> createMapper;
        private readonly IMapper<TEntity, TEditInput> editMapper;

        protected virtual string EditView
        {
            get { return "edit"; }
        }

        public Crudere(ICrudService<TEntity> service, IMapper<TEntity, TCreateInput> createMapper, IMapper<TEntity, TEditInput> editMapper)
        {
            this.service = service;
            this.createMapper = createMapper;
            this.editMapper = editMapper;
        }

        public virtual ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View(createMapper.MapToInput(new TEntity()));
        }

        [HttpPost]
        public ActionResult Create(TCreateInput input)
        {
            if (!ModelState.IsValid)
                return View(input);
            var id = service.Create(createMapper.MapToEntity(input, new TEntity()));
            var e = service.Get(id);
            return Json(new { Content = this.RenderView(RowViewName, new[] { e }) });
        }

        [OutputCache(Location = OutputCacheLocation.None)]//for ie
        public ActionResult Edit(int id)
        {
            var entity = service.Get(id);
            if (entity == null) throw new ProDinnerException("this entity doesn't exist anymore");
            return View(EditView, editMapper.MapToInput(entity));
        }

        [HttpPost]
        public ActionResult Edit(TEditInput input)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View(EditView, input);
                var e = editMapper.MapToEntity(input, service.Get(input.Id));
                service.Save();
                return Json(new { input.Id, Content = this.RenderView(RowViewName, new[] { e }), Type = typeof(TEntity).Name.ToLower() });
            }
            catch (ProDinnerException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            service.Delete(id);
            return Json(new { Id = id, Type = typeof(TEntity).Name.ToLower() });
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult Restore(int id)
        {
            service.Restore(id);

            return Json(new { Id = id, Content = this.RenderView(RowViewName, new[] { service.Get(id) }), Type = typeof(TEntity).Name.ToLower() });
        }

        protected abstract string RowViewName { get; }
    }
}