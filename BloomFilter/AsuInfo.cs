internal static class AsuInfo
{
    /*
    IntuneEvent
    | where env_time > ago(1d)
    | where env_cloud_role in ("FEFabric", "MTFabric", "HVFabric") // ASU
    | summarize AccountIds = dcount(AccountId) by env_cloud_name
    | sort by AccountIds desc
    | project Item = strcat('        { "', env_cloud_name, '", ', AccountIds, ' },')
    | summarize result = strcat_array(make_list(Item), '\n')
     */
    public static readonly IReadOnlyDictionary<string, int> AccountCount_7d = new Dictionary<string, int>
    {
        { "AMSUA0201", 18332791 },
        { "AMSUA0102", 17137976 },
        { "AMSUB0301", 16993707 },
        { "AMSUB0101", 16864927 },
        { "AMSUA0601", 16704022 },
        { "AMSUA0101", 16534253 },
        { "AMSUA0401", 15406615 },
        { "AMSUB0502", 14948545 },
        { "AMSUA0801", 14754212 },
        { "AMSUB0201", 14716019 },
        { "AMSUB0601", 14562192 },
        { "AMSUB0102", 13944266 },
        { "AMSUC0501", 13648308 },
        { "AMSUB0501", 12995733 },
        { "AMSUA0501", 12980716 },
        { "AMSUB0202", 12544327 },
        { "AMSUC0101", 11560763 },
        { "AMSUC0201", 11198406 },
        { "AMSUB0302", 11154873 },
        { "AMSUA0502", 10884806 },
        { "AMSUA0202", 9799664 },
        { "AMSUA0402", 8871102 },
        { "AMSUB0701", 7515615 },
        { "AMSUC0301", 6750499 },
        { "AMSUA0702", 6198837 },
        { "AMSUA0701", 4642321 },
        { "AMSUA0602", 4542297 },
        { "AMSUD0101", 4192022 },
        { "AMSUA0901", 3823748 },
        { "BAMSU01", 1870850 },
        { "AMSUC0601", 1216851 },
        { "AMSUIN01", 796536 },
        { "SHAMSUA01", 270017 },
        { "DAMSU01", 145513 },
        { "AMSUB0901", 21412 },
    };

    public static readonly IReadOnlyDictionary<string, int> AccountCount_1d = new Dictionary<string, int>
    {
        { "AMSUA0201", 9470543 },
        { "AMSUA0601", 9128675 },
        { "AMSUA0102", 8773343 },
        { "AMSUA0101", 8761118 },
        { "AMSUA0801", 8087184 },
        { "AMSUA0401", 7936271 },
        { "AMSUB0301", 7240623 },
        { "AMSUB0101", 7170702 },
        { "AMSUB0502", 6693243 },
        { "AMSUB0102", 6572493 },
        { "AMSUB0501", 6457777 },
        { "AMSUB0601", 6441120 },
        { "AMSUA0501", 6220994 },
        { "AMSUB0201", 6018565 },
        { "AMSUB0202", 5717700 },
        { "AMSUC0501", 5547082 },
        { "AMSUC0101", 5359723 },
        { "AMSUA0202", 5305678 },
        { "AMSUA0502", 5236968 },
        { "AMSUC0201", 5120970 },
        { "AMSUA0402", 5093286 },
        { "AMSUB0302", 5051467 },
        { "AMSUA0702", 4035519 },
        { "AMSUB0701", 3796868 },
        { "AMSUA0701", 3316051 },
        { "AMSUA0602", 3190052 },
        { "AMSUC0301", 3057676 },
        { "AMSUD0101", 2574459 },
        { "AMSUA0901", 2237387 },
        { "AMSUC0601", 729248 },
        { "BAMSU01", 706610 },
        { "AMSUIN01", 616124 },
        { "SHAMSUA01", 176753 },
        { "DAMSU01", 103211 },
        { "AMSUB0901", 10948 },
    };
}
