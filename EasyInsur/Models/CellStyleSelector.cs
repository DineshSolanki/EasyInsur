using System.Windows;
using System.Windows.Controls;
using Syncfusion.UI.Xaml.Grid;

namespace EasyInsur.Models
{
    public class CellStyleSelector : StyleSelector
    {

        public override Style? SelectStyle(object item, DependencyObject container)
        {
            var gridCell = container as GridCell;

            if (gridCell.ColumnBase?.GridColumn == null)
                base.SelectStyle(item, container);
            
            var record = item as EditableTableClass;

            return record.EditedColumns.Contains(gridCell.ColumnBase.GridColumn.MappingName)
                ? Application.Current.Resources["CellStyle"] as Style
                : base.SelectStyle(item, container);
        }
    }
}
