using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Linq;

namespace SpartaGame
{
    public struct MultipleItemStruct //여러 개 가질 수 있는 아이템
    {
        public MultipleItem item;
        public int amount;
    };

    

    internal class Program
    {
        private static Character player;
        private static SingleItem[] shop_single;
        private static MultipleItem[] shop_multiple; //이 세 개는 초기상태 그대로

        private static int AdditionalAtkPotion = 0;
        private static int AdditionalDefPotion = 0;

        static void Main(string[] args)
        {
            DataSetting();
            MainScreen();
        }

        static void DataSetting()
        {
            player = new Character("옥수수주먹밥", "전사", 1, 10, 5, 100, 1500);

            shop_single = new SingleItem[8] { new Lv1_Sword(), new Lv1_Spear(), new Lv10_Sword(), new Lv50_GreatSword(), new Lv1_Shield(), new Lv5_Armor(), new Lv20_Armor(), new Lv50_Armor() };
            shop_multiple = new MultipleItem[3] { new Lv1_HealthPotion(), new Lv1_AttackPotion(), new Lv10_HealthPotion() };
        }


        static void MainScreen()
        {
            Console.Clear();

            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 입장");
            //Console.WriteLine("5. 게임 종료");
            //Console.WriteLine("6. 불러오기\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n");
            Console.Write(">> ");

            switch (CheckValidInput(1, 4))
            {
                case 1:
                    StatusScreen(); //상태창
                    break;
                case 2:
                    InventoryScreen(); //아이템창
                    break;
                case 3:
                    ShopScreen(); //상점창
                    break;
                case 4:
                    DungeonScreen(); //던전창
                    break;
                //case 5:
                //    Save(); //게임종료하기전에 저장
                //    return;
                //case 6:
                //    LoadData(); //불러오기
                //    MainScreen();
                //    break;
            }
        }

        static void StatusScreen()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상태보기");
            Console.ResetColor();
            Console.WriteLine("캐릭터의 정보가 표시됩니다\n");

            Console.WriteLine("Lv. " + player.Level.ToString("D2"));
            Console.WriteLine(player.Name + " ( " + player.Job + " )");
            Console.Write("공격력 : " + player.Atk);
            if (player.AdditionalAtkEquip != 0)
            {
                Console.Write(" (+{0})", player.AdditionalAtkEquip);
            }
            Console.Write("\n방어력 : " + player.Def);
            if (player.AdditionalDefEquip != 0)
            {
                Console.Write(" (+{0})", player.AdditionalDefEquip);
            }
            Console.WriteLine("\n체  력 : " + player.Hp);
            Console.WriteLine(" Gold  : " + String.Format("{0:#,###}", player.Gold) + " G\n");

            Console.WriteLine("0. 나가기\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n");
            Console.Write(">> ");

            CheckValidInput(0, 0); //여기서 나오면 무조건 0 입력된 것
            MainScreen();
        }

        static void InventoryScreen()
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("인벤토리");
            Console.ResetColor();
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");
            Console.WriteLine("[아이템 목록]");

            foreach (SingleItem item in player.Inventory_single) //하나만 가질 수 있는 아이템
            {
                Console.WriteLine("- {0} | {1} +{2} | {3}", item.ItemName.PadRight(20 - item.ItemName.Length), ClassifyType(item), item.Stats.ToString().PadRight(3), item.ItemExplain);
            }
            foreach (MultipleItemStruct std in player.Inventory_multiple) //여러 개 가질 수 있는 아이템
            {
                if (std.item.InsertCondition == 1)
                {
                    Console.Write("- [I]");
                }
                else 
                {
                    Console.Write("- ");
                }
                Console.WriteLine("{0} | {1} +{2} | {3}개 | {4}", std.item.ItemName.PadRight(20 - std.item.ItemName.Length), ClassifyType_(std.item), std.item.Stats.ToString().PadRight(3), std.amount, std.item.ItemExplain);
            }
            Console.WriteLine("\n1. 장착 관리");
            Console.WriteLine("2. 슬롯 관리");
            Console.WriteLine("0. 나가기\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n");
            Console.Write(">> ");

            switch (CheckValidInput(0, 2))
            {
                case 0:
                    MainScreen();
                    break;
                case 1:
                    InventoryManageScreen();
                    break;
                case 2:
                    SlotManageScreen();
                    break;
            }
        }

        static void InventoryManageScreen() //장착 관리(고유아이템만 적용)
        {
            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("인벤토리 - 장착 관리");
                Console.ResetColor();
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");
                Console.WriteLine("[아이템 목록]");

                int i = 1;
                
                foreach (SingleItem item in player.Inventory_single)
                {
                    Console.WriteLine("- {0} {1} | {2} +{3} | {4}", i, item.ItemName.PadRight(20 - item.ItemName.Length), ClassifyType(item), item.Stats.ToString().PadRight(3), item.ItemExplain);
                    i++;
                }
                Console.WriteLine("\n0. 나가기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.\n");
                Console.Write(">> ");

                int flag = CheckValidInput(0, player.Inventory_single.Count());
                if (flag == 0)
                {
                    break;
                }
                else
                {
                    if (player.Inventory_single[flag - 1].EquipCondition == 0)
                    {
                        player.Equip(player.Inventory_single[flag - 1]);
                    }
                    else
                    {
                        player.CancelEquip(player.Inventory_single[flag - 1]);
                    }
                }
            }
            InventoryScreen();

        }

        static void SlotManageScreen() //슬롯 관리창(비고유아이템만 적용)
        {
            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("인벤토리 - 슬롯 관리");
                Console.ResetColor();
                Console.WriteLine("보유 중인 포션을 관리할 수 있습니다.\n");
                Console.WriteLine("[아이템 목록]");

                int i = 1;

                foreach (MultipleItemStruct std in player.Inventory_multiple)
                {
                    if (std.item.InsertCondition == 1)
                    {
                        Console.Write("- {0} [I]", i);
                    }
                    else
                    {
                        Console.Write("- {0}", i);
                    }
                    Console.WriteLine(" {0} | {1} +{2} | {3}개 | {4}", std.item.ItemName.PadRight(20 - std.item.ItemName.Length), ClassifyType_(std.item), std.item.Stats.ToString().PadRight(3), std.amount, std.item.ItemExplain);
                    i++;
                }

                Console.WriteLine("\n슬롯");
                foreach (MultipleItemStruct std in player.Slot)
                {
                    Console.WriteLine("- {0} | {1} +{2} | {3}개 | {4}", std.item.ItemName.PadRight(20 - std.item.ItemName.Length), ClassifyType_(std.item), std.item.Stats.ToString().PadRight(3), std.amount, std.item.ItemExplain);
                 }
                Console.WriteLine("\n0. 나가기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.\n");
                Console.Write(">> ");

                int flag = CheckValidInput(0, player.Inventory_multiple.Count());
                if (flag == 0)
                {
                    break;
                }
                else
                {
                    if (player.Inventory_multiple[flag - 1].item.InsertCondition == 0)
                    {
                        if (player.Slot.Count() < 2)
                        {
                            player.Insert(player.Inventory_multiple[flag - 1]);
                        }
                        else
                        {
                            Console.WriteLine("슬롯엔 2 종류까지만 장착 가능합니다.");
                            Thread.Sleep(1000);
                        }
                    }
                    else
                    {
                        player.CancelInsert(player.Inventory_multiple[flag - 1]);
                    }
                }
            }
            InventoryScreen();
        }

        static void ShopScreen() //상점창. 모든 아이템 보여줌
        {

            Console.Clear();

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("상점");
            Console.ResetColor();
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
            Console.WriteLine("[보유 골드]\n{0} G", String.Format("{0:#,###}", player.Gold));

            foreach (SingleItem item in shop_single)
            {
                Console.Write("- {0} | {1} +{2} | {3} | ", item.ItemName.PadRight(20 - item.ItemName.Replace(" ", "").Length), ClassifyType(item), item.Stats.ToString().PadRight(6), item.ItemExplain.PadRight(45 - item.ItemExplain.Replace(" ", "").Length));
                if (player.Inventory_single_name.Contains(item.ItemName))
                {
                    Console.WriteLine("{0}", "보유 중".PadLeft(6));
                }
                else
                {
                    Console.WriteLine("{0} G", item.Price.ToString().PadLeft(6));
                }
            }
            foreach (MultipleItem item in shop_multiple)
            {
                Console.Write("- {0} | {1} +{2} | {3} | ", item.ItemName.PadRight(20 - item.ItemName.Replace(" ", "").Length), ClassifyType_(item), item.Stats.ToString().PadRight(6), item.ItemExplain.PadRight(45 - item.ItemExplain.Replace(" ", "").Length));
                if (player.Inventory_multiple_name.Contains(item.ItemName))
                {
                    Console.Write("{0} G ", item.Price.ToString().PadLeft(6));
                    Console.WriteLine("| {0}개 보유 중", player.Inventory_multiple[player.Inventory_multiple_name.IndexOf(item.ItemName)].amount);
                }
                else
                {
                    Console.WriteLine("{0} G", item.Price.ToString().PadLeft(6));
                }
            }


            Console.WriteLine("\n1. 아이템 구매");
            Console.WriteLine("2. 아이템 판매\n");
            Console.WriteLine("0. 나가기\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n");
            Console.Write(">> ");

            switch (CheckValidInput(0, 2))
            {
                case 0:
                    MainScreen();
                    break;
                case 1:
                    ShopBuyScreen();
                    break;
                case 2:
                    ShopSellScreen();
                    break;
            }

        }

        static void ShopBuyScreen() //상점구입창
        {
            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("상점");
                Console.ResetColor();
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
                Console.WriteLine("[보유 골드]\n{0} G", String.Format("{0:#,###}", player.Gold));

                int i = 1;

                foreach (SingleItem item in shop_single)
                {
                    Console.Write("- {0} {1} | {2} +{3} | {4} | ", i, item.ItemName.PadRight(20 - item.ItemName.Replace(" ", "").Length), ClassifyType(item), item.Stats.ToString().PadRight(6), item.ItemExplain.PadRight(45 - item.ItemExplain.Replace(" ", "").Length));

                    if (player.Inventory_single_name.Contains(item.ItemName))
                    {
                        Console.WriteLine("보유 중");
                    }
                    else
                    {
                        Console.WriteLine("{0} G", item.Price.ToString().PadLeft(6));
                    }
                    i++;
                }
                foreach (MultipleItem item in shop_multiple)
                {
                    Console.Write("- {0} {1} | {2} +{3} | {4} | ", i, item.ItemName.PadRight(20 - item.ItemName.Replace(" ", "").Length), ClassifyType_(item), item.Stats.ToString().PadRight(6), item.ItemExplain.PadRight(45 - item.ItemExplain.Replace(" ", "").Length));
                    if (player.Inventory_multiple_name.Contains(item.ItemName))
                    {
                        Console.Write("{0} G ", item.Price.ToString().PadLeft(6));
                        Console.WriteLine("| {0}개 보유 중", player.Inventory_multiple[player.Inventory_multiple_name.IndexOf(item.ItemName)].amount);
                    }
                    else
                    {
                        Console.WriteLine("{0} G", item.Price.ToString().PadLeft(6));
                    }
                    i++;
                }
                Console.WriteLine("\n0. 나가기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.\n");
                Console.Write(">> ");

                int flag = CheckValidInput(0, shop_single.Count() + shop_multiple.Count());
                if (flag == 0)
                {
                    break;
                }
                else
                {
                    if (flag <= shop_single.Count())
                    {
                        if (player.Inventory_single_name.Contains(shop_single[flag - 1].ItemName))
                        {
                            Console.WriteLine("이미 보유하고 있어 구입할 수 없는 아이템입니다.");
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            if (player.Gold > shop_single[flag - 1].Price)
                            {
                                player.BuyingSingleItem(shop_single[flag - 1]);
                                Console.WriteLine("구매를 완료했습니다.");
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                Console.WriteLine("Gold가 부족합니다.");
                                Thread.Sleep(1000);
                            }

                        }
                    }
                    else
                    {
                        flag -= (1 + shop_single.Count());
                        player.BuyingMultipleItem(shop_multiple[flag]);
                    }
                }
            }
            ShopScreen();
        }

        static void ShopSellScreen() //상점 판매창
        {
            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("상점");
                Console.ResetColor();
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");
                Console.WriteLine("[보유 골드]\n{0} G", player.Gold);

                int i = 1;

                foreach (SingleItem item in player.Inventory_single)
                {
                    Console.WriteLine("- {0} {1} | {2} +{3} | {4} | {5}G", i, item.ItemName.PadRight(20 - item.ItemName.Replace(" ", "").Length), ClassifyType(item), item.Stats.ToString().PadRight(6), item.ItemExplain.PadRight(45 - item.ItemExplain.Replace(" ", "").Length), item.Price * 85 / 100);

                    i++;
                }
                Console.WriteLine("\n0. 나가기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.\n");
                Console.Write(">> ");

                int flag = CheckValidInput(0, player.Inventory_single.Count());
                if (flag == 0)
                {
                    break;
                }
                else
                {
                    player.SellingSingleItem(player.Inventory_single[flag - 1]);
                    Console.WriteLine("판매를 완료했습니다.");
                    Thread.Sleep(1000);
                }
            }
            ShopScreen();
        }

        static void DungeonScreen()
        {
            int i;

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("던전 입장");
                Console.ResetColor();
                Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");

                Console.WriteLine("슬롯");
                foreach (var itemstd in player.Slot)
                {
                    Console.Write(itemstd.item.ItemName + " ");
                    Console.WriteLine(itemstd.amount + "개");
                }
                Console.WriteLine();

                Console.WriteLine("1. 쉬운 던전");
                Console.WriteLine("2. 일반 던전");
                Console.WriteLine("3. 어려운 던전\n");
                Console.WriteLine("4. 슬롯 아이템 사용\n");
                Console.WriteLine("0. 나가기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.\n");
                Console.Write(">> ");

                List<MultipleItem> rewards;

                Stage stage;

                i = CheckValidInput(0, 4);

                if (i == 0)
                {
                    break;
                }
                else if (i == 4)
                {
                    break;
                }
                if(player.Hp > 0) 
                {
                    switch (i)
                    {
                        case 1:
                            rewards = new List<MultipleItem>() { new Lv1_HealthPotion() };
                            stage = new Stage(player, rewards, 1);
                            stage.Start();
                            break;
                        case 2:
                            rewards = new List<MultipleItem>() { new Lv1_HealthPotion(), new Lv1_AttackPotion() };
                            stage = new Stage(player, rewards, 2);
                            stage.Start();
                            break;
                        case 3:
                            rewards = new List<MultipleItem>() { new Lv1_HealthPotion(), new Lv10_HealthPotion() };
                            stage = new Stage(player, rewards, 3);
                            stage.Start();
                            break;
                    }

                    player.Atk -= AdditionalAtkPotion;
                    player.Def -= AdditionalDefPotion;
                    AdditionalAtkPotion = 0;
                    AdditionalDefPotion = 0;
                }
                else
                {
                    Console.WriteLine("체력이 없습니다.\n");
                    Thread.Sleep(1000);
                }
                
            }
            if (i == 0) MainScreen();
            else PotionUsingScreen();
        }

        static void PotionUsingScreen() 
        {
            Console.Clear();

            while (true)
            {
                Console.Clear();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("슬롯 아이템 사용");
                Console.ResetColor();
                Console.WriteLine("슬롯의 포션을 사용할 수 있습니다.\n");
                Console.WriteLine("[슬롯 목록]");

                int i = 1;

                foreach (MultipleItemStruct std in player.Slot)
                {
                    Console.WriteLine("- {0} {1} | {2} +{3} | {4}개 | {5}", i, std.item.ItemName.PadRight(20 - std.item.ItemName.Length), ClassifyType_(std.item), std.item.Stats.ToString().PadRight(3), std.amount, std.item.ItemExplain);
                    i++;
                }

                Console.WriteLine("\n0. 나가기\n");
                Console.WriteLine("원하시는 행동을 입력해주세요.\n");
                Console.Write(">> ");

                int flag = CheckValidInput(0, player.Slot.Count());
                if (flag == 0)
                {
                    break;
                }
                else
                {
                    MultipleItem item = player.Slot[flag - 1].item;
                    player.UsingMultipleItem(item);
                    if (item.TypeNumber == (int)MultipleItem.Typename.Attack)
                    {
                        AdditionalAtkPotion += item.Stats;
                        player.Atk += item.Stats;
                    }
                    else if(item.TypeNumber == (int)MultipleItem.Typename.Defense)
                    {
                        AdditionalDefPotion += item.Stats;
                        player.Def += item.Stats;
                    }
                    else
                    {
                        player.Hp += item.Stats;
                        if (player.Hp > 100) player.Hp = 100;
                    }
                }
            }
            DungeonScreen();
        }

        static string ClassifyType(SingleItem item)
        {
            switch (item.TypeNumber)
            {
                case 0:
                    return "공격력";
                case 1:
                    return "방어력";
                default:
                    return "";

            }
        }
        static string ClassifyType_(MultipleItem item)
        {
            switch (item.TypeNumber)
            {
                case 0:
                    return "공격력";
                case 1:
                    return "방어력";
                case 2:
                    return "체  력";
                default:
                    return "";

            }
        }


        static int CheckValidInput(int min, int max)
        {
            while (true)
            {
                string input = Console.ReadLine();

                bool parseSuccess = int.TryParse(input, out var ret);
                if (parseSuccess)
                {
                    if (ret >= min && ret <= max)
                        return ret;
                }

                Console.WriteLine("잘못된 입력입니다.");
                Thread.Sleep(1000);
                Console.Write(">> ");
            }
        }
    }

    public class Stage
    {
        private Character character; //캐릭터
        private Monster monster; // 몬스터
        private Monster monster2;
        private List<MultipleItem> rewards; // 보상 아이템들
        private int level;
        private int defense;
        private int rewardGold;
        private int pastHp;
        private int pastGold;

        public Stage(Character character, List<MultipleItem> rewards, int level) //스테이지 당 몬스터 한마리씩 나옴
        {
            this.character = character;
            this.rewards = rewards;
            this.level = level;
            this.pastHp = character.Hp;
            this.pastGold = character.Gold;
        }

        public void Start()
        {
            Console.Clear();
            Random random = new Random();
            int damage;

            Console.WriteLine($"스테이지 시작! 플레이어 정보: 체력({character.Hp}), 공격력({character.Atk})");
            switch (level)
            {
                case 1: //권장 방어력 5
                    defense = 5;
                    rewardGold = 1000 + random.Next(character.Atk * 11 / 10, character.Atk * 12 / 10);
                    monster = new Goblin("고블린");
                    monster.AttackMin = 20 - (character.Def - defense);
                    monster.AttackMax = 35 - (character.Def - defense);
                    break;
                case 2: //권장 방어력 11
                    defense = 11;
                    rewardGold = 1700 + random.Next(character.Atk * 11 / 10, character.Atk * 12 / 10);
                    monster = new Dragon("드래곤");
                    monster.AttackMin = 20 - (character.Def - defense);
                    monster.AttackMax = 35 - (character.Def - defense);
                    break;
                case 3: //권장 방어력 17
                    defense = 17;
                    rewardGold = 2500 + random.Next(character.Atk * 11 / 10, character.Atk * 12 / 10);
                    monster = new Goblin("고블린");
                    monster2 = new Dragon("드래곤");
                    int min = 20 - (character.Def - defense);
                    int max = 35 - (character.Def - defense);
                    monster.AttackMin = min * 3 / 10;
                    monster2.AttackMin = min * 7 / 10;
                    monster.AttackMax = max * 3 / 10;
                    monster2.AttackMax = max * 7 / 10;
                    break;
            }

            Console.WriteLine($"몬스터 정보: 이름({monster.Name}), 체력({monster.Health})");
            if (monster2 != null) Console.WriteLine($"몬스터 정보: 이름({monster2.Name}), 체력({monster2.Health})");
            Console.WriteLine("----------------------------------------------------");

            while (!character.IsDead) // 플레이어가 죽을 때까지
            {
                // 플레이어의 턴
                Console.WriteLine($"{character.Name}의 턴!");
                damage = character.Atk;
                if (monster2 != null)
                {
                    if ((monster.IsDead == false) && (monster2.IsDead == false))
                    {
                        switch (random.Next(0, 2))
                        {
                            case 0:
                                monster.TakeDamage(damage);
                                Thread.Sleep(1000);
                                break;
                            case 1:
                                monster.TakeDamage(damage);
                                Thread.Sleep(1000);
                                break;
                        }
                    }
                    else if (monster.IsDead == false)
                    {
                        monster.TakeDamage(damage);
                        Thread.Sleep(1000);
                        if (monster.IsDead) break;
                    }
                    else
                    {
                        monster2.TakeDamage(damage);
                        Thread.Sleep(1000);
                        if (monster2.IsDead) break;
                    }
                }
                else
                {
                    monster.TakeDamage(damage);
                    Thread.Sleep(1000);
                    if (monster.IsDead) break;

                }
                Console.WriteLine();

                // 몬스터의 턴
                if (monster2 != null)
                {
                    if ((monster.IsDead == false) && (monster2.IsDead == false))
                    {
                        Console.WriteLine($"{monster.Name}의 턴!");
                        damage = random.Next(monster.AttackMin, monster.AttackMax);
                        character.TakeDamage(damage);
                        Thread.Sleep(1000);
                        Console.WriteLine($"{monster2.Name}의 턴!");
                        damage = random.Next(monster2.AttackMin, monster2.AttackMax);
                        character.TakeDamage(damage);
                        Thread.Sleep(1000);
                    }
                    else if (monster.IsDead == false)
                    {
                        Console.WriteLine($"{monster.Name}의 턴!");
                        damage = random.Next(monster.AttackMin, monster.AttackMax);
                        character.TakeDamage(damage);
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        Console.WriteLine($"{monster2.Name}의 턴!");
                        damage = random.Next(monster2.AttackMin, monster2.AttackMax);
                        character.TakeDamage(damage);
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    Console.WriteLine($"{monster.Name}의 턴!");
                    damage = random.Next(monster.AttackMin, monster.AttackMax);
                    character.TakeDamage(damage);
                    Thread.Sleep(1000);
                }
                Console.WriteLine();
            }

            StageClear();
        }

        public void StageClear()
        {
            Console.Clear();
            if (character.IsDead == false)
            {
                if (monster2 != null)
                {
                    Console.WriteLine($"스테이지 클리어! {monster.Name}와 {monster2.Name}를 물리쳤습니다!\n");
                }
                else
                {
                    Console.WriteLine($"스테이지 클리어! {monster.Name}를 물리쳤습니다!\n");
                }
                Thread.Sleep(1000);

                character.Gold += rewardGold;

                Console.WriteLine("[탐험 결과]");
                Console.WriteLine($"체력 {pastHp} -> {character.Hp}");
                Console.WriteLine($"Gold {pastGold} G -> {character.Gold} G");
                if (rewards != null)
                {
                    Console.Write("보상 ");
                    foreach (MultipleItem item in rewards)
                    {
                        Console.Write(item.ItemName + " ");
                        character.HavingMultipleItem(item);
                    }
                    Console.WriteLine();
                }
                Thread.Sleep(2000);
            }
            else
            {
                character.Hp = 0;
            }
        }
    }

    public class Monster
    {
        public string Name { get; }
        public int Health { get; set; }
        public int AttackMin { get; set; }
        public int AttackMax { get; set; }

        public bool IsDead => Health <= 0;

        public Monster(string name, int health)
        {
            Name = name;
            Health = health;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (IsDead) Console.WriteLine($"{Name}이(가) 죽었습니다.");
            else Console.WriteLine($"{Name}이(가) {damage}의 데미지를 받았습니다. 남은 체력: {Health}");
        }
    }
    public class Goblin : Monster
    {
        public Goblin(string name) : base(name, 50) { }
    }
    public class Dragon : Monster
    {
        public Dragon(string name) : base(name, 1000) { }
    }

    public class Character
    {
        public string Name { get; set; }
        public string Job { get; }
        public int Level { get; set; }
        public int Atk { get; set; }
        public int AdditionalAtkEquip { get; set; }
        public int Def { get; set; }
        public int AdditionalDefEquip { get; set; }
        public int Hp { get; set; }
        public bool IsDead => Hp <= 0;
        public int Gold { get; set; }

        public List<SingleItem> Inventory_single;
        public List<String> Inventory_single_name; //shop에서 이미 같은 아이템이 있는지 판단하기 위해 사용
        public List<MultipleItemStruct> Inventory_multiple;
        public List<String> Inventory_multiple_name;
        public List<MultipleItemStruct> Slot;
        public List<String> Slot_name;

        public int Weapon_Able { get; set; } //0이면 장착하고 있는게 없는 것
        public int Armor_Able { get; set; }

        public Character(string name, string job, int level, int atk, int def, int hp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            AdditionalAtkEquip = 0;
            Def = def;
            Hp = hp;
            Gold = gold;

            Inventory_single = new List<SingleItem> { new Lv1_Sword(), new Lv1_Shield() };
            Inventory_single_name = new List<string> { "초보자용 단검", "초보자용 방패" };

            MultipleItemStruct flawedHealthPotion;
            flawedHealthPotion.item = new Lv1_HealthPotion();
            flawedHealthPotion.amount = 10;
            MultipleItemStruct flawedAttackPotion;
            flawedAttackPotion.item = new Lv1_AttackPotion();
            flawedAttackPotion.amount = 10;
            Inventory_multiple = new List<MultipleItemStruct> { flawedHealthPotion, flawedAttackPotion };
            Inventory_multiple_name = new List<string> { "하급 체력포션", "하급 공격력포션" };
            Slot = new List<MultipleItemStruct> { };
            Slot_name = new List<String> { };

            Weapon_Able = 0;
            Armor_Able = 0;
        }

        public void HavingSingleItem(SingleItem item)
        {
            Inventory_single.Add(item);
            Inventory_single_name.Add(item.ItemName);
        }
        public void BuyingSingleItem(SingleItem item)
        {
            HavingSingleItem(item);
            Gold -= item.Price;
        }
        public void SellingSingleItem(SingleItem item)
        {
            if (item.EquipCondition == 1) CancelEquip(item);

            Inventory_single_name.Remove(item.ItemName);
            Inventory_single.Remove(item);
            Gold += item.Price * 85 / 100;
        }

        public void Equip(SingleItem item)
        {
            switch (item.TypeNumber)
            {
                case 0:
                    if (Weapon_Able == 0)
                    {
                        Atk += item.Stats;
                        AdditionalAtkEquip += item.Stats;
                        Weapon_Able = 1;
                        item.ItemName = "[E] " + item.ItemName;
                        item.EquipCondition = 1;
                    }
                    else
                    {
                        Console.WriteLine("장착하고 있는 무기를 해제해주세요");
                        Thread.Sleep(1000);
                    }
                    break;
                case 1:
                    if (Armor_Able == 0)
                    {
                        Def += item.Stats;
                        AdditionalDefEquip += item.Stats;
                        Armor_Able = 1;
                        item.ItemName = "[E] " + item.ItemName;
                        item.EquipCondition = 1;
                    }
                    else
                    {
                        Console.WriteLine("장착하고 있는 방어구를 해제해주세요");
                        Thread.Sleep(1000);
                    }
                    break;
            }
        }
        public void CancelEquip(SingleItem item)
        {
            item.ItemName = item.ItemName.Replace("[E] ", "");
            switch (item.TypeNumber)
            {
                case 0:
                    Atk -= item.Stats;
                    AdditionalAtkEquip -= item.Stats;
                    Weapon_Able = 0;
                    item.EquipCondition = 0;
                    break;
                case 1:
                    Def -= item.Stats;
                    AdditionalDefEquip -= item.Stats;
                    Armor_Able = 0;
                    item.EquipCondition = 0;
                    break;
            }
        }
        public void Insert(MultipleItemStruct std)
        {
            std.item.ItemName = std.item.ItemName;
            std.item.InsertCondition = 1;
            Slot.Add(std);
            Slot_name.Add(std.item.ItemName);
        }
        public void CancelInsert(MultipleItemStruct std)
        {
            Slot.Remove(std);
            Slot_name.Remove(std.item.ItemName);
            std.item.InsertCondition = 0;
        }
        public void HavingMultipleItem(MultipleItem item)
        {
            if (Slot_name.Contains(item.ItemName)) //인벤토리에도 있고 슬롯에도 있는 경우
            {
                MultipleItemStruct temp;
                temp.amount = Inventory_multiple[Inventory_multiple_name.IndexOf(item.ItemName)].amount + 1;
                temp.item = Inventory_multiple[Inventory_multiple_name.IndexOf(item.ItemName)].item;

                CancelInsert(Slot[Slot_name.IndexOf(item.ItemName)]); 
                Insert(temp); 

                Inventory_multiple.RemoveAt(Inventory_multiple_name.IndexOf(item.ItemName));
                Inventory_multiple_name.Remove(item.ItemName);
                Inventory_multiple.Add(temp); 
                Inventory_multiple_name.Add(item.ItemName);
            }
            else if (Inventory_multiple_name.Contains(item.ItemName)) //인벤토리에는 있지만 슬롯에는 없는 경우
            {
                MultipleItemStruct temp;
                temp.amount = Inventory_multiple[Inventory_multiple_name.IndexOf(item.ItemName)].amount + 1;
                temp.item = item;

                Inventory_multiple.RemoveAt(Inventory_multiple_name.IndexOf(item.ItemName));
                Inventory_multiple_name.Remove(item.ItemName);
                Inventory_multiple.Add(temp);
                Inventory_multiple_name.Add(item.ItemName);
            }
            else //인벤토리에도 없는 경우
            {
                MultipleItemStruct temp;
                temp.amount = 1;
                temp.item = item;

                Inventory_multiple.Add(temp);
                Inventory_multiple_name.Add(item.ItemName);
            }

        }
        public void BuyingMultipleItem(MultipleItem item)
        {
            HavingMultipleItem(item);
            Gold -= item.Price;
        }
        public void UsingMultipleItem(MultipleItem item)
        {
            MultipleItemStruct temp;
            temp.amount = Inventory_multiple[Inventory_multiple_name.IndexOf(item.ItemName)].amount - 1;
            temp.item = Inventory_multiple[Inventory_multiple_name.IndexOf(item.ItemName)].item;

            CancelInsert(Slot[Slot_name.IndexOf(item.ItemName)]);

            Inventory_multiple.RemoveAt(Inventory_multiple_name.IndexOf(item.ItemName));
            Inventory_multiple_name.Remove(item.ItemName);

            if (temp.amount > 0) 
            {
                Insert(temp);
                Inventory_multiple.Add(temp);
                Inventory_multiple_name.Add(item.ItemName);
            } 
        }
        public void TakeDamage(int damage)
        {
            Hp -= damage;
            if (IsDead) Console.WriteLine($"{Name}이(가) 죽었습니다.");
            else Console.WriteLine($"{Name}이(가) {damage}의 데미지를 받았습니다. 남은 체력: {Hp}");
        }
    }

    public interface SingleItem
    {
        String ItemName { get; set; }
        String ItemExplain { get; set; }

        public enum Typename
        {
            Weapon,
            Armor
        }
        public int TypeNumber { get; }
        public int Price { get; }
        public int Stats { get; set; }
        public int EquipCondition { get; set; } //장착 안하면 0, 하면 1
    }

    public class Lv1_Sword : SingleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Weapon,
            Armor
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int EquipCondition { get; set; }

        public Lv1_Sword()
        {
            ItemName = "초보자용 단검";
            ItemExplain = "초보자들이 많이 사용하는 단검입니다.";
            TypeNumber = (int)Typename.Weapon;
            Price = 50;
            Stats = 2;
            EquipCondition = 0;
        }
    }

    public class Lv1_Spear : SingleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Weapon,
            Armor
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int EquipCondition { get; set; }

        public Lv1_Spear()
        {
            ItemName = "초보자용 단창";
            ItemExplain = "비주류들이 많이 사용하는 단창입니다.";
            TypeNumber = (int)Typename.Weapon;
            Price = 50;
            Stats = 2;
            EquipCondition = 0;
        }
    }

    public class Lv10_Sword : SingleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Weapon,
            Armor
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int EquipCondition { get; set; }

        public Lv10_Sword()
        {
            ItemName = "숙련자용 장검";
            ItemExplain = "가장 많은 사람이 사용하는 장검입니다.";
            TypeNumber = (int)Typename.Weapon;
            Price = 300;
            Stats = 10;
            EquipCondition = 0;
        }
    }
    public class Lv50_GreatSword : SingleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Weapon,
            Armor
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int EquipCondition { get; set; }

        public Lv50_GreatSword()
        {
            ItemName = "대검 ";
            ItemExplain = "왠만한 힘으로는 들 수 없는 대검입니다.";
            TypeNumber = (int)Typename.Weapon;
            Price = 1000;
            Stats = 100;
            EquipCondition = 0;
        }
    }

    public class Lv1_Shield : SingleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Weapon,
            Armor
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int EquipCondition { get; set; }

        public Lv1_Shield()
        {
            ItemName = "초보자용 방패";
            ItemExplain = "초보자들이 애용하는 방패입니다.";
            TypeNumber = (int)Typename.Armor;
            Price = 50;
            Stats = 5;
            EquipCondition = 0;
        }
    }
    public class Lv5_Armor : SingleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Weapon,
            Armor
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int EquipCondition { get; set; }

        public Lv5_Armor()
        {
            ItemName = "초보자용 가죽갑옷";
            ItemExplain = "초보자들의 목숨을 지켜주는 갑옷입니다.";
            TypeNumber = (int)Typename.Armor;
            Price = 100;
            Stats = 10;
            EquipCondition = 0;
        }
    }
    public class Lv20_Armor : SingleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Weapon,
            Armor
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int EquipCondition { get; set; }

        public Lv20_Armor()
        {
            ItemName = "판금 갑옷";
            ItemExplain = "든든한 갑옷입니다.";
            TypeNumber = (int)Typename.Armor;
            Price = 200;
            Stats = 50;
            EquipCondition = 0;
        }
    }
    public class Lv50_Armor : SingleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Weapon,
            Armor
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int EquipCondition { get; set; }

        public Lv50_Armor()
        {
            ItemName = "스파르타 갑옷";
            ItemExplain = "뚫을 수 없는 갑옷입니다.";
            TypeNumber = (int)Typename.Armor;
            Price = 700;
            Stats = 500;
            EquipCondition = 0;
        }
    }

    public interface MultipleItem
    {
        String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public enum Typename
        {
            Attack,
            Defense,
            Health
        }
        public int TypeNumber { get; }
        public int Price { get; }
        public int Stats { get; set; }
        public int InsertCondition { get; set; }
    }
    public class Lv1_HealthPotion : MultipleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Attack,
            Defense,
            Health
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int InsertCondition { get; set; }

        public Lv1_HealthPotion()
        {
            ItemName = "하급 체력포션";
            ItemExplain = "체력을 약간 회복시킵니다.";
            TypeNumber = (int)Typename.Health;
            Price = 10;
            Stats = 100;
            InsertCondition = 0;
        }
    }
    public class Lv10_HealthPotion : MultipleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Attack,
            Defense,
            Health
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int InsertCondition { get; set; }

        public Lv10_HealthPotion()
        {
            ItemName = "중급 체력포션";
            ItemExplain = "체력을 대량 회복시킵니다.";
            TypeNumber = (int)Typename.Health;
            Price = 100;
            Stats = 500;
            InsertCondition = 0;
        }
    }
    public class Lv1_AttackPotion : MultipleItem
    {
        public String ItemName { get; set; }
        public String ItemExplain { get; set; }
        public int TypeNumber { get; }
        public enum Typename
        {
            Attack,
            Defense,
            Health
        }
        public int Price { get; }
        public int Stats { get; set; }
        public int InsertCondition { get; set; }

        public Lv1_AttackPotion()
        {
            ItemName = "하급 공격력포션";
            ItemExplain = "한 스테이지동안 공격력을 소량 증가시킵니다.";
            TypeNumber = (int)Typename.Attack;
            Price = 100;
            Stats = 10;
            InsertCondition = 0;
        }

    }

}