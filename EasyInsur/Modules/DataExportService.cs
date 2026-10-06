using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using HandyControl.Controls;
using Microsoft.Win32;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Grid;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using MessageBox = HandyControl.Controls.MessageBox;

namespace EasyInsur.Modules
{
    public static class DataExportService
    {
        public static async Task ExportToExcelAsync(
            SfDataGrid dataGrid,
            ExcelExportingOptions options,
            string defaultFileName,
            bool applyStackedHeaderFilter = false)
        {
            try
            {
                var sfd = new SaveFileDialog
                {
                    FilterIndex = 2,
                    Filter = "Excel 97 to 2003 Files(*.xls)|*.xls|Excel 2007 to 2010 Files(*.xlsx)|*.xlsx|Excel 2013 File(*.xlsx)|*.xlsx",
                    FileName = defaultFileName
                };

                if (sfd.ShowDialog() != true) return;

                var excelEngine = dataGrid.ExportToExcel(dataGrid.View, options);
                var workBook = excelEngine.Excel.Workbooks[0];

                if (applyStackedHeaderFilter)
                {
                    var range = "A" + (dataGrid.StackedHeaderRows.Count + 1).ToString() + ":" + workBook.Worksheets[0].UsedRange.End.AddressLocal;
                    workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].Range[range];
                }
                else
                {
                    workBook.Worksheets[0].AutoFilters.FilterRange = workBook.Worksheets[0].UsedRange;
                }

                workBook.Version = sfd.FilterIndex switch
                {
                    1 => ExcelVersion.Excel97to2003,
                    2 => ExcelVersion.Excel2010,
                    _ => ExcelVersion.Excel2013
                };

                var filePath = sfd.FileName;
                await Task.Run(() =>
                {
                    using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    workBook.SaveAs(stream);
                });

                Growl.Success($"Excel file '{Path.GetFileName(filePath)}' exported successfully.");

                if (MessageBox.Ask("Do you want to view the exported Excel workbook now?", "Export Succeeded") == MessageBoxResult.OK)
                {
                    Util.StartProcess(filePath);
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex, "ExportToExcelAsync");
                MessageBox.Error($"Failed to export Excel file:\n\n{ex.Message}", "Export Error");
            }
        }

        public static async Task ExportToPdfAsync(
            SfDataGrid dataGrid,
            PdfExportingOptions options,
            PdfPageOrientation orientation,
            string defaultFileName)
        {
            try
            {
                var sfd = new SaveFileDialog
                {
                    Filter = "PDF Files(*.pdf)|*.pdf",
                    FileName = defaultFileName
                };

                if (sfd.ShowDialog() != true) return;

                var document = new PdfDocument();
                document.PageSettings.Orientation = orientation;
                var page = document.Pages.Add();
                var pdfGrid = dataGrid.ExportToPdfGrid(dataGrid.View, options);
                var format = new PdfGridLayoutFormat
                {
                    Layout = PdfLayoutType.Paginate,
                    Break = PdfLayoutBreakType.FitPage
                };

                pdfGrid.Draw(page, new PointF(), format);

                var filePath = sfd.FileName;
                await Task.Run(() =>
                {
                    using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
                    document.Save(stream);
                });

                Growl.Success($"PDF file '{Path.GetFileName(filePath)}' exported successfully.");

                if (MessageBox.Ask("Do you want to view the exported PDF file now?", "Export Succeeded") == MessageBoxResult.OK)
                {
                    Util.StartProcess(filePath);
                }
            }
            catch (Exception ex)
            {
                App.LogException(ex, "ExportToPdfAsync");
                MessageBox.Error($"Failed to export PDF document:\n\n{ex.Message}", "Export Error");
            }
        }
    }
}
