using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace AutoBuddy.MyChampLogic
{
    internal class Ashe : IChampLogic
    {
        public float MaxDistanceForAA => AutoWalker.p.AttackRange;
        public float OptimalMaxComboDistance => AutoWalker.p.AttackRange;
        public float HarassDistance => AutoWalker.p.AttackRange * 2;

        public Spell.Active Q;
        public Spell.Skillshot W, E, R;
        public float UltDamage;

        public Ashe()
        {
            SkillSequence = new[] {2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3};
            ShopSequence = "1055:Buy,2003:StartHpPot,3340:Buy,1051:Buy,1042:Buy,3086:Buy,1042:Buy,1042:Buy,1043:Buy,3085:Buy,1001:Buy,1042:Buy,3006:Buy,1018:Buy,1037:Buy,2003:StopHpPot,1038:Buy,3031:Buy,1036:Buy,1053:Buy,1036:Buy,1055:Sell,1038:Buy,3072:Buy,1036:Buy,1036:Buy,3133:Buy,1038:Buy,3508:Buy,1037:Buy,3035:Buy,3033:Buy";
            Q = new Spell.Active(SpellSlot.Q, 4294967295, DamageType.Physical);
            W = new Spell.Skillshot(SpellSlot.W, 1200, SkillShotType.Cone);
            E = new Spell.Skillshot(SpellSlot.E, 20000, SkillShotType.Linear);
            R = new Spell.Skillshot(SpellSlot.R, 20000, SkillShotType.Linear, 250, 1600, 130)
            {
                MinimumHitChance = HitChance.Medium,
                AllowedCollisionCount = 1
            };
            UltDamage = new[] {0f, 200f, 400f, 600f}[R.Level] + AutoWalker.p.TotalMagicalDamage;
            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(System.EventArgs args)
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (!enemy.IsHPBarRendered || enemy.IsDead || enemy.IsZombie || enemy.Distance(AutoWalker.p) > 2500) continue;

                if (AutoWalker.p.CalculateDamageOnUnit(enemy, DamageType.Magical, UltDamage) > enemy.Health)
                    R.CastMinimumHitchance(enemy, HitChance.High);
            }
        }

        public int[] SkillSequence { get; private set; }
        public LogicSelector Logic { get; set; }

        public string ShopSequence { get; set; }

        public void Harass(AIHeroClient target)
        {
            if (target.Distance(AutoWalker.p) < W.Range)
            {
                W.Cast(target);
            }
        }

        public void Survi()
        {
            if (R.IsReady() || W.IsReady() || Q.IsReady())
            {
                var chaser =
                    EntityManager.Heroes.Enemies.FirstOrDefault(
                        chase => chase.Distance(AutoWalker.p) < 600 && chase.IsVisible());
                if (chaser != null)
                {
                    if (W.IsReady())
                        W.Cast(chaser);
                    else if (Q.IsReady())
                        Q.Cast(Player.Instance);
                    else if (R.IsReady() && AutoWalker.p.HealthPercent() < 18)
                        R.Cast(chaser);
                }
            }
        }

        public void Combo(AIHeroClient target)
        {
            if (R.IsReady() && AutoWalker.p.CalculateDamageOnUnit(target, DamageType.Magical, UltDamage) > target.Health && AutoWalker.p.Distance(target) < 1600 && target.IsVisible())
                R.CastMinimumHitchance(target, 65);
            else if (AutoWalker.p.IsInAutoAttackRange(target) && Q.CanCast(AutoWalker.p))
            {
                Q.Cast(AutoWalker.p);
            }
            if (W.IsInRange(target))
            {
                W.CastMinimumHitchance(target, 65);
            }
        }

        public void UnkillableMinion(Obj_AI_Base target, float remainingHealth)
        {
            
        }
    }
}