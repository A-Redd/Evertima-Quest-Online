using System;
using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Items
{
    public class CreatureDeathEventExt
    {
        public static void Initialize()
        {
            EventSink.CreatureDeath += OnCreatureDeath;
        }
        public static void OnCreatureDeath(CreatureDeathEventArgs e)
        {
            BaseCreature bc = e.Creature as BaseCreature;
            Container c = e.Corpse;

			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return;
			
			if (m_ItemxmlSys.PlayerLevels == true)
			{
				if (e.Killer is PlayerMobile )
				{
					PlayerMobile pm = e.Killer as PlayerMobile;
					LevelHandlerExt.Set(pm, bc);
					DeleteBankPet(bc);
				}
				if (e.Killer is BaseCreature)
				{
					BaseCreature bcc = e.Killer as BaseCreature;
					if (bcc.Controlled == false)
						return;
					LevelHandlerExt.Set(bcc, bc);
					DeleteBankPet(bc);
				}
			}
        }		
		public static void DeleteBankPet (BaseCreature bc)
		{
			BankBox box = bc.BankBox;
			if (box != null && bc.IsBonded == false)
				box.Delete();
			else
				return;
		}
    }
}
