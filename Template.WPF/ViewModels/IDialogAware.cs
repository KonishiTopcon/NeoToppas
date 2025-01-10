using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.WPF.ViewModels
{
    public interface IDialogAware
    {
        event EventHandler<bool> RequestClose;

        void OnDialogOpened(object parameter);
    }
}
