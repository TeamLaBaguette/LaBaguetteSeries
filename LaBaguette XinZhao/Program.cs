﻿#region
using System;
using System.Linq;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
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

        // Declare SpellList
        public static List<Spell> SpellList = new List<Spell>();
        // Declare spells
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


        /////////////////////////////////////////
        //             ON GAME LOAD            //
        /////////////////////////////////////////
        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampionName) return;
            if (Player.IsDead) return;

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 600);
            R = new Spell(SpellSlot.R, 480);
            LaBaguetteMenu();

            Game.OnGameUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            Interrupter.OnPossibleToInterrupt += Interrupter_OnPosibleToInterrupt;

            MessageBienvenue();
        }


        /////////////////////////////////////////
        //            MENU ASSEMBLY            //
        /////////////////////////////////////////
        private static void LaBaguetteMenu()
        {
            Config = new Menu("LaBaguette XinZhao", ChampionName, true);

            /////////////////////////
            //    MENU ORBWALKER   //
            /////////////////////////
            Config.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(Config.SubMenu("Orbwalker"));

            /////////////////////////
            //    MENU TARGETSEL   //
            /////////////////////////
            TargetSelectorMenu = new Menu("Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            Config.AddSubMenu(TargetSelectorMenu);

            /////////////////////////
            //      MENU COMBO     //
            /////////////////////////
            Config.AddSubMenu(new Menu("BasicCombo", "BasicCombo"));

                /////////////////////////
                //    SUBMENU COMBO    //
                //     INSTAB COMBO    //
                /////////////////////////
                Config.AddSubMenu(new Menu("BasicCombo", "BasicCombo"));
                    Config.SubMenu("BasicCombo").AddItem(new MenuItem("BasicComboUseQ", "Use Q").SetValue(true));
                    Config.SubMenu("BasicCombo").AddItem(new MenuItem("BasicComboUseW", "Use W").SetValue(true));
                    Config.SubMenu("BasicCombo").AddItem(new MenuItem("BasicComboUseE", "Use E").SetValue(true));
                    Config.SubMenu("BasicCombo").AddItem(new MenuItem("EMinRange", "Min. E Range")
                        .SetValue(new Slider(Q.Range, 200, Q.Range)));
                    Config.SubMenu("BasicCombo").AddItem(new MenuItem("BasicComboActive", "Combo!")
                        .SetValue(new KeyBind("Space".ToCharArray()[0], KeyBindType.Press)));

                /////////////////////////
                //    SUBMENU COMBO    //
                //     BASIC COMBO     //
                /////////////////////////
                Config.AddSubMenu(new Menu("InstaBCombo", "InstaBCombo"));
                    Config.SubMenu("InstaBCombo").AddItem(new MenuItem("InstaBComboPreQ", "PreCharge Q").SetValue(true));
                    Config.SubMenu("InstaBCombo").AddItem(new MenuItem("InstaBComboUseW", "Use W").SetValue(true));
                    Config.SubMenu("InstaBCombo").AddItem(new MenuItem("InstaBComboUseE", "Use E").SetValue(true));
                    Config.SubMenu("InstaBCombo").AddItem(new MenuItem("EMinRange", "Min. E Range")
                            .SetValue(new Slider(Q.Range, 200, Q.Range)));
                    Config.SubMenu("InstaBCombo").AddItem(new MenuItem("InstaBComboActive", "InstaBumpCombo!")
                            .SetValue(new KeyBind("T".ToCharArray()[0], KeyBindType.Press)));

            /////////////////////////
            //    MENU LANECLEAR   //
            /////////////////////////
            Config.AddSubMenu(new Menu("LaneClear", "LaneClear"));
                Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseQ", "Use Q").SetValue(false));
                Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseW", "Use W").SetValue(false));
                Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearUseE", "Use E").SetValue(false));
                Config.SubMenu("LaneClear")
                    .AddItem(new MenuItem("LaneClearMana", "Min. Mana Percent: ").SetValue(new Slider(50, 100, 0)));
                Config.SubMenu("LaneClear").AddItem(new MenuItem("LaneClearActive", "LaneClear!")
                    .SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            /////////////////////////
            //   MENU JUNGLEFARM   //
            /////////////////////////
            Config.AddSubMenu(new Menu("JungleFarm", "JungleFarm"));
                Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmUseQ", "Use Q").SetValue(true));
                Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmUseW", "Use W").SetValue(false));
                Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmUseE", "Use E").SetValue(false));
                Config.SubMenu("JungleFarm")
                    .AddItem(new MenuItem("JungleFarmMana", "Min. Mana Percent: ").SetValue(new Slider(50, 100, 0)));
                Config.SubMenu("JungleFarm").AddItem(new MenuItem("JungleFarmActive", "JungleFarm!")
                    .SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));

            /////////////////////////
            //     MENU DRAWING    //
            /////////////////////////
            Config.AddSubMenu(new Menu("Drawings", "Drawings"));
                Config.SubMenu("Drawings")
                    .AddItem(new MenuItem("DrawQRange", "Q Range").SetValue(new Circle(false, Color.PowderBlue)));
                Config.SubMenu("Drawings")
                    .AddItem(new MenuItem("DrawERange", "E Range").SetValue(new Circle(false, Color.PowderBlue)));
                Config.SubMenu("Drawings")
                    .AddItem(new MenuItem("DrawRRange", "R Range").SetValue(new Circle(false, Color.PowderBlue)));
                Config.SubMenu("Drawings")
                    .AddItem(new MenuItem("DrawThrown", "Can be thrown enemy").SetValue(new Circle(false, Color.PowderBlue)));

            /////////////////////////
            //      MENU EXTRA     //
            /////////////////////////
            MenuExtras = new Menu("Extras", "Extras");
            Config.AddSubMenu(MenuExtras);
            MenuExtras.AddItem(new MenuItem("InterruptSpells", "Interrupt Spells").SetValue(true));

                /////////////////////////
                //    SUBMENU EXTRA    //
                /////////////////////////
                var menuUseItems = new Menu("Use Items", "menuUseItems");
                MenuExtras.AddSubMenu(menuUseItems);

                /////////////////////////
                //    SUBMENU EXTRA    //
                //   TARGETED ITEMS    //
                /////////////////////////
                MenuTargetedItems = new Menu("Targeted Items", "menuTargetItems");
                menuUseItems.AddSubMenu(MenuTargetedItems);
                MenuTargetedItems.AddItem(new MenuItem("item3153", "Blade of the Ruined King").SetValue(true));
                MenuTargetedItems.AddItem(new MenuItem("item3143", "Randuin's Omen").SetValue(true));
                MenuTargetedItems.AddItem(new MenuItem("item3144", "Bilgewater Cutlass").SetValue(true));
                MenuTargetedItems.AddItem(new MenuItem("item3146", "Hextech Gunblade").SetValue(true));
                MenuTargetedItems.AddItem(new MenuItem("item3184", "Entropy ").SetValue(true));

                /////////////////////////
                //    SUBMENU EXTRA    //
                //   TARGETED ITEMS    //
                /////////////////////////
                MenuNonTargetedItems = new Menu("AOE Items", "menuNonTargetedItems");
                menuUseItems.AddSubMenu(MenuNonTargetedItems);
                MenuNonTargetedItems.AddItem(new MenuItem("item3180", "Odyn's Veil").SetValue(true));
                MenuNonTargetedItems.AddItem(new MenuItem("item3131", "Sword of the Divine").SetValue(true));
                MenuNonTargetedItems.AddItem(new MenuItem("item3074", "Ravenous Hydra").SetValue(true));
                MenuNonTargetedItems.AddItem(new MenuItem("item3142", "Youmuu's Ghostblade").SetValue(true));
                Config.AddToMainMenu();
        }



        /////////////////////////////////////////
        //          INSTABUMP COMBO            //
        /////////////////////////////////////////
        public static void InstaBCombo()
        {
            /////////////////////////
            //      VARIABLES      //
            /////////////////////////
            var t = GetEnemy(Q.Range, TargetSelector.DamageType.Magical);
            var PreQ = Config.Item("InstaBComboPreQ").GetValue<bool>();
            var useW = Config.Item("InstaBComboUseW").GetValue<bool>();
            var useE = Config.Item("InstaBComboUseE").GetValue<bool>();


            if (useQ && t.IsValidTarget(E.Range) && Q.IsReady())
                Q.Cast();

            if (useW && t.IsValidTarget(E.Range) && W.IsReady())
                W.Cast();

            if (useE && t.IsValidTarget(E.Range) && E.IsReady())
            {
                var eMinRange = Config.Item("EMinRange").GetValue<Slider>().Value;
                if (ObjectManager.Player.Distance(t) >= eMinRange)
                    E.CastOnUnit(t);
            }

            if (Player.Distance(t) <= E.Range)
                UseItems(t);

            if (Player.Distance(t) <= E.Range)
                UseItems(t, true);

            if (IgniteSlot != SpellSlot.Unknown && Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite) >= t.Health)
                {
                    Player.Spellbook.CastSpell(IgniteSlot, t);
                }
            }

            if (Tiamat.IsReady() && Player.Distance(t) <= Tiamat.Range)
                Tiamat.Cast();

            if (Hydra.IsReady() && Player.Distance(t) <= Hydra.Range)
                Tiamat.Cast();
        }


        /////////////////////////////////////////
        //             BASIC COMBO             //
        /////////////////////////////////////////
        public static void BasicCombo()
        {
            var t = GetEnemy(Q.Range, TargetSelector.DamageType.Magical);

            var useQ = Config.Item("ComboUseQ").GetValue<bool>();
            var useW = Config.Item("ComboUseW").GetValue<bool>();
            var useE = Config.Item("ComboUseE").GetValue<bool>();

            if (useQ && t.IsValidTarget(E.Range) && Q.IsReady())
                Q.Cast();

            if (useW && t.IsValidTarget(E.Range) && W.IsReady())
                W.Cast();

            if (useE && t.IsValidTarget(E.Range) && E.IsReady())
            {
                var eMinRange = Config.Item("EMinRange").GetValue<Slider>().Value;
                if (ObjectManager.Player.Distance(t) >= eMinRange)
                    E.CastOnUnit(t);
            }

            if (Player.Distance(t) <= E.Range)
                UseItems(t);

            if (Player.Distance(t) <= E.Range)
                UseItems(t, true);

            if (IgniteSlot != SpellSlot.Unknown &&
                Player.Spellbook.CanUseSpell(IgniteSlot) == SpellState.Ready)
            {
                if (Player.GetSummonerSpellDamage(t, Damage.SummonerSpell.Ignite) >= t.Health)
                {
                    Player.Spellbook.CastSpell(IgniteSlot, t);
                }
            }

            if (Tiamat.IsReady() && Player.Distance(t) <= Tiamat.Range)
                Tiamat.Cast();

            if (Hydra.IsReady() && Player.Distance(t) <= Hydra.Range)
                Tiamat.Cast();
        }


        /////////////////////////////////////////
        //            ON GAME UPDATE           //
        /////////////////////////////////////////
        static void Game_OnGameUpdate(EventArgs args)
        {
            if (!Orbwalking.CanMove(100)) return;


            /////////////////////////
            //     BASIC COMBO     //
            /////////////////////////

            //Check InstaBump Combo Bind
            if (Config.Item("BasicComboActive").GetValue<KeyBind>().Active)
            {
                //Execute Basic Combo
                BasicCombo();
            }

            /////////////////////////
            //     INSTAB COMBO    //
            /////////////////////////

            //Check InstaBump Combo Bind
            if (Config.Item("InstaBComboActive").GetValue<KeyBind>().Active)
            {
                //Execute InstaBump Combo
                InstaBCombo();
            }

            /////////////////////////
            //      LANECLEAR      //
            /////////////////////////
            if (Config.Item("LaneClearActive").GetValue<KeyBind>().Active)
            {
                var ActualMana = Player.MaxMana / 100 * Config.Item("LaneClearMana").GetValue<Slider>().Value;
                if (Player.Mana >= ActualMana)
                    LaneClear();
            }

            /////////////////////////
            //      JUNGLEFARM     //
            /////////////////////////
            if (Config.Item("JungleFarmActive").GetValue<KeyBind>().Active)
            {
                var ActualMana = Player.MaxMana / 100 * Config.Item("JungleFarmMana").GetValue<Slider>().Value;
                if (Player.Mana >= ActualMana)
                    JungleFarm();
            }
        }

        /////////////////////////////////////////
        //            WELCOME MESSAGE          //
        /////////////////////////////////////////
        private static void MessageBienvenue()
        {
            Game.PrintChat(String.Format("<font color='#0000FF'>La </font> <font color='#FFFFFF'>Bagu </font> <font color='#FFFFFF'>ette </font><font color='#FFFFFF'>{0}</font> <font color='#70DBDB'>Loaded!</font>", ChampionName));
        }
    }
}
