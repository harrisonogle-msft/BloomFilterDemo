using BloomFilter.Core;
using System.Diagnostics;

/*
AMSUA0201 ε=0.0001, size=42MiB, n=18332791, m=351441744, k=14
AMSUA0201 ε=1E-05, size=53MiB, n=18332791, m=439302180, k=17
AMSUA0201 ε=1E-06, size=63MiB, n=18332791, m=527162616, k=20
AMSUA0201 ε=1E-07, size=74MiB, n=18332791, m=615023052, k=24
AMSUA0201 ε=1E-08, size=84MiB, n=18332791, m=702883488, k=27
 */

const int n = 18332791;
const int m = 702883488;
const int k = 27;
int negatives = 35 * n; // tenants from 35 scale units checking in against our 1 bloom filter for 1 scale unit

Console.WriteLine($"Starting. ({nameof(n)}={n}, {nameof(m)}={m}, {nameof(k)}={k}, {nameof(negatives)}={negatives}, ratio {nameof(negatives)}:{nameof(n)}: {negatives / n})");
var startingTimestamp = Stopwatch.GetTimestamp();
var previousTimestamp = startingTimestamp;
TimeSpan GetElapsedTime()
{
    var temp = Stopwatch.GetTimestamp();
    var retVal = TimeSpan.FromSeconds((temp - previousTimestamp) / (double)Stopwatch.Frequency);
    previousTimestamp = temp;
    return retVal;
}

IBloomFilter filter = new BloomFilter.Core.BloomFilter(m, k);
Console.WriteLine($"Allocated bloom filter of {m / 8 / 1024 / 1024}MiB. (delta: {GetElapsedTime().TotalSeconds:0.#####}s)");

var hashSet = new HashSet<Guid>();
{
    for (int i = 0; i < n; i++)
    {
        var guid = Guid.NewGuid();
        hashSet.Add(guid);
    }
    Console.WriteLine($"Generated {n} guids. (delta: {GetElapsedTime().TotalSeconds:0.#####}s)");
}

{
    foreach (Guid guid in hashSet)
    {
        filter.Add(guid);
    }
    Console.WriteLine($"Added {n} guids to bloom filter. (delta: {GetElapsedTime().TotalSeconds:0.#####}s)");
}

{
    foreach (Guid guid in hashSet)
    {
        if (!filter.Contains(guid))
        {
            throw new InvalidOperationException("Guid is missing.");
        }
    }
    Console.WriteLine($"Validated {n} guids in bloom filter. (delta: {GetElapsedTime().TotalSeconds:0.#####}s)");
}

{
    int falsePositives = 0;
    int negative = 0;
    var tasks = new List<Task>();
    for (int i = 0; i < Environment.ProcessorCount * 2; i++)
    {
        var task = Task.Run(() =>
        {
            while (Interlocked.Increment(ref negative) < negatives)
            {
                if (filter.Contains(Guid.NewGuid()))
                {
                    Interlocked.Increment(ref falsePositives);
                }
            }
        });
        tasks.Add(task);
    }
    await Task.WhenAll(tasks);
    Console.WriteLine($"Checked {negatives} negatives and found {falsePositives} false positives. (delta: {GetElapsedTime().TotalSeconds:0.#####}s)");
}

Console.WriteLine($"Done. (total elapsed: {(Stopwatch.GetTimestamp() - startingTimestamp) / (double)Stopwatch.Frequency:0.#####}s)");
