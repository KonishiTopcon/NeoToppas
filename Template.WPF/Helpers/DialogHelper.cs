using System.IO;

namespace Template.WPF.Helpers
{
    public static class DialogHelper
    {
        /// <summary>
        /// フォルダ選択ダイアログを表示
        /// </summary>
        /// <returns>選択されたパス</returns>
        public static string SelectFolderDialog()
        {
            return SelectFolderDialog(string.Empty);
        }
        /// <summary>
        /// フォルダ選択ダイアログを表示
        /// </summary>
        /// <param name="defaultDirectory">初期表示ディレクトリ</param>
        /// <returns>選択されたパス</returns>
        public static string SelectFolderDialog(string defaultDirectory)
        {
            // Configure open folder dialog box
            Microsoft.Win32.OpenFolderDialog dialog = new()
            {
                Multiselect = false,
                Title = "フォルダを選択してください"
            };

            if (Directory.Exists(defaultDirectory))
            {
                dialog.DefaultDirectory = defaultDirectory;
            }
            else
            {
                dialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }

            if (dialog.ShowDialog() is true)
            {
                return dialog.FolderName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// ファイル選択ダイアログを表示
        /// </summary>
        /// <returns>選択されたパス</returns>
        public static string SelectFileDialog()
        {
            return SelectFileDialog(string.Empty, null);
        }
        /// <summary>
        /// ファイル選択ダイアログを表示
        /// </summary>
        /// <param name="defaultDirectory">初期表示ディレクトリ</param>
        /// <returns>選択されたパス</returns>
        public static string SelectFileDialog(string defaultDirectory)
        {
            return SelectFileDialog(defaultDirectory, null);
        }
        /// <summary>
        /// ファイル選択ダイアログを表示
        /// </summary>
        /// <param name="filters">ファイル選択フィルタ</param>
        /// <returns>選択されたパス</returns>
        private static string SelectFileDialog(List<(string displayName, string extension)> filters)
        {
            return SelectFileDialog(string.Empty, filters);
        }
        /// <summary>
        /// ファイル選択ダイアログを表示
        /// </summary>
        /// <param name="defaultDirectory">初期表示ディレクトリ</param>
        /// <param name="filters">ファイル選択フィルタ</param>
        /// <returns>選択されたパス</returns>
        private static string SelectFileDialog(string defaultDirectory, List<(string displayName, string extension)> filters)
        {
            // Configure open file dialog box
            Microsoft.Win32.OpenFileDialog dialog = new()
            {
                Title = "ファイルを選択してください",
                Multiselect = false,
            };

            if (filters is not null && filters.Count > 0)
            {
                // フィルタ文字列を生成
                List<string> filterStrings = new();
                foreach (var filter in filters)
                {
                    filterStrings.Add($"{filter.displayName}|{filter.extension}");
                }
                // フィルタを設定
                dialog.Filter = string.Join("|", filterStrings);
            }

            if (Directory.Exists(defaultDirectory))
            {
                dialog.DefaultDirectory = defaultDirectory;
            }
            else
            {
                dialog.DefaultDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            }

            if (dialog.ShowDialog() is true)
            {
                return dialog.FileName;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 画像ファイル選択ダイアログを表示(bmp/jpg/png)
        /// </summary>
        /// <returns></returns>
        public static string SelectImageFileDialog()
        {
            return SelectImageFileDialog(string.Empty);
        }
        /// <summary>
        /// 画像ファイル選択ダイアログを表示(bmp/jpg/png)
        /// </summary>
        /// <param name="defaultDirectory">初期表示ディレクトリ</param>
        /// <returns></returns>
        public static string SelectImageFileDialog(string defaultDirectory)
        {
            var filters = new List<(string displayName, string extension)>
        {
            GetImageFileDialogFilter(),
        };

            return SelectFileDialog(defaultDirectory, filters);
        }

        /// <summary>
        /// CSV選択ダイアログを表示
        /// </summary>
        /// <returns></returns>
        public static string SelectCSVFileDialog()
        {
            return SelectCSVFileDialog(string.Empty);
        }
        /// <summary>
        /// CSV選択ダイアログを表示
        /// </summary>
        /// <param name="defaultDirectory">初期表示ディレクトリ</param>
        /// <returns></returns>
        public static string SelectCSVFileDialog(string defaultDirectory)
        {
            var filters = new List<(string displayName, string extension)>
        {
            GetCSVFileDialogFilter(),
        };

            return SelectFileDialog(defaultDirectory, filters);
        }

        /// <summary>
        /// 画像ファイルフィルタ取得
        /// </summary>
        /// <returns></returns>
        private static (string displayName, string extension) GetImageFileDialogFilter()
        {
            string displayName = "Image Files";
            string extension = "*.bmp;*.jpg;*.jpeg;*.png;*.gif;*.tiff;";

            return (displayName, extension);
        }

        /// <summary>
        /// CSVファイルフィルタ取得
        /// </summary>
        /// <returns></returns>
        private static (string displayName, string extension) GetCSVFileDialogFilter()
        {
            string displayName = "CSV File";
            string extension = "*.csv"; // 拡張子を指定

            return (displayName, extension);
        }
    }
}
