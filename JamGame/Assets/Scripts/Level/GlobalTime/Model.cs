using Common;
using UnityEngine;

namespace Level.GlobalTime
{
    [AddComponentMenu("Scripts/Level/GlobalTime/Level.GlobalTime.Model")]
    public class Model : MonoBehaviour
    {
        private static RealTimeSeconds dayLength_ = RealTimeSeconds.Zero;
        public static RealTimeSeconds DayLength => dayLength_;

        [SerializeField]
        private RealTimeSeconds dayLength;

        private float scale = 1.0f;

        private object setTimeScaleLockHolder = null;
        public bool IsLocked => setTimeScaleLockHolder != null;

        private void Awake()
        {
            if (dayLength_ != RealTimeSeconds.Zero)
            {
                Debug.LogError(
                    "Two or more instances of Level.GlobalTime.Model are detected in scene! Deleting one instance..."
                );
                Destroy(this);
                return;
            }

            dayLength_ = dayLength;
            Time.timeScale = scale;
        }

        // Called by buttons that changes time scale.
        public void SetTimeScale(float scale)
        {
            if (setTimeScaleLockHolder != null)
            {
                Debug.LogError($"Cannot set timescale: locked by {setTimeScaleLockHolder}");
                return;
            }

            this.scale = scale;
            Time.timeScale = scale;
        }

        public Result SetTimeScaleLock(object sender, float timeScaleOverride)
        {
            if (sender == null)
            {
                return new FailResult("Invalid sender: null");
            }
            else if (setTimeScaleLockHolder != null && sender != setTimeScaleLockHolder)
            {
                return new FailResult($"Cannot set lock: locked by {setTimeScaleLockHolder}");
            }

            setTimeScaleLockHolder = sender;
            Time.timeScale = timeScaleOverride;

            return new SuccessResult();
        }

        public Result RemoveTimeScaleLock(object sender)
        {
            if (sender == null)
            {
                return new FailResult("Invalid sender: null");
            }
            else if (setTimeScaleLockHolder != null && sender != setTimeScaleLockHolder)
            {
                return new FailResult($"Cannot remove lock: locked by {setTimeScaleLockHolder}");
            }

            setTimeScaleLockHolder = null;
            Time.timeScale = scale;

            return new SuccessResult();
        }
    }
}
