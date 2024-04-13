using System;

namespace DA_Assets.Shared
{
    public class SerializePropertyAttribute : Attribute
    {
        public SerializePropertyAttribute(string fieldName)
        {
            this.fieldName = fieldName;
        }

        private string fieldName;
        public string FieldName => fieldName;
    }
}