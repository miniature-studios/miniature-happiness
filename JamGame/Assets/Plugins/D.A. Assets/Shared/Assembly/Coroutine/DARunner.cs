using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

internal delegate void CallbackFunction();

namespace DA_Assets.Shared
{
#if UNITY_EDITOR
    [ExecuteAlways]
    [InitializeOnLoad]
#endif

    internal class DARunner
    {
        private static Queue<Action> _executionQueue = new Queue<Action>();
        internal static List<DACoroutine> coroutinesPool = new List<DACoroutine>();

        internal static double timeSinceScriptReload => stopwatch.Elapsed.TotalSeconds;
        internal static CallbackFunction update;

        internal static float deltaTime => (float)_deltaTime;
        internal static double _deltaTime;

        private static double previousTime;
        private static Stopwatch stopwatch;

        private static int frequency = 15;

        static DARunner()
        {
            update += () => { };
            stopwatch = Stopwatch.StartNew();
            SynchronizationContext context = SynchronizationContext.Current;

            Update(context);
            UpdateAsync();
        }

        private static void Update(SynchronizationContext context)
        {
            new Thread(() =>
            {
                while (true)
                {
                    double currentTime = stopwatch.Elapsed.TotalSeconds;
                    _deltaTime = currentTime - previousTime;
                    previousTime = currentTime;

                    context.Post(_ => update(), null);

                    Thread.Sleep(frequency);
                }
            }).Start();
        }

        private static async void UpdateAsync()
        {
            while (true)
            {
                if (_executionQueue.Count > 0)
                {
                    Action action = _executionQueue.Dequeue();
                    action?.Invoke();
                }

                await System.Threading.Tasks.Task.Delay(1000);
            }
        }

        internal static void ExecuteOnMainThread(Action action)
        {
            _executionQueue.Enqueue(action);
        }

        internal static object StartDARoutine(IEnumerator routine, object owner)
        {
            DACoroutine daCoroutine;

            if (owner == null)
            {
                daCoroutine = new DACoroutine(routine);
            }
            else
            {
                daCoroutine = new DACoroutine(routine, owner);
            }

            coroutinesPool.Add(daCoroutine);
            return daCoroutine;
        }

        internal static void StopDARoutine(IEnumerator routine)
        {
            for (int i = 0; i < coroutinesPool.Count(); i++)
            {
                if (coroutinesPool[i].Routine == routine)
                {
                    coroutinesPool[i].Stop();
                    break;
                }
            }
        }

        internal static void StopDARoutines(object owner)
        {
            for (int i = 0; i < coroutinesPool.Count(); i++)
            {
                if (coroutinesPool[i] == null)
                {
                    continue;
                }
                else if (coroutinesPool[i].Owner == null && owner == null)
                {
                    coroutinesPool[i].Stop();
                }
                else if (coroutinesPool[i].Owner == null || coroutinesPool[i].Owner.Target == null)
                {
                    coroutinesPool[i].Stop();
                }
                else if (coroutinesPool[i].Owner.Target.Equals(owner))
                {
                    coroutinesPool[i].Stop();
                }
            }
        }

        internal static void StopAllCoroutines()
        {
            for (int i = 0; i < coroutinesPool.Count(); i++)
            {
                if (coroutinesPool[i] == null)
                {
                    continue;
                }
                else
                {
                    coroutinesPool[i].Stop();
                }
            }
        }
    }

    public class Delay
    {
        public double WaitTime { get; }
        public float WaitTimeF => (float)WaitTime;
        public Delay(float seconds)
        {
            this.WaitTime = seconds;
        }
    }

    public class WaitFor
    {
        public static IEnumerator Iterations(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return null;
            }
        }

        public static Delay Delay(float seconds) => new Delay(seconds);
        /// <summary> 1f </summary>
        public static Delay Delay1() => new Delay(delay1);
        /// <summary> 0.1f </summary>
        public static Delay Delay01() => new Delay(delay01);
        /// <summary> 0.01f </summary>
        public static Delay Delay001() => new Delay(delay001);

        private static float delay001 = 0.01f;
        private static float delay01 = 0.1f;
        private static float delay1 = 1f;
    }
}