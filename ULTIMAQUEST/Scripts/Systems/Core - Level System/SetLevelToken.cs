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
    public class SetLevelToken : Item
    {
		public static void Initialize()
		{
			CommandHandlers.Register( "levelup", AccessLevel.GameMaster, new CommandEventHandler( levelup_OnCommand ) );
        }

		[Usage( "levelup" )]
		[Description( "Gives Level." )]
		public static void levelup_OnCommand( CommandEventArgs e )
		{
			PlayerMobile from = e.Mobile as PlayerMobile;
			if ( null != from )
				from.Target = new SetLevelTokenTarget( from, true );
        }

        [Constructable]
        public SetLevelToken()
            : base(0x1869)
        {
            Name = "A Level Token";
            Weight = 1.0;
            LootType = LootType.Blessed;
			ItemID = 10922;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add("Grants Exp to level up 1 time"); 
        }

        public override void OnDoubleClick(Mobile from)
        {
			LevelSheet xmlplayer = null;
			xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
            PlayerMobile pm = from as PlayerMobile;

            if (IsChildOf(pm.Backpack))
            {
				if (xmlplayer == null)
				{
					pm.SendMessage("You cannot use this!");
					return;
				}
				int CurrentLevel 	=	xmlplayer.Levell;
				int NeededToLevel	=	xmlplayer.ToLevell;
				int CurrentExp		=	xmlplayer.Expp;
				int CurrentKXP		=	xmlplayer.kxp;
				int DifferenceNeed	=	NeededToLevel - CurrentExp;
				if (xmlplayer.Levell >= xmlplayer.MaxLevel)
				{
					pm.SendMessage("Target has reached the max level, this doesn't work for them!");
					return;
				}
				else
				{
					xmlplayer.kxp += DifferenceNeed;
					xmlplayer.Expp += DifferenceNeed;
					
					if (xmlplayer.Expp >= xmlplayer.ToLevell && xmlplayer.Levell < xmlplayer.MaxLevel)
					{
                        LevelHandlerExt.DoLevel(pm);
						this.Delete();  
					}
				}
            }
            else
                pm.SendMessage("This must be in your pack!");
        }

        public SetLevelToken(Serial serial)
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
	public class SetLevelTokenTarget : Target
	{
		private bool m_StaffCommand;

		public SetLevelTokenTarget( Mobile from, bool staffCommand ) : base( 10, false, TargetFlags.None )
		{
			m_StaffCommand = staffCommand;
			from.SendMessage( "Who gets the Exp Award?." );
		}
		
		protected override void OnTarget( Mobile from, object target )
		{
			LevelSheet xmlplayer = null;
			xmlplayer = from.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
            PlayerMobile pm = from as PlayerMobile;
			BaseCreature pet = target as BaseCreature;

			if ( target == pet )
			{
				from.SendMessage( "This only works on Players!" );
			}
			else
			{
				if (target is PlayerMobile)
				{
					if (xmlplayer == null)
					{
						from.SendMessage( "They do not have a level sheet!" );
						return;
					}
					int CurrentLevel 	=	xmlplayer.Levell;
					int NeededToLevel	=	xmlplayer.ToLevell;
					int CurrentExp		=	xmlplayer.Expp;
					int CurrentKXP		=	xmlplayer.kxp;
					int DifferenceNeed	=	NeededToLevel - CurrentExp;
					if (xmlplayer.Levell >= xmlplayer.MaxLevel)
					{
						pm.SendMessage("Target has reached the max level, this doesn't work for them!");
						return;
					}
					else
					{
						xmlplayer.kxp += DifferenceNeed;
						xmlplayer.Expp += DifferenceNeed;
						
						if (xmlplayer.Expp >= xmlplayer.ToLevell && xmlplayer.Levell < xmlplayer.MaxLevel)
						{
							LevelHandlerExt.DoLevel(pm);
						}
					}
				}         
			}		
		}
	}
}