using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EasyInsur.Views
{
    public partial class ImageEditorDialog : Window
    {
        private readonly string _originalPath;
        public string? ResultPath { get; private set; }

        public ImageEditorDialog(string imagePath)
        {
            InitializeComponent();
            _originalPath = imagePath;
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (File.Exists(_originalPath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.UriSource = new Uri(_originalPath, UriKind.Absolute);
                    bitmap.EndInit();
                    bitmap.Freeze();
                    Editor.ImageSource = bitmap;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load image into editor: {ex.Message}", "Image Editor", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "EasyInsur");
                Directory.CreateDirectory(tempDir);
                string outPath = Path.Combine(tempDir, $"profile_{Guid.NewGuid():N}.jpg");

                // Save using SfImageEditor
                Editor.Save(".jpg", new Size(0, 0), outPath);
                ResultPath = outPath;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save edited image: {ex.Message}", "Image Editor", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
