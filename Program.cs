using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Threading;

namespace SpartaGame
{
    internal class Program
    {
        private static Character player;

        static void Main(string[] args)
        {
            DataSetting();
            MainScreen();
        }

        static void DataSetting()
        {
            player = new Character("옥수수주먹밥", "전사", 1, 10, 5, 100, 1500);
        }

        static void MainScreen()
        {
            Console.Clear();

            Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
            Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n");
            Console.Write(">> ");
                        
            switch (CheckValidInput(1, 2))
            {
                case 1: 
                    StatusScreen();
                    break;
                case 2:
                    InventoryScreen();
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
            //Console.WriteLine(String.Format("{0,-4} : {1}", "공격력", player.Atk));
            //Console.WriteLine(String.Format("{0,-4} : {1}", "방어력", player.Def));
            //Console.WriteLine(String.Format("{0,-4} : {1}", "체  력", player.Hp));
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
                Console.WriteLine("- {0} | {1} +{2} | {3}",item.ItemName, ClassifyType(item), item.Stats, item.ItemExplain);
            }
            Console.WriteLine("\n1. 장착 관리");
            Console.WriteLine("0. 나가기\n");
            Console.WriteLine("원하시는 행동을 입력해주세요.\n");
            Console.Write(">> ");

            switch (CheckValidInput(0, 1))
            {
                case 0:
                    MainScreen();
                    break;
                case 1:
                    InventoryManageScreen();
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
                    Console.WriteLine("- {0} {1} | {2} +{3} | {4}", i, item.ItemName, ClassifyType(item), item.Stats, item.ItemExplain);
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
                        player.Inventory_single[flag - 1].EquipCondition = 1;
                    }
                    else
                    {
                        player.CancelEquip(player.Inventory_single[flag - 1]);
                        player.Inventory_single[flag - 1].EquipCondition = 0;

                    }
                }
            }
            InventoryScreen();

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
        public List<MultipleItem> Inventory_multiple;

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
        }

        public void HavingSingleItem(SingleItem item)
        {
            Inventory_single.Add(item);
        }
        public void BuyingSingleItem(SingleItem item)
        {
            HavingSingleItem(item);
            Gold -= item.Price;
        }
        public void SellingSingleItem(SingleItem item)
        {
            Inventory_single.Remove(item);
            Gold += item.Price;
        }

        public void Equip(SingleItem item)
        {
            item.ItemName = "[E] " + item.ItemName;
            switch (item.TypeNumber)
            {
                case 0:
                    Atk += item.Stats;
                    AdditionalAtk += item.Stats;
                    break;
                case 1:
                    Def += item.Stats;
                    AdditionalDef += item.Stats;
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
                    break;
                case 1:
                    Def -= item.Stats;
                    AdditionalDef -= item.Stats;
                    break;
            }
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

    public interface MultipleItem
    {
        String ItemName { get; set; }

        public enum Typename
        {
            Potion,
            Weapon,
            Armor
        }

        void Use(Character character);
        void Insert(Character character);
    }
}