using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace CoinBot.Core;

/// <summary>
///     Coin information.
/// </summary>
[DebuggerDisplay(value: "{Symbol}: {Name}")]
public sealed class Currency
{
    /// <summary>
    ///     Coin information.
    /// </summary>
    private readonly List<ICoinInfo> _details;

    /// <summary>
    ///     Constructor.
    /// </summary>
    /// <param name="name">Currency name.</param>
    /// <param name="symbol">Currency symbol.</param>
    public Currency(string name, string symbol)
    {
        this.Name = name;
        this.Symbol = symbol;
        this._details = new();
    }

    /// <summary>
    ///     The coin name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     The token symbol.
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    ///     The currency image url.
    /// </summary>
    [SuppressMessage(category: "Microsoft.Design", checkId: "CA1056:UriPropertiesShouldNotBeStrings", Justification = "Model for serialization")]
    public string? ImageUrl { get; set; }

    public bool IsFiat { get; set; }

    public void AddDetails(ICoinInfo details)
    {
        this._details.Add(details);
    }

    public T? Getdetails<T>()
        where T : class, ICoinInfo
    {
        return this._details.OfType<T>()
                   .FirstOrDefault();
    }
}