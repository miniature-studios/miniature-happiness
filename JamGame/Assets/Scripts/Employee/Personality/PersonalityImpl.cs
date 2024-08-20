using System;
using System.Collections.Generic;
using Pickle;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Employee.Personality
{
    public interface IConfig
    {
        public PersonalityImpl GetPersonality();
    }

    public class PersonalityImpl
    {
        private string name;
        private int hireCost;
        private string profession;
        private List<Quirk> quirks;

        public string Name => name;
        public int HireCost => hireCost;
        public string Profession => profession;
        public List<Quirk> Quirks => quirks;

        public PersonalityImpl(string name, int hireCost, string profession, List<Quirk> quirks)
        {
            this.name = name;
            this.hireCost = hireCost;
            this.profession = profession;
            this.quirks = quirks;
        }
    }

    [Serializable]
    public class FixedConfig : IConfig
    {
        [SerializeField]
        [FoldoutGroup("@" + nameof(Label))]
        private string name;

        [SerializeField]
        [FoldoutGroup("@" + nameof(Label))]
        private int hireCost;

        [SerializeField]
        [FoldoutGroup("@" + nameof(Label))]
        private string profession;

        [SerializeField]
        [FoldoutGroup("@" + nameof(Label))]
        private List<Quirk> quirks;

        private string Label => $"Employee - {name}";

        public PersonalityImpl GetPersonality()
        {
            return new PersonalityImpl(name, hireCost, profession, quirks);
        }
    }

    [Serializable]
    public class RandomConfig : IConfig
    {
        [Serializable]
        private struct CostRange
        {
            public int Min;
            public int Max;
            public int Multiply;

            public int GenerateCost()
            {
                return UnityEngine.Random.Range(Min, Max) * Multiply;
            }
        }

        [Serializable]
        private struct QuirkList
        {
            public float QuirkChance;
            public List<Quirk> Quirks;

            public List<Quirk> GenerateQuirks()
            {
                List<Quirk> quirks = new();
                foreach (Quirk quirk in Quirks)
                {
                    if (UnityEngine.Random.Range(0f, 1f) <= QuirkChance)
                    {
                        quirks.Add(quirk);
                    }
                }
                return quirks;
            }
        }

        [SerializeField]
        [Pickle(typeof(NameList), LookupType = ObjectProviderType.Assets)]
        [FoldoutGroup("Employee - Random")]
        private NameList nameList;

        [SerializeField]
        [FoldoutGroup("Employee - Random")]
        private CostRange costRange;

        [SerializeField]
        [FoldoutGroup("Employee - Random")]
        private QuirkList quirkList;

        public PersonalityImpl GetPersonality()
        {
            // TODO: #48
            return new PersonalityImpl(
                nameList.GenerateName(),
                costRange.GenerateCost(),
                "Programmer",
                quirkList.GenerateQuirks()
            );
        }
    }
}
