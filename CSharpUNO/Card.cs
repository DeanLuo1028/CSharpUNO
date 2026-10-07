using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUNO
{
    public enum ColorEnum
    {
        R, Y, G, B, Special
    }
    
    public enum RankEnum
    {
        Zero, One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Skip, Reverse, DrawTwo, Wild, WildDrawFour
    }
    public record Card
    {
        public static readonly string[] ColorString = ["R", "Y", "G", "B", "Special"];
        public static readonly string[] RankString = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Skip", "Reverse", "+2", "Wild", "+4"];
        public ColorEnum Color { 
            get { 
                return _color;
            } 
            set {
                if (IsSpecial())
                    _color = value;
                 else
                    throw new InvalidOperationException("Cannot change the color of a non-special card.");
            }
        }
        public RankEnum Rank { get; init; }

        private ColorEnum _color;

        public Card(ColorEnum color, RankEnum rank)
        {
            _color = color;
            Rank = rank;
        }
        public string ColorStr => ColorString[(int)Color];
        public string RankStr => RankString[(int)Rank];
        public override string ToString()
        {
            return $"{ColorStr} {RankStr}";
        }

        public bool HasComplianceRules(Card discardLastCard) {
        return Color == discardLastCard.Color ||
               Rank == discardLastCard.Rank ||
               Color == ColorEnum.Special;
        }

        public bool IsSpecial() {
            // 不能用 Color == ColorEnum.Special 來判斷，因為特殊牌的顏色可能被改，可能不是 ColorEnum.Special
            return Rank == RankEnum.Wild || Rank == RankEnum.WildDrawFour;
        }
    }
}
