using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Network;
using Server.Commands;
using Server.Targeting;
using Server.Mobiles;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Gumps
{
    public class ExpBar : Gump
    {
        public static void Initialize()
        {
			CommandSystem.Register("ExpBar", AccessLevel.Player, new CommandEventHandler(ExpBar_OnCommand));
        }

        private static void ExpBar_OnCommand(CommandEventArgs e)
        {
			LevelSheet xmlplayer = null;
			xmlplayer = e.Mobile.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
			
			
			if (xmlplayer == null)
			{
				e.Mobile.SendMessage("You may not use this!");
				return;
			}
			else
			{
				e.Mobile.CloseGump(typeof(ExpBar));
				e.Mobile.SendGump(new ExpBar(e.Mobile));
			}
        }

        public ExpBar(Mobile m)
            : base(0, 0)
        {
            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;
            AddPage(0);

            PlayerMobile pm = m as PlayerMobile;
			LevelSheet xmlplayer = null;
			xmlplayer = m.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
            AddBackground(35, 30, 300, 300, 9270);
           // AddLabel(45, 25, 50, "EXP:");
           // AddLabel(60, 25, 3, "" + xmlplayer.Expp.ToString("#,0"));
			
			AddLabel(50, 45, 50, "Level: ");
			AddLabel(95, 45, 50, "" + xmlplayer.Levell.ToString("#,0"));
			
			AddLabel(150, 100+40, 50, "Hp: " + m.Hits + " / "+m.HitsMax );
            AddLabel(150, 120+40, 50, "Mana: " + m.Mana + " / " + m.ManaMax);
            AddLabel(150, 140+40, 50, "ATK:");
            //AddLabel(248, 25, 3, "" + xmlplayer.MaxLevel.ToString("#,0"));
            //AddLabel(25, 40, 50, "Level At:");
            //AddLabel(85, 40, 3, "" + xmlplayer.ToLevell.ToString("#,0"));
            AddLabel(150, 105, 50, "" + GetPercentage((int)xmlplayer.Expp, xmlplayer.ToLevell, 2) + "%");
           // AddLabel(179, 40, 50, "(" + AddSpaces(GetPercentage((int)xmlplayer.Expp, xmlplayer.ToLevell, 2) + "%") + "  Reached)");
            ////////////////////////Classes
            AddLabel(50, 60, 1153, "Class:  " + m.MobClass.ToString());
            AddLabel(50, 100 + 40, 1153, "Str: " + m.Str.ToString("#,0"));
            AddLabel(50, 120 + 40, 1153, "Con: 100");
            AddLabel(50, 140 + 40, 1153, "Agi: 85");
            AddLabel(50, 160 + 40, 1153, "Dex: 70");
            AddLabel(50, 180 + 40, 1153, "Wis: 50");
            AddLabel(50, 200 + 40, 1153, "Int: 55");
            AddLabel(50, 220 + 40, 1153, "Cha: 65");

            double ShowBarAt = xmlplayer.ToLevell / 100;
            double NextExtendAt = 0;
            int LengthOfBar = 0;

            if (NextExtendAt == 0)
                NextExtendAt = ShowBarAt;

            for (int i = 0; xmlplayer.Expp >= NextExtendAt; i++)
            {
                NextExtendAt += ShowBarAt;
                LengthOfBar = (int)(2.24 * i);
            }
            AddLabel(50, 105, 1153,  "_____________________________");
            //AddImage(50, 84, 2627, 0);//x, y, Width, Heigth, ID
            AddImageTiled(50, 120, LengthOfBar, 15, 58);//x, y, Width, Heigth, ID
           // AddImageTiled(50, 120, LengthOfBar/25, 15, 58);//x, y, Width, Heigth, ID
            //AddImage(50, 92, 2627, 0);//x, y, Width, Heigth, ID
            AddLabel(50, 116, 1153, "(____________________________)");
        }

        public static string AddSpaces(string SpaceNeeded)
        {
            int Number = 0;
            string Spaces = "";

            for (int i = 0; i < SpaceNeeded.Length; i++)
            {
                if (i == 0)
                    Number = 1;
                else
                    Number = i;
            }

            while (Spaces.Length < Number)
            { Spaces += " "; }

            return Spaces;
        }

        public static string GetPercentage(int value, int total, int places)
        {
            Decimal percent = 0;
            string retval = string.Empty;
            String strplaces = new String('0', places);
            
            if (value == 0 || total == 0)
            { percent = 0; }
            else
            {
                percent = Decimal.Divide(value, total) * 100;

                if (places > 0)
                { strplaces = "." + strplaces; }
            }

            retval = percent.ToString("#" + strplaces);
            return retval;
        }
    }
}
