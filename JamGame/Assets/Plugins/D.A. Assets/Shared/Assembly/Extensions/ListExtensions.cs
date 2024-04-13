using System;
using System.Collections.Generic;
using System.Linq;

namespace DA_Assets.Shared.Extensions
{
    public static class ListExtensions
    {
        /// <summary>
        /// https://stackoverflow.com/a/55182157
        /// </summary>
        public static IEnumerable<T> SelectRecursive<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (var parent in source)
            {
                yield return parent;

                var children = selector(parent);
                foreach (var child in SelectRecursive(children, selector))
                    yield return child;
            }
        }

        public static bool TryAddValue<T>(this HashSet<T> hs, T value)
        {
            try
            {
                hs.Add(value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryAddValue<TKey, TValue>(this IDictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            try
            {
                dic.Add(key, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/110570/8265642
        /// </summary>
        public static void Shuffle<T>(this IList<T> array)
        {
            System.Random rng = new System.Random();

            int n = array.Count();

            while (n > 1)
            {
                int k = rng.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }

        public static bool ContainsAll<T>(this List<T> a, List<T> b)
        {
            return !b.Except(a).Any();
        }

        public static IEnumerable<T> GetRangeSafe<T>(this IEnumerable<T> _list, int index, int count = 0)
        {
            if (_list.IsEmpty())
                return new List<T>();

            if (_list.Count() == 1)
                return new List<T>() { _list.First() };

            if (index < 0 || count < 0)
                return new List<T>();

            if (index > _list.Count() - 1)
                return new List<T>();

            if (index <= _list.Count() - 1 && (_list.Count() - 1 < index + count) || count == 0)
                return _list.ToList().GetRange(index, _list.Count() - index);

            try
            {
                return _list.ToList().GetRange(index, count);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// https://stackoverflow.com/a/6488922
        /// </summary>
        public static IEnumerable<T2> GetDuplicates<T1, T2>(this Dictionary<T1, List<T2>> data)
        {
            // find duplicates
            var dupes = new HashSet<T2>(
                            from list1 in data.Values
                            from list2 in data.Values
                            where list1 != list2
                            from item in list1.Intersect(list2)
                            select item);

            // return a list of the duplicates
            return dupes;
        }

        public static IEnumerable<TSource> Exclude<TSource, TKey>(this IEnumerable<TSource> source,
            IEnumerable<TSource> exclude, Func<TSource, TKey> keySelector)
        {
            var excludedSet = new HashSet<TKey>(exclude.Select(keySelector));
            return source.Where(item => !excludedSet.Contains(keySelector(item)));
        }

        /// <summary>
        /// https://stackoverflow.com/a/36856433
        /// </summary>
        public static bool ContainsAll<T>(this IEnumerable<T> source, IEnumerable<T> values)
        {
            return values.All(value => source.Contains(value));
        }

        /// <summary>
        /// The method checks if collection1 contains at least one element of collection2.
        /// </summary>
        public static bool Contains<T>(this IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            foreach (T item1 in collection1)
            {
                if (collection2.Contains(item1))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if the IEnumerable contains only duplicates.
        /// </summary>
        public static bool IsContainsElementOnly<T>(this IEnumerable<T> ienum, T element)
        {
            if (ienum == null)
            {
                return false;
            }

            if (ienum.Count() == 0)
            {
                return false;
            }

            foreach (var item in ienum)
            {
                if (item.Equals(element) == false)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Finds the maximum element in the list by a specific field, not by all fields of an object.
        /// <para><see href="https://stackoverflow.com/a/31864296"/></para>
        /// </summary>
        public static T MaxBy<T, R>(this IEnumerable<T> en, Func<T, R> evaluate) where R : IComparable<R>
        {
            return en.Select(t => new Tuple<T, R>(t, evaluate(t)))
                .Aggregate((max, next) => next.Item2.CompareTo(max.Item2) > 0 ? next : max).Item1;
        }

        public static IEnumerable<T> GetBetweenElement<T>(this T element, IEnumerable<T> ienumerable)
        {
            List<T> between = new List<T>();

            foreach (T item in ienumerable)
            {
                if (element.Equals(item) == false)
                {
                    between.Add(item);
                }
                else
                {
                    break;
                }
            }

            return between;
        }

        public static bool IsEmpty(this byte[] byteArray)
        {
            if (byteArray == null || byteArray.Length < 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Extensions to shorten the check for the presence of elements in the IEnumerable.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> collection)
        {
            if (collection != null && collection.Count() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Split's list to X parts.
        /// <para><see href="https://stackoverflow.com/a/419063"/></para>
        /// </summary>
        public static List<List<T>> Split<T>(this IEnumerable<T> array, int partItemCount)
        {
            int i = 0;
            List<List<T>> result = array.GroupBy(s => i++ / partItemCount).Select(g => g.ToList()).ToList();
            return result;
        }

        public static int CountAll<T>(this IEnumerable<IEnumerable<T>> listOfLists)
        {
            if (listOfLists.IsEmpty())
            {
                return 0;
            }

            int totalCount = 0;

            foreach (var innerList in listOfLists)
            {
                if (innerList.IsEmpty())
                    continue;

                totalCount += innerList.Count();
            }

            return totalCount;
        }

        public static List<T> FromChunks<T>(this IEnumerable<IEnumerable<T>> chunks)
        {
            try
            {
                var notDefault = chunks.SelectMany(x => x).Where(x => x.IsDefault() == false);
                return notDefault.ToList();
            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        /// <summary>
        /// Adds dictionary to another dictionary.
        /// <para><see href="https://stackoverflow.com/a/3982463"/></para>
        /// </summary>
        public static void AddRange<T>(this ICollection<T> target, IEnumerable<T> source)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            foreach (T element in source)
                target.Add(element);
        }

        public static List<string> Reverse(this List<string> array)
        {
            for (int i = 0; i < array.Count() / 2; i++)
            {
                string tmp = array[i];
                array[i] = array[array.Count() - i - 1];
                array[array.Count() - i - 1] = tmp;
            }

            return array;
        }
    }
}