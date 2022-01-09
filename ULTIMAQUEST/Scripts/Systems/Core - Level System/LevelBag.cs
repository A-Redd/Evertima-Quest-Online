/*
Original Script though Open use, was done by 
BackpackOfReduction.cs
snicker7 v1.3 [RunUO 2.0]
06/19/06

Script altered with additional features and to integrate into level system extreme. 
*/
using System;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using System.Collections;
using Server.Gumps;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Items 
{
	public class LevelBag : Backpack 
	{
		public int m_BoundToPlayer = 0;
		public int m_NotedLevel = 0;
		private double m_Redux;	
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int NotedLevel
        {
            get { return m_NotedLevel; }
            set { m_NotedLevel = value; }
        }
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int BagOwner
        {
            get { return m_BoundToPlayer; }
            set { m_BoundToPlayer = value; }
        }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int ReduxPercent
		{
			get
			{
				return (int)(m_Redux*100);
			}
			set
			{
				if(value<0)
					value=0;
				if(value>100)
					value=100;
				m_Redux=((double)value)/100;
				if ( Parent is Item ){
					( Parent as Item ).UpdateTotals();
					( Parent as Item ).InvalidateProperties();
				}
				else if ( Parent is Mobile )
					( Parent as Mobile ).UpdateTotals();
				else
					UpdateTotals();
				
				InvalidateProperties();
			}
		}
		[Constructable]
		public LevelBag() : this(5) 
		{
		}
		
		[Constructable]
		public LevelBag(int redux) : base() 
		{
			ReduxPercent = redux;
			Name = "Level Bag";
		}
		
		public LevelBag(Serial serial) : base(serial) 
		{	
		}
				
        public virtual bool Accepted
        {
            get
            {
                return this.Deleted;
            }
        }
		
        public override bool DropToWorld(Mobile from, Point3D p)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return false;

            bool ret = base.DropToWorld(from, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack && m_ItemxmlSys.Preventbagdrop)
            {
                if (from.IsStaff())
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
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return false;
			
            bool ret = base.DropToMobile(from, target, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack && m_ItemxmlSys.Preventbagdrop)
            {
                if (from.IsStaff())
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
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return false;
			
            bool ret = base.DropToItem(from, target, p);
            if (ret && !this.Accepted && this.Parent != from.Backpack && m_ItemxmlSys.Preventbagdrop)
            {
                if (from.IsStaff())
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
		
		public override void OnDoubleClick( Mobile from )
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
			
			PlayerMobile pm = (PlayerMobile)from;
			LevelSheet xmlplayer = null;
			LevelBag lb = (LevelBag)this;
			xmlplayer = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
			if (xmlplayer == null)
				return;
			
			if (m_ItemxmlSys.Bagsystemmaintoggle == true)
			{
				if (BagOwner != 0)
				{
					if (xmlplayer.Levell != NotedLevel)
					{
						UpdateAddLevelInfo(pm, lb);
						Open(from);
					}
				}
				
				
				if ( BagOwner != from.Serial && m_ItemxmlSys.Level1groupbagownership == true)
				{
					from.SendMessage( "{0}", m_ItemxmlSys.Level1groupownermsg );
					return;
				}
				else
				{
					Open(from);
				}
			}
		}

        public override void OnAdded(object parent)
        {
				LevelBag lb = (LevelBag)this;
				PlayerMobile pm = (PlayerMobile)RootParent;
				UpdateAddLevelInfo(pm, lb);
        }
		
		public static void UpdateAddLevelInfo (Mobile from, LevelBag bag)
		{
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys bagconfig = null;
			bagconfig = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (bagconfig == null)
				return;
			
			PlayerMobile pmm = (PlayerMobile)from;
			LevelBag lb = (LevelBag)bag;
			int BagOwner = lb.m_BoundToPlayer;

			if (bagconfig.Bagsystemmaintoggle == true)
			{
				if (from is PlayerMobile)
				{
					PlayerMobile pm = (PlayerMobile)from;
					LevelSheet xmlplayer = null;
					xmlplayer = pm.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
					if (xmlplayer == null)
						return;
					else
					{
						if (bagconfig.Bagblessed == true)
						{
							lb.LootType = LootType.Blessed;
						}
							
						/* Group 1*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup1reqLevel && bagconfig.Levelgroup1 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup1maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup1reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level1groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup1msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level1groupownernow ); 
								return;
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						/* Group 2*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup2reqLevel && bagconfig.Levelgroup2 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup2maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup2reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level2groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup2msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level2groupownernow ); 
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						/* Group 3*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup3reqLevel && bagconfig.Levelgroup3 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup3maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup3reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level3groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup3msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level3groupownernow );  
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						/* Group 4*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup4reqLevel && bagconfig.Levelgroup4 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup4maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup4reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level4groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup4msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level4groupownernow ); 
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						/* Group 5*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup5reqLevel && bagconfig.Levelgroup5 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup5maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup5reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level5groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup5msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level5groupownernow ); 
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						/* Group 6*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup6reqLevel && bagconfig.Levelgroup6 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup6maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup6reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level6groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup6msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level6groupownernow ); 
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						/* Group 7*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup7reqLevel && bagconfig.Levelgroup7 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup7maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup7reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level7groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup7msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level7groupownernow ); 
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						/* Group 8*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup8reqLevel && bagconfig.Levelgroup8 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup8maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup8reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level8groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup8msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level8groupownernow ); 
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						/* Group 9*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup9reqLevel && bagconfig.Levelgroup9 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup9maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup9reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level9groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup9msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level9groupownernow ); 
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						/* Group 10*/
						if (xmlplayer.Levell <= bagconfig.Levelgroup10reqLevel && bagconfig.Levelgroup10 == true)
						{
							lb.MaxItems = bagconfig.Levelgroup10maxitems;
							lb.ReduxPercent = bagconfig.Levelgroup10reducetotal;
							if (BagOwner == 0 && pm is PlayerMobile && bagconfig.Level10groupbagownership == true)
							{
								pm.SendMessage("{0}", bagconfig.Levelgroup10msg);
								lb.m_BoundToPlayer = from.Serial;
								lb.m_NotedLevel = xmlplayer.Levell;
								lb.Name = pm.Name.ToString() + "'s Level Bag";
								pm.SendMessage( "{0}", bagconfig.Level10groupowner ); 
							}
							lb.m_NotedLevel = xmlplayer.Levell;
							return;
						}
						return;
					}
				}
			}
		}
		

		
		public override void UpdateTotal(Item sender, TotalType type, int delta)
		{
			base.UpdateTotal(sender,type,delta);
			if(type==TotalType.Weight){
				if ( Parent is Item )
					( Parent as Item ).UpdateTotal( sender, type, (int)(delta*m_Redux)*-1 );
				else if ( Parent is Mobile )
					( Parent as Mobile ).UpdateTotal( sender, type, (int)(delta*m_Redux)*-1 );
			}
		}
		
		public override int GetTotal(TotalType type)
		{
			if(type==TotalType.Weight)
				return (int)(base.GetTotal(type)*(1.0-m_Redux));
			return base.GetTotal(type);
		}
		
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
			if (ReduxPercent != 0)
				list.Add("Weight Reduction: {0}%", ReduxPercent);
        }
		
		public override void Serialize( GenericWriter writer ) 
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			writer.Write((double)m_Redux);
			writer.Write((int) m_BoundToPlayer);
			writer.Write((int) m_NotedLevel);
		}
		
		public override void Deserialize( GenericReader reader ) 
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			m_Redux = reader.ReadDouble();
			m_BoundToPlayer = reader.ReadInt();
			m_NotedLevel = reader.ReadInt();
		}
	}
}