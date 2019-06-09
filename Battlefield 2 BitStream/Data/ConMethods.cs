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
            processor.RegisterConMethod("fileManager.mountArchive", MountArchive);
            processor.RegisterConMethod("ObjectTemplate.create", CreateObjectTemplate);
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
            uint ranked = (uint)variable;
            Mod.Instance.ServerSettings.Ranked = ranked;
            return 0;
        }
        public static int SetAllowFreeCam(object variable, object variable2)
        {
            uint allowFreeCam = (uint)variable;
            Mod.Instance.ServerSettings.AllowFreeCam = allowFreeCam;
            return 0;
        }
        public static int SetAllowExternalViews(object variable, object variable2)
        {
            uint allowExternalViews = (uint)variable;
            Mod.Instance.ServerSettings.AllowExternalViews = allowExternalViews;
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
    }
}