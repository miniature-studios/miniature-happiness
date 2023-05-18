using System.Collections.Generic;

public static class RandomTools
{
    public static int RandomlyChooseWithWeights(List<float> weights)
    {
        float sum_weights = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            sum_weights += weights[i];
        }
        float random = UnityEngine.Random.Range(0, sum_weights);
        for (int i = 0; i < weights.Count; i++)
        {
            if (weights[i] <= random)
            {
                return i;
            }
            else
            {
                random -= weights[i];
            }
        }
        return 0;
    }
}