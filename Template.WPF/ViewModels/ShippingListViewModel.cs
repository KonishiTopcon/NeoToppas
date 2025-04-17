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
using DocumentFormat.OpenXml.Wordprocessing;

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
                        string pcName = Environment.MachineName;
                        DateTime now = DateTime.Now;
                        string datetimeStr = now.ToString("yyyyMMddHHmmss");
                        long datetimeNum = long.Parse(datetimeStr);
                        string hexStr = datetimeNum.ToString("X");
                        worksheetx.Cell("D11").Value = hexStr + "1";
                        worksheetx.Cell("D32").Value = hexStr + "2";
                        int p1 = 0;
                        int p2 = 0;
                        foreach (var ship in ShippingListTbl)
                        {
                            worksheetx.Cell("D2").Value = DateTime.Today;
                            worksheetx.Cell("D23").Value = DateTime.Today;

                            if (!string.IsNullOrWhiteSpace(ship.UnitName))
                            {
                                if (ship.Page == 1)
                                {
                                    worksheetx.Range("A" + (p1 + 5).ToString()).Value = ship.UnitCode;
                                    worksheetx.Range("B" + (p1 + 5).ToString()).Value = ship.UnitName;
                                    worksheetx.Range("C" + (p1 + 5).ToString()).Value = ship.ShippingCnt;
                                    worksheetx.Range("D" + (p1 + 5).ToString()).Value = ship.Bikou;
                                    ++p1;
                                } 
                                else
                                {
                                    worksheetx.Range("A" + (p2 + 26).ToString()).Value = ship.UnitCode;
                                    worksheetx.Range("B" + (p2 + 26).ToString()).Value = ship.UnitName;
                                    worksheetx.Range("C" + (p2 + 26).ToString()).Value = ship.ShippingCnt;
                                    worksheetx.Range("D" + (p2 + 26).ToString()).Value = ship.Bikou;
                                    ++p2;
                                }

                            }
                            worksheetx.PageSetup.PrintAreas.Clear();

                            if (p2 > 0)
                            {
                                worksheetx.PageSetup.PrintAreas.Add("A1:D42");
                            }
                            else worksheetx.PageSetup.PrintAreas.Add("A1:D21");
                        }
                        // 保存
                        workbooky.Save();
                        if (p2 <= 0 && p1 <= 0) return;
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
                    _message.ShowSnackbar("梱包明細のエクセルを開きます。");
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
                ResetDatagrid();
            });

            ResetDatagrid();
        }

        private void ResetDatagrid()
        {
            ShippingListTbl.Clear();
            for (int s = 0; s < 12; s++)
            {
                var entity = new ShippingListEntities
                {
                    Page = (s < 6) ? 1 : 2
                };

                // 変更監視を追加
                entity.PropertyChanged += ShippingItem_PropertyChanged;

                ShippingListTbl.Add(entity);
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ShippingItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ShippingListEntities item && e.PropertyName == nameof(item.UnitCode))
            {
                if (!string.IsNullOrWhiteSpace(item.UnitCode))
                {
                    var index = ShippingListTbl.IndexOf(item);
                    item.UnitName = $"{item.UnitCode}{index}";
                    item.RegisteredCnt = 777;
                }
                else
                {
                    item.UnitName = string.Empty;
                }
            }
        }
    }
}
