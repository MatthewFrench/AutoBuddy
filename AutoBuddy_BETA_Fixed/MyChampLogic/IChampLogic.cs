using AutoBuddy.MainLogics;
using EloBuddy;

namespace AutoBuddy.MyChampLogic
{
    internal interface IChampLogic
    {
        int[] SkillSequence { get; }
        float MaxDistanceForAA { get; }
        float OptimalMaxComboDistance { get; }
        float HarassDistance { get; }
        LogicSelector Logic { set; }
        string ShopSequence { get; set; }
        void Harass(AIHeroClient target);
        void Survi();
        void Combo(AIHeroClient target);
        void UnkillableMinion(Obj_AI_Base target, float remainingHealth);
    }
}