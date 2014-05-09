using Omu.ProDinner.Core.Model;
using Omu.ProDinner.WebUI.Dto;

namespace Omu.ProDinner.WebUI.Mappers
{
    public class DinnerMapper : Mapper<Dinner, DinnerInput>
    {
        public override Dinner MapToEntity(DinnerInput input, Dinner e)
        {
            var entity = base.MapToEntity(input, e);

            entity.Start = entity.Start.AddHours(input.Hour).AddMinutes(input.Minute);
            entity.End = entity.Start.AddMinutes(input.Duration);
            
            return entity;
        }

        public override DinnerInput MapToInput(Dinner entity)
        {
            var input = base.MapToInput(entity);

            input.Minute = entity.Start.Minute;
            input.Hour = entity.Start.Hour;
            input.Duration = (int)entity.End.Subtract(entity.Start).TotalMinutes;

            return input;
        }
    }
}