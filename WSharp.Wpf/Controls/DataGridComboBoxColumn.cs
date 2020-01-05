using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WSharp.Wpf.Controls
{
    public class DataGridComboBoxColumn : System.Windows.Controls.DataGridComboBoxColumn //DataGridBoundColumn
    {
        static DataGridComboBoxColumn()
        {
            ElementStyleProperty.OverrideMetadata(typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(DefaultElementStyle));
            EditingElementStyleProperty.OverrideMetadata(typeof(DataGridComboBoxColumn), new FrameworkPropertyMetadata(DefaultEditingElementStyle));
        }

        public Binding ItemsSourceBinding { get; set; }

        public bool? IsEditable { get; set; }

        public static new Style DefaultElementStyle
        {
            get
            {
                var brushKey = new ComponentResourceKey(typeof(ComboBox), "MaterialDataGridComboBoxColumnStyle");
                return (Style)new ComboBox().TryFindResource(brushKey);
            }
        }

        public static new Style DefaultEditingElementStyle
        {
            get
            {
                var brushKey = new ComponentResourceKey(typeof(ComboBox), "MaterialDataGridComboBoxColumnEditingStyle");
                return (Style)new ComboBox().TryFindResource(brushKey);
            }
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var comboBox = base.GenerateElement(cell, cell);

            if (ItemsSourceBinding != null)
                _ = comboBox.SetBinding(ItemsControl.ItemsSourceProperty, ItemsSourceBinding);

            ApplyStyle(false, false, comboBox);

            return comboBox;
        }

        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            var comboBox = (ComboBox)base.GenerateElement(cell, cell);
         
            if (IsEditable is bool isEditable)
                comboBox.IsEditable = isEditable;

            if (ItemsSourceBinding != null)
                _ = comboBox.SetBinding(ItemsControl.ItemsSourceProperty, ItemsSourceBinding);

            ApplyStyle(true, false, comboBox);

            return comboBox;
        }

        private void ApplyStyle(bool isEditing, bool defaultToElementStyle, FrameworkElement element)
        {
            var style = PickStyle(isEditing, defaultToElementStyle);
            if (style != null)
                element.Style = style;
        }

        private Style PickStyle(bool isEditing, bool defaultToElementStyle)
        {
            var style = isEditing ? EditingElementStyle : ElementStyle;
            if (isEditing && defaultToElementStyle && (style == null))
                style = ElementStyle;

            return style;
        }
    }
}
