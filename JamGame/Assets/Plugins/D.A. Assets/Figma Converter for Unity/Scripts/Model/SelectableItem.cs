using System;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public class SelectableItem
    {
        public string Id;
        public string ParentId;
        public string ParentName;
        public string Name;
        public bool Selected;
    }
}