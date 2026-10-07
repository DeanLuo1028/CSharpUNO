using CSharpUNO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CSharpUNO
{
    public class RobotPlayer(string name) : Player(name)
    {

        // 機器人收到指定張數的牌時，從牌庫抽牌並放到手牌。
        public override void Deal(int cardsNum, Deck deck) {
            for (int i = 0; i<cardsNum; ++i) {
                Hand.Add(deck.Draw());
            }
        }

        // 這個機器人出牌策略很簡單：
        // 1) 優先出與棄牌堆頂牌顏色或數字相符的普通牌（非 special）。
        // 2) 若沒有符合的普通牌，則出 special 牌 (wild/+4)。
        // 3) 無法出牌時，抽一張牌並結束回合。
        public override bool Play(Deck deck) {
            Card discardLastCard = deck.Discard[^1];
            Card? cardToBePlayed = null;
            // 優先出非特殊牌
            foreach (var card in Hand) {
                if (card.Color != ColorEnum.Special && card.HasComplianceRules(discardLastCard)) {
                    cardToBePlayed = card;
                }
            }
            if (cardToBePlayed is null) // 出特殊牌
            {
                foreach (var card in Hand)
                {
                    if (card.Color == ColorEnum.Special)
                    {
                        cardToBePlayed = card;
                    }
                }
            }
            if (cardToBePlayed is not null) // 有牌可出
            {
                deck.Discard.Add(cardToBePlayed);
                Hand.Remove(cardToBePlayed); // 移除手牌中的該張牌
                Print($"{Name} 出了 {cardToBePlayed}");
                return true;
            }
            

            Print($"{Name} 沒牌可出，抽一張");
            Deal(1, deck);
            return false;
        }

        // 當出現 wild/+4 需要選擇顏色時，此機器人會選擇手牌中數量最多的顏色。
        public override ColorEnum ConvertColor() {
            Dictionary<ColorEnum, int> colorNum = [];
            foreach (ColorEnum color in Enum.GetValues(typeof(ColorEnum))) {
                colorNum[color] = 0;
            }
            foreach (var card in Hand) {
                colorNum[card.Color]++;
            }
            colorNum.Remove(ColorEnum.Special); // 移除特殊牌的計數

            int maxNum = -1;
            ColorEnum maxColor = ColorEnum.R;
            foreach (var pair in colorNum)
            {
                if (pair.Value > maxNum)
                {
                    maxNum = pair.Value;
                    maxColor = pair.Key;
                }
            }
            Print($"{Name} 選擇了 {Card.ColorString[(int)maxColor]}");
            return maxColor;
        }
    }
}
