using Microsoft.Extensions.Hosting;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Template.WPF.Services;
using Template.WPF.UIEntities;
using Template.WPF.Views;

namespace Template.WPF.ViewModels
{
    public class TanabanPrintViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public ReactivePropertySlim<bool> BarcodeExist { get; } = new ReactivePropertySlim<bool>();
        public ReactivePropertySlim<string> hinmoku0 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmoku1 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmoku2 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmokutext0 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmokutext1 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> hinmokutext2 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> shelf0 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> shelf1 { get; } = new ReactivePropertySlim<string>();
        public ReactivePropertySlim<string> shelf2 { get; } = new ReactivePropertySlim<string>();


        public ReactiveCommand DeleteHinmokuBtn0 { get; }
        public ReactiveCommand DeleteHinmokuBtn1 { get; }
        public ReactiveCommand DeleteHinmokuBtn2 { get; }
        public ReactiveCommand CancelBtn { get; }
        public TanabanPrintViewModel(TransitionService contentNavigation)
        {
            BarcodeExist.Value = false;
            hinmoku0.Value = "T47001441A";
            hinmoku1.Value = "T47001441B";
            hinmoku2.Value = "T47001441C";
            hinmokutext0.Value = "AD9920ARSX";
            hinmokutext1.Value = "AD9920ARSY";
            hinmokutext2.Value = "AD9920ARSZ";
            shelf0.Value = "IC-J-06";
            shelf1.Value = "IC-J-07";
            shelf2.Value = "IC-J-08";

            DeleteHinmokuBtn0 = new ReactiveCommand().WithSubscribe(async () =>
            {
                hinmoku0.Value = "";
                hinmokutext0.Value = "";
                shelf0.Value = "";
                return;
            });
            DeleteHinmokuBtn1 = new ReactiveCommand().WithSubscribe(async () =>
            {
                hinmoku1.Value = "";
                hinmokutext1.Value = "";
                shelf1.Value = "";
                return;
            });
            DeleteHinmokuBtn2 = new ReactiveCommand().WithSubscribe(async () =>
            {
                hinmoku2.Value = "";
                hinmokutext2.Value = "";
                shelf2.Value = "";
                return;
            });
            CancelBtn = new ReactiveCommand().WithSubscribe(async () =>
            {
                ResetForm();
                return;
            });


        }

        public void ResetForm()
        {
            hinmoku0.Value = "";
            hinmoku1.Value = "";
            hinmoku2.Value = "";
            hinmokutext0.Value = "";
            hinmokutext1.Value = "";
            hinmokutext2.Value = "";
            shelf0.Value = "";
            shelf1.Value = "";
            shelf2.Value = "";
        }

        public void Hinmoku0_LostFocus()
        {
            if (hinmoku0.Value != "")
            {
                hinmokutext0.Value = "AD9920ARSX";
                shelf0.Value = "IC-J-06";
                return;
            }
        }
        public void Hinmoku1_LostFocus()
        {
            if (hinmoku1.Value != "")
            {
                hinmokutext1.Value = "AD9920ARSY";
                shelf1.Value = "IC-J-07";
                return;
            }
        }
        public void Hinmoku2_LostFocus()
        {
            if (hinmoku2.Value != "")
            {
                hinmokutext2.Value = "AD9920ARSZ";
                shelf2.Value = "IC-J-08";
                return;
            }
        }
    }
}
