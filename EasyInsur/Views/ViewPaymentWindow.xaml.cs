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

        private void ExportExcelButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new ExcelExportingOptions
            {
                ExcelVersion = ExcelVersion.Excel2013,
                AllowOutlining = (bool)AllowOutlining.IsChecked!,
                ExportAllPages = !(bool)ExportCurrentPageOnly.IsChecked!,
                ExportStackedHeaders = (bool)ShowStackedHeader.IsChecked!

            };
            var excelEngine = DataGrid.ExportToExcel(DataGrid.View, options);
            var workBook = excelEngine.Excel.Workbooks[0];
            IRange filterRange;
            if ((bool)ShowStackedHeader.IsChecked!)
            {
                var range = "A" + (DataGrid.StackedHeaderRows.Count + 1).ToString() + ":" + workBook.Worksheets[0].UsedRange.End.AddressLocal;
                filterRange = workBook.Worksheets[0].Range[range];
            }
            else
            {
                filterRange = workBook.Worksheets[0].UsedRange;
            }
            workBook.Worksheets[0].AutoFilters.FilterRange = filterRange;
            var sfd = new SaveFileDialog
            {
                FilterIndex = 2,
                Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                FileName = "EasyInsure_Transactions"
            };

            if (sfd.ShowDialog() == true)
            {
                using (var stream = sfd.OpenFile())
                {
                    workBook.Version = sfd.FilterIndex == 1 ? ExcelVersion.Excel97to2003 : sfd.FilterIndex == 2 ? ExcelVersion.Excel2010 : ExcelVersion.Excel2013;
                    workBook.SaveAs(stream);
                }
                if (MessageBox.Show("Do you want to view the workbook?",
                                    "Workbook has been created",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    Util.StartProcess(sfd.FileName);
                }
            }
        }
        private void ExportPDFButton_Click(object sender, RoutedEventArgs e)
        {
            var options = new PdfExportingOptions()
            {
                FitAllColumnsInOnePage = (bool)FitOnOnePage.IsChecked!,
                ExportFormat = (bool)ExportFormatted.IsChecked!,
                ExportStackedHeaders = (bool)ShowStackedHeader.IsChecked!,
                ExportAllPages = !(bool)ExportCurrentPageOnly.IsChecked!
            };
            var document = new PdfDocument();
            document.PageSettings.Orientation = (bool)RadioLandscape.IsChecked! ? PdfPageOrientation.Landscape : PdfPageOrientation.Portrait;
            var page = document.Pages.Add();
            var pdfGrid = DataGrid.ExportToPdfGrid(DataGrid.View, options);
            var format = new PdfGridLayoutFormat()
            {
                Layout = PdfLayoutType.Paginate,
                Break = PdfLayoutBreakType.FitPage
            };

            pdfGrid.Draw(page, new PointF(), format);
            var sfd = new SaveFileDialog
            {
                Filter = "PDF Files(*.pdf)|*.pdf",
                FileName = "EasyInsure_Transactions"
            };

            if (sfd.ShowDialog() == true)
            {
                try
                {
                    using var stream = sfd.OpenFile();
                    document.Save(stream);
                }
                catch (System.Exception ex)
                {
                    MessageBox.Error(ex.Message);
                    return;
                }

                if (MessageBox.Show("Do you want to view the Pdf file?", "Pdf file has been created",
                                    MessageBoxButton.YesNo, MessageBoxImage.Information) == MessageBoxResult.Yes)
                {
                    Util.StartProcess(sfd.FileName);
                }
            }
        }
    }
}
