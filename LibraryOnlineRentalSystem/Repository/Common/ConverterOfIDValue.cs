using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LibraryOnlineRentalSystem.Repository.Common;

public class ConverterOfIDValue<TTypedIdValue> : ValueConverter<TTypedIdValue, string>
    where TTypedIdValue : EntityId
{
    public ConverterOfIDValue(ConverterMappingHints mappingHints = null)
        : base(id => id.Value, value => Create(value), mappingHints)
    {
    }

    private static TTypedIdValue Create(string id)
    {
        return Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue;
    }
}