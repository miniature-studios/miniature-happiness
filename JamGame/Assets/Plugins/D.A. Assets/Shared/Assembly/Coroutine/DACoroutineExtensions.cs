using System.Collections;

namespace DA_Assets.Shared
{
    public static class DACoroutineExtensions
    {
        public static object StartDARoutine(this IEnumerator routine, object owner) =>
            DARunner.StartDARoutine(routine, owner);

        public static void StopDARoutines(this object owner) =>
            DARunner.StopDARoutines(owner);

        public static void StopDARoutine(this IEnumerator routine) =>
            DARunner.StopDARoutine(routine);

        public static float deltaTime =>
            DARunner.deltaTime;
    }
}