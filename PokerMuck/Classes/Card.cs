using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;

namespace PokerMuck
{
    public enum CardFace { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King };
    public enum CardSuit { Clubs = 1, Diamonds, Hearts, Spades };

	/// <summary>
	/// Card Class
	/// </summary>
	public class Card
	{
        public CardFace Face { get; set; }
        public CardSuit Suit { get; set; }

		public Card(CardFace face, CardSuit suit)
		{
			this.Face = face;
			this.Suit = suit;
		}
	}
}
