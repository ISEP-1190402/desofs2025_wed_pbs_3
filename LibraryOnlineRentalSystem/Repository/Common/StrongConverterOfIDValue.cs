using System.Collections.Concurrent;
using LibraryOnlineRentalSystem.Domain.Common;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace LibraryOnlineRentalSystem.Repository.Common;
/// <summary>
///     Based on
///     https://andrewlock.net/strongly-typed-ids-in-ef-core-using-strongly-typed-entity-ids-to-avoid-primitive-obsession-part-4/
/// </summary>
public class StrongConverterOfIDValue : ValueConverterSelector
{
    private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo>
        _converters =
            new();

    public StrongConverterOfIDValue(ValueConverterSelectorDependencies dependencies)
        : base(dependencies)
    {
    }

    public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type providerClrType = null)
    {
        var baseConverters = base.Select(modelClrType, providerClrType);
        foreach (var converter in baseConverters) yield return converter;

        var underlyingModelType = UnwrapNullableType(modelClrType);
        var underlyingProviderType = UnwrapNullableType(providerClrType);

        if (underlyingProviderType is null || underlyingProviderType == typeof(string))
        {
            var isTypedIdValue = typeof(EntityId).IsAssignableFrom(underlyingModelType);
            if (isTypedIdValue)
            {
                var converterType = typeof(ConverterOfIDValue<>).MakeGenericType(underlyingModelType);

                yield return _converters.GetOrAdd((underlyingModelType, typeof(string)), _ =>
                {
                    return new ValueConverterInfo(
                        modelClrType,
                        typeof(string),
                        valueConverterInfo =>
                            (ValueConverter) Activator.CreateInstance(converterType,
                                valueConverterInfo.MappingHints));
                });
            }
        }
    }

    private static Type UnwrapNullableType(Type type)
    {
        if (type is null) return null;

        return Nullable.GetUnderlyingType(type) ?? type;
    }
}