using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;

namespace AutoBuddy.MyChampLogic
{
    internal class Ahri : IChampLogic
    {
        public float MaxDistanceForAA => AutoWalker.p.AttackRange;
        public float OptimalMaxComboDistance => AutoWalker.p.AttackRange;
        public float HarassDistance => AutoWalker.p.AttackRange;
        public int[] SkillSequence => new[] { 2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3 };
        public LogicSelector Logic { get; set; }
        public string ShopSequence { get; set; }
        private static SpellData GetSpellData(SpellSlot slot)
        {
            return AutoWalker.p.Spellbook.GetSpell(slot).SData;
        }
        public Spell.Skillshot _Q = new Spell.Skillshot(SpellSlot.Q, (uint)GetSpellData(SpellSlot.Q).CastRangeDisplayOverride, SkillShotType.Linear, (int)GetSpellData(SpellSlot.Q).CastTime, (int)GetSpellData(SpellSlot.Q).MissileSpeed, (int)GetSpellData(SpellSlot.Q).LineWidth, DamageType.Magical)
        {
            AllowedCollisionCount = -1
        };
        public Spell.Active _W = new Spell.Active(SpellSlot.W, (uint)GetSpellData(SpellSlot.W).CastRangeDisplayOverride, DamageType.Magical);
        public Spell.Skillshot _E = new Spell.Skillshot(SpellSlot.E, (uint)GetSpellData(SpellSlot.E).CastRangeDisplayOverride, SkillShotType.Linear, (int)GetSpellData(SpellSlot.E).CastTime, (int)GetSpellData(SpellSlot.E).MissileSpeed, (int)GetSpellData(SpellSlot.E).LineWidth, DamageType.Magical)
        {
            AllowedCollisionCount = 0
        };
        public Spell.SimpleSkillshot _R = new Spell.SimpleSkillshot(SpellSlot.R, (uint)GetSpellData(SpellSlot.R).CastRangeDisplayOverride, DamageType.Magical);

        public float QDamage => new[] {40, 65, 90, 115, 140}[_Q.Level - 1] + Player.Instance.TotalMagicalDamage * .35f;
        public float WDamagePerHit => new[] {40, 65, 90, 115, 140}[_W.Level - 1] + Player.Instance.TotalMagicalDamage * .4f;
        public float TotalWDamage => WDamagePerHit + WDamagePerHit * .3f + WDamagePerHit * .3f;
        public float EDamage => new[] {60, 95, 130, 165, 200}[_E.Level - 1] + Player.Instance.TotalMagicalDamage * .5f;
        public float RDamagePerHit => new[] {70, 110, 150}[_R.Level - 1] + Player.Instance.TotalMagicalDamage * .3f;
        public float TotalRDamage => RDamagePerHit * 3;

        public Ahri()
        {
             ShopSequence = "3340:Buy,1056:Buy,2003:StartHpPot,1052:Buy,1033:Buy,1057:Buy,1052:Buy,3108:Buy,3001:Buy,1001:Buy,3020:Buy,1052:Buy,2003:StopHpPot,1052:Buy,3108:Buy,1052:Buy,3802:Buy,3165:Buy,1026:Buy,1052:Buy,3135:Buy,1058:Buy,1056:Sell,1026:Buy,3089:Buy,1052:Buy,3113:Buy,3285:Buy";

        Gapcloser.OnGapcloser += (sender, args) =>
            {
                if (_R.State == SpellState.Ready && args.Type != Gapcloser.GapcloserType.Targeted)
                {
                    _R.Cast(AutoWalker.p.GetNearestTurret(false).ServerPosition);
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
            var chaser = EntityManager.Heroes.Enemies.OrderBy(x => x.Distance(AutoWalker.p)).FirstOrDefault();
            if (chaser != null && _E.State == SpellState.Ready)
            {
                _E.CastMinimumHitchance(chaser, HitChance.High);
                return;
            }
            if (_R.State == SpellState.Ready && Program.Logic.localAwareness.LocalDomination(AutoWalker.p) < 0)
            {
                _R.Cast(AutoWalker.p.GetNearestTurret(false).ServerPosition);
            }
        }

        public void Combo(AIHeroClient target)
        {
            if (_E.State == SpellState.Ready && _Q.State == SpellState.Ready)
            {
                _E.CastMinimumHitchance(target, HitChance.High);
                Core.DelayAction(() =>
                {
                    if (target.IsDead || target.IsZombie || target.IsInvulnerable || target.IsTargetable) return;
                    _Q.CastMinimumHitchance(target, HitChance.High);
                }, 400);
                Core.DelayAction(() =>
                {
                    if (_W.State == SpellState.Ready)
                    {
                        _W.Cast();
                    }
                }, 400 + (int)GetSpellData(SpellSlot.Q).CastTime + 20);
            }
        }

        public void UnkillableMinion(Obj_AI_Base target, float remainingHealth)
        {

        }
    }
}