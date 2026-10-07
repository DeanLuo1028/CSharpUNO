using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUNO
{
    public abstract class Player(string name)
    {
        public string Name { get; } = name;
        protected List<Card> Hand { get; } = [];

        // 純虛擬函式：強迫子類別實作
        public abstract void Deal(int cardsNum, Deck deck); 
        public abstract bool Play(Deck deck); 
        public abstract ColorEnum ConvertColor();

        public void ClearHand() {
            Hand.Clear();
        }

        // 判斷玩家是否已出完手牌
        // 若手牌數為 0，即視為獲勝。
        public bool IsWin() {
        if (Hand.Count == 0) {
            Print($"{Name} 獲勝了！");
            return true;
        }
            return false;
        }

        // 顯示玩家目前手牌，並標示每張牌的編號（方便玩家選擇出牌）。
        public void DisplayHand() {
            Print($"{Name}的手牌:");
            for (int i = 0; i<Hand.Count; ++i) {
                Console.Write($"第{i+1}張: {Hand[i]} ");
            }
            Print("");
        }

        // 執行玩家這一輪的動作：出牌或抽牌。
        // 回傳值表示動作結果，用於後續處理狀態（例如 skip、Reverse、+2、+4、Win）。
        public RoundResult OneRound(Deck deck)
        {
            if (deck.Discard.Count == 0)
            {
                throw new InvalidOperationException("錯誤：棄牌堆為空，遊戲狀態異常！");
            }

            Print($"現在牌堆最上方的牌: {deck.Discard[^1]}");

            // 是否不須抽牌
            bool played = Play(deck); // 動態綁定

            if (!played) return RoundResult.Normal; // 抽牌是普通情況
            if (IsWin()) return RoundResult.Win;

            Card playedCard = deck.Discard[^1];

            if (playedCard.Color == ColorEnum.Special)
            {
                playedCard.Color = ConvertColor(); // 動態綁定
                if (playedCard.Rank == RankEnum.Wild)
                {
                    return RoundResult.Normal;
                }
                else if (playedCard.Rank == RankEnum.WildDrawFour)
                {
                    return RoundResult.DrawFour;
                }
                else
                {
                    throw new InvalidOperationException("錯誤!程式應該不會執行到這裡");
                }
            }
            else if (playedCard.Rank < RankEnum.Skip)
            { // 一般數字牌
                return RoundResult.Normal;
            }
            else
            {
                if (playedCard.Rank == RankEnum.Skip) return RoundResult.Skip;
                if (playedCard.Rank == RankEnum.Reverse) return RoundResult.Reverse;
                if (playedCard.Rank == RankEnum.DrawTwo) return RoundResult.DrawTwo;
                throw new InvalidOperationException("錯誤!程式應該不會執行到這裡");
            }
        }

        // 顯示玩家手牌數。當只剩一張時顯示 "UNO" 提示。
        public void SayCardNum() {
        if (Hand.Count == 1) {
            Print($"{Name} 說: UNO!");
        } else {
            Print($"{Name} 剩 {Hand.Count} 張");
        }
    }

        public static void Print<T>(T value)
        {
            Console.WriteLine(value);
        }
    }

    public enum RoundResult
    {
        Normal,
        Win,
        Skip,
        Reverse,
        DrawTwo,
        DrawFour
    };
}
