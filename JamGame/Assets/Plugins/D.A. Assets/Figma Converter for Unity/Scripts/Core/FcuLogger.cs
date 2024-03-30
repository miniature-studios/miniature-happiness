using DA_Assets.Shared;
using System.Collections;

namespace DA_Assets.FCU
{
    internal class FcuLogger
    {
        public static bool WriteLogBeforeApiTimeout(ref int requestCount, ref int remainingTime, string log)
        {
            if (requestCount != 0 && requestCount % FcuConfig.Instance.ApiRequestsCountLimit == 0)
            {
                if (remainingTime > 0)
                {
                    DALogger.Log(log);
                    remainingTime -= 10;
                }
                else if (remainingTime == 0)
                {
                    DALogger.Log(log);
                }
            }

            return remainingTime > 0;
        }

        public static bool WriteLogBeforeEqual(ICollection list1, ICollection list2, FcuLocKey locKey, int count1, int count2, ref int tempCount)
        {
            if (list1.Count != list2.Count)
            {
                if (tempCount != list1.Count)
                {
                    tempCount = list1.Count;
                    DALogger.Log(locKey.Localize(count1, count2));
                }

                return true;
            }

            if (tempCount != list2.Count)
            {
                DALogger.Log(locKey.Localize(count1, count2));
            }

            return false;
        }

        public static bool WriteLogBeforeEqual(ref int count1, ref int count2, string log, ref int tempCount)
        {
            if (count1 != count2)
            {
                if (tempCount != count1)
                {
                    tempCount = count1;
                    DALogger.Log(log);
                }

                return true;
            }

            if (tempCount != count2)
            {
                DALogger.Log(log);
            }

            return false;
        }

        public static bool WriteLogBeforeEqual(ICollection list1, ICollection list2, FcuLocKey locKey, ref int tempCount)
        {
            if (list1.Count != list2.Count)
            {
                if (tempCount != list1.Count)
                {
                    tempCount = list1.Count;
                    DALogger.Log(locKey.Localize(list1.Count, list2.Count));
                }

                return true;
            }

            if (tempCount != list2.Count)
            {
                DALogger.Log(locKey.Localize(list1.Count, list2.Count));
            }

            return false;
        }
    }
}
