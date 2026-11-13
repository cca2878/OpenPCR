using System;

if (args.Length > 1 || args.Length == 1 && args[0] != "--master-cache") { Console.Error.WriteLine("Usage: OpenPCR.Growth.Tests [--master-cache]"); return 2; }
Console.WriteLine($"Growth immutable masterdata cache: {MasterDataCacheTests.Run()} assertions passed.");
return 0;
