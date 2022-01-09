using System;
using Server.Items;

namespace Server.SkillHandlers
{
    class Meditation
    {
        public static void Initialize()
        {
            SkillInfo.Table[46].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan OnUse(Mobile m)
        {
            m.RevealingAction();

            if (m.Target != null)
            {
                m.SendLocalizedMessage(501845); // You are busy doing something else and cannot focus.

                return TimeSpan.FromSeconds(1.0);
            }

            else if (m.Mana >= m.ManaMax)
            {
                m.SendLocalizedMessage(501846); // You are at peace.
            }
        

                double skillVal = m.Skills[SkillName.Meditation].Value;
                double chance = (50.0 + ((skillVal - (m.ManaMax - m.Mana)) * 2)) / 100;

                // must bypass normal checks so passive skill checks aren't triggered
                CrystalBallOfKnowledge.TellSkillDifficultyActive(m, SkillName.Meditation, chance);

                if (chance > Utility.RandomDouble())
                {
                    m.CheckSkill(SkillName.Meditation, 0.0, 100.0);

                    m.SendLocalizedMessage(501851); // You enter a meditative trance.
                    m.Meditating = true;
                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.ActiveMeditation, 1075657));

                    if (m.Player || m.Body.IsHuman)
                        m.PlaySound(0xF9);

                    m.ResetStatTimers();
                }
                else 
                {
                    m.SendLocalizedMessage(501850); // You cannot focus your concentration.
                }

                return TimeSpan.FromSeconds(1.0);
            }
        }
    }
