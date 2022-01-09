using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Commands;
using System.Collections;
using Server.Engines.PartySystem;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Items
{
    public class ExpPowerHourToken : Item
    {
		public static void Initialize()
		{
			CommandHandlers.Register( "exphour", AccessLevel.GameMaster, new CommandEventHandler( exphour_OnCommand ) );
        }

		[Usage( "exphour" )]
		[Description( "Gives 1 hour of Exp boost." )]
		public static void exphour_OnCommand( CommandEventArgs e )
		{
			PlayerMobile from = e.Mobile as PlayerMobile;
			if ( null != from )
				from.Target = new EPowerHourTarget( from, true );
        }
        
        [Constructable]
        public ExpPowerHourToken()
            : base(0x1869)
        {
            Name = "EXP Power Hour Coin";
            Weight = 1.0;
            LootType = LootType.Blessed;
			ItemID = 10922;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("1 Hour Exp Boost Token"); // value: ~1_val~
        }

        public override void OnDoubleClick(Mobile from)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return;
			PlayerMobile pm = from as PlayerMobile;
			var p = Party.Get(from);
			int range = m_ItemxmlSys.PartyExpShareRange;
			
			
			if (IsChildOf(pm.Backpack))
			{
				LevelSheet xmlplayer = null;
				xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
				if (xmlplayer == null)
				{
					pm.SendMessage("This wont work for you!");
					return;
				}
				else if (xmlplayer.PowerHour == true)
				{
					pm.SendMessage("You must wait for your current Exp Power Hour to end!");
					return;
				}
				else
				{
					if (p != null)
					{
						foreach (PartyMemberInfo mi in p.Members)
						{
							pm = mi.Mobile as PlayerMobile;
							if (pm.Alive && pm.InRange(pm, range))
							{								
								LevelSheet xmlplayer2 = null;
								xmlplayer2 = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
								
								if (xmlplayer2 == null)
								{
									pm.SendMessage("You lack the level sheet!");
									return;
								}
								
								if (xmlplayer2.PowerHour == true)
								{
									pm.SendMessage("You already have a power hour, your party gains their bonus!");
									return;
								}
								else
								{
									xmlplayer2.TogglePowerHour(true, from);
									this.Delete();
								}
							}
						}
					
					}
					xmlplayer.TogglePowerHour(true, from);
					this.Delete();
				}   
            }
            else
                pm.SendMessage("This must be in your pack!");

        }

        public ExpPowerHourToken(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }
    }
	public class EPowerHourTarget : Target
	{

		private bool m_StaffCommand;

		public EPowerHourTarget( Mobile from, bool staffCommand ) : base( 10, false, TargetFlags.None )
		{
			m_StaffCommand = staffCommand;
			from.SendMessage( "Who gets the Power Hour?" );
		}
		
		protected override void OnTarget( Mobile from, object target )
		{
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return;
			
			LevelSheet xmlplayer = null;
			xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
			PlayerMobile pm = from as PlayerMobile;
	
			if (xmlplayer == null)
			{
				pm.SendMessage("You cant give them power hour! They lack the LevelSheet!");
				return;
			}
			else if (xmlplayer.PowerHour == true)
			{
				pm.SendMessage("They already have a power hour!");
				return;
			}
			else
			{
				pm.SendMessage("They have been awarded Power Hour!");
				xmlplayer.TogglePowerHour(true, from);
			}   

		}
	}
}