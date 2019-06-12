using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VFS;

namespace Battlefield_2_BitStream.Data
{
    public static class ConMethods
    {
        public static void Initialize()
        {
            var processor = Mod.Instance.GetConFileProcessor();
            processor.RegisterConMethod("sv.serverName", SetServerName);
            processor.RegisterConMethod("sv.password", SetServerPassword);
            processor.RegisterConMethod("sv.ranked", SetRanked);
            processor.RegisterConMethod("sv.allowFreeCam", SetAllowFreeCam);
            processor.RegisterConMethod("sv.allowExternalViews", SetAllowExternalViews);
            processor.RegisterConMethod("sv.serverPort", SetServerPort);
            processor.RegisterConMethod("fileManager.mountArchive", MountArchive);
            processor.RegisterConMethod("ObjectTemplate.create", CreateObjectTemplate);
            processor.RegisterConMethod("Object.create", CreateObject);
            processor.RegisterConMethod("run", RunConFile);
            processor.RegisterConMethod("mapList.append", AppendMap);
        }

        public static int SetServerName(object variable, object variable2)
        {
            string name = (string)variable;
            Mod.Instance.ServerSettings.ServerName = name;
            return 0;
        }
        public static int SetServerPassword(object variable, object variable2)
        {
            string password = (string)variable;
            Mod.Instance.ServerSettings.ServerPassword = password;
            return 0;
        }
        public static int SetRanked(object variable, object variable2)
        {
            uint ranked = Convert.ToUInt32(variable);
            Mod.Instance.ServerSettings.Ranked = ranked;
            return 0;
        }
        public static int SetAllowFreeCam(object variable, object variable2)
        {
            uint allowFreeCam = Convert.ToUInt32(variable);
            Mod.Instance.ServerSettings.AllowFreeCam = allowFreeCam;
            return 0;
        }
        public static int SetAllowExternalViews(object variable, object variable2)
        {
            uint allowExternalViews = Convert.ToUInt32(variable);
            Mod.Instance.ServerSettings.AllowExternalViews = allowExternalViews;
            return 0;
        }
        public static int SetServerPort(object variable, object variable2)
        {
            Mod.BF2Engine.SetServerPort(Convert.ToUInt32(variable));
            return 0;
        }
        public static int MountArchive(object variable, object variable2)
        {
            var archivePath = Path.Combine(Application.StartupPath, "mods", "bf2", Convert.ToString(variable));
            VFileSystemManager.MountArchive(Convert.ToString(variable2), archivePath);
            return 0;
        }
        private static int i = 0;
        public static int CreateObjectTemplate(object variable, object variable2)
        {
            if (Convert.ToString(variable2) == "Ladder" || Convert.ToString(variable) == "Ladder")
                return 1;
            i++;
            return 0;
        }
        private static int p = 0;
        public static int CreateObject(object variable, object variable2)
        {
            if (Convert.ToString(variable) == "ladder_4m")
                return 1;
            p++;
            return 0;
        }
        public static int RunConFile(object variable, object variable2)
        {
            Mod.Instance.GetConFileProcessor().ExecuteConFile(Convert.ToString(variable));
            return 0;
        }
        private static int AppendMap(object variable, object variable2)
        {
            var map = new Map();
            map.MapName = Convert.ToString(variable);
            map.GameMode = Convert.ToString(variable2);
            map.MaxPlayers = 64;//hardcode for now
            Mod.BF2Engine.MapList.Add(map);
            return 0;
        }
    }
}