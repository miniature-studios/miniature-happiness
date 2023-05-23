using System.Collections.Generic;
using System.Linq;

namespace Common
{
    public static class RandomTools
    {
        // Выбирает случайный индекс от 0 до размера входного массива, с учетом весов на местах индексов
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