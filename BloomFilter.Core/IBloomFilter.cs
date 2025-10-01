namespace BloomFilter.Core;

/// <summary>
/// Represents a bloom filter.
/// </summary>
// NOTE: Due to endianness considerations with respect to serialization,
// this bloom filter operates on byte spans only (BYO item serialization).
// A nice side effect of this is that any type may be added to the same filter.
public interface IBloomFilter
{
    /// <summary>
    /// Add a value to the bloom filter.
    /// </summary>
    /// <param name="value">A span of bytes representing the item to add.</param>
    /// <returns>
    /// <see langword="true"/> if the value was newly added as a result of this operation,
    /// otherwise <see langword="false"/>.
    /// </returns>
    public bool Add(ReadOnlySpan<byte> value);

    /// <summary>
    /// Indicates whether or not the bloom filter contains the given value.
    /// </summary>
    /// <param name="value">The value to qualify.</param>
    /// <returns>
    /// <see langword="true"/> if the bloom filter contains the given <paramref name="value"/>,
    /// otherwise <see langword="false"/>.
    /// </returns>
    public bool Contains(ReadOnlySpan<byte> value);

    /// <summary>
    /// Indicates the byte order ("endianness") in which data is stored.
    /// </summary>
    public bool BigEndian { get; }
}
