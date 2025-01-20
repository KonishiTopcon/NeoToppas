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
using System.Xml.Linq;
using System.Windows;

namespace Template.WPF.ViewModels
{
    public class LabelViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private readonly MessageService _message;
        private readonly DialogService _dialog;
        public ReactiveCommand PrintLabelButton { get; }
        public ReactivePropertySlim<string> AppText { get; } = new ReactivePropertySlim<string>();

        public LabelViewModel(TransitionService contentNavigation)
        {

            PrintLabelButton = new ReactiveCommand().WithSubscribe(() =>
            {
                // COMオブジェクトの作成
                var doc = new bpac.DocumentClass();

                // ファイルの読み込み (例: ラベルテンプレートファイル)
                if (doc.Open("テンプレートファイル名.lbx", false))
                {
                    // ラベルの印刷処理などを実行
                    doc.PrintOut(1, bpac.PrintOptionConstants.bpoDefault);
                    doc.Close();
                }
                else
                {
                    MessageBox.Show("ラベルテンプレートを開けませんでした。");
                }
            });
        }
    }
}
