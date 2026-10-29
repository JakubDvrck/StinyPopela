using System;
using UnityEngine;

namespace StinyPopela
{
    [Serializable]
    public sealed class CharacterProgress
    {
        public int level = 1;
        public int experience;
        public int strength = 5, dexterity = 5, intelligence = 5, vitality = 5;
        public int attributePoints, skillPoints;
        public int RequiredExperience => level * 40;
        public int MaxHealth => 50 + vitality * 10;

        public void GainExperience(int amount)
        {
            experience += Mathf.Max(0, amount);
            while (experience >= RequiredExperience)
            {
                experience -= RequiredExperience;
                level++;
                attributePoints += 5;
                skillPoints++;
            }
        }

        public bool SpendAttribute(int index)
        {
            if (attributePoints <= 0 || index < 0 || index > 3) return false;
            switch (index)
            {
                case 0: strength++; break;
                case 1: dexterity++; break;
                case 2: intelligence++; break;
                case 3: vitality++; break;
            }
            attributePoints--;
            return true;
        }
    }
}
