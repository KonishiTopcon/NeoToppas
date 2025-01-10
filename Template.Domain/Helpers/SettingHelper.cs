using System.Reflection;
using System.Text;

namespace Template.Domain.Helpers
{
    public sealed class Settinghelper
    {
        private static readonly string _settingFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\settings.config";

        //Setting data
        public bool IsEnabledSettingPasswordLock { get; set; } = false;
        public string SettingPasswordHash { get; set; } = string.Empty;
        public string SettingPasswordSalt { get; set; } = string.Empty;


        private static Settinghelper _instance;
        public static Settinghelper Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new Settinghelper();
                return _instance;
            }
            private set { _instance = value; }
        }

        /// <summary>
        /// 設定をファイルへ保存
        /// </summary>
        public static void SettingSave()
        {
            string fileDirectory = Settinghelper._settingFilePath;

            StreamWriter sw = new StreamWriter(fileDirectory, false, new UTF8Encoding(false));
            System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Settinghelper));

            xs.Serialize(sw, Instance);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 設定ファイルを読み込み
        /// </summary>
        public static void SettingLoad()
        {
            if (File.Exists(_settingFilePath))
            {
                string fileDirectory = Settinghelper._settingFilePath;

                using (StreamReader sr = new StreamReader(fileDirectory, new UTF8Encoding(false)))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(Settinghelper));

                    object obj = xs.Deserialize(sr);
                    sr.Close();

                    Instance = (Settinghelper)obj;
                };
            }
        }

        /// <summary>
        /// 設定ファイルを削除
        /// </summary>
        public static void SettingDelete()
        {
            if (File.Exists(_settingFilePath))
            {
                File.Delete(_settingFilePath);
            }
        }
    }
}