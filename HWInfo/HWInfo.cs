using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HWInfo
{
    class HWInfo
    {

        static System.Timers.Timer t;

        private static PerformanceCounter CPUUsage;
        private static PerformanceCounter AvailableMemory;

        public struct DiskInfo
        {
            public string Name { get; set; }
            public long TotalSize { get; set; }
            public long AvailableFreeSpace { get; set; }
        }

        public struct CPUInfo
        {
            public string ModelName { get; set; }
            public string Manufacturer { get; set; }
            public uint MaxClockSpeed { get; set; }
            public uint CurrentClockSpeed { get; set; }
        }

        static HWInfo()
        {
            t = new System.Timers.Timer();
            CPUUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            AvailableMemory = new PerformanceCounter("Memory", "Available MBytes");

            t.AutoReset = false;
            t.Elapsed += new System.Timers.ElapsedEventHandler(UpdateCPU);
            t.Interval = GetInterval();
            t.Start();

        }

        private static double GetInterval()
        {
            DateTime now = DateTime.Now;
            return ((now.Second > 30 ? 120 : 60) - now.Second) * 1000 - now.Millisecond;
        }

        public static string MachineName
        {
            get { return Environment.MachineName; }
        }

        public static string UserDomainName
        {
            get { return Environment.UserDomainName; }
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }


        //MBYTE
        public static int GetTotalPhysicalMemory()
        {
            var totalRam = new ComputerInfo().TotalPhysicalMemory;
            totalRam = totalRam / 1024 / 1024;
            return (int)totalRam;
        }

        public static float GetAvailableMemory()
        {
            return AvailableMemory.NextValue();
        }

        public static int GetMemoryInUse()
        {
            int inUse = GetTotalPhysicalMemory() - (int)GetAvailableMemory();
            return inUse;
        }


        //gives you is the average % of cpu seance the last NextValue() was made on the counter.
        //if you want the average cpu over the last 5 min, just call NextValue() only once every 5 min.

        public static float GetCPUUsage()
        {
            var cpu = CPUUsage.NextValue();
            return (float)Math.Round(cpu, 2);
        }


        public static CPUInfo GetCPUInfo()
        {
            using (ManagementObject Mo = new ManagementObject("Win32_Processor.DeviceID='CPU0'"))
            {
                CPUInfo cc = new CPUInfo
                {
                    ModelName = Mo["Name"].ToString(),
                    MaxClockSpeed = (uint)Mo["MaxClockSpeed"],
                    CurrentClockSpeed = (uint)Mo["CurrentClockSpeed"],
                    Manufacturer = Mo["Manufacturer"].ToString()
                };
                return cc;
            }
        }


        public static List<DiskInfo> GetDiskList()
        {
            List<DiskInfo> DriveList = new List<DiskInfo>();
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    DiskInfo nd = new DiskInfo
                    {
                        Name = drive.Name,
                        TotalSize = drive.TotalSize / 1024 / 1024 / 1024,
                        AvailableFreeSpace = drive.AvailableFreeSpace / 1024 / 1024 / 1024
                    };
                    DriveList.Add(nd);
                }
            }
            return DriveList;
        }


        private static void UpdateCPU(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("SAAT: " + DateTime.Now);
            Console.WriteLine("Son 1dklik kullanim orani: " + GetCPUUsage() + "%");
            Console.WriteLine();
            Console.WriteLine("Available RAM:" + GetAvailableMemory() + "MB");
            Console.WriteLine("IN-USE MEMO:" + GetMemoryInUse() + "MB");
            Console.WriteLine();
            Console.WriteLine();
            GC.Collect();
            t.Interval = GetInterval();
            t.Start();
        }
    }
}
