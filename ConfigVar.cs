using RIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RIS
{
    public class ConfigVar
    {
        public static string adminGroup = "مدير النظام";
        public static string recepGroup = "الاستقبال";
        public static string adminUser = "HosAdmin";

    }
    public enum RegStatus
    {
        update = 1,
        delete,
        insert
    }
}