using System;
using Omu.ValueInjecter;

namespace Omu.ProDinner.WebUI.Mappers
{
    public class NormalToNullables : ConventionInjection
    {
        protected override bool Match(ConventionInfo c)
        {
            //ignore int = 0 and DateTime = to 1/01/0001
            if (c.SourceProp.Type == typeof(DateTime) && (DateTime)c.SourceProp.Value == default(DateTime) ||
                (c.SourceProp.Type == typeof(int) && (int)c.SourceProp.Value == default(int)))
                return false;

            return (c.SourceProp.Name == c.TargetProp.Name &&
                    c.SourceProp.Type == Nullable.GetUnderlyingType(c.TargetProp.Type));
        }
    }
}