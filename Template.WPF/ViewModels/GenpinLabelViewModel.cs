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

namespace Template.WPF.ViewModels
{
    public class GenpinLabelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ReactivePropertySlim<string> SearchText { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> ItemCode { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<int> Rev { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<String> ItemName { get; } = new ReactivePropertySlim<String>();
        public ReactivePropertySlim<DateOnly> ReceiptDate { get; } = new ReactivePropertySlim<DateOnly>();
        public ReactivePropertySlim<String> ShelfNo { get; } = new ReactivePropertySlim<String>();
        public ReactivePropertySlim<int> NumberOfPartsReceived { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<int> PrintCnt { get; } = new ReactivePropertySlim<int>();
        public ReactivePropertySlim<bool> UpdateReelNum { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<int> SplitSize { get; } = new ReactivePropertySlim<int>();



        public ReactiveCommand DeleteSearchTextBtn { get; }

        public GenpinLabelViewModel(TransitionService contentNavigation)
        {
            SearchText.Value = "";
            ItemCode.Value = "T47001441A";
            Rev.Value = 0;
            ItemName.Value = "AD9920ARSZ";
            ReceiptDate.Value = DateOnly.FromDateTime(DateTime.Now);//今日の日時を取得
            ShelfNo.Value = "IC-J-06";
            NumberOfPartsReceived.Value = 340;
            PrintCnt.Value = 2;
            UpdateReelNum.Value = true;
            SplitSize.Value = 170;
            DeleteSearchTextBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
                SearchText.Value = "";
                return;
            });
        }
    }
}
