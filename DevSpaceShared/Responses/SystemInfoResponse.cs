using System.Text.RegularExpressions;

namespace DevSpaceShared.Responses;
public class SystemInfoResponse
{
    public HostInfo Host = new HostInfo();
    public ProcessInfo Process = new ProcessInfo();
}
public class HostInfo
{
    public string Release;
    public string Uptime;
    public string MachineName;
    public string SystemVersion;
    public double RamTotal;
    public double RamUsed;
    public double RamFree;
    public double SwapTotal;
    public double SwapUsed;
    public double SwapFree;
    public LinuxCpuInfo Cpu = new LinuxCpuInfo();
}
public class ProcessInfo
{
    public int ProcessId;
    public string ProcessPath;
    public string DotnetVersion;
    public List<string> EnvironmentVariables = new List<string>();
}

public class LinuxCpuInfo
{
    public string VendorId { get; set; }
    public int CpuCount { get; set; }
    public int CpuFamily { get; set; }
    public int ThreadsPerCore { get; set; }
    public int CoresPerSocket { get; set; }
    public int Sockets { get; set; }
    public string VirtualizationType { get; set; }
    public string VirtualizationHypervisor { get; set; }
    public string VirtualizationMode { get; set; }
    public Dictionary<string, string> Vulnerabilities = new Dictionary<string, string>();
    public int Model { get; set; }
    public string ModelName { get; set; }
    public string Flags { get; set; }
    public double MHz { get; set; }
    public string CacheSize { get; set; }

    public void GetValues()
    {

        string[] cpuInfoLines = File.ReadAllLines(@"/proc/cpuinfo");

        CpuInfoMatch[] cpuInfoMatches =
        {
                new CpuInfoMatch(@"^vendor_id\s+:\s+(.+)", value => VendorId = value),
                new CpuInfoMatch(@"^cpu family\s+:\s+(.+)", value => CpuFamily = int.Parse(value)),
                new CpuInfoMatch(@"^model\s+:\s+(.+)", value => Model = int.Parse(value)),
                new CpuInfoMatch(@"^model name\s+:\s+(.+)", value => ModelName = value),
                new CpuInfoMatch(@"^cpu MHz\s+:\s+(.+)", value => MHz = double.Parse(value)),
                new CpuInfoMatch(@"^cache size\s+:\s+(.+)", value => CacheSize = value),
                new CpuInfoMatch(@"^flags\s+:\s+(.+)", value => Flags = value),
            };

        foreach (string cpuInfoLine in cpuInfoLines)
        {
            foreach (CpuInfoMatch cpuInfoMatch in cpuInfoMatches)
            {
                Match match = cpuInfoMatch.regex.Match(cpuInfoLine);
                if (match.Groups[0].Success)
                {
                    string value = match.Groups[1].Value;
                    cpuInfoMatch.updateValue(value);
                }
            }
        }
    }

    public class CpuInfoMatch
    {
        public Regex regex;
        public Action<string> updateValue;

        public CpuInfoMatch(string pattern, Action<string> update)
        {
            this.regex = new Regex(pattern, RegexOptions.Compiled);
            this.updateValue = update;
        }
    }
}