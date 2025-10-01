using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace BloomFilter.Core;

internal static class ThrowHelper
{
    public static void ThrowIfNull([NotNull] object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
    {
        // We can't play with JIT intrinsics like dotnet/runtime can, so no value types allowed.
        Debug.Assert(!(argument?.GetType()?.IsValueType is true), "to avoid boxing, do not use this internal API with value types");

        if (argument is null)
        {
            ThrowArgumentNullException(paramName);
        }
    }

    [DoesNotReturn]
    public static void ThrowArgumentNullException(string? paramName)
    {
        throw new ArgumentNullException(paramName);
    }

    [DoesNotReturn]
    public static void ThrowInvalidOperationException(string? message)
    {
        throw new InvalidOperationException(message);
    }
}
