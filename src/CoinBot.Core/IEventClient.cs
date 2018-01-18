using System.Threading.Tasks;

namespace CoinBot.Core
{
	public interface IEventClient
	{
		Task<EventsDto> GetEvents(Currency currency);
	}
}
