using System;
using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;

namespace AutoBuddy.MyChampLogic
{
    internal class Anivia : IChampLogic
    {
        public float MaxDistanceForAA => AutoWalker.p.AttackRange;
        public float OptimalMaxComboDistance => AutoWalker.p.AttackRange;
        public float HarassDistance => AutoWalker.p.AttackRange;
        public int[] SkillSequence => new[] {2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3};
        public LogicSelector Logic { get; set; }
        public string ShopSequence { get; set; }

        private static SpellData GetSData(SpellSlot slot)
        {
            return Player.GetSpell(slot).SData;
        }
        public Spell.Skillshot _Q = new Spell.Skillshot(SpellSlot.Q, (uint)GetSData(SpellSlot.Q).CastRangeDisplayOverride, SkillShotType.Linear, (int)GetSData(SpellSlot.Q).CastTime, (int) GetSData(SpellSlot.Q).MissileSpeed, (int)GetSData(SpellSlot.Q).LineWidth, DamageType.Magical)
        {
            AllowedCollisionCount = -1
        };
        public Spell.Active _W = new Spell.Active(SpellSlot.W, (uint)GetSData(SpellSlot.W).CastRangeDisplayOverride);
        public Spell.Targeted _E = new Spell.Targeted(SpellSlot.E, (uint)GetSData(SpellSlot.E).CastRangeDisplayOverride, DamageType.Magical);
        public Spell.Active _R = new Spell.Active(SpellSlot.R, (uint)GetSData(SpellSlot.R).CastRangeDisplayOverride, DamageType.Magical);

        public float QDamage => new[] {60, 85, 110, 135, 160}[_Q.Level - 1] + Player.Instance.TotalMagicalDamage * 0.4f;
        public float WDamage => 1;
        public float EDamage => new[] {50, 75, 100, 125, 150}[_E.Level - 1] + Player.Instance.TotalMagicalDamage * 0.5f;
        public float EEmpoweredDamage => EDamage * 2;
        public float RDamage => new[] {40, 60, 80}[_R.Level - 1] + Player.Instance.TotalMagicalDamage * 0.125f;
        public float REmpoweredDamage => RDamage * 3;

        public Anivia()
        {
            ShopSequence = "3340:Buy,2003:StartHpPot,1056:Buy,3010:Buy,1026:Buy,3027:Buy,1058:Buy,3020:Buy,3191:Buy,3157:Buy,1058:Buy,2003:StopHpPot,1011:Buy,3116:Buy,1056:Sell,1058:Buy,1026:Buy,3089:Buy,3136:Buy,3151:Buy";

            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            Game.OnTick += Game_OnTick;
        }

        public bool UltIsActive => Player.HasBuff("GlacialStorm");

        private void Game_OnTick(EventArgs args)
        {
            if (Player.Instance.ManaPercent < 50 && UltIsActive)
            {
                _R.Cast();
            }
            if (QMissile != null && EntityManager.Heroes.Enemies.Any(x => x.IsInRange(QMissile, 225)) && _Q.State == SpellState.Ready)
            {
                _Q.Cast();
            }
        }

        private GameObject QMissile;
        private void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Anivia_Base_Q_AOE_Mis.troy")
            {
                QMissile = null;
            }
        }

        private void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Anivia_Base_Q_AOE_Mis.troy")
            {
                QMissile = sender;
            }
        }

        public void Harass(AIHeroClient target)
        {
            if (_E.State == SpellState.Ready && _E.CanCast(target))
            {
                _E.Cast(target);
            }
            else if (_Q.State == SpellState.Ready && _Q.IsInRange(target))
            {
                _Q.CastMinimumHitchance(target, HitChance.High);
            }
        }

        public void Survi()
        {
            var nearestTarget = EntityManager.Heroes.Enemies.OrderBy(x => x.Distance(Player.Instance)).FirstOrDefault();
            if (nearestTarget == null || nearestTarget.Distance(Player.Instance) > GetSData(SpellSlot.R).CastRangeDisplayOverride)
            {
                return;
            }
            if (!UltIsActive && _R.State == SpellState.Ready && Player.Instance.ManaPercent >= 50 && nearestTarget != null && nearestTarget.Distance(Player.Instance) < GetSData(SpellSlot.R).CastRangeDisplayOverride)
            {
                _R.Cast(nearestTarget.ServerPosition);
            }
            else if (_W.State == SpellState.Ready && _W.IsInRange(nearestTarget))
            {
                _W.Cast(nearestTarget.ServerPosition.Extend(Player.Instance, nearestTarget.BoundingRadius + 50).To3DWorld());
            }
        }

        public void Combo(AIHeroClient target)
        {
            if (target.IsChilled())
            {
                if (_E.State == SpellState.Ready && _E.IsInRange(target))
                {
                    _E.Cast(target);
                }
            }
            else
            {
                if (_Q.State == SpellState.Ready && _Q.IsInRange(target))
                {
                    _Q.CastMinimumHitchance(target, HitChance.High);
                }
                else if (!UltIsActive && _R.State == SpellState.Ready && _R.IsInRange(target))
                {
                    _R.Cast(target.ServerPosition);
                }
            }
        }

        public void UnkillableMinion(Obj_AI_Base target, float remainingHealth)
        {
            if (remainingHealth < (target.IsChilled() ? EEmpoweredDamage : EDamage) && _E.IsInRange(target))
            {
                _E.Cast(target);
            }
        }
    }
}