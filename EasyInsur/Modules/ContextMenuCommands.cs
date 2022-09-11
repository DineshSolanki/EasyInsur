#region Copyright Syncfusion Inc. 2001-2021
// Copyright Syncfusion Inc. 2001-2021. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion

using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Utility;

namespace EasyInsur.Modules
{
    public static class ContextMenuCommands
    {
        #region ClearGroups
        static ICommand _clearGroups;
        public static ICommand ClearGroups => _clearGroups ??= new BaseCommand(OnClearGroupsClicked, CanClearGroupsClicked);

        private static void OnClearGroupsClicked(object obj)
        {
            if (obj is not GridContextMenuInfo info) return;
            var grid = info.DataGrid;
            grid.GroupColumnDescriptions.Clear();
        }

        private static bool CanClearGroupsClicked(object obj)
        {
            if (obj is not GridContextMenuInfo info) return false;
            var grid = info.DataGrid;
            return grid.GroupColumnDescriptions != null && grid.GroupColumnDescriptions.Any();
        }
        #endregion

        #region ClearGrouping
        static ICommand _clearGroup;
        public static ICommand ClearGroup => _clearGroup ??= new BaseCommand(OnClearGroupClicked);

        private static void OnClearGroupClicked(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return;
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.GroupColumnDescriptions.Any(x => x.ColumnName == column.MappingName))
                grid.GroupColumnDescriptions.Remove(grid.GroupColumnDescriptions.FirstOrDefault(x => x.ColumnName == column.MappingName));
        }

        #endregion
        #region ShowHideGroupArea
        static BaseCommand _showHideGroupArea;
        public static BaseCommand ShowHideGroupArea => _showHideGroupArea ??= new BaseCommand(OnShowHideGroupAreaClicked);


        private static void OnShowHideGroupAreaClicked(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return;
            var grid = info.DataGrid;
            grid.IsGroupDropAreaExpanded = !grid.IsGroupDropAreaExpanded;
        }

        #endregion
        #region ExpandAll
        static ICommand _expandAll;
        public static ICommand ExpandAll => _expandAll ??= new BaseCommand(OnFullExpandClicked, CanFullExpand);

        private static void OnFullExpandClicked(object obj)
        {
            if (obj is not GridContextMenuInfo info) return;
            var grid = info.DataGrid;
            grid.ExpandAllGroup();
        }

        private static bool CanFullExpand(object obj)
        {
            if (obj is not GridContextMenuInfo info) return false;
            var grid = info.DataGrid;
            return grid.View is
            {
                TopLevelGroup:
                {
                }
            } &&
                   grid.View.TopLevelGroup.Groups.Count > 0 &&
                   grid.View.TopLevelGroup.Groups.Any(x => !x.IsExpanded);
        }
        #endregion

        #region CollapseAll
        static ICommand _collapseAll;
        public static ICommand CollapseAll => _collapseAll ??= new BaseCommand(OnFullCollapseClicked, CanFullCollapse);

        private static void OnFullCollapseClicked(object obj)
        {
            if (obj is not GridContextMenuInfo info) return;
            var grid = info.DataGrid;
            grid.CollapseAllGroup();
        }

        private static bool CanFullCollapse(object obj)
        {
            if (obj is not GridContextMenuInfo info) return false;
            var grid = info.DataGrid;
            return grid.View is { TopLevelGroup: { } } && grid.View.TopLevelGroup.Groups.Count > 0
&& grid.View.TopLevelGroup.Groups.Any(x => x.IsExpanded);
        }
        #endregion

        #region Expand
        static ICommand _expand;
        public static ICommand Expand => _expand ??= new BaseCommand(OnExpandClicked, CanExpandClicked);

        private static void OnExpandClicked(object obj)
        {
            if (obj is not GridRecordContextMenuInfo info) return;
            var grid = info.DataGrid;
            var group = info.Record as Group;
            grid.ExpandGroup(@group);
        }

        private static bool CanExpandClicked(object obj)
        {
            if (obj is not GridRecordContextMenuInfo info) return false;
            var grid = info.DataGrid;
            var group = info.Record as Group;
            return !@group.IsExpanded;
        }
        #endregion

        #region Collapse
        static ICommand _collapse;
        public static ICommand Collapse => _collapse ??= new BaseCommand(OnCollapseClicked, CanCollapseClicked);

        private static void OnCollapseClicked(object obj)
        {
            if (obj is not GridRecordContextMenuInfo info) return;
            var grid = info.DataGrid;
            var group = info.Record as Group;
            grid.CollapseGroup(@group);
        }

        private static bool CanCollapseClicked(object obj)
        {
            if (obj is not GridRecordContextMenuInfo info) return false;
            var grid = info.DataGrid;
            var group = info.Record as Group;
            return @group.IsExpanded;
        }
        #endregion

        #region SortAscending
        static ICommand _sortAscending;
        public static ICommand SortAscending => _sortAscending ??= new BaseCommand(OnSortAscendingClicked, CanSortAscending);


        private static void OnSortAscendingClicked(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return;
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions != null)
                return;

            grid.SortColumnDescriptions.Clear();
            grid.SortColumnDescriptions.Add(new SortColumnDescription() { ColumnName = column.MappingName, SortDirection = ListSortDirection.Ascending });
        }

        private static bool CanSortAscending(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return false;
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions == null) return grid.AllowSorting;
            var sortColumn = grid.SortColumnDescriptions.FirstOrDefault(x => x.ColumnName == column.MappingName);
            return sortColumn == null
                ? grid.AllowSorting
                : sortColumn.SortDirection != ListSortDirection.Ascending && grid.AllowSorting;
        }
        #endregion

        #region SortDescending
        static ICommand _sortDescending;
        public static ICommand SortDescending =>
            _sortDescending ??= new BaseCommand(OnSortDescendingClicked, CanSortDescending);


        private static void OnSortDescendingClicked(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return;
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions == null)
                return;
            grid.SortColumnDescriptions.Clear();
            grid.SortColumnDescriptions.Add(new SortColumnDescription() { ColumnName = column.MappingName, SortDirection = ListSortDirection.Descending });
        }

        private static bool CanSortDescending(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return false;
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions == null) return grid.AllowSorting;
            var sortColumn = grid.SortColumnDescriptions.FirstOrDefault(x => x.ColumnName == column.MappingName);
            return sortColumn == null
                ? grid.AllowSorting
                : sortColumn.SortDirection != ListSortDirection.Descending && grid.AllowSorting;
        }
        #endregion

        #region ClearSorting
        static ICommand _clearSorting;
        public static ICommand ClearSorting
        {
            get
            {
                if (_clearSorting != null)
                    return _clearSorting;
                _clearSorting = new BaseCommand(OnClearSortingClicked, CanClearSort);

                return _clearSorting;
            }
        }

        private static bool CanClearSort(object obj)
        {
            if (obj == null)
                return false;

            var grid = (obj as GridContextMenuInfo).DataGrid;
            var column = (obj as GridColumnContextMenuInfo).Column;

            return grid.SortColumnDescriptions != null && grid.SortColumnDescriptions.Any(x => x.ColumnName == column.MappingName);
        }

        private static void OnClearSortingClicked(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return;
            var grid = info.DataGrid;
            var column = info.Column;
            if (grid.SortColumnDescriptions != null && grid.SortColumnDescriptions.Any(x => x.ColumnName == column.MappingName))
                grid.SortColumnDescriptions.Remove(grid.SortColumnDescriptions.FirstOrDefault(x => x.ColumnName == column.MappingName));
        }
        #endregion

        #region ClearFiltering
        static ICommand _clearFiltering;
        public static ICommand ClearFiltering =>
            _clearFiltering ??= new BaseCommand(OnClearFilteringClicked, CanClearFiltering);


        private static void OnClearFilteringClicked(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return;
            var column = info.Column;
            if (column.FilterPredicates.Any())
                column.FilterPredicates.Clear();
        }

        private static bool CanClearFiltering(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return false;
            var column = info.Column;
            return column.FilterPredicates.Any();
        }
        #endregion

        #region GroupThisColumn
        static ICommand _groupThisColumn;
        public static ICommand GroupThisColumn =>
            _groupThisColumn;


        private static void OnGroupThisColumnClicked(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return;
            var grid = info.DataGrid;
            var column = info.Column;

            if (grid.GroupColumnDescriptions != null && grid.GroupColumnDescriptions.All(x => x.ColumnName != column.MappingName))
                grid.GroupColumnDescriptions.Add(new GroupColumnDescription() { ColumnName = column.MappingName });
        }

        private static bool CanGroupThisColumn(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return false;
            var grid = info.DataGrid;
            var column = info.Column;
            var canGroup = false;
            if (grid.GroupColumnDescriptions == null ||
                grid.GroupColumnDescriptions.Any(x => x.ColumnName == column.MappingName)) return canGroup;
            var groupcolumn = column.ReadLocalValue(GridColumn.AllowGroupingProperty);
            if (grid.AllowGrouping)
                canGroup = true;
            if (groupcolumn != DependencyProperty.UnsetValue || canGroup)
                canGroup = column.AllowGrouping;
            return canGroup;
        }
        #endregion

        #region BestFit
        static ICommand _bestFit;
        public static ICommand BestFit => _bestFit ??= new BaseCommand(OnBestFitClicked);


        private static void OnBestFitClicked(object obj)
        {
            if (obj is not GridColumnContextMenuInfo info) return;
            var grid = info.DataGrid;
            var column = info.Column;
            column.ColumnSizer = GridLengthUnitType.SizeToCells;
        }
        #endregion

        #region Copy
        static ICommand _copy;
        public static ICommand Copy
        {
            get
            {
                if (_copy == null)
                    _copy = new BaseCommand(OnCopyClicked);

                return _copy;
            }
        }


        private static void OnCopyClicked(object obj)
        {
            if (obj is GridRecordContextMenuInfo info)
            {
                var grid = info.DataGrid;
                
            }
        }

        #endregion

        #region Cut
        static ICommand _cut;
        public static ICommand Cut => _cut ??= new BaseCommand(OnCutClicked);

        private static void OnCutClicked(object obj)
        {
            if (obj is not GridRecordContextMenuInfo info) return;
            var grid = info.DataGrid;
            var copypasteoption = grid.GridCopyOption;
            grid.GridCopyOption = GridCopyOption.CutData;
            grid.GridCopyPaste.Cut();
            grid.GridCopyOption = copypasteoption;
        }
        #endregion

        #region Paste
        static ICommand _paste;
        public static ICommand Paste => _paste ??= new BaseCommand(OnPasteClicked, CanPaste);

        private static bool CanPaste(object obj)
        {
            return Clipboard.GetDataObject() != null && (Clipboard.GetDataObject() as DataObject).ContainsText();
        }

        private static void OnPasteClicked(object obj)
        {
            if (obj is not GridRecordContextMenuInfo info) return;
            var grid = info.DataGrid;
            grid.GridCopyPaste.Paste();
        }


        #endregion

        #region TotalSummaryCount
        static ICommand _totalsummaryCount;
        public static ICommand TotalSummaryCount
        {
            get
            {
                if (_totalsummaryCount == null)
                    _totalsummaryCount = new BaseCommand(OnTotalSummaryCountClicked);

                return _totalsummaryCount;
            }
        }


        private static void OnTotalSummaryCountClicked(object obj)
        {
            if (obj is GridRecordContextMenuInfo info)
            {
                var grid = info.DataGrid;
                var record = info.Record as SummaryRecordEntry;
                if (record != null)
                {
                    var summaryrow = new GridSummaryRow() { Name = "totalgroupsummaryrow", Title = "{totalSummary}", ShowSummaryInRow = true };
                    summaryrow.SummaryColumns.Add(new GridSummaryColumn() { Name = "totalSummary", MappingName = "EmployeeId", SummaryType = SummaryType.CountAggregate, Format = "Total Employee Count : {Count}" });
                    grid.TableSummaryRows.Clear();
                    grid.TableSummaryRows.Add(summaryrow);

                }


            }
        }

        #endregion

        #region TotalSummaryMax
        static ICommand _totalsummaryMax;
        public static ICommand TotalSummaryMax
        {
            get
            {
                if (_totalsummaryMax == null)
                    _totalsummaryMax = new BaseCommand(OnTotalSummaryMaxClicked);

                return _totalsummaryMax;
            }
        }


        private static void OnTotalSummaryMaxClicked(object obj)
        {
            if (obj is GridRecordContextMenuInfo info)
            {
                var grid = info.DataGrid;
                var record = info.Record as SummaryRecordEntry;
                if (record != null)
                {
                    var summaryrow = new GridSummaryRow() { ShowSummaryInRow = true, Name = "totalgroupsummaryrow", Title = "{totalSummary}" };
                    summaryrow.SummaryColumns.Add(new GridSummaryColumn() { Name = "totalSummary", MappingName = "EmployeeAge", SummaryType = SummaryType.DoubleAggregate, Format = "Maximum age of Employee : {Max}" });
                    grid.TableSummaryRows.Clear();
                    grid.TableSummaryRows.Add(summaryrow);
                }


            }
        }

        #endregion

        #region TotalSummaryMin
        static ICommand _totalsummaryMin;
        public static ICommand TotalSummaryMin
        {
            get
            {
                if (_totalsummaryMin == null)
                    _totalsummaryMin = new BaseCommand(OnTotalSummaryMinClicked);

                return _totalsummaryMin;
            }
        }


        private static void OnTotalSummaryMinClicked(object obj)
        {
            if (obj is GridRecordContextMenuInfo info)
            {
                var grid = info.DataGrid;
                var record = info.Record as SummaryRecordEntry;
                if (record != null)
                {
                    var summaryrow = new GridSummaryRow() { ShowSummaryInRow = true, Name = "totalgroupsummaryrow", Title = "{totalSummary}" };
                    summaryrow.SummaryColumns.Add(new GridSummaryColumn() { Name = "totalSummary", MappingName = "EmployeeAge", SummaryType = SummaryType.DoubleAggregate, Format = "Minimum age of Employee : {Min}" });
                    grid.TableSummaryRows.Clear();
                    grid.TableSummaryRows.Add(summaryrow);
                }


            }
        }

        #endregion

        #region TableAverage
        static ICommand _tableAverage;
        public static ICommand TableAverage
        {
            get
            {
                if (_tableAverage == null)
                    _tableAverage = new BaseCommand(OnTableAverageClicked);

                return _tableAverage;
            }
        }


        private static void OnTableAverageClicked(object obj)
        {
            if (obj is GridRecordContextMenuInfo info)
            {
                var grid = info.DataGrid;
                var record = info.Record as SummaryRecordEntry;
                if (record != null)
                {
                    var summaryrow = new GridSummaryRow() { ShowSummaryInRow = true, Name = "totalgroupsummaryrow", Title = "{totalSummary}" };
                    summaryrow.SummaryColumns.Add(new GridSummaryColumn() { Name = "totalSummary", MappingName = "EmployeeAge", SummaryType = SummaryType.DoubleAggregate, Format = "Average Employee age : {Average}" });
                    grid.TableSummaryRows.Clear();
                    grid.TableSummaryRows.Add(summaryrow);
                }


            }
        }

        #endregion

        #region TableSum
        static ICommand _tableSum;
        public static ICommand TableSum
        {
            get
            {
                if (_tableSum == null)
                    _tableSum = new BaseCommand(OnTableSumClicked);

                return _tableSum;
            }
        }


        private static void OnTableSumClicked(object obj)
        {
            if (obj is GridRecordContextMenuInfo info)
            {
                var grid = info.DataGrid;
                var record = info.Record as SummaryRecordEntry;
                if (record != null)
                {
                    var summaryrow = new GridSummaryRow() { ShowSummaryInRow = true, Name = "totalgroupsummaryrow", Title = "{totalSummary}" };
                    summaryrow.SummaryColumns.Add(new GridSummaryColumn() { Name = "totalSummary", MappingName = "EmployeeAge", SummaryType = SummaryType.DoubleAggregate, Format = "Sum Employee Salary : {Sum}" });
                    grid.TableSummaryRows.Clear();
                    grid.TableSummaryRows.Add(summaryrow);
                }
            }
        }

        #endregion

        #region Add Payment
        static ICommand _addPayment;
        public static ICommand AddPayment
        {
            get
            {
                if (_addPayment == null)
                    _addPayment = new BaseCommand(OnAddPaymentClicked);

                return _copy;
            }
        }


        private static void OnAddPaymentClicked(object obj)
        {
            if (obj is GridRecordContextMenuInfo info)
            {
                var grid = info.DataGrid;
                grid.GridCopyPaste.Copy();
            }
        }

        #endregion

    }

}
