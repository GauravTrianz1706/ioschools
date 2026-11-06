namespace ioschools.Data
{
    /// <summary>
    /// Defines the comparison direction used by a <see cref="Sorter"/>.
    /// Migrated from legacy ASP.NET MVC enum — no value changes required.
    /// </summary>
    public enum SorterType
    {
        /// <summary>Filter for values greater than the threshold.</summary>
        GREATERTHAN,

        /// <summary>Filter for values less than the threshold.</summary>
        LESSTHAN
    }

    /// <summary>
    /// Parses a comparison expression string (e.g. "&gt;5" or "&lt;10") into a typed
    /// threshold value and direction.
    /// Migrated from legacy ASP.NET MVC Sorter.cs to idiomatic .NET 8 C#:
    /// - Uses <see cref="ReadOnlySpan{T}"/> for zero-allocation string slicing.
    /// - Throws <see cref="ArgumentException"/> instead of <see cref="NotImplementedException"/>
    ///   for invalid input, which is more appropriate for a parsing error.
    /// - Exposes a static <see cref="TryParse"/> factory for safe parsing.
    /// </summary>
    public sealed class Sorter
    {
        /// <summary>Gets the numeric threshold value.</summary>
        public int Value { get; }

        /// <summary>Gets the comparison direction.</summary>
        public SorterType Type { get; }

        /// <summary>
        /// Initialises a new <see cref="Sorter"/> by parsing the supplied expression string.
        /// </summary>
        /// <param name="expression">
        /// A string whose first character is either '&gt;' or '&lt;', followed by an integer
        /// (e.g. "&gt;5", "&lt;10").
        /// </param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="expression"/> is null, empty, or does not start with
        /// '&gt;' or '&lt;'.
        /// </exception>
        /// <exception cref="FormatException">
        /// Thrown when the numeric portion of <paramref name="expression"/> cannot be parsed
        /// as an integer.
        /// </exception>
        public Sorter(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                throw new ArgumentException("Expression must not be null or empty.", nameof(expression));

            Type = expression[0] switch
            {
                '>' => SorterType.GREATERTHAN,
                '<' => SorterType.LESSTHAN,
                _   => throw new ArgumentException(
                           $"Expression must start with '>' or '<', but got '{expression[0]}'.",
                           nameof(expression))
            };

            Value = int.Parse(expression.AsSpan(1));
        }

        /// <summary>
        /// Attempts to parse the supplied expression string into a <see cref="Sorter"/>.
        /// </summary>
        /// <param name="expression">The expression string to parse.</param>
        /// <param name="sorter">
        /// When this method returns <see langword="true"/>, contains the parsed
        /// <see cref="Sorter"/>; otherwise <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if parsing succeeded; otherwise <see langword="false"/>.
        /// </returns>
        public static bool TryParse(string? expression, out Sorter? sorter)
        {
            sorter = null;
            if (string.IsNullOrEmpty(expression)) return false;
            if (expression[0] != '>' && expression[0] != '<') return false;
            if (!int.TryParse(expression.AsSpan(1), out _)) return false;

            sorter = new Sorter(expression);
            return true;
        }

        /// <inheritdoc />
        public override string ToString() =>
            Type == SorterType.GREATERTHAN ? $">{Value}" : $"<{Value}";
    }
}
