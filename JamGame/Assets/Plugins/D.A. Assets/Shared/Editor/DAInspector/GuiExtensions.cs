using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DA_Assets.Shared
{
    public static class GUIExtensions
    {
        public static string[] GetFieldsArray<T>(this Expression<Func<T, object>> pathExpression)
        {
            MemberExpression me;

            switch (pathExpression.Body.NodeType)
            {
                case ExpressionType.Convert:
                case ExpressionType.ConvertChecked:
                    UnaryExpression ue = pathExpression.Body as UnaryExpression;
                    me = ((ue != null) ? ue.Operand : null) as MemberExpression;
                    break;
                default:
                    me = pathExpression.Body as MemberExpression;
                    break;
            }

            List<string> fieldNames = new List<string>();

            while (me != null)
            {
                var serInfo = me.Member.GetCustomAttributes<SerializePropertyAttribute>().ToArray()[0];

                if (serInfo == null)
                {
                    if (me.Member.Name.Contains("CS$") == false)
                    {
                        fieldNames.Add(me.Member.Name);
                    }
                }
                else
                {
                    fieldNames.Add(serInfo.FieldName);
                }

                me = me.Expression as MemberExpression;
            }

            fieldNames.Reverse();
            // fieldNames.RemoveAt(0);

            return fieldNames.ToArray();
        }
    }
}
