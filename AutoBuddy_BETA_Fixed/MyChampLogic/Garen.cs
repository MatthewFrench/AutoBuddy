using System;
using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;

namespace AutoBuddy.MyChampLogic
{
    class Garen : IChampLogic
    {
        public int[] SkillSequence => new[] {1, 3, 2, 3, 3, 4, 3, 1, 3, 1, 4, 1, 1, 2, 2, 4, 2, 2};
        public float MaxDistanceForAA => AutoWalker.p.AttackRange;
        public float OptimalMaxComboDistance => 2000;
        public float HarassDistance => 300;
        public LogicSelector Logic { get; set; }
        public string ShopSequence { get; set; }
        private Spell.Active Q, W, E;
        private Spell.Targeted R;
        private bool QIsActive => AutoWalker.p.HasBuff("GarenQ");
        private bool EIsActive => AutoWalker.p.HasBuff("GarenE");

        public Garen()
        {
            ShopSequence = "1051:Buy,2003:StartHpPot,3340:Buy,1042:Buy,3086:Buy,1001:Buy,1036:Buy,3111:Buy,1028:Buy,3044:Buy,2003:StopHpPot,1027:Buy,3057:Buy,1042:Buy,1042:Buy,3101:Buy,3078:Buy,1042:Buy,1042:Buy,3046:Buy,1051:Buy,1042:Buy,3086:Buy,1042:Buy,2015:Buy,3087:Buy,1018:Buy,1037:Buy,3031:Buy,3026:Buy";

        Game.OnUpdate += Game_OnUpdate;
            Q = new Spell.Active(SpellSlot.Q);
            W = new Spell.Active(SpellSlot.W);
            E = new Spell.Active(SpellSlot.E, 300);
            R = new Spell.Targeted(SpellSlot.R, 400, DamageType.Magical);
        }

        private void Game_OnUpdate(EventArgs args)
        {
            var enemies = EntityManager.Heroes.Enemies.ToList();
            enemies.RemoveAll(x => x.Distance(AutoWalker.p) > E.Range * 2.5f);
            if (!enemies.Any() && EIsActive)
            {
                E.Cast(AutoWalker.p);
            }
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.Distance(AutoWalker.p) > 800)
                {
                    return;
                }
                var missingHealth = enemy.TotalMissingHealth();
                var damage = new[] {0f, 175f, 350f, 525f}[R.Level] + new[] {0f, 0.286f, 0.333f, 0.4f}[R.Level] * missingHealth;
                var predict = AutoWalker.p.CalculateDamageOnUnit(enemy, DamageType.Magical, damage);
                if (predict >= enemy.Health && R.CanCast(enemy))
                {
                    R.Cast(enemy);
                    return;
                }
            }
        }

        public void Harass(AIHeroClient target)
        {
            if (E.CanCast(AutoWalker.p) && E.IsInRange(target))
            {
                E.Cast(AutoWalker.p);
            }
        }

        public void Survi()
        {
            if (W.CanCast(AutoWalker.p))
            {
                W.Cast(AutoWalker.p);
            }
            if ((AutoWalker.p.HasBuffOfType(BuffType.Slow) || AutoWalker.p.CountEnemyHeroesInRangeWithPrediction(500) > 2) && Q.CanCast(AutoWalker.p))
            {
                Q.Cast(AutoWalker.p);
            }
        }

        public void Combo(AIHeroClient target)
        {
            if (!QIsActive && Q.CanCast(AutoWalker.p))
            {
                Q.Cast(AutoWalker.p);
            }
            if (target.Distance(AutoWalker.p) < E.Range && !EIsActive && E.CanCast(AutoWalker.p))
            {
                E.Cast(AutoWalker.p);
            }
        }

        public void UnkillableMinion(Obj_AI_Base target, float remainingHealth)
        {
            
        }
    }
}
