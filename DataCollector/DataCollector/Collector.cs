﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Threading;
using System.Threading.Tasks;

namespace DataCollector
{
#if Windows
    public class Collector
    {
        private static readonly Lazy<Collector> Value = new Lazy<Collector>(() => new Collector());
        private readonly Dictionary<int, PerformanceCounter> _processCpuCounters;
        private readonly Dictionary<string, PerformanceCounter> _systemCounters;

        private readonly float _totalRamMbyte;

        private Collector()
        {
            _processCpuCounters = new Dictionary<int, PerformanceCounter>();
            _systemCounters = new Dictionary<string, PerformanceCounter>
            {
                { "FreeRam", new PerformanceCounter("Memory", "Available MBytes") },
                { "Interrupts", new PerformanceCounter("Processor", "Interrupts/sec", "_Total") },
                { "DiskFreeMb", new PerformanceCounter("LogicalDisk", "Free Megabytes", "C:") },
                { "CPU", new PerformanceCounter("Processor Information", "% Processor Time", "_Total") },
                { "RAM", new PerformanceCounter("Memory", "% Committed Bytes In Use") },
                { "InterruptsTime", new PerformanceCounter("Processor", "Interrupts/sec", "_Total") },
                { "LocalDisk", new PerformanceCounter("LogicalDisk", "% Free Space", "C:") }
            };
            _systemCounters["CPU"].NextValue();
            _totalRamMbyte = GetTotalRAM();
            Thread.Sleep(1000);
        }

        public static Collector Instance => Value.Value;

        public async Task<CollectedData> Collect()
        {
            CollectedData dataItem = null;
            try
            {
                var allProcesses = await GetProcesses();
                var freeRam = _systemCounters["FreeRam"].NextValue();
                var freeDiskMb = _systemCounters["DiskFreeMb"].NextValue();
                var localDisk = _systemCounters["LocalDisk"].NextValue();
                var diskFreeMb = _systemCounters["DiskFreeMb"].NextValue();

                float GetDiskTotalMbytes() => (diskFreeMb / localDisk) * 100.0f;
                float GetDiskUsageMbytes() => GetDiskTotalMbytes() - diskFreeMb;

                dataItem = new CollectedData
                {
                    InterruptsPerSeconds = (float)Math.Round(_systemCounters["Interrupts"].NextValue(), 2),
                    InterruptsTimePercent = (float)Math.Round(_systemCounters["InterruptsTime"].NextValue(), 2),

                    TotalRamMBytes = (float)Math.Round(_totalRamMbyte, 2),
                    RamUsagePercentage = (float)Math.Round(100 - (freeRam / (_totalRamMbyte / 100)), 2),
                    UsageRamMBytes = (float)Math.Round(_totalRamMbyte - freeRam, 2),

                    CpuUsagePercentage = (float)Math.Round(_systemCounters["CPU"].NextValue(), 2),

                    LocalDiskTotalMBytes = (float)Math.Round(GetDiskTotalMbytes()),
                    LocalDiskUsageMBytes = (float)Math.Round(GetDiskUsageMbytes()),
                    LocalDiskUsagePercentage = (float)Math.Round(100 - localDisk, 2),

                    Processes = allProcesses,
                    ProcessesCount = allProcesses.Count,
                    Time = DateTime.Now
                };

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return dataItem;
        }

        private float GetTotalRAM()
        {
            var totalRam = 1.0f;

            using (var ramMonitor = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize,FreePhysicalMemory FROM Win32_OperatingSystem"))
            {
                foreach (ManagementObject objram in ramMonitor.Get())
                {
                    totalRam = Convert.ToUInt64(objram["TotalVisibleMemorySize"]) / 1024;    // Total RAM
                }
            }

            return totalRam;
        }


        private async Task<List<ProcessData>> GetProcesses()
        {
            var processes = Process.GetProcesses().Where(item => item.ProcessName != "Idle").ToArray();
            var result = new List<ProcessData>(processes.Length);
            var ListCPU = new Dictionary<int, float>(processes.Length);

            _processCpuCounters.Clear();

            foreach (var item in processes)
            {
                if (_processCpuCounters.ContainsKey(item.Id)) continue;

                var cpu = new PerformanceCounter("Process", "% Processor Time", item.ProcessName, true);
                cpu.NextValue();
                _processCpuCounters.Add(item.Id, cpu);
            }

            await Task.Delay(1000);

            foreach (var item in processes)
            {
                if (!_processCpuCounters.TryGetValue(item.Id, out var counter)) continue;
                ListCPU.Add(item.Id, (float)Math.Round(counter.NextValue() / Environment.ProcessorCount, 2));
                counter.Dispose();
            }

            foreach (var item in processes)
            {
                try
                {
                    var name = item.ProcessName;
                    item.Refresh();
                    var ramMBytes = (item.PrivateMemorySize64 / 1024) / 1024;
                    var pCpu = 0f;
                    if (ListCPU.TryGetValue(item.Id, out var cpuP))
                    {
                        pCpu = cpuP;
                    }
                    var pRam = (ramMBytes / _totalRamMbyte) * 100;
                    if (_processCpuCounters.TryGetValue(item.Id, out var counter))
                    {
                        counter.Dispose();
                    }
                    result.Add(new ProcessData
                    {
                        Name = name,
                        RamMBytes = ramMBytes,
                        PCpu = pCpu,
                        PRam = pRam
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            result = GroupProcesses(result);

            return result;
        }

        private List<ProcessData> GroupProcesses(List<ProcessData> processes)
        {
            var temp = processes.GroupBy(proc => proc.Name).Select(
                    group => new ProcessData
                    {
                        Name = group.Key,
                        PCpu = (float)Math.Round(group.Sum(proc => proc.PCpu), 2),
                        PRam = (float)Math.Round(group.Sum(proc => proc.PRam), 2),
                        RamMBytes = (float)Math.Round(group.Sum(proc => proc.RamMBytes), 2)
                    }).ToList();
            return temp;
        }
    }
#endif
}
