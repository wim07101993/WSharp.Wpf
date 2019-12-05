using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using WSharp.Wpf.LabelProxy;

namespace WSharp.Wpf
{
    public static class LabelProxyProvider
    {
        private static readonly List<LabelProxyBuilder> Builders = new List<LabelProxyBuilder>();

        static LabelProxyProvider()
        {
            Builders.Add(new LabelProxyBuilder(c => c is ComboBox, c => new ComboBoxLabelProxy((ComboBox)c)));
            Builders.Add(new LabelProxyBuilder(c => c is TextBox, c => new TextBoxLabelProxy((TextBox)c)));
            Builders.Add(new LabelProxyBuilder(c => c is RichTextBox, c => new RichTextBoxLabelProxy((RichTextBox)c)));
            Builders.Add(new LabelProxyBuilder(c => c is PasswordBox, c => new PasswordBoxLabelProxy((PasswordBox)c)));
        }

        public static void RegisterBuilder(Func<Control, bool> canBuild, Func<Control, ILabelProxy> build)
        {
            Builders.Add(new LabelProxyBuilder(canBuild, build));
        }

        public static ILabelProxy Get(Control control)
        {
            var builder = Builders.FirstOrDefault(v => v.CanBuild(control));

            if (builder == null) 
                throw new NotImplementedException();

            return builder.Build(control);
        }

        private sealed class LabelProxyBuilder
        {
            private readonly Func<Control, bool> _canBuild;
            private readonly Func<Control, ILabelProxy> _build;

            public LabelProxyBuilder(Func<Control, bool> canBuild, Func<Control, ILabelProxy> build)
            {
                _canBuild = canBuild ?? throw new ArgumentNullException(nameof(canBuild));
                _build = build ?? throw new ArgumentNullException(nameof(build));
            }

            public bool CanBuild(Control control) => _canBuild(control);
            public ILabelProxy Build(Control control) => _build(control);
        }
    }
}
