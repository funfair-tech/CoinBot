using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Credfeto.Enumeration.Source.Generation.Attributes;
using NonBlocking;

namespace CoinBot.Clients.Extensions;

[EnumText(typeof(HttpStatusCode))]
[SuppressMessage(category: "ReSharper", checkId: "PartialTypeWithSinglePart", Justification = "Needed for generated code")]
public static partial class EnumExtensions
{
    private static readonly ConcurrentDictionary<Enum, string> CachedNames = new();

    public static string GetName<T>(this T member)
        where T : Enum
    {
        if (CachedNames.TryGetValue(key: member, out string? name))
        {
            return name;
        }

        return CachedNames.GetOrAdd(key: member, ExtractMemberName(member));
    }

    [SuppressMessage(category: "ToStringWithoutOverrideAnalyzer",
                     checkId: "ExplicitToStringWithoutOverrideAnalyzer: Calling ToString() on object of type 'T' but it does not override ToString()",
                     Justification = "OK as T is an enum")]
    private static string ExtractMemberName<T>(T member)
        where T : Enum
    {
        return Enum.GetName(typeof(T), value: member) ?? member.ToString();
    }
}