using DA_Assets.Shared;
using UnityEditor;
using UnityEngine;

namespace DA_Assets.Shared
{
    public class DaiEditor<T1, T2> : Editor
    where T1 : DaiEditor<T1, T2>
    where T2 : MonoBehaviour
    {
        public T2 monoBeh;
        public DAInspector gui => DAInspector.Instance;

        private void OnEnable()
        {
            monoBeh = (T2)target;
            this.StartUiRepaint();
            OnShow();
        }

        public virtual void OnShow() { }
    }

    public class DaiEditorWindow : EditorWindow
    {
        public DAInspector gui => DAInspector.Instance;

        private void OnEnable()
        {
            OnShow();
        }

        public virtual void OnShow() { }
    }
}
