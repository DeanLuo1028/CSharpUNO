using CSharpUNO;
using System;

static void print(string message)
{
    Console.WriteLine(message);
}

print("歡迎來到UNO遊戲!");

int playerNum;
while (true)
{
    print("請輸入玩家人數(2~10): ");
    string inputStr = Console.ReadLine() ?? "";
    string s = inputStr.Trim();

    if (int.TryParse(s, out playerNum)) {
        if (playerNum < 2 || playerNum > 10)
        {
            print("請輸入2~10之間的整數!");
            continue;
        }
    }
    else
    {
        print("請輸入一個有效的整數！");
        continue;
    }
    break;
}

print("請輸入人類玩家的名字: ");
string? input = Console.ReadLine();
if (input is null || input.Length == 0) input = "Player"; // 避免輸入全空白字元
string humanName = input.Trim();

// 開始遊戲！
do { 
    UNO game = new(playerNum, humanName);
    game.GameLoop();
} while (UNO.AskRestart());


internal class UNO
{
    private const int CLOCKWISE = 1;
    private const int COUNTERCLOCKWISE = -1;
    private static readonly string[] ROBOTSNAME = ["Anna", "Bob", "Charlotte", "Danny", "Emily", "Frank", "Grace", "Henry", "Isabella", "Jessica"];

    private readonly Deck deck = new();
    private readonly List<Player> players = [];
    private readonly int playerNum;

    public UNO(int playerNumber, string humanName)
    {
        playerNum = playerNumber;

        players.Add(new HumanPlayer(humanName));
        for (int i = 0; i < playerNum - 1; ++i)
        {
            players.Add(new RobotPlayer(ROBOTSNAME[i]));
        }

        // 設定遊戲初始狀態：清空手牌、發牌、並決定棄牌堆的第一張牌。
        // 若第一張牌是特殊牌（如 +2/Skip/Reverse 或 special），則放回牌庫重抽。
        foreach (var p in players)
        {
            p.ClearHand();
            p.Deal(7, deck);
        }

        deck.SetFirstCard();
    }

    private int NormalizeIndex(int idx)
    {
        return (idx % playerNum + playerNum) % playerNum;
    }
    // 主要遊戲迴圈，依序讓玩家出牌，並根據牌的效果變更順序、跳過玩家、強迫抽牌等。 
    public void GameLoop()
    {
        Print("遊戲開始!");
        int now_index = 0;
        int direction = CLOCKWISE;

        while (true)
        {
            // 玩家行動後回傳狀態
            RoundResult state = players[now_index].OneRound(deck);
            players[now_index].SayCardNum();

            switch (state)
            {
                case RoundResult.Normal:
                    now_index = NormalizeIndex(now_index + direction);
                    break;
                case RoundResult.Skip:
                    // 跳過下一位玩家
                    now_index = NormalizeIndex(now_index + direction);
                    Print($"{players[now_index].Name} 被跳過了");
                    now_index = NormalizeIndex(now_index + direction);
                    break;
                case RoundResult.Reverse:
                    // 反轉出牌方向
                    direction *= -1;
                    Print(direction == COUNTERCLOCKWISE ? "換成逆時針" : "換成順時針");
                    now_index = NormalizeIndex(now_index + direction);
                    break;
                case RoundResult.DrawTwo:
                    // +2 牌會讓下一位玩家抽2張牌，並跳過該玩家
                    now_index = NormalizeIndex(now_index + direction);
                    Print($"{players[now_index].Name} 抽2張牌，且被跳過");
                    players[now_index].Deal(2, deck);
                    now_index = NormalizeIndex(now_index + direction);
                    break;
                case RoundResult.DrawFour:
                    // +4 牌會讓下一位玩家抽4張牌，並跳過該玩家
                    now_index = NormalizeIndex(now_index + direction);
                    Print($"{players[now_index].Name} 抽4張牌，且被跳過");
                    players[now_index].Deal(4, deck);
                    now_index = NormalizeIndex(now_index + direction);
                    break;
                case RoundResult.Win:
                    return;
                default:
                    throw new InvalidOperationException("錯誤!未知狀態");
            }

            Print($"下一個人是 {players[now_index].Name}\n====================");
        }
    }

    // 詢問玩家是否要重新開始遊戲
    public static bool AskRestart() {
        Print("是否要重新開始遊戲？(y/n)");
        while (true) { 
            string input = Console.ReadLine() ?? "";
            string s = input.Trim().ToLower();
            if (s.Equals("y")) // 重新開始遊戲
                return true;
            else if (s.Equals("n")) // 結束遊戲
            { 
                Print("遊戲結束!");
                return false;
            }
            else
                Print("請輸入y或n!");
        }
    }
    public static void Print<T>(T value)
    {
        Console.WriteLine(value);
    }
}
