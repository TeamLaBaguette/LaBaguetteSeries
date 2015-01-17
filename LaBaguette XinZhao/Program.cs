#region
using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using Color = System.Drawing.Color;
#endregion

namespace LaBaguette_XinZhao
{
    class Program
    {
        ////////////////////////
        //    Declare items   //
        ////////////////////////

        // Declare ChampionName
        public static string ChampionName = "XinZhao";

        // Declare
        private static readonly Obj_AI_Hero Player = ObjectManager.Player;
        
        // Declare OrbWalker
        public static Orbwalking.Orbwalker Orbwalker;
        
        ////////////////////////
        //   Declare spells   //
        ////////////////////////
        public static List<Spell> SpellList = new List<Spell>();
        public static Spell Q, W, E, R;

        private static readonly SpellSlot IgniteSlot = Player.GetSpellSlot("SummonerDot");

        ////////////////////////
        //    Declare items   //
        ////////////////////////
        public static Items.Item Tiamat = new Items.Item(3077, 375);
        public static Items.Item Hydra = new Items.Item(3074, 375); 

        public static Menu Config;
        public static Menu TargetSelectorMenu;
        public static Menu MenuExtras;
        public static Menu MenuTargetedItems;
        public static Menu MenuNonTargetedItems;

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampionName) return;
            if (Player.IsDead) return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 480);
            AssemblyMenu();

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPosibleToInterrupt;

            WelcomeMessage();
        }

        private static void AssemblyMenu()
        {
            Config = new Menu("LaBaguette XinZhao", ChampionName, true);

            Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Config.AddSubMenu(TargetSelectorMenu);

            /* [ Combo ] */
            Config.AddSubMenu(new Menu("Combo", "Combo"));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboUseQ", "Use Q").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboUseW", "Use W").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboUseE", "Use E").SetValue(true));
            Config.SubMenu("Combo").AddItem(new MenuItem("EMinRange", "Min. E Range")
                .SetValue(new Slider(Q.Range, 200, Q.Range)));
            Config.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!")
                .SetValue(new KeyBind("Z".ToCharArray()[0], KeyBindType.Press)));

            /* [ Lane Clear ] */
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseQ", "Use Q").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseW", "Use W").SetValue(false));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseE", "Use E").SetValue(false));
            Config.SubMenu("LaneClear")
                .AddItem(new MenuItem("LaneClearMana", "Min. Mana Percent: ").SetValue(new Slider(50, 100, 0)));
            Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearActive", "LaneClear!")
                .SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            /* [ Jungling Farm ] */
            Config.AddSubMenu(new Menu("JungleFarm", "JungleFarm"));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmUseQ", "Use Q").SetValue(true));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmUseW", "Use W").SetValue(false));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmUseE", "Use E").SetValue(false));
            Config.SubMenu("JungleFarm")
                .AddItem(new MenuItem("JungleFarmMana", "Min. Mana Percent: ").SetValue(new Slider(50, 100, 0)));
            Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmActive", "JungleFarm!")
                .SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            /* [ Drawing ] */
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("DrawQRange", "Q Range").SetValue(new Circle(false, Color.PowderBlue)));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("DrawERange", "E Range").SetValue(new Circle(false, Color.PowderBlue)));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("DrawRRange", "R Range").SetValue(new Circle(false, Color.PowderBlue)));
            Config.SubMenu("Drawings")
                .AddItem(new MenuItem("DrawThrown", "Can be thrown enemy").SetValue(new Circle(false, Color.PowderBlue)));

            /* [  Extras -> Use Items ] */
            MenuExtras = new Menu("Extras", "Extras");
            Config.AddSubMenu(MenuExtras);
            MenuExtras.AddItem(new MenuItem("InterruptSpells", "Interrupt Spells").SetValue(true));

            /* [  Extras -> Use Items ] */
            var menuUseItems = new Menu("Use Items", "menuUseItems");
            MenuExtras.AddSubMenu(menuUseItems);

            /* [ Extras -> Use Items -> Targeted Items ] */
            MenuTargetedItems = new Menu("Targeted Items", "menuTargetItems");
            menuUseItems.AddSubMenu(MenuTargetedItems);
            MenuTargetedItems.AddItem(new MenuItem("item3153", "Blade of the Ruined King").SetValue(true));
            MenuTargetedItems.AddItem(new MenuItem("item3143", "Randuin's Omen").SetValue(true));
            MenuTargetedItems.AddItem(new MenuItem("item3144", "Bilgewater Cutlass").SetValue(true));
            MenuTargetedItems.AddItem(new MenuItem("item3146", "Hextech Gunblade").SetValue(true));
            MenuTargetedItems.AddItem(new MenuItem("item3184", "Entropy ").SetValue(true));

            /* [ Extras -> Use Items -> AOE Items ] */
            MenuNonTargetedItems = new Menu("AOE Items", "menuNonTargetedItems");
            menuUseItems.AddSubMenu(MenuNonTargetedItems);
            MenuNonTargetedItems.AddItem(new MenuItem("item3180", "Odyn's Veil").SetValue(true));
            MenuNonTargetedItems.AddItem(new MenuItem("item3131", "Sword of the Divine").SetValue(true));
            MenuNonTargetedItems.AddItem(new MenuItem("item3074", "Ravenous Hydra").SetValue(true));
            MenuNonTargetedItems.AddItem(new MenuItem("item3142", "Youmuu's Ghostblade").SetValue(true));

            new PotionManager();
            new AssassinManager();
            Config.AddToMainMenu();
        }

        private static void WelcomeMessage()
        {
            Game.PrintChat(String.Format("<font color='#0000FF'>La </font> <font color='#FFFFFF'>Bagu </font> <font color='#FFFFFF'>ette </font><font color='#FFFFFF'>{0}</font> <font color='#70DBDB'>Loaded!</font>", ChampionName));
        }
    }
}
