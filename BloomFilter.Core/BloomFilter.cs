using System.Buffers.Binary;
using System.Diagnostics;
using System.IO.Hashing;

namespace BloomFilter.Core;

public sealed class BloomFilter : IBloomFilter
{
    private readonly long _length;
    private readonly ulong _capacity;
    private byte[][] _segments;

    public BloomFilter(long capacity, int hashCount, bool bigEndian = false)
    {
        if (capacity < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        if (hashCount < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(hashCount));
        }

        long length = capacity / 8; // byte count

        if (capacity % 8 != 0)
        {
            length++; // ceil
        }

        Capacity = capacity;
        HashCount = hashCount;
        BigEndian = bigEndian;

        _capacity = (ulong)capacity; // for faster mod later
        _length = length;

        if (length / int.MaxValue > int.MaxValue)
        {
            throw new ArgumentOutOfRangeException(nameof(capacity));
        }

        int q = (int)(length / int.MaxValue);
        int r = (int)(length % int.MaxValue);
        int segmentCount = r == 0 ? q : q + 1;

        _segments = new byte[segmentCount][];

        int i;
        for (i = 0; i < q; i++)
        {
            _segments[i] = new byte[int.MaxValue];
        }
        if (r != 0)
        {
            _segments[i++] = new byte[r];
        }
        Debug.Assert(i == segmentCount);
    }

    public long Capacity { get; }
    public int HashCount { get; }
    public bool BigEndian { get; }

    public bool Add(ReadOnlySpan<byte> value)
    {
        Span<byte> buffer = stackalloc byte[16];
        ComputeHash(value, buffer);

        ulong lower = BinaryPrimitives.ReadUInt64BigEndian(buffer.Slice(0, 8));
        ulong upper = BinaryPrimitives.ReadUInt64BigEndian(buffer.Slice(8, 8));

        bool flag = true;

        for (uint i = 0; i < HashCount; i++)
        {
            // Kirsch, A., Mitzenmacher, M. (2006). Less Hashing, Same Performance: Building a Better Bloom Filter. In: Azar, Y., Erlebach, T. (eds) Algorithms – ESA 2006. ESA 2006. Lecture Notes in Computer Science, vol 4168. Springer, Berlin, Heidelberg. https://doi.org/10.1007/11841036_42
            // https://www.eecs.harvard.edu/~michaelm/postscripts/tr-02-05.pdf
            long index = (long)(unchecked(lower + i * upper) % _capacity);

            flag &= Set(index);
        }

        return flag;
    }

    public bool Contains(ReadOnlySpan<byte> value)
    {
        Span<byte> buffer = stackalloc byte[16];
        ComputeHash(value, buffer);

        ulong lower = BinaryPrimitives.ReadUInt64BigEndian(buffer.Slice(0, 8));
        ulong upper = BinaryPrimitives.ReadUInt64BigEndian(buffer.Slice(8, 8));

        for (uint i = 0; i < HashCount; i++)
        {
            // Kirsch, A., Mitzenmacher, M. (2006). Less Hashing, Same Performance: Building a Better Bloom Filter. In: Azar, Y., Erlebach, T. (eds) Algorithms – ESA 2006. ESA 2006. Lecture Notes in Computer Science, vol 4168. Springer, Berlin, Heidelberg. https://doi.org/10.1007/11841036_42
            // https://www.eecs.harvard.edu/~michaelm/postscripts/tr-02-05.pdf
            long index = (long)(unchecked(lower + i * upper) % _capacity);

            if (!Get(index))
            {
                return false;
            }
        }
        return true;
    }

    private void ComputeHash(ReadOnlySpan<byte> value, Span<byte> destination)
    {
        // The hash function used in a bloom filter must be very fast because
        // it will be evaluated multiple times per atomic bloom filter operation.
        // XxHash is a non-cryptographic hash algorithm widely regarded as one of the fastest.
        // .NET provides a portable implementation "out of the box".
        if (!XxHash128.TryHash(value, destination, out int bytesWritten))
        {
            throw new InvalidOperationException($"Unable to hash. (received {bytesWritten} bytes; destination length: {destination.Length})");
        }

        if (bytesWritten != 16)
        {
            throw new InvalidOperationException($"Unable to hash. Expected 16 bytes, received {bytesWritten} (destination length: {destination.Length})");
        }
    }

    private bool Get(long index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        int mask = 1 << ((int)(index % 8));
        index /= 8; // bits -> bytes

        int q = (int)(index / int.MaxValue);
        int r = (int)(index % int.MaxValue);
        byte octet = _segments[q][r];

        return (octet & mask) != 0;
    }

    /// <summary>
    /// Set the bit at the given index.
    /// </summary>
    /// <param name="index">The index of the bit to set.</param>
    /// <returns><see langword="false"/> if the bit was already set, otherwise <see langword="true"/>.</returns>
    private bool Set(long index)
    {
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }

        int mask = 1 << ((int)(index % 8));
        index /= 8; // bits -> bytes

        int q = (int)(index / int.MaxValue);
        int r = (int)(index % int.MaxValue);
        byte octet = _segments[q][r];

        if ((octet & mask) == 0)
        {
            _segments[q][r] = (byte)(octet | mask);
            return true;
        }
        else
        {
            return false;
        }
    }
}
