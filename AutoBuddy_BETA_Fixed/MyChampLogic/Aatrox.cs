using System;
using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace AutoBuddy.MyChampLogic
{
    class Aatrox : IChampLogic
    {
        public int[] SkillSequence => new[] {3, 2, 3, 1, 3, 4, 3, 2, 3, 2, 4, 2, 2, 1, 1, 4, 1, 1};
        public float MaxDistanceForAA => Player.Instance.AttackRange;
        public float OptimalMaxComboDistance => Player.GetSpell(SpellSlot.Q).SData.CastRangeDisplayOverride;
        public float HarassDistance => Player.GetSpell(SpellSlot.E).SData.CastRangeDisplayOverride;
        public LogicSelector Logic { get; set; }
        public string ShopSequence { get; set; }
        public int QDamage => (int)(new[] {10,35,60,95,120}[Player.GetSpell(SpellSlot.Q).Level + 1] + Player.Instance.TotalAttackDamage * 1.1f);
        public int EDamage => (int)(new[] {70,110,150,190,230}[Player.GetSpell(SpellSlot.E).Level + 1] + (Player.Instance.TotalAttackDamage - Player.Instance.BaseAttackDamage) * 0.7f);
        public int RDamage => (int)(new[] {200,300,400}[Player.GetSpell(SpellSlot.R).Level + 1] + Player.Instance.TotalMagicalDamage);
        
        private static SpellData GetSpellData(SpellSlot slot)
        {
            return AutoWalker.p.Spellbook.GetSpell(slot).SData;
        }
        public Spell.Skillshot _Q = new Spell.Skillshot(SpellSlot.Q, (uint)GetSpellData(SpellSlot.Q).CastRangeDisplayOverride, SkillShotType.Linear, (int)GetSpellData(SpellSlot.Q).CastTime, (int)GetSpellData(SpellSlot.Q).MissileSpeed, (int)GetSpellData(SpellSlot.Q).LineWidth, DamageType.Physical);
        public Spell.Active _W = new Spell.Active(SpellSlot.W, (uint)GetSpellData(SpellSlot.W).CastRangeDisplayOverride, DamageType.Physical);
        public Spell.Skillshot _E = new Spell.Skillshot(SpellSlot.E, (uint)GetSpellData(SpellSlot.E).CastRangeDisplayOverride, SkillShotType.Linear, (int)GetSpellData(SpellSlot.E).CastTime, (int)GetSpellData(SpellSlot.E).MissileSpeed, (int)GetSpellData(SpellSlot.E).LineWidth, DamageType.Physical);
        public Spell.Active _R = new Spell.Active(SpellSlot.R, (uint)GetSpellData(SpellSlot.R).CastRangeDisplayOverride, DamageType.Magical);

        public Aatrox()
        {
            ShopSequence = "1055:Buy,2003:StartHpPot,3340:Buy,1006:Buy,1036:Buy,1036:Buy,3077:Buy,1036:Buy,1053:Buy,1037:Buy,3074:Buy,1036:Buy,1053:Buy,1036:Buy,3144:Buy,1042:Buy,1042:Buy,1043:Buy,3153:Buy,1029:Buy,1029:Buy,3082:Buy,1028:Buy,1011:Buy,3143:Buy,1033:Buy,2003:StopHpPot,1028:Buy,3211:Buy,1028:Buy,3067:Buy,3065:Buy,1029:Buy,1031:Buy,1055:Sell,1033:Buy,1057:Buy,3026:Buy,1001:Buy,3111:Buy";
            Game.OnUpdate += delegate
            {
                if (Player.Instance.HealthPercent < 50 && _W.Name == "AatroxW2")
                {
                    _W.Cast();
                }
                if (Player.Instance.HealthPercent > 50 && _W.Name == "AatroxW")
                {
                    _W.Cast();
                }
            };
        }

        public void Harass(AIHeroClient target)
        {
            if (_E.State == SpellState.Ready)
            {
                _E.CastMinimumHitchance(target, HitChance.High);
            }
        }

        public void Survi()
        {
            var closestHero = EntityManager.Heroes.Enemies.OrderBy(x => x.Distance(Player.Instance)).FirstOrDefault();
            if (closestHero != null && _E.State == SpellState.Ready)
            {
                _E.CastMinimumHitchance(closestHero, HitChance.High);
                return;
            }
            if (_Q.State == SpellState.Ready)
            {
                _Q.Cast(Player.Instance.GetNearestTurret(false).ServerPosition);
            }
        }

        public void Combo(AIHeroClient target)
        {
            if (_R.State == SpellState.Ready && Player.Instance.CountEnemyChampionsInRange(_R.Range) >= 2)
            {
                _R.Cast();
                return;
            }
            if (_E.State == SpellState.Ready)
            {
                _E.CastMinimumHitchance(target, HitChance.High);
                return;
            }
            if (_Q.State == SpellState.Ready)
            {
                _Q.CastMinimumHitchance(target, HitChance.High);
            }
        }

        public void UnkillableMinion(Obj_AI_Base target, float remainingHealth)
        {
            if (!Orbwalker.ModeIsActive(Orbwalker.ActiveModes.LaneClear))
            {
                return;
            }
            if (_E.State == SpellState.Ready && Player.Instance.CalculateDamageOnUnit(target, DamageType.Physical, EDamage) < remainingHealth)
            {
                _Q.CastMinimumHitchance(target, HitChance.Medium);
            }
        }
    }
}
