using System;

namespace DA_Assets.FCU.Model
{
    [Serializable]
    public struct DependencyItem
    {
        public string Name;
        public string Type;
        public string ScriptingDefineName;
        public bool Enabled { get; set; }
    }
}