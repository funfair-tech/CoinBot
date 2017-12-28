using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoinBot.Core
{
	public interface ICoinClient
	{
		/// <summary>
		/// Get all coin info.
		/// </summary>
		/// <returns></returns>
		Task<IReadOnlyCollection<ICoinInfo>> GetCoinInfo();

		/// <summary>
		/// Get global info.
		/// </summary>
		/// <returns></returns>
		Task<IGlobalInfo> GetGlobalInfo();
	}
}
