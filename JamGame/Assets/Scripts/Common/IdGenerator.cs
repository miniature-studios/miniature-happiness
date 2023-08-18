using System;
using System.Linq;
using System.Text;

namespace Common
{
    public readonly struct UniqueId
    {
        public string Id { get; }
        public UniqueId(string id) { Id = id; }

        public static bool operator ==(UniqueId value1, UniqueId value2)
        {
            return value1.Id == value2.Id;
        }

        public static bool operator !=(UniqueId value1, UniqueId value2)
        {
            return value1.Id != value2.Id;
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
                .ToList().ForEach(e => builder.Append(e));
            return new UniqueId(builder.ToString());
        }
    }
}
