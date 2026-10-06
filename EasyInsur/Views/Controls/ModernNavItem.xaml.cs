using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EasyInsur.Views.Controls
{
    public partial class ModernNavItem : UserControl
    {
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(string), typeof(ModernNavItem), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register(nameof(Icon), typeof(ImageSource), typeof(ModernNavItem), new PropertyMetadata(null));

        public static readonly DependencyProperty ShortcutProperty =
            DependencyProperty.Register(nameof(Shortcut), typeof(string), typeof(ModernNavItem), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty TargetViewProperty =
            DependencyProperty.Register(nameof(TargetView), typeof(string), typeof(ModernNavItem), new PropertyMetadata(string.Empty, OnViewPropsChanged));

        public static readonly DependencyProperty CurrentViewProperty =
            DependencyProperty.Register(nameof(CurrentView), typeof(string), typeof(ModernNavItem), new PropertyMetadata(string.Empty, OnViewPropsChanged));

        public static readonly DependencyProperty IsActiveProperty =
            DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(ModernNavItem), new PropertyMetadata(false));

        public static readonly DependencyProperty IsCollapsedProperty =
            DependencyProperty.Register(nameof(IsCollapsed), typeof(bool), typeof(ModernNavItem), new PropertyMetadata(false));

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ModernNavItem), new PropertyMetadata(null));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ImageSource Icon
        {
            get => (ImageSource)GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        public string Shortcut
        {
            get => (string)GetValue(ShortcutProperty);
            set => SetValue(ShortcutProperty, value);
        }

        public string TargetView
        {
            get => (string)GetValue(TargetViewProperty);
            set => SetValue(TargetViewProperty, value);
        }

        public string CurrentView
        {
            get => (string)GetValue(CurrentViewProperty);
            set => SetValue(CurrentViewProperty, value);
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            private set => SetValue(IsActiveProperty, value);
        }

        public bool IsCollapsed
        {
            get => (bool)GetValue(IsCollapsedProperty);
            set => SetValue(IsCollapsedProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public ModernNavItem()
        {
            InitializeComponent();
        }

        private static void OnViewPropsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ModernNavItem item)
            {
                item.IsActive = !string.IsNullOrWhiteSpace(item.TargetView) &&
                                string.Equals(item.TargetView, item.CurrentView, StringComparison.OrdinalIgnoreCase);
            }
        }

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            if (Command != null && Command.CanExecute(TargetView))
            {
                Command.Execute(TargetView);
            }
        }
    }
}
