using System.Windows;
using System.Windows.Controls;
using HandyControl.Data;
using HandyControl.Tools.Extension;

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
    }
}
