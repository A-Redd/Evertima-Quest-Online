using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using Server.Commands;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Items
{
    public class ExpCoin : Item
    {
		private int m_SCV = 100;
		private bool m_expadded = false;
		private bool m_mod		= false;
		
		public static void Initialize()
		{
			CommandHandlers.Register( "expaward", AccessLevel.GameMaster, new CommandEventHandler( expaward_OnCommand ) );
        }

		[Usage( "expaward" )]
		[Description( "Gives ExpAward." )]
		public static void expaward_OnCommand( CommandEventArgs e )
		{
			PlayerMobile from = e.Mobile as PlayerMobile;
			if ( null != from )
				from.Target = new AwardExpTarget( from, true );
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Mod
        {
            get { return m_mod; }
            set 
			{ 
				m_mod = value; 
				InvalidateProperties(); 
			}
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int SCV
        {
            get { return m_SCV; }
            set 
			{ 
				m_SCV = value; 
				m_mod = true;
				InvalidateProperties(); 
			}
        }
        
        [Constructable]
        public ExpCoin()
            : base(0x1869)
        {
            Name = "A Exp Coin";
            Weight = 1.0;
            LootType = LootType.Blessed;
			ItemID = 10922;
        }
		
        public override void OnAdded(object parent)
        {
            base.OnAdded(parent);
			
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));	
			if (m_ItemxmlSys != null && m_ItemxmlSys.PlayerLevels == true)
			{
				if (parent is Mobile)
				{
					Mobile from = (Mobile)parent;
					if (Mod != true)
					{
						SCV = m_ItemxmlSys.ExpCoinValue;
						m_expadded = true;
					}
					this.InvalidateProperties();
					return;
				}
				else
				{
					if (Mod != true)
					{
						SCV = m_ItemxmlSys.ExpCoinValue;
						m_expadded = true;
					}
					this.InvalidateProperties();
				}
			}
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
			if (m_ItemxmlSys != null && m_ItemxmlSys.PlayerLevels == true)
			{
				if (Mod != true)
				{
					SCV = m_ItemxmlSys.ExpCoinValue;
					m_expadded = true;
				}
				
				this.InvalidateProperties();
				LevelSheet xmlplayer = null;
				xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
				PlayerMobile pm = from as PlayerMobile;

				if (IsChildOf(pm.Backpack))
				{
					if (xmlplayer.Levell >= m_ItemxmlSys.EndMaxLvl)  /* Max Level per System */
					{
						pm.SendMessage("You have reached the max level, this doesn't work for you!");
						return;
					}
					else
						xmlplayer.kxp += m_SCV;
						pm.SendMessage("You have been awarded {0} EXP points", m_SCV);
						if (xmlplayer.Expp >= xmlplayer.ToLevell && xmlplayer.Levell < xmlplayer.EndMaxLvl)
							LevelHandlerExt.DoLevel(pm);
						this.Consume();               
				}
				else
					pm.SendMessage("This must be in your pack!");

			}
			else
			{
				from.SendMessage("Level System Disabled!!");
				return;
			}

        }
		
        public override void GetProperties(ObjectPropertyList list)
        {
			base.GetProperties(list);
			if (m_expadded == true)
			{
				list.Add("+{0}", m_SCV.ToString(), "Exp Points"); // value: ~1_val~
			}
        }

        public ExpCoin(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            
            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
				{
					m_SCV = reader.ReadInt();
					break;
				}
            }
			m_expadded = reader.ReadBool();
			m_mod = reader.ReadBool();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
            writer.Write((int)SCV);
			writer.Write((bool)m_expadded);
			writer.Write((bool)m_mod);
        }
    }
	public class AwardExpTarget : Target
	{


		ExpCoin xp = new ExpCoin();
//		private int m_SCV = 100;
		private bool m_StaffCommand;

		public AwardExpTarget( Mobile from, bool staffCommand ) : base( 10, false, TargetFlags.None )
		{
			m_StaffCommand = staffCommand;
			from.SendMessage( "Who gets the Exp Award?." );
		}
		
		protected override void OnTarget( Mobile from, object target )
		{
			/* LevelSystemExt */
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));	
			if (m_ItemxmlSys != null && m_ItemxmlSys.PlayerLevels == true)
			{
				LevelSheet xmlplayer = null;
				xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
				PlayerMobile pm = from as PlayerMobile;
				BaseCreature pet = target as BaseCreature;

				if ( target == pet )
				{
					from.SendMessage( "This only works on Players!" );
				}
				else if (xmlplayer.Levell >= m_ItemxmlSys.EndMaxLvl)  /* Max Level per System */
				{
					pm.SendMessage("Target has reached the max level, this doesn't work for them!");
				}
				else
				{
					if (target is Mobile)
					{
						Mobile mt = (Mobile)target;
						mt.SendMessage("You have been awarded {0} EXP points", xp.SCV);
						xmlplayer.kxp += xp.SCV;
						if (xmlplayer.Expp >= xmlplayer.ToLevell && xmlplayer.Levell < xmlplayer.MaxLevel)
							LevelHandlerExt.DoLevel(pm);
					}         
				}		
			}
			else
			{
				from.SendMessage("Level System Disabled!");
				return;
			}
		}
	}
}