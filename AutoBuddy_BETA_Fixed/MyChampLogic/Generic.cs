using AutoBuddy.MainLogics;
using EloBuddy;

namespace AutoBuddy.MyChampLogic
{
    internal class Generic : IChampLogic
    {
        public float MaxDistanceForAA => AutoWalker.p.AttackRange;
        public float OptimalMaxComboDistance => AutoWalker.p.AttackRange;
        public float HarassDistance => AutoWalker.p.AttackRange;

        public Generic()
        {
            SkillSequence = new[] {2, 1, 3, 2, 2, 4, 2, 1, 2, 1, 4, 1, 1, 3, 3, 4, 3, 3};
            /*
            ShopSequence =
                "3340:Buy," + //"Warding Totem (Trinket)"
                "1036:Buy," + //"Long Sword"
                "2003:StartHpPot," + //"Health Potion"
                "1053:Buy," + //"Vampiric Scepter"
                "1042:Buy," + //"Dagger"
                "1001:Buy," + //"Boots of Speed"
                "3006:Buy," + //"Berserker's Greaves"
                "1036:Buy," + //"Long Sword"
                "1038:Buy," + //"B. F. Sword"
                "3072:Buy," + //"The Bloodthirster"
                "2003:StopHpPot," + //"Health Potion"
                "1042:Buy," + //"Dagger"
                "1051:Buy," + //"Brawler's Gloves"
                "3086:Buy," + //"Zeal"
                "1042:Buy," + //"Dagger"
                "1042:Buy," + //"Dagger"
                "1043:Buy," + //"Recurve Bow"
                "3085:Buy," + //"Runaan's Hurricane"
                "2015:Buy," + //"Kircheis Shard"
                "3086:Buy," + //"Zeal"
                "3094:Buy," + //"Rapid Firecannon"
                "1018:Buy," + //"Cloak of Agility"
                "1038:Buy," + //"B. F. Sword"
                "3031:Buy," + //"Infinity Edge"
                "1037:Buy," + //"Pickaxe"
                "3035:Buy," + //"Last Whisper"
                "3033:Buy"; //"Mortal Reminder"
                */
            ShopSequence =
            "3340:Buy," + //"Warding Totem (Trinket)"
            "1036:Buy," + //"Long Sword"
            "2003:StartHpPot," + //"Health Potion"

            "1004:Buy," + //Faerie Charm
            "1027:Buy," + //Sapphire Crystal
            "3070:Buy," + //Tear of the Goddess
            "1037:Buy," + //Pickaxe
            "3004:Buy," + //Manamune

            "2003:StopHpPot," + //"Health Potion"

            "1082:Buy," + //The dark seal
            "3041:Buy," + // Mejai's Soulstealer


            "1052:Buy," + //Amplifying Tome
            "3108:Buy," + //Fiendish Codex
            "1004:Buy," + //Faerie Charm
            "1004:Buy," + //Faerie Charm
            "1033:Buy," + //Null-Magic Mantle
            "3028:Buy," + //Chalice of Harmony
            "3174:Buy," + // Athene's Unholy Grail


            "1036:Buy," + //Long Sword
            "1053:Buy," + //Vamperic Sceptor
            "1036:Buy," + //Long Sword
            "3144:Buy," + //Bilgewater Cutlass
            "1052:Buy," + //Amplifying Tome
            "1052:Buy," + //Amplifying Tome
            "3145:Buy," + //Hextech Revolver
            "3146:Buy," + // Hextech Gunblade


            "1033:Buy," + //Null Magic Mantle
            "1029:Buy," + //Cloth Armor
            "3105:Buy," + //Aegis of the Legion
            "1027:Buy," + //Sapphire Crystal
            "3024:Buy," + //Glacial Shroud
            "3060:Buy," + // Banner of Command


            "1029:Buy," + //Cloth Armor
            "1031:Buy," + //Chain Vest
            "3075:Buy"; //Thornmail
        }

        public int[] SkillSequence { get; private set; }
        public LogicSelector Logic { get; set; }


        public string ShopSequence { get; set; }

        public void SetShopSequence(string sequence)
        {
            ShopSequence = sequence;
        }

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