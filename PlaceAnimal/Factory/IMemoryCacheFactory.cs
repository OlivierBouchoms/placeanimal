using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace PlaceAnimal.Factory;

public static class IMemoryCacheFactory
{
    private static IMemoryCache _default;
    public static IMemoryCache Instance => _default ??= new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions()));
}