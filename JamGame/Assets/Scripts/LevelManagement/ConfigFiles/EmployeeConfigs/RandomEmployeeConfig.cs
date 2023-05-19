using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class ProfessionWeight
{
    public Profession Profession;
    [Range(0, 1)] public float Weight;
}

[Serializable]
public class PeculiarityWeight
{
    public Peculiarity Peculiarity;
    [Range(0, 1)] public float Weight;
}

[Serializable]
[CreateAssetMenu(fileName = "RandomEmployeeConfig", menuName = "Level/Employee/RandomEmployeeConfig", order = 1)]
public class RandomEmployeeConfig : AbstractEmployeeConfig
{
    [SerializeField] private List<ProfessionWeight> professionWeights;
    [SerializeField][Min(0)] private int maxPeculiarityCount;
    [SerializeField] private List<PeculiarityWeight> peculiarityWeights;

    private static readonly string[] firstNames = { "John", "Paul", "Ringo", "George" };
    private static readonly string[] lastNames = { "Lennon", "McCartney", "Starr", "Harrison" };

    public static string GenerateName()
    {
        System.Random random = new();
        string first_name = firstNames[random.Next(0, firstNames.Length)];
        string last_name = lastNames[random.Next(0, lastNames.Length)];

        return $"{first_name} {last_name}";
    }

    public override EmployeeConfig GetEmployeeConfig()
    {
        List<float> list_weights = professionWeights.Select(x => x.Weight).ToList();
        Profession profession = professionWeights[RandomTools.RandomlyChooseWithWeights(list_weights)].Profession;
        List<Peculiarity> peculiarities = new();
        list_weights = peculiarityWeights.Select(x => x.Weight).ToList();
        for (int i = 0; i < maxPeculiarityCount; i++)
        {
            if (peculiarities.Count == peculiarityWeights.Count)
            {
                break;
            }
            int choosed_int = RandomTools.RandomlyChooseWithWeights(list_weights);
            Peculiarity choosed_percularity = peculiarityWeights[choosed_int].Peculiarity;
            peculiarities.Add(choosed_percularity);
            _ = peculiarityWeights.Remove(peculiarityWeights[choosed_int]);
        }
        return new EmployeeConfig(GenerateName(), profession, peculiarities);
    }
}
