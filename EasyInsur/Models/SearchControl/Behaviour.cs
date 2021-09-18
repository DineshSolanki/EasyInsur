using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;
using Syncfusion.UI.Xaml.Grid;

namespace EasyInsur.Models.SearchControl
{
    public class Behaviour : Behavior<UserControl>
    {
        SfDataGrid? _dataGrid;
        SearchControl? _searchControl;
        protected override void OnAttached()
        {
            var window = AssociatedObject;
            _dataGrid = window.FindName("DataGrid") as SfDataGrid;
            _dataGrid.KeyDown += OnDataGridKeyDown;
            _searchControl = window.FindName("searchControl") as SearchControl;
        }

        /// <summary>
        /// Invokes this Event to show the AdonerDecorator in the view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnDataGridKeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None && e.Key == Key.F)
                _searchControl.UpdateSearchControlVisibility(true);
            else
                _searchControl.UpdateSearchControlVisibility(false);
        }
        protected override void OnDetaching()
        {
            _dataGrid.KeyDown -= OnDataGridKeyDown;
        }
    }
}
