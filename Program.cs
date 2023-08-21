using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading;
using System.Text;
using System.Numerics;

namespace SpartaGame
{
    public struct MultipleItemStruct
    {
        public MultipleItem item;
        public int amount;
    };

    internal class Program
    {
        private static Character player;
        private static SingleItem[] shop_single;
        private static MultipleItem[] shop_multiple;

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
            Console.WriteLine("3. 상점\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n");
            Console.Write(">> ");
                        
            switch (CheckValidInput(1, 3))
            {
                case 1: 
                    StatusScreen();
                    break;
                case 2:
                    InventoryScreen();
                    break;
                case 3:
                    ShopScreen();
                    break;
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
            if (player.AdditionalAtk != 0)
            {
                Console.Write(" (+{0})",player.AdditionalAtk);
            }
            Console.Write("\n방어력 : " + player.Def);
            if (player.AdditionalDef != 0)
            {
                Console.Write(" (+{0})",player.AdditionalDef);
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

            foreach(SingleItem item in player.Inventory_single)
            {
                Console.WriteLine("- {0} | {1} +{2} | {3}", item.ItemName.PadRight(20 - item.ItemName.Length), ClassifyType(item), item.Stats.ToString().PadRight(3), item.ItemExplain);
            }
            foreach (MultipleItemStruct std in player.Inventory_multiple)
            {
                Console.WriteLine("- {0} | {1} +{2} | {3}개 | {4}", std.item.ItemName.PadRight(20 - std.item.ItemName.Length), ClassifyType_(std.item), std.item.Stats.ToString().PadRight(3), std.amount, std.item.ItemExplain);
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

        static void InventoryManageScreen()
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

        static void SlotManageScreen()
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
                    Console.WriteLine("- {0} {1} | {2} +{3} | {4}개 | {5}", i, std.item.ItemName.PadRight(20 - std.item.ItemName.Length), ClassifyType_(std.item), std.item.Stats.ToString().PadRight(3), std.amount, std.item.ItemExplain);
                    i++;
                }

                Console.WriteLine("\n슬롯");
                foreach (MultipleItemStruct std in player.Slot)
                {
                    Console.WriteLine("- {0} | {1} +{2} | {3}개 | {4}", std.item.ItemName.PadRight(20 - std.item.ItemName.Length), ClassifyType_(std.item), std.item.Stats.ToString().PadRight(3), std.amount, std.item.ItemExplain);
                    i++;
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

        static void ShopScreen()
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
                    Console.WriteLine("{0} G",item.Price.ToString().PadLeft(6));
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

        static void ShopBuyScreen()
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
                        Console.WriteLine(item.Price + " G");
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

        static void ShopSellScreen()
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

    public class Character
    {
        public string Name { get; }
        public string Job { get; }
        public int Level { get; set; }
        public int Atk { get; set; }
        public int AdditionalAtk { get; set; }
        public int Def { get; set; }
        public int AdditionalDef { get; set; }
        public int Hp { get; set; }
        public int Gold { get; set; }

        public List<SingleItem> Inventory_single;
        public List<String> Inventory_single_name; //shop에서 이미 같은 아이템이 있는지 판단하기 위해 사용
        public List<MultipleItemStruct> Inventory_multiple;
        public List<String> Inventory_multiple_name;
        public List<MultipleItemStruct> Slot;

        public int Weapon_Able { get; set; } //0이면 장착하고 있는게 없는 것
        public int Armor_Able { get; set; }

        public Character(string name, string job, int level, int atk, int def, int hp, int gold)
        {
            Name = name;
            Job = job;
            Level = level;
            Atk = atk;
            AdditionalAtk = 0;
            Def = def;
            Hp = hp;
            Gold = gold;

            Inventory_single = new List<SingleItem> { new Lv1_Sword() , new Lv1_Shield()};
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
            Gold += item.Price * 85 /100;
        }

        public void Equip(SingleItem item)
        {
            switch (item.TypeNumber)
            {
                case 0:
                    if(Weapon_Able == 0) 
                    {
                        Atk += item.Stats;
                        AdditionalAtk += item.Stats;
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
                        AdditionalDef += item.Stats;
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
            item.ItemName = item.ItemName.Replace("[E] ","");
            switch (item.TypeNumber)
            {
                case 0:
                    Atk -= item.Stats;
                    AdditionalAtk -= item.Stats;
                    Weapon_Able = 0;
                    item.EquipCondition = 0;
                    break;
                case 1:
                    Def -= item.Stats;
                    AdditionalDef -= item.Stats;
                    Armor_Able = 0;
                    item.EquipCondition = 0;
                    break;
            }
        }
        public void Insert(MultipleItemStruct std)
        {
            std.item.ItemName = "[I] " + std.item.ItemName;
            std.item.InsertCondition = 1;
            Slot.Add(std);
        }
        public void CancelInsert(MultipleItemStruct std)
        {
            std.item.ItemName = std.item.ItemName.Replace("[I] ", "");
            std.item.InsertCondition = 0;
            Slot.Remove(std);
        }
        public void HavingMultipleItem(MultipleItem item)
        {
            if (Inventory_multiple_name.Contains(item.ItemName))
            {
                MultipleItemStruct temp;
                temp.amount = Inventory_multiple[Inventory_multiple_name.IndexOf(item.ItemName)].amount + 1;
                temp.item = item;

                Inventory_multiple.RemoveAt(Inventory_multiple_name.IndexOf(item.ItemName));
                Inventory_multiple_name.RemoveAt(Inventory_multiple_name.IndexOf(item.ItemName));

                Inventory_multiple.Add(temp);
                Inventory_multiple_name.Add(item.ItemName);

            }
            else
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