using System;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Text.RegularExpressions;

namespace PokerMuck
{
    public enum CardFace { Ace = 1, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King };
    public enum CardSuit { Clubs = 1, Diamonds, Hearts, Spades };

	/// <summary>
	/// Card Class
	/// </summary>
	public class Card : ICloneable
	{
        public CardFace Face { get; set; }
        public CardSuit Suit { get; set; }

		public Card(CardFace face, CardSuit suit)
		{
			this.Face = face;
			this.Suit = suit;
		}

        /* Creates a card starting from a path/filename that uses our filenaming convention
         * suit_face.ext , starting from 1, face = 1 = ace */
        public static Card CreateFromPath(String path)
        {
            Regex fileToCardRegex = new Regex(@"(?<suit>[1-4])_(?<face>[1-9][0-3]?)\.[\w]{3}$");
            Match match = fileToCardRegex.Match(path);
            if (match.Success)
            {
                CardFace face = (CardFace)Int32.Parse(match.Groups["face"].Value);
                CardSuit suit = (CardSuit)Int32.Parse(match.Groups["suit"].Value);

                return new Card(face, suit);
            }
            else
            {
                throw new Exception("Failed to create a card starting from a path/filename: " + path);
            }
        }

        /* Takes as input the standard format (As, Kh, ...) */
        public static Card CreateFromString(String card)
        {
            Trace.Assert(card.Length == 2, "Cannot create a card from a string of length != 2");

            String faceStr= new String(card[0],1).ToUpper();
            String suitStr = new String(card[1], 1).ToUpper();

            CardFace face = Card.CharToCardFace(faceStr[0]);
            CardSuit suit = Card.CharToCardSuit(suitStr[0]);

            return new Card(face, suit);
        }

        /* Shallow copy, no objects to take care of */
        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return CardFaceToChar(Face).ToString() + CardSuitToChar(Suit).ToString().ToLower();
        }

        /* Returns the filename that this card used to display itself
         * using our naming convention */
        public string ToFilename(String extension = "png")
        {
            return String.Format("{0}_{1}.{2}", (int)Suit, (int)Face, extension); 
        }

        public override bool Equals(object obj)
        {
            Card otherCard = (Card)obj;

            return (otherCard.Face == Face && otherCard.Suit == Suit);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        // Returns an int identifying the value of the face alone (Two = lower, Ace = higher)
        public int GetFaceValue(bool countAceAsHigh = true)
        {
            if (Face != CardFace.Ace) return (int)Face;
            else
            {
                if (countAceAsHigh) return 14;
                else return 1;
            }
        }

        /* Helper method to convert a char into a CardFace enum value */
        public static CardFace CharToCardFace(Char c)
        {
            switch (c)
            {
                case 'A': return CardFace.Ace;
                case '2': return CardFace.Two;
                case '3': return CardFace.Three;
                case '4': return CardFace.Four;
                case '5': return CardFace.Five;
                case '6': return CardFace.Six;
                case '7': return CardFace.Seven;
                case '8': return CardFace.Eight;
                case '9': return CardFace.Nine;
                case 'T': return CardFace.Ten;
                case 'J': return CardFace.Jack;
                case 'Q': return CardFace.Queen;
                case 'K': return CardFace.King;
                default:
                    Trace.Assert(false, "Invalid char detected during conversion to CardFace: " + c);
                    return CardFace.Ace; // Never to be executed
            }
        }

        /* Helper method to convert a char into a CardSuit enum value */
        public static CardSuit CharToCardSuit(Char c)
        {
            switch (c)
            {
                case 'S': return CardSuit.Spades;
                case 'C': return CardSuit.Clubs;
                case 'D': return CardSuit.Diamonds;
                case 'H': return CardSuit.Hearts;
                default:
                    Trace.Assert(false, "Invalid char detected during conversion to CardSuit: " + c);
                    return CardSuit.Hearts; // Never to be executed
            }
        }

        /* Helper method to convert a CardFace enum value into a char */
        public static Char CardFaceToChar(CardFace c)
        {
            switch (c)
            {
                case CardFace.Ace: return 'A';
                case CardFace.Two: return '2';
                case CardFace.Three: return '3';
                case CardFace.Four: return '4';
                case CardFace.Five: return '5';
                case CardFace.Six: return '6';
                case CardFace.Seven: return '7';
                case CardFace.Eight: return '8';
                case CardFace.Nine: return '9';
                case CardFace.Ten: return 'T';
                case CardFace.Jack: return 'J';
                case CardFace.Queen: return 'Q';
                case CardFace.King: return 'K';

                default:
                    Trace.Assert(false, "Invalid cardface detected during conversion to char: " + c);
                    return 'A'; // Never to be executed
            }
        }

        /* Helper method to convert CardSuit enum value into a char */
        public static Char CardSuitToChar(CardSuit s)
        {
            switch (s)
            {
                case CardSuit.Spades: return 'S';
                case CardSuit.Clubs: return 'C';
                case CardSuit.Diamonds: return 'D';
                case CardSuit.Hearts: return 'H';
                default:
                    Trace.Assert(false, "Invalid cardsuit detected during conversion to char: " + s);
                    return 'H'; // Never to be executed
            }
        }
	}
}
