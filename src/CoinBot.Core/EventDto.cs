using System;

namespace CoinBot.Core
{
	public sealed class EventDto
	{
		/// <summary>
		/// A percentage indicating the reliability / accuracy of the event.
		/// </summary>
		public int Reliability { get; set; }

		/// <summary>
		/// The <see cref="Currency"/> the event is for.
		/// </summary>
		public Currency Currency { get; set; }

		/// <summary>
		/// The <see cref="DateTime"/> of the event.
		/// </summary>
		public DateTime Date { get; set; }

		/// <summary>
		/// The event description.
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// If true, the <see cref="Date"/> is a deadline.
		/// </summary>
		public bool CanOccurBefore { get; set; } = false;

		/// <summary>
		/// The proof link.
		/// </summary>
		public Uri ProofImage { get; set; }

		/// <summary>
		/// The proof source.
		/// </summary>
		public Uri ProofSource { get; set; }

		/// <summary>
		/// The event title.
		/// </summary>
		public string Title { get; set; }
	}
}
