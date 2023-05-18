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
        string firstName = firstNames[random.Next(0, firstNames.Length)];
        string lastName = firstNames[random.Next(0, firstNames.Length)];

        return $"{firstName} {lastName}";
    }

    public override EmployeeConfig GetEmployeeConfig()
    {
        List<float> listWeights = professionWeights.Select(x => x.Weight).ToList();
        Profession profession = professionWeights[RandomTools.RandomlyChooseWithWeights(listWeights)].Profession;
        List<Peculiarity> peculiarities = new();
        listWeights = peculiarityWeights.Select(x => x.Weight).ToList();
        for (int i = 0; i < maxPeculiarityCount; i++)
        {
            if (peculiarities.Count == peculiarityWeights.Count)
            {
                break;
            }
            int choosedInt = RandomTools.RandomlyChooseWithWeights(listWeights);
            Peculiarity choosedPercularity = peculiarityWeights[choosedInt].Peculiarity;
            peculiarities.Add(choosedPercularity);
            _ = peculiarityWeights.Remove(peculiarityWeights[choosedInt]);
        }
        return new EmployeeConfig(GenerateName(), profession, peculiarities);
    }
}
