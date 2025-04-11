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
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using MaterialDesignThemes.Wpf;
using System.Windows.Forms;

namespace Template.WPF.ViewModels
{

    public class BomConverterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<BomExcelEntities> BomExcelTbl { get; set; } = new ObservableCollection<BomExcelEntities>();
        private BomExcelEntities _selectedBom;
        private readonly MessageService _message;
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

        public BomConverterViewModel(TransitionService contentNavigation, MessageService message, DialogService dialogService)
        {
            _message = message;
            FileSelectButton = new ReactiveCommand().WithSubscribe(async () =>
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Excel Files (*.xlsx)|*.xlsx",
                    Multiselect = false,
                    Title = "BOMファイル選択(事前にエクセルでxlsxに変換してください)"
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

                if (BomExcelTbl.Count <= 0) return;

                try
                {
                    using (var workbookx = new XLWorkbook(templatePath))
                    {
                        workbookx.SaveAs(outputExcelPath);
                    }
                    using (var workbooky = new XLWorkbook(outputExcelPath)) 
                    {
                        var worksheetx = workbooky.Worksheet(1);
                        int r_out = 2; //書き込み行番号 
                        foreach (var bom in BomExcelTbl)
                        {
                            int MeisaiNo = 10;
                            using (var workbooki = new XLWorkbook(bom.BomPath))
                            {
                                foreach (var s in workbooki.Worksheets)
                                {
                                    if (s.Name.StartsWith("差分"))
                                    {
                                        continue;
                                    }
                                    
                                    int r = 12; //読み込み行番号 
                                    while (s.Cell("E"+ r).Value.ToString() != "")
                                    {
                                        if (s.Cell("G" + r).Value.ToString() != "")
                                        {
                                            worksheetx.Cell("A" + r_out).Value = "M";
                                            worksheetx.Cell("B" + r_out).Value = s.Cell("L5").Value;
                                            worksheetx.Cell("C" + r_out).Value = "5335";
                                            worksheetx.Cell("D" + r_out).Value = "1";
                                            worksheetx.Cell("E" + r_out).Value = "1";
                                            worksheetx.Cell("H" + r_out).Value = "1";
                                            worksheetx.Cell("I" + r_out).Value = "MIG110001";
                                            worksheetx.Cell("J" + r_out).Value = DateTime.Today.ToString("yyyyMMdd");
                                            worksheetx.Cell("K" + r_out).Value = MeisaiNo;
                                            worksheetx.Cell("L" + r_out).Value = "L";
                                            worksheetx.Cell("M" + r_out).Value = s.Cell("G" + r).Value;
                                            worksheetx.Cell("N" + r_out).Value = s.Cell("J" + r).Value;
                                            worksheetx.Cell("O" + r_out).Value = "EA";
                                            worksheetx.Cell("S" + r_out).Value = "1000";
                                            ++r_out;
                                            MeisaiNo = MeisaiNo + 10;
                                        }
                                        r++;
                                    }
                                }
                            }
                        }
                        // 保存
                        workbooky.Save();
                    }
                }
                catch
                {
                    System.Windows.MessageBox.Show("エクセルファイルの保存に失敗しました。" + Environment.NewLine +
                        "エクセルファイルが開かれている場合は閉じてリトライしてください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                try
                {
                    string excelAppPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Microsoft Office", "root", "Office16", "EXCEL.EXE");

                    // Excelアプリケーションを起動し、ファイルパスを引数として渡す
                    Process.Start(excelAppPath, outputExcelPath);
                    _message.ShowSnackbar("SAP登録用のエクセルを開きます。");
                    //System.Windows.MessageBox.Show("SAP登録用のエクセルを開きます。");
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


        //public string ConvertXlsToXlsx(string xlsPath)
        //{
        //    var excelApp = new Excel.Application();
        //    Workbooks workbooks = excelApp.get_Workbooks();
        //    string xlsxPath = xlsPath + "x";

        //    try
        //    {
        //        excelApp = new Excel.Application();
        //        Excel.Workbook workbook = workbooks.Open(xlsPath, ReadOnly: false, Editable: true, CorruptLoad: Excel.XlCorruptLoad.xlNormalLoad);
        //        workbook.SaveAs2(xlsxPath, Excel.XlFileFormat.xlOpenXMLWorkbook);
        //        workbook.Close();

        //    }
        //    finally
        //    {
        //        workbooks?.Close();
        //        excelApp.Quit();
        //    }

        //    return xlsxPath;
        //}


    }
}
