using System.Windows;
using System.Windows.Controls;
using EasyInsur.Models;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;

namespace EasyInsur.Views
{
    /// <summary>
    /// Interaction logic for ViewPeopleWindow
    /// </summary>
    public partial class ViewPeopleWindow : UserControl
    {
        public ViewPeopleWindow()
        {
            InitializeComponent();
            DataGrid.SearchHelper.AllowFiltering = true;
        }

        private void SearchBar_OnSearchStarted(object? sender, FunctionEventArgs<string> e)
        {
            DataGrid.SearchHelper.Search(e.Info);
        }

        private void FindNextClicked(object sender, RoutedEventArgs e)
        {
            if (SearchBarForGrid.Text.IsNullOrEmpty()) return;
            DataGrid.SearchHelper.FindNext(SearchBarForGrid.Text);
            DataGrid.SelectionController.MoveCurrentCell(DataGrid.SearchHelper.CurrentRowColumnIndex);
        }
        private void FindPreviousClicked(object sender, RoutedEventArgs e)
        {
            if (SearchBarForGrid.Text.IsNullOrEmpty()) return;
            DataGrid.SearchHelper.FindPrevious(SearchBarForGrid.Text);
            DataGrid.SelectionController.MoveCurrentCell(DataGrid.SearchHelper.CurrentRowColumnIndex);
        }

        private void DataGrid_OnCurrentCellValueChanged(object? sender, CurrentCellValueChangedEventArgs args)
        {
            if (!(args.Record as Person).EditedColumns.Contains(args.Column.MappingName))
                (args.Record as Person).EditedColumns.Add(args.Column.MappingName);

            //updates the current row index
            DataGrid.UpdateDataRow(args.RowColumnIndex.RowIndex);
        }
    }
}
