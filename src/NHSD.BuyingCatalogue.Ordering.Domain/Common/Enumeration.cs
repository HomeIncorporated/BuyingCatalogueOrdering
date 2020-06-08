using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHSD.BuyingCatalogue.Ordering.Domain.Common
{
    public abstract class Enumeration : IEquatable<Enumeration>
    {
        public int Id { get; }

        public string Name { get; }

        protected Enumeration(int id, string name)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public static IEnumerable<T> GetAll<T>() where T : Enumeration
        {
            FieldInfo[] fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly);
            return fields.Select(f => f.GetValue(null)).Cast<T>();
        }

        public static T FromValue<T>(int value) where T : Enumeration
            => Parse<T, int>(value, "value", item => item.Id == value);

        public static T FromName<T>(string name) where T : Enumeration
            => Parse<T, string>(name, "name", item => string.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase));

        private static T Parse<T, TV>(TV value, string description, Func<T, bool> predicate) where T : Enumeration
            => GetAll<T>().FirstOrDefault(predicate) ?? throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");

        public bool Equals(Enumeration other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Id == other.Id;
        }

        public override bool Equals(object obj) 
            => obj is Enumeration other && Equals(other);

        public override int GetHashCode() 
            => HashCode.Combine(Id);
    }
}
