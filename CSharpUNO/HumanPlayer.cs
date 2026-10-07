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
    
    public class HumanPlayer(string name) : Player(name)
    {

        // 人類玩家抽牌，並顯示抽到的牌。
        public override void Deal(int cardsNum, Deck deck) 
        {
            for (int i = 0; i<cardsNum; ++i) {
                Card c = deck.Draw();
                Hand.Add(c);
                Print($"{Name} 抽到了 {c}");
            }
        }

        // 讓人類玩家選擇要出的牌，或選擇抽一張牌。
        // 這裡會持續詢問輸入直到拿到合法的選項。
        public override bool Play(Deck deck) 
        {
            Card discardLastCard = deck.Discard[^1];
            Print("以下是你的牌:");
            DisplayHand();

            while (true)
            {
                Print($"{Name} 請問你要出第幾張牌(若不想出牌請輸入0)? ");
                string s = Input() ?? "";
                s = s.Trim();

                if (s.ToLower().Equals("quit"))
                {
                    Print("遊戲結束!");
                    Environment.Exit(0);
                }

                if (!int.TryParse(s, out int user_input))
                {
                    Print("請輸入一個有效的整數！");
                    continue;
                }

                if (user_input < 0 || user_input > Hand.Count)
                {
                    Print($"請輸入0~{Hand.Count}之間的整數!");
                    continue;
                }

                if (user_input == 0)
                {
                    Deal(1, deck);
                    return false;
                }
                else
                {
                    Card card = Hand[user_input - 1]; // 減1是因為List的索引從0開始
                    if (!card.HasComplianceRules(discardLastCard))
                    {
                        Print($"請輸入顏色為{discardLastCard.ColorStr}或special的牌或者點數為{discardLastCard.RankStr}的牌!");
                        continue;
                    }

                    deck.Discard.Add(card);
                    Hand.Remove(card);
                    Print($"{Name} 出了 {deck.Discard[^1]}");
                    return true;
                }
            }
        }

        // 當出特殊牌 (wild / +4) 時，詢問人類玩家選擇新的顏色。
        // 會持續詢問直到輸入有效顏色代號為止。
        public override ColorEnum ConvertColor()
        {
            while (true)
            {
                Print($"{Name} 請選擇顏色(請輸入R/Y/G/B其中之一): ");
                string? input = Input();
                if (input == null) continue;
                input = input.Trim().ToUpper();
                ColorEnum c = input switch
                {
                    "R" => ColorEnum.R,
                    "Y" => ColorEnum.Y,
                    "G" => ColorEnum.G,
                    "B" => ColorEnum.B,
                    _ => ColorEnum.Special // 這裡用 Special 來表示無效輸入
                };
                if (c != ColorEnum.Special)
                {
                    Print($"{Name} 選擇了顏色 {c}");
                    return c;
                }
                else
                {
                    Print("請輸入有效的顏色代號(R/Y/G/B)!");
                }
            }
        }

        static string? Input()
        {
            return Console.ReadLine();
        }
    }
}
