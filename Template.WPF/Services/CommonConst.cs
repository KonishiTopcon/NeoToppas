using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NeoToppas.WPF.Services
{
    public static class CommonConst
    {
        //自動アップデート
        public static bool ENABLE_AUTOUPDATE = true;
        public static string DATA_FOLDER = @"\\PCD01851\SharedFolder\NeoToppasLatestVersion";//@"S:\トプコン\【生産本】\〈生技〉\〔生ＤＸ〕\!小西\2.NeoToppas\1.AutoUpdater";
        public static string APP_VERSION = "1.000";

        //ラベル印刷
        public static string TEMPLATE_DIRECTORY = @"C:\Users\l057277\OneDrive - Topcon\ドキュメント\各種案件\7.TOPPAS置き換え\3.ラベルプリンタ\ブラザーラベルプリンタ\テンプレート\";
        public static string TEMPLATE_FILE = "PCBtemplate1.LBX";

    }
}
