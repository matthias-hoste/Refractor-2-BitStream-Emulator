using Battlefield_BitStream.Core.Data;
using Battlefield_BitStream.Core.Engine;
using Battlefield_BitStream.Core.Registry;
using Battlefield_BitStream.Forms;
using Battlefield_BitStream_Common;
using Battlefield_BitStream_Common.GameEvents;
using Battlefield_BitStream_Common.Processors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VFS;

namespace Battlefield_BitStream
{
    internal static class Config
    {
        internal static string ModName { get; private set; }
        internal static string GamePassword { get; private set; }
        internal static BF2Engine BF2Engine { get; private set; }
        internal static string ModPath { get; set; }
        internal static IEventRegistry EventRegistry { get; set; }
        internal static IMod LoadedMod { get; private set; }
        internal static Assembly ModAssembly { get; private set; }
        internal static IConFileProcessor ConFileProcessor { get; private set; }
        internal static IBlockEvent ServerInfo { get; set; }
        internal static void Initialize(string[] args = null)
        {
            if(args != null)
            {
                for(int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "--mod" && Directory.Exists(Path.Combine(Application.StartupPath, "mods", args[i + 1])))
                        ModName = args[i + 1];
                    if (args[i] == "--visisble3d")//this opens a form and renders the gameplay in realtime(will certainly make it slower)
                        new FrmMain().Show();
                }
            }
            EventRegistry = new GameEventRegistry();
            BF2Engine = new BF2Engine();
            if (string.IsNullOrEmpty(ModName))
            {
                Console.WriteLine("No mod was defined, which mod would you like to load?");
                var directories = Directory.EnumerateDirectories(Path.Combine(Application.StartupPath, "mods")).Select(d => new DirectoryInfo(d).Name);
                for (int i = 0; i < directories.Count(); i++)
                    Console.WriteLine("[" + i + "] " + directories.ToArray()[i]);
                Console.Write(">");
                var mod = Console.ReadLine();
                if(!directories.Contains(mod))
                {
                    Console.WriteLine("Invalid mod! Press any key to exit...");
                    Console.ReadKey();
                    Environment.Exit(-1);
                }
                ModName = "mods/" + mod;
                ModAssembly = Assembly.Load(File.ReadAllBytes(Path.Combine(Application.StartupPath, ModName, "phoenixmod.dll")));
                ModPath = Path.Combine(Application.StartupPath, ModName);
                LoadedMod = (IMod)Activator.CreateInstance(ModAssembly.GetTypes().Where(x => x.GetInterfaces().Contains(typeof(IMod))).First());
                ConFileProcessor = LoadedMod.GetConFileProcessor();
            }
            LoadedMod.Initialize(BF2Engine, EventRegistry);
            Console.WriteLine("Loaded " + LoadedMod.Name);
            BF2Engine.InitEngine();
            BF2Engine.LoadServerArchives();
        }
    }
}