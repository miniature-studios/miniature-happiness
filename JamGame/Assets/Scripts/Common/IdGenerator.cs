using Level.Room;
using System;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common
{
    public interface IUniqueIdHandler
    {
        public UniqueId UniqueId { get; set; }
        public CoreModel CoreModel { get; }
    }

    [Serializable]
    public struct UniqueId
    {
        [SerializeField, InspectorReadOnly]
        private string id;
        public string Id => id;

        public UniqueId(string id)
        {
            this.id = id;
        }

        public static bool operator ==(UniqueId value1, UniqueId value2)
        {
            return value1.Id == value2.Id;
        }

        public static bool operator !=(UniqueId value1, UniqueId value2)
        {
            return value1.Id != value2.Id;
        }

        public override bool Equals(object obj)
        {
            return obj != null
                && GetType() == obj.GetType()
                && ((UniqueId)obj).Id == Id
                && base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }

    public static class IdGenerator
    {
        public static UniqueId Generate()
        {
            StringBuilder builder = new();
            Enumerable
                .Range(65, 26)
                .Select(e => ((char)e).ToString())
                .Concat(Enumerable.Range(97, 26).Select(e => ((char)e).ToString()))
                .Concat(Enumerable.Range(0, 10).Select(e => e.ToString()))
                .OrderBy(e => Guid.NewGuid())
                .Take(25)
                .ToList()
                .ForEach(e => builder.Append(e));
            return new UniqueId(builder.ToString());
        }
    }
}
