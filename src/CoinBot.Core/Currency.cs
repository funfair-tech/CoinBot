using System.Collections.Generic;
using System.Linq;

namespace CoinBot.Core
{
	/// <summary>
	/// Coin information.
	/// </summary>
	public class Currency
	{
		/// <summary>
		/// The coin name.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The token symbol.
		/// </summary>
		public string Symbol { get; set; }

		/// <summary>
		/// The currency image url.
		/// </summary>
		public string ImageUrl { get; set; }

		/// <summary>
		/// Coin information.
		/// </summary>
		private readonly List<ICoinInfo> _details;

		public Currency()
		{
		    this._details = new List<ICoinInfo>();
		}
		public void AddDetails(ICoinInfo details)
		{
		    this._details.Add(details);
		}

		public T Getdetails<T>()
		{
			return this._details.OfType<T>().FirstOrDefault();
		}
	}
}
