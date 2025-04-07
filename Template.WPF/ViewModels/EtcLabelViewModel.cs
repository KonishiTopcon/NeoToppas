using MaterialDesignThemes.Wpf.Internal;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.WPF.Services;
using Template.WPF.UIEntities;
using Template.WPF.Views;
using bpac;
using Microsoft.Extensions.Hosting;
using System.Windows;
using Toppas4.Services;
using NeoToppas.Infrastructure.BrotherPrinter;

namespace Template.WPF.ViewModels
{
    public class EtcLabelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ReactivePropertySlim<string> SearchText { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ItemCode { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<int> Rev { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<String> ItemName { get; } = new ReactivePropertySlim<String>();
        public ReactivePropertySlim<DateOnly> ReceiptDate { get; } = new ReactivePropertySlim<DateOnly>();
        public ReactivePropertySlim<String> ShelfNo { get; } = new ReactivePropertySlim<String>();
        public ReactivePropertySlim<int> NumberOfPartsReceived { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<float> Cost { get; } = new ReactivePropertySlim<float>();
        public ReactivePropertySlim<int> PrintCnt { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<bool> RadioVersion { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> RadioSequence { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> RadioBT53Q { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<bool> Radio3DOCT { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<Visibility> VersionVisible { get; } = new ReactivePropertySlim<Visibility>();
        public ReactivePropertySlim<Visibility> SequenceVisible { get; } = new ReactivePropertySlim<Visibility>();
        public ReactivePropertySlim<Visibility> BT53QVisible { get; } = new ReactivePropertySlim<Visibility>();
        public ReactivePropertySlim<Visibility> OCTVisible { get; } = new ReactivePropertySlim<Visibility>();


        public ReactivePropertySlim<bool> isTainetsu { get; } = new ReactivePropertySlim<bool>();

        
        public ReactiveCommand PrintBtn { get; }
        public ReactiveCommand CancelBtn { get; }

        public EtcLabelViewModel(TransitionService contentNavigation)
        {
            ResetForm();
            RadioVersion.Value = true;
            VersionVisible.Value = Visibility.Visible;
            isTainetsu.Value = false;

            PrintBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
            });
            CancelBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
            });

        }

        public void ResetForm()
        {
            SearchText.Value = "";
            ItemCode.Value = "";
            Rev.Value = 0;
            ItemName.Value = "";
            ReceiptDate.Value = new DateOnly(1970, 1, 1);
            ShelfNo.Value = "";
            NumberOfPartsReceived.Value = 0;
            Cost.Value = 0;
            PrintCnt.Value = 1;
        }

        public void RadioVersionChange()
        {
            VersionVisible.Value = Visibility.Visible;
            SequenceVisible.Value = Visibility.Hidden;
            BT53QVisible.Value = Visibility.Hidden;
            OCTVisible.Value = Visibility.Hidden;
            return;
        }
        public void RadioSequenceChange()
        {
            VersionVisible.Value = Visibility.Hidden;
            SequenceVisible.Value = Visibility.Visible;
            BT53QVisible.Value = Visibility.Hidden;
            OCTVisible.Value = Visibility.Hidden;
            return;
        }

        public void RadioBT53QChange()
        {
            VersionVisible.Value = Visibility.Hidden;
            SequenceVisible.Value = Visibility.Hidden;
            BT53QVisible.Value = Visibility.Visible;
            OCTVisible.Value = Visibility.Hidden;
            return;
        }
        public void Radio3DOCTChange()
        {
            VersionVisible.Value = Visibility.Hidden;
            SequenceVisible.Value = Visibility.Hidden;
            BT53QVisible.Value = Visibility.Hidden;
            OCTVisible.Value = Visibility.Visible;  
            return;
        }
    }
}
