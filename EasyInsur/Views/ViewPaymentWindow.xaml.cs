using EasyInsur.Models;
using EasyInsur.Modules;
using HandyControl.Data;
using HandyControl.Tools.Extension;
using Microsoft.Win32;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.XlsIO;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using MessageBox = HandyControl.Controls.MessageBox;

namespace EasyInsur.Views
{
    /// <summary>
    /// Interaction logic for ViewPaymentWindow
    /// </summary>
    public partial class ViewPaymentWindow : UserControl
    {
        public ViewPaymentWindow()
        {
            InitializeComponent();
            DataGrid.SearchHelper.AllowFiltering = true;
            DataGrid.SearchHelper.AllowCaseSensitiveSearch = false;
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
            if (!(args.Record as Transactions).EditedColumns.Contains(args.Column.MappingName))
                (args.Record as Transactions).EditedColumns.Add(args.Column.MappingName);

            //updates the current row index
            DataGrid.UpdateDataRow(args.RowColumnIndex.RowIndex);
        }

        private async void ExportExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions
            {
                ExcelVersion = ExcelVersion.Excel2013,
                AllowOutlining = (bool)AllowOutlining.IsChecked!,
                ExportAllPages = !(bool)ExportCurrentPageOnly.IsChecked!,
                ExportStackedHeaders = (bool)ShowStackedHeader.IsChecked!
            };
            await DataExportService.ExportToExcelAsync(DataGrid, options, "EasyInsure_Transactions", (bool)ShowStackedHeader.IsChecked!);
        }

        private async void ExportPDFButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new PdfExportingOptions
            {
                FitAllColumnsInOnePage = (bool)FitOnOnePage.IsChecked!,
                ExportFormat = (bool)ExportFormatted.IsChecked!,
                ExportStackedHeaders = (bool)ShowStackedHeader.IsChecked!,
                ExportAllPages = !(bool)ExportCurrentPageOnly.IsChecked!
            };
            var orientation = (bool)RadioLandscape.IsChecked! ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;
            await DataExportService.ExportToPdfAsync(DataGrid, options, orientation, "EasyInsure_Transactions");
        }
    }
}
