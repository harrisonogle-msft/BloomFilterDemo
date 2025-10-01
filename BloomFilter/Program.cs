//const int m = 137_045_600; // An empty Bloom filter is a bit array of m bits
//const int Length = m / 8;

const double Err2 = 0.0001; // 0.01% - "two nines"
const double Err3 = 0.00001; // 0.001% - "three nines"
const double Err4 = 0.000001; // 0.0001% - "four nines"
const double Err5 = 0.0000001; // 0.00001% - "five nines"
const double Err6 = 0.00000001; // 0.000001% - "six nines"

IReadOnlyDictionary<string, int> accountIdCounts = AsuInfo.AccountCount_7d;

var totalMiB = new Dictionary<string, double>();

foreach (var (scaleUnit, accountIdCount) in accountIdCounts)
{
    Console.WriteLine();
    foreach ((string errName, double err) in new (string, double)[] {
        ("two nines", Err2), ("three nines", Err3), ("four nines", Err4), ("five nines", Err5), ("six nines", Err6) })
    {
        int n = accountIdCount;
        (int m, int k) = GetBloomFilterParameters(n, err);

        int size_B = (int)Math.Ceiling(m / 8.0);
        int size_KiB = (int)Math.Ceiling(size_B / Math.Pow(2, 10));
        int size_MiB = (int)Math.Ceiling(size_B / Math.Pow(2, 20));

        Console.WriteLine($"{scaleUnit} ε={err}, size={size_MiB}MiB, n={n}, m={m}, k={k}");

        double curr_MiB;
        if (!totalMiB.TryGetValue(errName, out curr_MiB))
        {
            curr_MiB = 0;
        }
        totalMiB[errName] = curr_MiB + size_MiB;
    }
}

Console.WriteLine($"\nDisplaying total size (all {accountIdCounts.Count} scale units) for each desired error rate (cache miss rate).");
foreach ((string errName, double totalSizeMiB) in totalMiB)
{
    Console.WriteLine($"  {errName}: {totalSizeMiB}MiB");
}



static (int m, int k) GetBloomFilterParameters(int n, double eps)
{
    if (!TryGetBloomFilterParameters(n, eps, out int m, out int k))
    {
        throw new InvalidOperationException($"Unable to create bloom filter for n={n}, eps={eps}");
    }

    return (m, k);
}

static bool TryGetBloomFilterParameters(int n, double eps, out int m, out int k)
{
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(n, 0);
    ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(eps, 0);
    ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(eps, 1);

    double ln_eps = Math.Log(eps);
    double ln_2 = Math.Log(2);

    double k64 = Math.Ceiling(-1 * ln_eps / ln_2);
    if (k64 > int.MaxValue)
        goto False;
    k = (int)k64;

    double m64 = Math.Ceiling(-1 * n * ln_eps / ln_2 / ln_2);
    if (m64 > int.MaxValue)
        goto False;
    m = (int)m64;

    return true;

False:
    m = default;
    k = default;
    return false;
}

