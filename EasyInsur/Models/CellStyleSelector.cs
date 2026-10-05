using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.Grid;

namespace EasyInsur.Models
{
    public class CellStyleSelector : StyleSelector
    {

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            if (container is not GridCell gridCell || gridCell.ColumnBase?.GridColumn == null)
                return base.SelectStyle(item, container);
            
            var record = item as EditableTableClass;

            if (record == null)
                return base.SelectStyle(item, container);

            return record.EditedColumns.Contains(gridCell.ColumnBase.GridColumn.MappingName)
                ? Application.Current.Resources["CellStyle"] as Style
                : base.SelectStyle(item, container);
        }
    }
}
