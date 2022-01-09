using System;
using Server;
using Server.Multis;
using Server.Network;
using Server.Items;
using Server.Spells;
using Server.Targeting;
using Server.Commands;
using Server.Mobiles;
using Server.Engines.Quests;

namespace Server.Items
{
    public class BootsOfTravel : Boots
    {
        [Constructable]
        public BootsOfTravel() : base( 0x5726 )
        {
            Movable = true;
            Weight = 0.0;
            Name = "Boots of the Eternal Mount"; 
            m_TimeLeft = 900;
			Hue = 1152;
            LootType = LootType.Blessed;
		}
   
        private int m_TimeLeft;
        public int TimeLeft { get{ return m_TimeLeft; } set { m_TimeLeft = value; InvalidateProperties(); } }
   
        private RunTimer m_Timer;
 
        public override void OnDoubleClick( Mobile from )
        {
			if ( Parent != from )
				from.SendMessage( "You must equip this item to use it." );
			
            else if (from.Flying == false)
            {
				BlockMountType type = BaseMount.GetMountPrevention(from);
								
				if (!from.Alive)
				{
					 from.SendMessage("The dead cannot move that fast...");
				}
         
                else
                {
                    ToggleFlight(true, from);
					
                }
            }
     
            else
            {
                ToggleFlight(false, from);
				
            }
        }

		public void NewTimer(Mobile from)
		{
			  if (m_Timer != null)
					m_Timer.Stop();
			  m_Timer = new RunTimer(from, this);
			  m_Timer.Start();
		}
		
        public void ToggleFlight(bool turnon, Mobile from)
        {
            if (turnon)
            {
				this.Hue = 1174;
                from.Freeze(TimeSpan.FromSeconds(1));
                from.RevealingAction();
                from.Animate(900, 10, 1, true, false, 0);
                from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "You are Running Fast!");
                from.Flying = true;
                if (m_Timer != null)
                    m_Timer.Stop();
                m_Timer = new RunTimer(from, this);
                m_Timer.Start();
            }
            else
            {
				this.Hue =1152;
                from.Freeze(TimeSpan.FromSeconds(1));
                from.RevealingAction();
                from.Animate(61, 10, 1, true, false, 0);
                from.Flying = false;
                from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "You've Decided to take it easy!");
                if (m_Timer != null)
                    m_Timer.Stop();
                m_Timer = new RunTimer(from, this);
                m_Timer.Start();
            }
        }
        private class RunTimer : Timer
        {
            private readonly Mobile m;
            private BootsOfTravel fi;
            public RunTimer(Mobile from, BootsOfTravel item)
                : base(TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(1), 900)
            {
                m = from;
                fi = item;
                Priority = TimerPriority.EveryTick;
            }
            protected override void OnTick()
            {
				if (fi == null || fi.Deleted) Stop();
                if (m.Flying)
                {
                    fi.TimeLeft--;
                    if (fi.TimeLeft <= 0)
                    {
                        fi.ToggleFlight(false, m);
                    }
                }
                else
                {
                    fi.TimeLeft++;
                    if (fi.TimeLeft >= 900)
                    {
                        m.SendMessage("Your Running Timer has reached maximum charge of 15 minutes available.");
                        this.Stop();
                    }
                }
            }
        }
 
        public virtual bool Accepted
        {
            get
            {
                return this.Deleted;
            }
        }
		
		public override bool OnEquip( Mobile from ) 
		{ 
			if (from is PlayerMobile)
			{
				ToggleFlight(true, from);
				return true;

			}
			else
			{
				return false;
			}
		}
		
        public override void OnRemoved(object parent)
        {
            if (parent is Mobile)
            {
				Mobile m = (Mobile)parent;
				ToggleFlight(false, m);
				return;
			}
        }

        public override bool DropToWorld(Mobile from, Point3D p)
        {
            bool ret = base.DropToWorld(from, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    return true;
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "You feel silly for wanting to drop something so useful...");
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
        public override bool DropToMobile(Mobile from, Mobile target, Point3D p)
        {
            bool ret = base.DropToMobile(from, target, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    return true;
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "This cannot be traded!");
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
        public override bool DropToItem(Mobile from, Item target, Point3D p)
        {
            bool ret = base.DropToItem(from, target, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack)
            {
                if (from.AccessLevel >= AccessLevel.GameMaster)
                {
                    return true;
                }
                else
                {
                    from.LocalOverheadMessage(MessageType.Emote, 0x22, true, "This can only exist on the top level of the backpack!");
                    return false;
                }
            }
            else
            {
                return ret;
            }
        }
			
		public override void AddNameProperty(ObjectPropertyList list)
        {
			base.AddNameProperty( list );

			list.Add( "<BASEFONT COLOR=#7FCAE7>" + "Boots of the Eternal Mount" + "<BASEFONT COLOR=#FFFFFF>"/*Back to White*/ );

        }
		
        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );
           
            list.Add( "<BASEFONT COLOR=#7FCAE7>Ability: Run Fast \nAuto Activate Upon Equip \nDouble Click To Toggle While Equipped \n<BASEFONT COLOR=#7FCAE7>Time Remaining: {0} Seconds" + "<BASEFONT COLOR=#FFFFFF>", m_TimeLeft.ToString());
        }
		 
        public BootsOfTravel( Serial serial ) : base( serial )
        {
        }
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 ); // version
			writer.Write((int)m_TimeLeft);
			
        }
        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
			m_TimeLeft = reader.ReadInt();
        }
    }
}