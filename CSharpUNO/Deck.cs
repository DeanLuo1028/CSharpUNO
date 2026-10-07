using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CSharpUNO
{
    public class Deck
    {
        static readonly Random rng = new();

        public List<Card> Cards = [];
        public List<Card> Discard = [];

        public Deck()
        {
            foreach (ColorEnum color in Enum.GetValues(typeof(ColorEnum)))
            {
                if (color == ColorEnum.Special) // 特殊牌沒有普通數字
                    continue;
                foreach (RankEnum rank in Enum.GetValues(typeof(RankEnum)))
                {
                    if (rank == RankEnum.Wild || rank == RankEnum.WildDrawFour) // 特殊牌不在這裡產生
                        continue;
                    // 普通顏色的牌，0 只有一張，1~9 各有兩張
                    Cards.Add(new Card(color, rank));
                    if ((int)rank != 0)
                        Cards.Add(new Card(color, rank));
                
                    Cards.Add(new Card(color, rank));
                    Cards.Add(new Card(color, rank));
                }
            }
            // 特殊牌: wild 和 +4 各有4張
            for (int i = 0; i < 4; i++)
            {
                Cards.Add(new Card(ColorEnum.Special, RankEnum.Wild));
                Cards.Add(new Card(ColorEnum.Special, RankEnum.WildDrawFour));
            }

            Shuffle();
        }

        // 將牌庫內的牌隨機重新排列。
        // 這在遊戲開始時，以及需要補牌時使用。
        public void Shuffle()
        {
            // Fisher–Yates Shuffle for List<T>
            for (int i = Cards.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
            }
            Print("洗牌完成!");
        }

        // 從牌堆中抽取一張牌。如果牌堆為空，則從棄牌堆補充牌堆。
        public Card Draw()
        {
            if (Cards.Count == 0)
            {
                Print("牌已經沒了！");
                Replenish();
            }
            Card c = Cards[^1]; // 把牌組最後一張拿出來
            Cards.RemoveAt(Cards.Count - 1); // cards 減少一張
            return c;
        }

        private void Replenish() {
            // 用棄牌堆補充牌組
            // 先取出棄牌堆頂端的牌
            Card discardLastCard = Discard[^1];
            Discard.RemoveAt(Discard.Count - 1);
            // 在洗回牌組前，將所有特殊牌顏色重設為 SPECIAL
            foreach (var card in Discard) {
                if (card.IsSpecial())
                    card.Color = ColorEnum.Special;
            }
            // 牌組已經沒了，將棄牌堆的牌拿來當成新牌組
            Cards = Discard;
            // 原本棄牌堆的那個列表被cards拿走了，將棄牌堆指向一個只有原先最後出的牌的新列表，為了讓下一個人能出
            Discard = [ discardLastCard ];
            Shuffle(); // 重新洗牌

        }
        public void SetFirstCard()
        {
            while (true)
            {
                Card card = Draw();

                if (card.IsSpecial() || card.Rank == RankEnum.DrawTwo ||
                    card.Rank == RankEnum.Skip || card.Rank == RankEnum.Reverse)
                { // 不能當底牌的牌
                    Cards.Insert(0, card); // 插回最底部
                }
                else // card 可以當底牌
                {
                    Discard.Add(card);
                    Print($"底牌是 {card}");
                    break;
                }
            }
        }

        static void Print<T>(T value)
        {
            Console.WriteLine(value);
        }
    }
}
