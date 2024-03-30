
using UnityEngine;

namespace DA_Assets.Shared
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1">Object type.</typeparam>
    public struct DAResult<T1>
    {
        private bool success;
        public bool Success
        {
            get => success;
            set
            {
                success = value;
            }
        }
        public T1 Object { get; set; }
        public IDAError Error { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T1">Object type.</typeparam>
    public delegate void Return<T1>(DAResult<T1> result);
}