using System;
using Server;
using System.IO;
using System.Text;
using Server.Misc;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;
using Server.ContextMenus;
using System.Collections;
using System.Collections.Generic;
using Server.Engines.LevelSystemExtAtt;

namespace Server.Items
{
	public class PetLockPicks : Item
	{			
		[Constructable]
		public PetLockPicks() : base( 5372 )
		{
			Name = "Pet Lock Picks";
			Weight = 1.0;
			Hue = 20;
			Stackable = true;
		}

		public PetLockPicks( Serial serial ) : base( serial )
		{
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
			
			if (m_ItemxmlSys.EnabledLevelPets == false)
			{
				from.SendMessage( "The pet system is disabled!" );
				return;
			}
			else
			{
				if (m_ItemxmlSys.EnablePetpicks == false)
				{
					from.SendMessage( "The pet lock picks are disabled!" );
					return;
				}
				else
				{
					from.Target = new StolenPetTarg( from, false, this);
				}
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
	
	
	public class StolenPetTarg : Target
	{
		private readonly PetLockPicks m_PetLockPicks;
		
		public StolenPetTarg( Mobile from, bool staffCommand,PetLockPicks petlocks) : base( 10, false, TargetFlags.None )
		{
			from.SendMessage( "What pet collar will you attempt?" );
			m_PetLockPicks = petlocks;
		}

		protected override void OnTarget( Mobile from, object target)
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
			
			BaseCreature pet = target as BaseCreature;
			Mobile master = pet.GetMaster();
			
			if (pet == null)
			{
				from.SendMessage( "This only works on tamed pets!" );
				return;
			}
			
			PetLevelOrb petorb = null;
			BankBox petbox = pet.BankBox; 
			
			if (petbox == null)
			{
				from.SendMessage( "This pet is missing something!" );
				return;
			}
			petorb = petbox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;
			
			
			double skillValue = from.Skills[SkillName.Lockpicking].Value;
			double skillValueSteal = from.Skills[SkillName.Stealing].Value;

			var failedtosteal = (skillValue < Utility.Random(150));
			var nostealbonus = (skillValueSteal < Utility.Random(150));
			
			if ( target != pet )
			{
				from.SendMessage( "This only works on Players!" );
				return;
			}
			else if (pet == null || pet.Deleted)
			{
				from.SendMessage( "You lost track of the pet." );
			}
			else if (petorb == null)
			{
				from.SendMessage( "This pet is missing something!" );
				return;
			}
			else if (pet.IsBonded == true && m_ItemxmlSys.PreventBondedPetpick == true)
			{
				from.SendMessage( "The bond is too strong!" );
				return;
			}
			else if (from.Followers + pet.ControlSlots > from.FollowersMax )
			{
				from.SendMessage( "You have to many followers to attempt to steal this pet." );
				return;
			}
			else if (!pet.CanBeControlledBy(from))
			{
				from.SendMessage( "Can't steal a pet you can't control.");
			}
			else if (!IsEmptyHanded(from))
			{
                from.SendMessage( "You fumble as your hands are busy with something else.");
			}
			else if (pet.ControlMaster == from)
			{
				from.SendMessage( "Can't steal from yourself.");
			}
			else if (skillValue < m_ItemxmlSys.MinSkillReqPickSteal)
			{
				from.SendMessage( "Your Lock picking skill isn't high enough!");
			}
			else
			{
				if (failedtosteal == false)
				{
					pet.ControlMaster = from;
					pet.ControlTarget = null;
					petorb.WhoLastStoleMe = from.Serial;
					
					if (petorb.StolenPet == false)
					{
						petorb.StolenPet = true;
					}
					
					pet.ControlOrder = OrderType.Come;
					from.SendMessage( "You stole the pet!.");
					master.SendMessage( "You pet has been stolen by {0}!", from.Name);
					pet.InvalidateProperties();
					if (nostealbonus == true)
					{
						if (petorb.Levell != 0)
						{
							petorb.Levell = 0;
							petorb.Expp = 0;
							petorb.StatPoints = 0;
							petorb.kxp = 0;
							ScaleStats(pet, 0.7);
							m_PetLockPicks.Consume(); 
							return;
						}
						return;
					}
					else
					{
						from.SendMessage( "Your skill in stealing managed to keep the pets battle experience intact!.");
						m_PetLockPicks.Consume(); 
					}
				}
				else 
				{
					from.SendMessage( "You failed to steal the pet!.");
					master.SendMessage( "{0} tried to steal your pet!", from.Name);
					from.CriminalAction(true);
					BrokeLockPickTest(from, m_PetLockPicks);
				}
				
			}		
		}
		
        protected virtual void BrokeLockPickTest(Mobile from, PetLockPicks petlocks)
        {
            // When failed, a 25% chance to break the lockpick
            if (Utility.Random(4) == 0)
            {
                from.SendMessage( "You broke the pick!.");
                from.PlaySound(0x3A4);
                petlocks.Consume();
            }
        }

        public static bool IsEmptyHanded(Mobile from)
        {
            if (from.FindItemOnLayer(Layer.OneHanded) != null)
            {
                return false;
            }

            if (from.FindItemOnLayer(Layer.TwoHanded) != null)
            {
                return false;
            }

            return true;
        }
		
        public static void ScaleStats(BaseCreature bc, double scalar)
        {
            if (bc.RawStr > 0)
            {
                bc.RawStr = (int)Math.Max(1, bc.RawStr * scalar);
            }

            if (bc.RawDex > 0)
            {
                bc.RawDex = (int)Math.Max(1, bc.RawDex * scalar);
            }

            if (bc.RawInt > 0)
            {
                bc.RawInt = (int)Math.Max(1, bc.RawInt * scalar);
            }

            if (bc.HitsMaxSeed > 0)
            {
                bc.HitsMaxSeed = (int)Math.Max(1, bc.HitsMaxSeed * scalar);
                bc.Hits = bc.Hits;
            }

            if (bc.StamMaxSeed > 0)
            {
                bc.StamMaxSeed = (int)Math.Max(1, bc.StamMaxSeed * scalar);
                bc.Stam = bc.Stam;
            }
        }
	}
}

