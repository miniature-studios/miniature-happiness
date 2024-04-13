//based on https://docs.unity3d.com/Packages/com.unity.editorcoroutines@1.0/api/Unity.EditorCoroutines.Editor.EditorCoroutine.html

using System;
using System.Collections;
using System.Collections.Generic;
//using Runner = DA_Assets.Shared.Runner;

namespace DA_Assets.Shared
{
    internal class DACoroutine
    {
        private WeakReference m_Owner;
        private IEnumerator m_Routine;
        private ProcessorData data;
        private bool m_IsDone;
        private static Stack<IEnumerator> kIEnumeratorProcessingStack = new Stack<IEnumerator>(32);

        internal WeakReference Owner => m_Owner;
        internal IEnumerator Routine => m_Routine;

        internal DACoroutine(IEnumerator routine)
        {
            m_Owner = null;
            m_Routine = routine;

            DARunner.update += MoveNext;
        }

        internal DACoroutine(IEnumerator routine, object owner)
        {
            m_Owner = new WeakReference(owner);
            m_Routine = routine;

            DARunner.update += MoveNext;
        }

        internal void MoveNext()
        {
            if (m_Owner != null && m_Owner.IsAlive == false)
            {
                DARunner.update -= MoveNext;
                return;
            }

            bool done = ProcessIEnumeratorRecursive(m_Routine);
            m_IsDone = !done;

            if (m_IsDone)
            {
                DARunner.update -= MoveNext;
            }
        }

        private bool ProcessIEnumeratorRecursive(IEnumerator enumerator)
        {
            IEnumerator root = enumerator;

            if (root == null)
            {
                return false;
            }

            while (enumerator.Current as IEnumerator != null)
            {
                kIEnumeratorProcessingStack.Push(enumerator);
                enumerator = enumerator.Current as IEnumerator;
            }

            //process leaf
            Set(enumerator.Current);
            bool result = MoveNext(enumerator);

            while (kIEnumeratorProcessingStack.Count > 1)
            {
                if (!result)
                {
                    result = kIEnumeratorProcessingStack.Pop().MoveNext();
                }
                else
                {
                    kIEnumeratorProcessingStack.Clear();
                }
            }

            if (kIEnumeratorProcessingStack.Count > 0 && !result && root == kIEnumeratorProcessingStack.Pop())
            {
                result = root.MoveNext();
            }

            return result;
        }

        internal void Set(object yield)
        {
            if (yield == data.Current)
                return;

            Type type = yield.GetType();
            DataType dataType = DataType.None;
            double targetTime = -1;

            if (type == typeof(Delay))
            {
                Delay delay = yield as Delay;
                targetTime = DARunner.timeSinceScriptReload + delay.WaitTime;
                dataType = DataType.Delay;
            }
            else if (type == typeof(DACoroutine))
            {
                dataType = DataType.DACoroutine;
            }

            data = new ProcessorData
            {
                Current = yield,
                TargetTime = targetTime,
                Type = dataType
            };
        }

        internal bool MoveNext(IEnumerator enumerator)
        {
            bool advance;

            switch (data.Type)
            {
                case DataType.Delay:
                    advance = data.TargetTime <= DARunner.timeSinceScriptReload;
                    break;
                case DataType.DACoroutine:
                    advance = (data.Current as DACoroutine).m_IsDone;
                    break;
                default:
                    advance = data.Current == enumerator.Current;
                    break;
            }

            if (advance)
            {
                data = default(ProcessorData);
                return enumerator.MoveNext();
            }

            return true;
        }

        internal static void StopAllInternal()
        {
            DARunner.update = () => { };
        }

        internal void Stop()
        {
            DARunner.update -= MoveNext;
            DARunner.coroutinesPool.Remove(this);

            m_Owner = null;
            m_Routine = null;
        }
    }

    enum DataType
    {
        None,
        DACoroutine,
        Delay
    }

    struct ProcessorData
    {
        internal DataType Type;
        internal object Current;
        internal double TargetTime;
    }
}
