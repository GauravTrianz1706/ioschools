namespace ioschools.Data
{
    /// <summary>
    /// A generic key-value pair container.
    /// Migrated from legacy ASP.NET MVC Pair.cs to idiomatic .NET 8 C#:
    /// - Auto-properties replace explicit backing fields.
    /// - Primary constructor pattern used for the parameterised overload.
    /// - Equals / GetHashCode updated to use pattern matching and null-coalescing.
    /// </summary>
    /// <typeparam name="K">The type of the first element.</typeparam>
    /// <typeparam name="V">The type of the second element.</typeparam>
    public class Pair<K, V>
    {
        /// <summary>Initialises a new empty <see cref="Pair{K,V}"/>.</summary>
        public Pair() { }

        /// <summary>Initialises a new <see cref="Pair{K,V}"/> with the supplied values.</summary>
        /// <param name="first">The first element.</param>
        /// <param name="second">The second element.</param>
        public Pair(K first, V second)
        {
            First = first;
            Second = second;
        }

        /// <summary>Gets or sets the first element of the pair.</summary>
        public K? First { get; set; }

        /// <summary>Gets or sets the second element of the pair.</summary>
        public V? Second { get; set; }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (obj is not Pair<K, V> other) return false;
            return Equals(First, other.First) && Equals(Second, other.Second);
        }

        /// <inheritdoc />
        public override int GetHashCode() =>
            HashCode.Combine(First, Second);

        /// <inheritdoc />
        public override string ToString() => $"({First}, {Second})";
    }
}
