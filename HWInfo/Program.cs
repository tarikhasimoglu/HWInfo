using System;
using System.Collections.Generic;
using static HWInfo.HWInfo;

namespace HWInfo
{
    class Program
    {
        static void Main(string[] args)
        {

            CPUInfo cpu = GetCPUInfo();
            List<DiskInfo> disks = GetDiskList();

            Console.WriteLine();
            Console.WriteLine();

            Console.Write("Bilgisayar Ismi:" + MachineName);
            Console.Write("  IP:" + GetLocalIPAddress());
            Console.WriteLine("  Ag Adi:" + UserDomainName);

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("***CPU***");
            Console.WriteLine();
            Console.WriteLine("Marka: " + cpu.Manufacturer);
            Console.WriteLine("Model: " + cpu.ModelName);
            Console.WriteLine("MAX HIZ: " + cpu.MaxClockSpeed / (1000f) + " GHZ");

            Console.WriteLine();
            Console.WriteLine();


            Console.WriteLine("***Memory***");
            Console.WriteLine();
            Console.WriteLine("TOTAL RAM:" + GetTotalPhysicalMemory() + "MB");
            Console.WriteLine("Available RAM:" + GetAvailableMemory() + "MB");
            Console.WriteLine("IN-USE MEMO:" + GetMemoryInUse() + "MB");

            Console.WriteLine();
            Console.WriteLine();

            Console.WriteLine("***DISK***");
            Console.WriteLine();

            foreach (var disk in disks)
            {
                Console.WriteLine(
                    "DISK:" + disk.Name
                    + " Toplam:" + disk.TotalSize + "GB"
                    + " Kullanilan:" + (disk.TotalSize - disk.AvailableFreeSpace) + "GB"
                    + " Kullanilabilir:" + disk.AvailableFreeSpace + "GB");

            }

            Console.WriteLine("----------------------------------------------------");

            Console.WriteLine();
            Console.WriteLine();
            Console.ReadKey();
        }
    }
}
