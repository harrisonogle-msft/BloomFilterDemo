using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace BloomFilter.Core;

/// <summary>
/// Primitive type serialization extension methods
/// for determinism irrespective of endianness.
/// </summary>
public static class BloomFilterSerializationExtensions
{
    /// <summary>
    /// Add a <see cref="System.Guid"/> to the bloom filter.
    /// </summary>
    /// <param name="filter">The bloom filter.</param>
    /// <param name="value">The value to add.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public static bool Add(this IBloomFilter filter, Guid value)
    {
        ThrowHelper.ThrowIfNull(filter);
        Span<byte> buffer = stackalloc byte[16];
        if (!TryWriteBytes(value, buffer, filter.BigEndian, out _))
        {
            ThrowHelper.ThrowInvalidOperationException("Unexpected error: unable to write guid.");
        }
        return filter.Add(buffer);
    }

    /// <summary>
    /// Indicates whether or not the bloom filter contains the given value.
    /// </summary>
    /// <param name="value">The value to qualify.</param>
    /// <returns>
    /// <see langword="true"/> if the bloom filter contains the given <paramref name="value"/>,
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool Contains(this IBloomFilter filter, Guid value)
    {
        ThrowHelper.ThrowIfNull(filter);
        Span<byte> buffer = stackalloc byte[16];
        if (!TryWriteBytes(value, buffer, filter.BigEndian, out _))
        {
            ThrowHelper.ThrowInvalidOperationException("Unexpected error: unable to write guid.");
        }
        return filter.Contains(buffer);
    }

    // Returns whether bytes are successfully written to given span.
    // For the benefit of .NET standard 2.0.
    private static bool TryWriteBytes(Guid value, Span<byte> destination, bool bigEndian, out int bytesWritten)
    {
        // NOTE: On little endian machines (Windows x86 or x64), guids are serialized as "little endian" by default.
        // First-class support for big endian guids was added in .NET 8.
        // https://learn.microsoft.com/en-us/dotnet/api/system.guid.-ctor?view=net-8.0#system-guid-ctor(system-readonlyspan((system-byte))-system-boolean)
        //   https://github.com/dotnet/runtime/blob/27d23218a56d484638692d56b3abd27e0b21b591/src/libraries/System.Private.CoreLib/src/System/Guid.cs#L89
        // https://learn.microsoft.com/en-us/dotnet/api/system.guid.trywritebytes?view=net-8.0#system-guid-trywritebytes(system-span((system-byte))-system-boolean-system-int32@)
        //   https://github.com/dotnet/runtime/blob/27d23218a56d484638692d56b3abd27e0b21b591/src/libraries/System.Private.CoreLib/src/System/Guid.cs#L960

#if NET8_0_OR_GREATER
        return value.TryWriteBytes(destination, bigEndian, out bytesWritten);
#else
        if (destination.Length < 16)
        {
            bytesWritten = 0;
            return false;
        }

        MemoryMarshal.Write(destination, ref value);

        if (BitConverter.IsLittleEndian == bigEndian)
        {
            // Slower path for reverse.

            uint a = MemoryMarshal.Read<uint>(destination.Slice(0, 4));
            ushort b = MemoryMarshal.Read<ushort>(destination.Slice(4, 2));
            ushort c = MemoryMarshal.Read<ushort>(destination.Slice(6, 2));

            a = BinaryPrimitives.ReverseEndianness(a);
            b = BinaryPrimitives.ReverseEndianness(b);
            c = BinaryPrimitives.ReverseEndianness(c);

            MemoryMarshal.Write(destination.Slice(0, 4), ref a);
            MemoryMarshal.Write(destination.Slice(4, 2), ref b);
            MemoryMarshal.Write(destination.Slice(6, 2), ref c);
        }

        bytesWritten = 16;
        return true;
#endif
    }
}
