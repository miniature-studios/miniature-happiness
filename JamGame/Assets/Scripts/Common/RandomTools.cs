using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class RandomTools
    {
        // Select weighted random numbers in range [0..weights.Length]
        // Probability of selecting i is weights[i] / sum(weights)
        public static int RandomlyChooseWithWeights(List<float> weights)
        {
            float sum_weights = weights.Sum();
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
}
