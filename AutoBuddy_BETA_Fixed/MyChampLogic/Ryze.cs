using AutoBuddy.MainLogics;
using EloBuddy;
using EloBuddy.SDK;

namespace AutoBuddy.MyChampLogic
{
    internal class Ryze : IChampLogic
    {

        public float MaxDistanceForAA { get { return int.MaxValue; } }
        public float OptimalMaxComboDistance { get { return AutoWalker.p.AttackRange; } }
        public float HarassDistance { get { return AutoWalker.p.AttackRange; } }

        public Spell.Active Q;
        public Spell.Skillshot W, E, R;

        public Ryze()
        {
            SkillSequence = new[] {2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3};
            ShopSequence =
                "3340:Buy,2003:StartHpPot,1056:Buy,3070:Buy,1001:Buy,1058:Buy,3003:Buy,3020:Buy,2003:StopHpPot,3010:Buy,1026:Buy,3027:Buy,1058:Buy,3191:Buy,3157:Buy,1058:Buy,1056:Sell,1011:Buy,3116:Buy,3136:Buy,3151:Buy";
        }

        public int[] SkillSequence { get; private set; }
        public LogicSelector Logic { get; set; }


        public string ShopSequence { get; set; }

        public void Harass(AIHeroClient target)
        {
        }

        public void Survi()
        {
        }

        public void Combo(AIHeroClient target)
        {
        }

        public void UnkillableMinion(Obj_AI_Base target, float remainingHealth)
        {

        }
    }
}