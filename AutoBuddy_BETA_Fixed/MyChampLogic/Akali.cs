using System.Linq;
using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;

namespace AutoBuddy.MyChampLogic
{
    class Akali : IChampLogic
    {
        public int[] SkillSequence => new[] {1, 2, 3, 1, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2};
        public float MaxDistanceForAA => Player.Instance.AttackRange;
        public float OptimalMaxComboDistance => Player.Instance.AttackRange;
        public float HarassDistance => Player.Instance.AttackRange;
        public LogicSelector Logic { get; set; }
        public string ShopSequence { get; set; }
        private static SpellData GetSData(SpellSlot slot) => Player.GetSpell(slot).SData;
        public Spell.SimpleSkillshot _Q = new Spell.SimpleSkillshot(SpellSlot.Q, (uint)GetSData(SpellSlot.Q).CastRangeDisplayOverride, DamageType.Magical);
        public Spell.Active _W = new Spell.Active(SpellSlot.W, (uint)GetSData(SpellSlot.W).CastRangeDisplayOverride);
        public Spell.Active _E = new Spell.Active(SpellSlot.E, (uint)GetSData(SpellSlot.E).CastRangeDisplayOverride, DamageType.Physical);
        public Spell.Targeted _R = new Spell.Targeted(SpellSlot.R, (uint)GetSData(SpellSlot.R).CastRangeDisplayOverride, DamageType.Magical);

        public float QDamage => new[] {35, 55, 75, 95, 115}[_Q.Level - 1] + Player.Instance.TotalMagicalDamage * 0.4f;
        public float QDetonationDamage => new[] {45, 70, 95, 120, 145}[_Q.Level - 1] + Player.Instance.TotalMagicalDamage * 0.5f;
        public float EDamage => new[] {70, 100, 130, 160, 190}[_E.Level - 1] + (Player.Instance.TotalAttackDamage - Player.Instance.BaseAttackDamage) * 0.8f + Player.Instance.TotalMagicalDamage * 0.6f;
        public float RDamage => new[] {50, 100, 150}[_R.Level - 1] + Player.Instance.TotalMagicalDamage * 0.35f;
        public int RStackCount => Player.Instance.GetBuffCount("AkaliShadowDance");

        public Akali()
        {
            ShopSequence = "1056:Buy,2003:StartHpPot,3340:Buy,1036:Buy,1053:Buy,1036:Buy,3144:Buy,1052:Buy,1052:Buy,3145:Buy,3146:Buy,1001:Buy,1033:Buy,3111:Buy,1052:Buy,3108:Buy,1052:Buy,3191:Buy,3157:Buy,2003:StopHpPot,1028:Buy,3067:Buy,1028:Buy,3211:Buy,3065:Buy,1056:Sell,1052:Buy,3113:Buy,1026:Buy,3100:Buy,1033:Buy,1057:Buy,3026:Buy";

            Game.OnUpdate += Game_OnUpdate;
        }

        private void Game_OnUpdate(System.EventArgs args)
        {
            var target = EntityManager.Heroes.Enemies.OrderBy(x => x.Health).ThenBy(x => x.Distance(Player.Instance)).FirstOrDefault();
            if (target == null)
            {
                return;
            }
            if (_E.State == SpellState.Ready && _E.IsInRange(target))
            {
                _E.Cast();
            }
            if (_Q.State == SpellState.Ready && _Q.IsInRange(target))
            {
                _Q.Cast(target);
            }
        }

        public void Harass(AIHeroClient target)
        {
            
        }

        public void Survi()
        {
            var chaser = EntityManager.Heroes.Enemies.OrderBy(x => x.Distance(Player.Instance)).FirstOrDefault();
            if (chaser != null || chaser.Distance(Player.Instance) > 400 || _W.State == SpellState.Ready)
            {
                var nearestTurret = Player.Instance.ServerPosition.GetNearestTurret(false);
                _W.Cast(nearestTurret.ServerPosition.Randomized());
            }
        }

        public void Combo(AIHeroClient target)
        {
            if (_Q.State == SpellState.Ready && _R.State == SpellState.Ready && _Q.IsInRange(target) && _R.IsInRange(target))
            {
                _Q.Cast(target);
                Core.DelayAction(() => _R.Cast(target), _Q.CastDelay);
            }
        }

        public void UnkillableMinion(Obj_AI_Base target, float remainingHealth)
        {
            if (RStackCount > 1 && remainingHealth < RDamage && _R.IsInRange(target) && _R.State == SpellState.Ready && target.Distance(Player.Instance.GetNearestTurret()) > 905)
            {
                _R.Cast(target);
            }
            else if (_E.State == SpellState.Ready && remainingHealth < EDamage)
            {
                _E.Cast();
            }
            else if (_Q.State == SpellState.Ready && remainingHealth < QDamage)
            {
                _Q.Cast(target);
            }
        }
    }
}