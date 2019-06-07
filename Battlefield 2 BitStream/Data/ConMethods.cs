﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }
        public static int SetServerName(object variable)
        {
            string name = (string)variable;
            Mod.Instance.ServerSettings.ServerName = name;
            return 0;
        }
        public static int SetServerPassword(object variable)
        {
            string password = (string)variable;
            Mod.Instance.ServerSettings.ServerPassword = password;
            return 0;
        }
        public static int SetRanked(object variable)
        {
            uint ranked = (uint)variable;
            Mod.Instance.ServerSettings.Ranked = ranked;
            return 0;
        }
        public static int SetAllowFreeCam(object variable)
        {
            uint allowFreeCam = (uint)variable;
            Mod.Instance.ServerSettings.AllowFreeCam = allowFreeCam;
            return 0;
        }
        public static int SetAllowExternalViews(object variable)
        {
            uint allowExternalViews = (uint)variable;
            Mod.Instance.ServerSettings.AllowExternalViews = allowExternalViews;
            return 0;
        }
    }
}