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

        private bool _isSaving;

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (_isSaving) return;
            _isSaving = true;

            try
            {
                string tempDir = Path.Combine(Path.GetTempPath(), "EasyInsur");
                Directory.CreateDirectory(tempDir);
                string outPath = Path.Combine(tempDir, $"profile_{Guid.NewGuid():N}.jpg");

                var tcs = new System.Threading.Tasks.TaskCompletionSource<string?>();

                EventHandler<Syncfusion.UI.Xaml.ImageEditor.ImageSavedEventArgs>? handler = null;
                handler = (s, args) =>
                {
                    Editor.ImageSaved -= handler;
                    tcs.TrySetResult(!string.IsNullOrEmpty(args.Location) && File.Exists(args.Location) ? args.Location : outPath);
                };
                Editor.ImageSaved += handler;

                // Syncfusion SfImageEditor format parameter does NOT take a leading dot
                Editor.Save("jpg", new Size(0, 0), outPath);

                var completedTask = await System.Threading.Tasks.Task.WhenAny(tcs.Task, System.Threading.Tasks.Task.Delay(3000));
                if (completedTask == tcs.Task)
                {
                    ResultPath = await tcs.Task;
                    DialogResult = true;
                    Close();
                    return;
                }

                // If event didn't fire within 3s, verify if file exists anyway
                if (File.Exists(outPath))
                {
                    ResultPath = outPath;
                    DialogResult = true;
                    Close();
                    return;
                }

                MessageBox.Show("Image could not be saved. Please try again.", "Image Editor", MessageBoxButton.OK, MessageBoxImage.Warning);
                _isSaving = false;
            }
            catch (Exception ex)
            {
                _isSaving = false;
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
