using System;
using Server;
using Server.Network;
using Server.Items;
using Server.Mobiles;
using Server.Engines.Craft;
using Server.Engines.Harvest;
using Server.Engines.LevelSystemExtAtt;

namespace Server
{
    //This File Gets And Sets Exp
    //It Also Sets The Level For Creatures
    public class LevelCore
    {
        public static int GetExp(Mobile klr)
        {
            return 0; //Stats(klr) + (Skills(klr) / 10);
        }

        public static int Stats(Mobile m)
        {
			// Possible future changes here
			return 0; //((int)Math.Round(11.111 * m.RawStatTotal));
        }

        public static int Skills(Mobile m)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return 0;
	

			if (m_ItemxmlSys.AdvancedSkillExp)
			{
				if (Core.AOS)
				{ return (int)9.375 * AoS(m); }
				else if (Core.SE || Core.ML)
				{ return (int)7.5 * SE(m); }
				else
				{ return (int)6.25 * Pre(m); }
			}
			else
				return ((int)1.388 * m.Skills.Total);

        }

        public static int Pre(Mobile m)
        {
            return (int)(m.Skills[SkillName.Archery].Base + m.Skills[SkillName.Fencing].Base
                + m.Skills[SkillName.Macing].Base + m.Skills[SkillName.Magery].Base +
                m.Skills[SkillName.Parry].Base + m.Skills[SkillName.Swords].Base +
                m.Skills[SkillName.Tactics].Base + m.Skills[SkillName.Wrestling].Base);
        }

        public static int AoS(Mobile m)
        {
            return (int)(Pre(m) + m.Skills[SkillName.Chivalry].Base
                + m.Skills[SkillName.Necromancy].Base);
        }

        public static int SE(Mobile m)
        {
            return (int)(AoS(m) + m.Skills[SkillName.Ninjitsu].Base
                + m.Skills[SkillName.Bushido].Base);
        }

        public static int Base(Mobile kld)
        {
				LevelControlSysItem loccontrol = null;
				foreach (Item lister in World.Items.Values)
				{
					if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
				}
				LevelControlSys m_ItemxmlSys = null;
				m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
				if (m_ItemxmlSys == null)
					return 0;
	
            double amnt;

            if (kld is BaseCreature)
            {
                BaseCreature bc = kld as BaseCreature;

                if (m_ItemxmlSys.TablesAdvancedExp)
                {
                    amnt = bc.HitsMax / 4;

                    #region Get Table Exp

                    foreach (Type creat in exp.NoExp)
                    {
                        if (creat == bc.GetType())
                            return 0;
                    }

                    foreach (Type creat in exp.ExtLowExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(20, 40));
                    }

                    foreach (Type creat in exp.VeryLowExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(40, 60));
                    }

                    foreach (Type creat in exp.LowExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(60, 80));
                    }

                    foreach (Type creat in exp.LowMedExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(80, 95));
                    }

                    foreach (Type creat in exp.MedExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(95, 125));
                    }

                    foreach (Type creat in exp.HighMedExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(125, 145));
                    }

                    foreach (Type creat in exp.HighExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(145, 175));
                    }

                    foreach (Type creat in exp.VeryHighExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(175, 200));
                    }

                    foreach (Type creat in exp.ExtHighExp)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(200, 225));
                    }

                    foreach (Type creat in exp.ExtHigh)
                    {
                        if (creat == bc.GetType())
                            return (int)(amnt + Utility.RandomMinMax(225, 400));
                    }

                    #endregion
                }
                else
                {
                    amnt = bc.HitsMax + bc.RawStatTotal;

                    if (IsMageryCreature(bc))
                        amnt += 10;

                    if (IsFireBreathingCreature(bc))
                        amnt += 15;

                    if (kld is VampireBat || kld is VampireBatFamiliar)
                        amnt += 5;

                    amnt += GetPoisonLevel(bc) * 10;
                }
            }
            else
            {
                PlayerMobile pm = kld as PlayerMobile;

                amnt = ((pm.Str * pm.Hits) + (pm.Dex * pm.Stam)
                    + (pm.Int * pm.Mana)) / 5;

                if (pm.Skills.Total >= 100)
                    amnt += pm.Skills.Total;
                else
                    amnt += 100;
            }
            
            return (int)amnt / 10;       
        }

        private static bool IsMageryCreature(BaseCreature bc)
        {
            return (bc != null && bc.AI == AIType.AI_Mage && bc.Skills[SkillName.Magery].Base > 5.0);
        }

        private static bool IsParagon(BaseCreature bc)
        {
            if (bc == null)
                return false;

            return bc.IsParagon;
        }

		public static bool IsFireBreathingCreature(BaseCreature bc)
        {
            if (bc == null)
                return false;

            var profile = bc.AbilityProfile;

            if (profile != null)
            {
                return profile.HasAbility(SpecialAbility.DragonBreath);
            }

            return false;
        }

        private static bool IsPoisonImmune(BaseCreature bc)
        {
            return (bc != null && bc.PoisonImmune != null);
        }

        private static int GetPoisonLevel(BaseCreature bc)
        {
            if (bc == null)
                return 0;

            Poison p = bc.HitPoison;

            if (p == null)
                return 0;

            return p.Level + 1;
        }

        public static int CreatureLevel(Mobile m)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return 0;
				
            BaseCreature bc = m as BaseCreature;

            int Lvl = ((bc.HitsMax + bc.RawStatTotal) / 40) + ((bc.DamageMax * bc.DamageMin) / 30);

            if (bc is BaseVendor)
                return 0;
            else
            {
                if (Lvl < 1)
                    return 1;
                else if (IsParagon(bc))
                    return Lvl += Utility.RandomMinMax(3, 7);
                else
                    return Lvl;
            }
        }
		
        public static int BaseVendorLevel(Mobile m)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return 0;
				
            BaseCreature bc = m as BaseCreature;

            int Lvl = ((bc.HitsMax + bc.RawStatTotal) / 40) + ((bc.DamageMax * bc.DamageMin) / 30);

            if (bc is BaseVendor && !(m_ItemxmlSys.ShowVendorLevels))
                return 0;
            else
            {
                if (Lvl < 1)
                    return 1;
                else if (IsParagon(bc))
                    return Lvl += Utility.RandomMinMax(3, 7);
                else
                    return Lvl;
            }
        }
		
        public static int PetLevelXML(Mobile m)
        {
			PetLevelOrb petxml = null;
			petxml = m.BankBox.FindItemByType(typeof(PetLevelOrb), false) as PetLevelOrb;

            if (petxml == null)
                return 0;
            else
            {
				int Lvl = petxml.Levell;
				
				return Lvl;
            }
        }

        public static string Display(Mobile m)
        {
			LevelControlSysItem loccontrol = null;
			foreach (Item lister in World.Items.Values)
			{
				if (lister is LevelControlSysItem) loccontrol = lister as LevelControlSysItem;
			}
			LevelControlSys m_ItemxmlSys = null;
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));
			if (m_ItemxmlSys == null)
				return null;
			
			m_ItemxmlSys = (LevelControlSys)XmlAttachExt.FindAttachment(loccontrol, typeof(LevelControlSys));

			LevelSheet xmlplayer = null;
			xmlplayer = m.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
			
            PlayerMobile pm = m as PlayerMobile;

            string dsp;

			if (xmlplayer == null)
			{
				return null;
			}
			
            if (pm.AccessLevel > AccessLevel.Player && !(m_ItemxmlSys.StaffHasLevel))
            {
                dsp = "";
            }
            else
            {
                if (m_ItemxmlSys.MaxLevel)
                {
                    dsp = "" + xmlplayer.Levell + "/" + xmlplayer.MaxLevel;
                }
                else
                {
                    dsp = "" + xmlplayer.Levell;
                }
            }

            return dsp;
        }

        public static int TExp(Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;
			
			LevelSheet xmlplayer2 = null;
			xmlplayer2 = m.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;


			return xmlplayer2.kxp + LevelCore.GetExp(pm);
        }
/*
        public static int TExpPet(BaseCreature bc)
        {
			XMLPetLevelAtt petxml = (XMLPetLevelAtt)XmlAttach.FindAttachment(bc, typeof(XMLPetLevelAtt));


			return petxml.kxp + LevelCore.GetExp(bc);

        }
*/
        public static void Taming(Mobile m)
        {
			Container petpack2 = m.Backpack;
			if (petpack2 != null)
			{
				LevelSheet xmlplayer = null;
				xmlplayer = m.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
				PlayerMobile pm = m as PlayerMobile;
				LevelHandlerExt lh = new LevelHandlerExt();

				if (xmlplayer == null)
					return;
				else
				{
					if (xmlplayer.Expp < xmlplayer.ToLevell)
					{

						 pm.SendMessage("You have gained 5 Experience for taming a wild creature");
						 xmlplayer.kxp += 8;

						 if (xmlplayer.Expp >= xmlplayer.ToLevell && xmlplayer.Levell < xmlplayer.MaxLevel)
							 LevelHandlerExt.DoLevel(pm);
					}
				}
			}
			else
				return;
        }
		
        public static void Harvest(Mobile m, Item i, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
			LevelSheet xmlplayer = null;
			xmlplayer = m.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
            PlayerMobile pm = m as PlayerMobile;
            LevelHandlerExt lh = new LevelHandlerExt();
            if (xmlplayer == null)
				return;
			else
			{
				if (xmlplayer.Expp < xmlplayer.ToLevell)
				{
					int xp = (int)Math.Round(2 + (LevelCore.HarvestExp(m, i, def, map, loc, resource)));
					int give = LevelHandlerExt.ExpFilter(pm, xp, null, true);

					if (give > 0)
					{
						pm.SendMessage("You gained {0} experience for harvesting some Resources.", give);
						xmlplayer.kxp += (int)give;
				   
					if (xmlplayer.Expp >= xmlplayer.ToLevell && xmlplayer.Levell < xmlplayer.MaxLevel)
						LevelHandlerExt.DoLevel(pm);
					}
				}
			}
        }

        public static double HarvestExp(Mobile m, Item i, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            double rtn = 0;
            PlayerMobile pm = m as PlayerMobile;

            if (resource is BaseOre)
            {
                BaseOre bo = i as BaseOre;

                if (bo.Resource == CraftResource.Iron)
                    rtn += 1;
                else if (bo.Resource == CraftResource.DullCopper)
                    rtn += 2;
                else if (bo.Resource == CraftResource.ShadowIron)
                    rtn += 3;
                else if (bo.Resource == CraftResource.Copper)
                    rtn += 4;
                else if (bo.Resource == CraftResource.Bronze)
                    rtn += 5;
                else if (bo.Resource == CraftResource.Gold)
                    rtn += 6;
                else if (bo.Resource == CraftResource.Agapite)
                    rtn += 7;
                else if (bo.Resource == CraftResource.Verite)
                    rtn += 8;
                else if (bo.Resource == CraftResource.Valorite)
                    rtn += 9;
            }
            if (i is Fish)
            {
                rtn += 0.2;
            }

            if (i is Log)
            {
                Log lo = i as Log;
                if (lo.Resource == CraftResource.RegularWood)
                     rtn += 1;/*
                else if (lo.Resource == CraftResource.OakWood)
                    rtn += 2;
                else if (lo.Resource == CraftResource.AshWood)
                    rtn += 3;
                else if (lo.Resource == CraftResource.YewWood)
                    rtn += 4;
                else if (lo.Resource == CraftResource.Heartwood)
                    rtn += 5;
                else if (lo.Resource == CraftResource.Bloodwood)
                    rtn += 6;
                else if (lo.Resource == CraftResource.Frostwood)
                    rtn += 7;*/

            }
            return (rtn);
        }

		/* Keeping intact for future integration for improving new system. */
        public static void Craft(Item i, int q, double ch, double e, Mobile m)
        {
			LevelSheet xmlplayer = null;
			xmlplayer = m.Backpack.FindItemByType(typeof(LevelSheet), false) as LevelSheet;
            PlayerMobile pm = m as PlayerMobile;
            LevelHandlerExt lh = new LevelHandlerExt();

            if (xmlplayer.Expp < xmlplayer.ToLevell)
            {
                int xp = (int)Math.Round(3.6 + (LevelCore.CraftExp(i, q, ch, e, pm)));
                int give = LevelHandlerExt.ExpFilter(pm, xp, null, true);

                if (give > 0)
                {
                    pm.SendMessage("You gained {0} experience for crafting an item.", give);
                    xmlplayer.kxp += (int)give;


                    if (xmlplayer.Expp >= xmlplayer.ToLevell && xmlplayer.Levell < xmlplayer.MaxLevel)
                        LevelHandlerExt.DoLevel(pm);
                }
            }
        }

        public static double CraftExp(Item i, int q, double c, double e, Mobile m)
        {
            PlayerMobile pm = m as PlayerMobile;

            double rtn = 0;

            if (i is BaseWeapon)
            {
                #region Weapon Resource Settings

                BaseWeapon bw = i as BaseWeapon;

                if (bw.Resource == CraftResource.Iron)
                    rtn += 0.2;
                else if (bw.Resource == CraftResource.DullCopper)
                    rtn += 0.4;
                else if (bw.Resource == CraftResource.ShadowIron)
                    rtn += 0.8;
                else if (bw.Resource == CraftResource.Copper)
                    rtn += 1.6;
                else if (bw.Resource == CraftResource.Bronze)
                    rtn += 1.8;
                else if (bw.Resource == CraftResource.Gold)
                    rtn += 2.2;
                else if (bw.Resource == CraftResource.Agapite)
                    rtn += 3.0;
                else if (bw.Resource == CraftResource.Verite)
                    rtn += 4.6;
                else if (bw.Resource == CraftResource.Valorite)
                    rtn += 4.8;
                else if (bw.Resource == CraftResource.RegularWood)
                    rtn += 0.2;/*
                else if (bw.Resource == CraftResource.OakWood)
                    rtn += 0.4;
                else if (bw.Resource == CraftResource.AshWood)
                    rtn += 0.8;
                else if (bw.Resource == CraftResource.YewWood)
                    rtn += 1.4;
                else if (bw.Resource == CraftResource.Heartwood)
                    rtn += 2.0;
                else if (bw.Resource == CraftResource.Bloodwood)
                    rtn += 3.8;
                else if (bw.Resource == CraftResource.Frostwood)
                    rtn += 4.8;*/


                #endregion
            }
            else if (i is BaseArmor)
            {
                #region Armour Resource Settings

                BaseArmor ba = i as BaseArmor;

                #region Armour Type (Ring/Chain/Plate)

                foreach (Type t in armours.Ringmail)
                {
                    if (t == ba.GetType())
                        rtn += 0.6;
                }

                foreach (Type t in armours.Chainmail)
                {
                    if (t == ba.GetType())
                        rtn += 1.3;
                }

                foreach (Type t in armours.Platemail)
                {
                    if (t == ba.GetType())
                        rtn += 2.1;
                }

                #endregion

                #region Metals

                if (ba.Resource == CraftResource.Iron)
                    rtn += 0.2;
                else if (ba.Resource == CraftResource.DullCopper)
                    rtn += 0.4;
                else if (ba.Resource == CraftResource.ShadowIron)
                    rtn += 0.8;
                else if (ba.Resource == CraftResource.Copper)
                    rtn += 1.6;
                else if (ba.Resource == CraftResource.Bronze)
                    rtn += 1.8;
                else if (ba.Resource == CraftResource.Gold)
                    rtn += 2.2;
                else if (ba.Resource == CraftResource.Agapite)
                    rtn += 3.0;
                else if (ba.Resource == CraftResource.Verite)
                    rtn += 4.6;
                else if (ba.Resource == CraftResource.Valorite)
                    rtn += 4.8;

                #endregion

                #region Wood
                else if (ba.Resource == CraftResource.RegularWood)
                    rtn += 0.2;/*
                else if (ba.Resource == CraftResource.OakWood)
                    rtn += 0.4;
                else if (ba.Resource == CraftResource.AshWood)
                    rtn += 0.8;
                else if (ba.Resource == CraftResource.YewWood)
                    rtn += 1.4;
                else if (ba.Resource == CraftResource.Heartwood)
                    rtn += 2.0;
                else if (ba.Resource == CraftResource.Bloodwood)
                    rtn += 3.8;
                else if (ba.Resource == CraftResource.Frostwood)
                    rtn += 4.8;*/
                #endregion

                #region Leathers

                else if (ba.Resource == CraftResource.RegularLeather)
                    rtn += 0.8;
                else if (ba.Resource == CraftResource.SpinedLeather)
                    rtn += 1.6;
                else if (ba.Resource == CraftResource.HornedLeather)
                    rtn += 2.6;
                else if (ba.Resource == CraftResource.BarbedLeather)
                    rtn += 3.8;

                #endregion

                #region Scales 
                else if (ba.Resource == CraftResource.RedScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.YellowScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.GreenScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.BlueScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.WhiteScales)
                    rtn += 1.0;
                else if (ba.Resource == CraftResource.BlackScales)
                    rtn += 1.0;


                #endregion

                #endregion
            }
           

            double chance = 100 * c;

            if (chance > 0 && chance < 20)
                rtn += 6.0;
            else if (chance > 20 && chance < 40)
                rtn += 4.4;
            else if (chance > 40 && chance < 60)
                rtn += 3.2;
            else if (chance > 60 && chance < 80)
                rtn += 2.0;
            else
                rtn += 0.3;

            if (q > 1)
                rtn += 3.2;

            return (rtn - e);
        }
    }
}
