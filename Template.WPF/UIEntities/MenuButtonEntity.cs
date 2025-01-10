using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.WPF.UIEntities
{
    public class MenuButtonEntity
    {
        public string Text { get; }
        public ReactiveCommand Command { get; }

        public MenuButtonEntity(string text, Action command)
        {
            Text = text;
            Command = new ReactiveCommand().WithSubscribe(command);
        }
    }
}
