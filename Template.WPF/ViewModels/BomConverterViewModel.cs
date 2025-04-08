using ClosedXML.Excel;
using NeoToppas.Domain.Entities;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Template.WPF.Services;
using Template.WPF.UIEntities;
using Template.WPF.Views;
using Toppas4.Services;

namespace Template.WPF.ViewModels
{
    public class BomConverterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<BomExcelEntities> BomExcelTbl { get; set; } = new ObservableCollection<BomExcelEntities>();
        private BomExcelEntities _selectedBom;
        public BomExcelEntities SelectedBom
        {
            get => _selectedBom;
            set
            {
                if (_selectedBom != value)
                {
                    _selectedBom = value;
                    OnPropertyChanged(nameof(SelectedBom));
                    OnSelectedBomChanged();
                }
            }
        }

        public ReactiveCommand FileSelectButton { get; }
        public ReactiveCommand FileDelButton { get; }
        public ReactiveCommand ConvertButton { get; }
        public ReactiveCommand CancelButton { get; }

        public BomConverterViewModel(TransitionService contentNavigation)
        {
            FileSelectButton = new ReactiveCommand().WithSubscribe(async () =>
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Excel Files (*.xlsx;*.xls)|*.xlsx;*.xls",
                    Multiselect = false
                };
                if (openFileDialog.ShowDialog() == true)
                {
                    var filePath = openFileDialog.FileName;
                    if (BomExcelTbl.Any(x => x.BomPath == filePath))
                    {
                        System.Windows.MessageBox.Show("同じファイルが既に追加されています。", "エラー", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                        return; // 処理中断
                    }

                    BomExcelTbl.Add(new BomExcelEntities
                    {
                        BomName = System.IO.Path.GetFileName(filePath),
                        BomPath = filePath
                    });
                }
            });

            FileDelButton = new ReactiveCommand().WithSubscribe(async () =>
            {
                if (SelectedBom != null)
                {
                    BomExcelTbl.Remove(SelectedBom);
                }
            });
            ConvertButton = new ReactiveCommand().WithSubscribe(async () =>
            {
                string templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates") +
                 @"\" + CommonConst.BOM_TEMPLATE_FILE;
                string outputExcelPath = ($"C\\Toppas4\\Temporary") + @"\" + "SAP登録用.xlsx";
                // ファイルが使用中かどうかを確認
                try
                {
                    // 1. Excelを開いてデータを入力
                    using (var workbookx = new XLWorkbook(templatePath))
                    {
                        workbookx.SaveAs(outputExcelPath);
                    }
                    using (var workbooky = new XLWorkbook(outputExcelPath)) 
                    {
                        var worksheetx = workbooky.Worksheet(1);
                        int i = 2;
                        foreach (var bom in BomExcelTbl)
                        {
                            using (var workbooki = new XLWorkbook(bom.BomPath))
                            {
                                worksheetx.Cell("B" + i).Value = workbooki.Worksheet(1).Cell("L5").Value;
                                i++;
                            }
                        }
                        // 保存
                        workbooky.Save();
                        workbooky.Dispose();
                    }
                }
                catch
                {
                    System.Windows.MessageBox.Show("エクセルファイルの保存に失敗しました。" + Environment.NewLine +
                        "棚札印刷用.xlsxが開かれている場合は閉じてリトライしてください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    string excelAppPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft Office", "root", "Office16", "EXCEL.EXE");

                    // Excelアプリケーションを起動し、ファイルパスを引数として渡す
                    Process.Start(excelAppPath, outputExcelPath);
                    System.Windows.MessageBox.Show("SAP登録用のエクセルを開きます。");
                }
                catch
                {
                    System.Windows.MessageBox.Show("エクセルファイルを開けませんでした", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                return;
            });

            CancelButton = new ReactiveCommand().WithSubscribe(async () =>
            {
                BomExcelTbl.Clear();
            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        private void OnSelectedBomChanged()
        {
        }

    }
}
