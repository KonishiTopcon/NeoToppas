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

    public class ShippingListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ObservableCollection<ShippingListEntities> ShippingListTbl { get; set; } = new ObservableCollection<ShippingListEntities>();
        private ShippingListEntities _selectedShippingList;
        private readonly MessageService _message;
        public ShippingListEntities SelectedShippingList
        {
            get => _selectedShippingList;
            set
            {
                if (_selectedShippingList != value)
                {
                    _selectedShippingList = value;
                    OnPropertyChanged(nameof(SelectedShippingList));
                }
            }
        }

        public ReactiveCommand ConvertButton { get; }
        public ReactiveCommand CancelButton { get; }

        public ShippingListViewModel(TransitionService contentNavigation, MessageService message, DialogService dialogService)
        {
            _message = message;
            ConvertButton = new ReactiveCommand().WithSubscribe(async () =>
            {
                string templatePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Templates") +
                 @"\" + CommonConst.SHIPPINGLIST_TEMPLATE_FILE;
                string outputExcelPath = ($"C\\Toppas4\\Temporary") + @"\" + "梱包明細.xlsm";

                if (ShippingListTbl.Count <= 0) return;

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
                        foreach (var ship in ShippingListTbl)
                        {
                            int MeisaiNo = 10;
                            //using (var workbooki = new XLWorkbook(ship.BomPath))
                            //{
                            //    foreach (var s in workbooki.Worksheets)
                            //    {
                            //        if (s.Name.StartsWith("差分"))
                            //        {
                            //            continue;
                            //        }
                                    
                            //        int r = 12; //読み込み行番号 
                            //        while (s.Cell("E"+ r).Value.ToString() != "")
                            //        {
                            //            if (s.Cell("G" + r).Value.ToString() != "")
                            //            {
                            //                worksheetx.Cell("A" + r_out).Value = "M";
                            //                worksheetx.Cell("B" + r_out).Value = s.Cell("L5").Value;
                            //                worksheetx.Cell("C" + r_out).Value = "5335";
                            //                worksheetx.Cell("D" + r_out).Value = "1";
                            //                worksheetx.Cell("E" + r_out).Value = "1";
                            //                worksheetx.Cell("H" + r_out).Value = "1";
                            //                worksheetx.Cell("I" + r_out).Value = "MIG110001";
                            //                worksheetx.Cell("J" + r_out).Value = DateTime.Today.ToString("yyyyMMdd");
                            //                worksheetx.Cell("K" + r_out).Value = MeisaiNo;
                            //                worksheetx.Cell("L" + r_out).Value = "L";
                            //                worksheetx.Cell("M" + r_out).Value = s.Cell("G" + r).Value;
                            //                worksheetx.Cell("N" + r_out).Value = s.Cell("J" + r).Value;
                            //                worksheetx.Cell("O" + r_out).Value = "EA";
                            //                worksheetx.Cell("S" + r_out).Value = "1000";
                            //                ++r_out;
                            //                MeisaiNo = MeisaiNo + 10;
                            //            }
                            //            r++;
                            //        }
                            //    }
                            //}
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
                ShippingListTbl.Clear();
                for (int s = 0; s < 12; s++)
                {
                    if (s < 6)
                    {
                        ShippingListTbl.Add(new ShippingListEntities
                        {
                            Page = 1
                        });
                    }
                    else
                    {
                        ShippingListTbl.Add(new ShippingListEntities
                        {
                            Page = 2
                        });
                    }
                }
            });
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
