using Omu.ValueInjecter;

namespace Omu.ProDinner.WebUI.Mappers
{
    public class Mapper<TEntity, TInput> : IMapper<TEntity, TInput>
        where TEntity : class, new()
        where TInput : new()
    {
        public virtual TInput MapToInput(TEntity entity)
        {
            var input = new TInput();
            input.InjectFrom(entity)
                .InjectFrom<NormalToNullables>(entity)
                .InjectFrom<EntitiesToInts>(entity);
            return input;
        }

        public virtual TEntity MapToEntity(TInput input, TEntity e)
        {
            e.InjectFrom(input)
               .InjectFrom<IntsToEntities>(input)
               .InjectFrom<NullablesToNormal>(input);
            return e;
        }
    }
}