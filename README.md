# Intune Endpoint Discovery Bloom Filter Demo

The `BloomFilter.Demo` project is an executable that tests the usage of a bloom filter built for the AMSUA0201 scale unit to provide a mechanism for set membership of AAD tenants in that scale unit.

The number of distinct accounts in the scale unit was ascertained via Kusto query.

# The demo

1. Builds a bloom filter using the same number of randomly-generated guids as there are AAD tenants in AMSUA0201.
2. Validates set membership (100% true positive rate) by testing set inclusion of each of the guids used to build the bloom filter.
3. Tests the false positive rate by generating new guids and determining whether or not the bloom filter indicates membership.

# The results

The demo builds an 83MiB bloom filter for AMSUA0201 using 18 million guids - the same number of AAD tenants in AMSUA0201.

- The bloom filter was built in less than ten seconds.
- All 18 million guids were tested against the bloom filter (in less than 15 seconds) with a 100% true positive rate
- 641 million random guids - not contained in the bloom filter - were tested for set membership (in less than 40 seconds) with only 4 false positives.

```
C:\Users\harrisonogle\source\repos\sandbox\BloomFilter\BloomFilter.Demo\bin\Release\net8.0>BloomFilter.Demo.exe
Starting. (n=18332791, m=702883488, k=27, negatives=641647685, ratio negatives:n: 35)
Allocated bloom filter of 83MiB. (delta: 0.00029s)
Generated 18332791 guids. (delta: 5.08118s)
Added 18332791 guids to bloom filter. (delta: 14.25725s)
Validated 18332791 guids in bloom filter. (delta: 12.43064s)
Checked 641647685 negatives and found 4 false positives. (delta: 39.44877s)
Done. (total elapsed: 71.21826s)
```

To run the demo on your machine, clone the repo, build the `BloomFilter.Demo` project, and run the executable on the command line or in an IDE.