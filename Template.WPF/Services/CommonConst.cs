﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Toppas4.Services
{
    public static class CommonConst
    {
        //自動アップデート
        public static string AUTOUPDATE_VERSION_FILE = "NeoToppasLatestVersionTXT.txt";
        public static string CONFIGFILE_NAME = "Toppas4.dll.config";

        //ラベル印刷
        public static string GENPINLABEL_TEMPLATE_FILE = "Genpintemplate1.LBX";
        public static string TANABANLABEL_TEMPLATE_FILE = "Tanabantemplate.LBX";
        public static string TANABANLABELB_TEMPLATE_FILE = "Tanabantemplate_b.LBX";

        //棚札
        public static string TANAFUDA_TEMPLATE_FILE = "Tanafudatemplate.xlsx";
        //BOM
        public static string BOM_TEMPLATE_FILE = "BomForSAPtemplate.xlsx";

        public static string SHIPPINGLIST_TEMPLATE_FILE = "ShippingListtemplate.xlsm";

        public static string AppVersion;
    }
}
