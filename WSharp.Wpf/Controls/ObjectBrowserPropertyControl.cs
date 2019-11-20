using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WSharp.Reflection;

namespace WSharp.Wpf.Controls
{
    public class ObjectBrowserPropertyControl : AControl
    {
        #region DEPENDENCY PROPERTIES

        public static readonly DependencyProperty ObjectBrowserPropertyProperty = DependencyProperty.Register(
            nameof(ObjectBrowserProperty),
            typeof(ObjectBrowserProperty),
            typeof(ObjectBrowserPropertyControl),
            new PropertyMetadata(default(ObjectBrowserProperty), OnObjectBrowserPropertyChanged));

        public static readonly DependencyProperty VisiblePropertiesProperty = DependencyProperty.Register(
            nameof(VisibleProperties),
            typeof(IEnumerable<ObjectBrowserProperty>),
            typeof(ObjectBrowserPropertyControl));

        public static readonly DependencyProperty AuthorizedLevelProperty = DependencyProperty.Register(
            nameof(AuthorizedLevel),
            typeof(int?),
            typeof(ObjectBrowserPropertyControl),
            new PropertyMetadata(null, OnAuthorizedLevelChanged));

        public static readonly DependencyProperty TemplateSelectorProperty = DependencyProperty.Register(
            nameof(TemplateSelector),
            typeof(DataTemplateSelector),
            typeof(ObjectBrowserPropertyControl),
            new PropertyMetadata(null, OnTemplateSelectorChanged));

        #endregion DEPENDENCY PROPERTIES

        #region CONSTRUCTORS

        static ObjectBrowserPropertyControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ObjectBrowserPropertyControl), new FrameworkPropertyMetadata(typeof(ObjectBrowserPropertyControl)));
        }

        #endregion CONSTRUCTORS

        #region PROPERTIES

        public ObjectBrowserProperty ObjectBrowserProperty
        {
            get => (ObjectBrowserProperty)GetValue(ObjectBrowserPropertyProperty);
            set => SetValue(ObjectBrowserPropertyProperty, value);
        }

        public IEnumerable<ObjectBrowserProperty> VisibleProperties
        {
            get => (IEnumerable<ObjectBrowserProperty>)GetValue(VisiblePropertiesProperty);
            private set => SetValue(VisiblePropertiesProperty, value);
        }

        public int? AuthorizedLevel
        {
            get => (int?)GetValue(AuthorizedLevelProperty);
            set => SetValue(AuthorizedLevelProperty, value);
        }

        public DataTemplateSelector TemplateSelector
        {
            get => (DataTemplateSelector)GetValue(TemplateSelectorProperty);
            set => SetValue(TemplateSelectorProperty, value);
        }

        #endregion PROPERTIES

        #region METHODS

        #region callbacks

        private static void OnObjectBrowserPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ObjectBrowserPropertyControl control) || Equals(e.NewValue, e.OldValue))
                return;

            control.UpdateVisibleProperties();
        }

        private static void OnAuthorizedLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ObjectBrowserPropertyControl control) || Equals(e.NewValue, e.OldValue))
                return;

            control.UpdateVisibleProperties();
        }

        private static void OnTemplateSelectorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ObjectBrowserPropertyControl control) || Equals(e.NewValue, e.OldValue))
                return;

            control.UpdateTreeView();
        }

        #endregion callbacks

        public void UpdateVisibleProperties()
        {
            if (ObjectBrowserProperty?.TypeProperties == null)
            {
                VisibleProperties = null;
                return;
            }

            var props = ObjectBrowserProperty.TypeProperties.Where(x => x.IsBrowsable);

            var level = AuthorizedLevel;
            props = AuthorizedLevel == null
                   ? props.Where(x => x.AuthorizedLevels == 0)
                   : props.Where(x => (x.AuthorizedLevels & level) > 0);

            VisibleProperties = props;
        }

        public void UpdateTreeView()
        {
            //if (_treeView == null)
            //    return;

            //_treeView.Items.Clear();
            //foreach (var item in VisibleProperties.Select(GenerateTreeViewItem))
            //    _treeView.Items.Add(item);
        }

        //private object GenerateTreeViewItem(ObjectBrowserProperty property)
        //{
        //    var binding = new Binding()
        //    {
        //        Source = property,
        //        Mode = BindingMode.OneWay
        //    };
        //    ContentControl control = new TreeViewItem { ContentTemplate = TemplateSelector.SelectTemplate(property, _treeView) };
        //    control.SetBinding(ContentControl.ContentProperty, binding);
        //    return control;
        //}

        #endregion METHODS
    }
}