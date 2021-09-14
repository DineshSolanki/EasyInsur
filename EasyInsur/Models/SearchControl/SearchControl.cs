using Syncfusion.UI.Xaml.Grid;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace EasyInsur.Models.SearchControl
{
    /// <summary>
    /// class which helps to Search the Text in the DataGrid.
    /// </summary>
    [TemplatePart(Name = "PART_FindNext", Type = typeof(Button))]
    [TemplatePart(Name = "PART_FindPrevious", Type = typeof(Button))]
    //[TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    [TemplatePart(Name = "PART_AdornerLayer", Type = typeof(AdornerDecorator))]
    public class SearchControl : Control, IDisposable
    {
        #region Fields

        internal AdornerDecorator? AdornerLayer;
        internal Button? CloseButton;
        internal Button? FindNextButton;

        internal Button? FindPreviousButton;
        internal TextBox? SearchTextBox;
        #endregion Fields

        #region Properties

        public static readonly DependencyProperty DataGridProperty =
                    DependencyProperty.Register("DataGrid", typeof(SfDataGrid), typeof(SearchControl), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the DataGrid for the corresponding search operation.
        /// </summary>
        public SfDataGrid? DataGrid
        {
            get => (SfDataGrid)GetValue(DataGridProperty);
            set => SetValue(DataGridProperty, value);
        }
        #endregion Properties

        #region Ctor

        public SearchControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SearchControl), new FrameworkPropertyMetadata(typeof(SearchControl)));
        }

        public SearchControl(SfDataGrid datagrid)
        {
            DataGrid = datagrid;
        }

        #endregion Ctor

        #region Methods

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            FindNextButton = GetTemplateChild("PART_FindNext") as Button;
            FindPreviousButton = GetTemplateChild("PART_FindPrevious") as Button;
            CloseButton = GetTemplateChild("PART_Close") as Button;
            SearchTextBox = GetTemplateChild("PART_TextBox") as TextBox;
            AdornerLayer = GetTemplateChild("PART_AdornerLayer") as AdornerDecorator;
            SearchTextBox?.Focus();
            WireEvents();
        }

        /// <summary>
        /// Method to open Search Control.
        /// </summary>
        /// <param name="visible"></param>
        public void UpdateSearchControlVisibility(bool visible)
        {
            if (visible)
            {
                Visibility = Visibility.Visible;
                SearchTextBox?.Focus();
            }
            else
            {
                Visibility = Visibility.Hidden;
                SearchTextBox?.Clear();
                DataGrid?.SearchHelper.ClearSearch();
                DataGrid?.Focus();
            }
        }
        #endregion Methods

        #region Events

        /// <summary>
        /// Event handler to handle AdornerLayer key down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAdornerLayerKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F when (e.KeyboardDevice.Modifiers & ModifierKeys.Control) != ModifierKeys.None:
                    UpdateSearchControlVisibility(true);
                    break;
                //case Key.Escape:
                //    UpdateSearchControlVisibility(false);
                //    break;
            }
        }

        /// <summary>
        /// Event handler to handle when clicking on Close button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            SearchTextBox?.Clear();
            DataGrid?.SearchHelper.ClearSearch();
            Visibility = Visibility.Collapsed;
            DataGrid?.Focus();
        }

        /// <summary>
        ///  Event handler to handle when clicking on FindNext button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindNextButtonClick(object sender, RoutedEventArgs e)
        {
            DataGrid?.SearchHelper.FindNext(SearchTextBox?.Text);
            SetSelectedItem();
        }

        /// <summary>
        /// Event handler to handle when clicking on FindPrevious button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnFindPreviousButtonClick(object sender, RoutedEventArgs e)
        {
            DataGrid?.SearchHelper.FindPrevious(SearchTextBox?.Text);
            SetSelectedItem();
        }

        /// <summary>
        /// Event handler to handle when text value is changed in SearchTextBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            DataGrid.SearchHelper = new SearchHelperExt(DataGrid);
            DataGrid.SearchHelper.Search(SearchTextBox?.Text);
        }

        /// <summary>
        /// Based on searched text, the row will be selected.
        /// </summary>
        private void SetSelectedItem()
        {
            var rowIndex = DataGrid?.SearchHelper.CurrentRowColumnIndex.RowIndex;
            if (rowIndex == null) return;
            var recordIndex = DataGrid.ResolveToRecordIndex((int)rowIndex);
            DataGrid.SelectedIndex = recordIndex;
        }

        /// <summary>
        /// Method to wire the required events.
        /// </summary>
        private void WireEvents()
        {
            FindNextButton.Click += OnFindNextButtonClick;
            FindPreviousButton.Click += OnFindPreviousButtonClick;
            //CloseButton.Click += OnCloseButtonClick;
            SearchTextBox.TextChanged += OnTextChanged;
            AdornerLayer.KeyDown += OnAdornerLayerKeyDown;
        }
        #endregion Events

        public void Dispose()
        {
            UnWireEvents();
            DataGrid = null;
        }

        /// <summary>
        /// Method to UnWire the wired events.
        /// </summary>
        private void UnWireEvents()
        {
            FindNextButton.Click -= OnFindNextButtonClick;
            FindPreviousButton.Click -= OnFindPreviousButtonClick;
            CloseButton.Click -= OnCloseButtonClick;
            SearchTextBox.TextChanged -= OnTextChanged;
        }
    }

    public class SearchHelperExt : SearchHelper
    {
        public SearchHelperExt(SfDataGrid datagrid)
            : base(datagrid)
        {
        }

        protected override bool SearchCell(DataColumnBase column, object record, bool ApplySearchHighlightBrush)
        {
            var colIndex = DataGrid.SelectionController.CurrentCellManager.CurrentCell.ColumnIndex;
            var mapName = DataGrid.Columns[colIndex].MappingName;
            return column.GridColumn.MappingName == mapName && base.SearchCell(column, record, ApplySearchHighlightBrush);
        }
    }
}