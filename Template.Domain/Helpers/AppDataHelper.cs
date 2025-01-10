using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Domain.Helpers
{
    public class AppDataHelper
    {
        // 終了時に保持されない一時データ
        public string LoginUser { get; set; }


        private static AppDataHelper _instance;
        public static AppDataHelper Instance => _instance ??= new AppDataHelper();

        private AppDataHelper() { }
    }
}
