using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using Microsoft.Win32;
using SubscriptionManager.Models;
using SubscriptionManager.Views;

namespace SubscriptionManager.Services
{
    /// <summary>
    /// Service for handling printing and PDF generation of invoices
    /// </summary>
    public class PrintService : IPrintService
    {
        private string _defaultPrinterName = string.Empty;
        private const double InchesToPixels = 96.0; // WPF uses 96 DPI

        public async Task<bool> PrintInvoiceAsync(InvoiceData invoiceData)
        {
            try
            {
                return await Task.Run(() =>
                {
                    return Application.Current.Dispatcher.Invoke(() =>
                    {
                        var printDialog = new PrintDialog();

                        // Set default printer if specified
                        if (!string.IsNullOrEmpty(_defaultPrinterName))
                        {
                            var printQueue = GetPrintQueue(_defaultPrinterName);
                            if (printQueue != null)
                            {
                                printDialog.PrintQueue = printQueue;
                            }
                        }

                        // Create and configure the invoice template
                        var invoiceTemplate = CreateInvoiceTemplate(invoiceData);

                        // Configure print settings
                        var pageSize = new Size(8.5 * InchesToPixels, 11 * InchesToPixels);
                        invoiceTemplate.Measure(pageSize);
                        invoiceTemplate.Arrange(new Rect(pageSize));
                        invoiceTemplate.UpdateLayout();

                        // Print the document
                        printDialog.PrintVisual(invoiceTemplate, $"Invoice - {invoiceData.CustomerName}");

                        return true;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing invoice: {ex.Message}", "Print Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> ShowPrintPreviewAsync(InvoiceData invoiceData)
        {
            try
            {
                return await Task.Run(() =>
                {
                    return Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Create print preview window
                        var previewWindow = new PrintPreviewWindow();
                        var invoiceTemplate = CreateInvoiceTemplate(invoiceData);

                        // Set up the preview content
                        var pageSize = new Size(8.5 * InchesToPixels, 11 * InchesToPixels);
                        invoiceTemplate.Measure(pageSize);
                        invoiceTemplate.Arrange(new Rect(pageSize));
                        invoiceTemplate.UpdateLayout();

                        previewWindow.PreviewContent = invoiceTemplate;
                        previewWindow.InvoiceData = invoiceData;
                        previewWindow.PrintService = this;

                        return previewWindow.ShowDialog() ?? false;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error showing print preview: {ex.Message}", "Preview Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> ExportToPdfAsync(InvoiceData invoiceData, string filePath)
        {
            try
            {
                return await Task.Run(() =>
                {
                    return Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Create XPS document
                        var xpsDocument = CreateXpsDocument(invoiceData);

                        // Convert XPS to PDF (requires additional library for full PDF support)
                        // For now, we'll save as XPS which can be converted to PDF
                        var xpsFilePath = Path.ChangeExtension(filePath, ".xps");

                        using (var package = Package.Open(xpsFilePath, FileMode.Create))
                        using (var xpsDoc = new XpsDocument(package))
                        {
                            var writer = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
                            var invoiceTemplate = CreateInvoiceTemplate(invoiceData);

                            var pageSize = new Size(8.5 * InchesToPixels, 11 * InchesToPixels);
                            invoiceTemplate.Measure(pageSize);
                            invoiceTemplate.Arrange(new Rect(pageSize));
                            invoiceTemplate.UpdateLayout();

                            writer.Write(invoiceTemplate);
                        }

                        // If PDF is specifically needed, you can add PDF conversion here
                        // using libraries like iTextSharp or PdfSharp

                        return true;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to PDF: {ex.Message}", "Export Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> ExportToPdfWithDialogAsync(InvoiceData invoiceData)
        {
            try
            {
                return await Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    var saveFileDialog = new SaveFileDialog
                    {
                        Filter = "XPS Documents (*.xps)|*.xps|All Files (*.*)|*.*",
                        DefaultExt = "xps",
                        FileName = $"Invoice_{invoiceData.CustomerName}_{invoiceData.InvoiceDate:yyyyMMdd}"
                    };

                    if (saveFileDialog.ShowDialog() == true)
                    {
                        return ExportToPdfAsync(invoiceData, saveFileDialog.FileName).Result;
                    }

                    return false;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting invoice: {ex.Message}", "Export Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> PrintCustomerInvoiceAsync(CustomerSubscription customer, Settings settings, List<CounterHistory> readingHistory)
        {
            try
            {
                var invoiceData = InvoiceData.CreateFromCustomer(customer, settings, readingHistory, true);
                return await PrintInvoiceAsync(invoiceData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing customer invoice: {ex.Message}", "Print Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public async Task<bool> PrintSingleReadingInvoiceAsync(CustomerSubscription customer, Settings settings, CounterHistory reading)
        {
            try
            {
                var invoiceData = InvoiceData.CreateFromSingleReading(customer, settings, reading);
                return await PrintInvoiceAsync(invoiceData);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing reading invoice: {ex.Message}", "Print Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public List<string> GetAvailablePrinters()
        {
            try
            {
                var printers = new List<string>();
                var printServer = new LocalPrintServer();

                foreach (var printQueue in printServer.GetPrintQueues())
                {
                    printers.Add(printQueue.Name);
                }

                return printers;
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public void SetDefaultPrinter(string printerName)
        {
            _defaultPrinterName = printerName;
        }

        #region Private Helper Methods

        /// <summary>
        /// Creates an invoice template UserControl with the provided data
        /// </summary>
        private InvoiceTemplate CreateInvoiceTemplate(InvoiceData invoiceData)
        {
            var template = new InvoiceTemplate
            {
                DataContext = invoiceData
            };

            return template;
        }

        /// <summary>
        /// Gets a print queue by name
        /// </summary>
        private PrintQueue? GetPrintQueue(string printerName)
        {
            try
            {
                var printServer = new LocalPrintServer();
                return printServer.GetPrintQueue(printerName);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Creates an XPS document from invoice data
        /// </summary>
        private XpsDocument CreateXpsDocument(InvoiceData invoiceData)
        {
            var tempFilePath = Path.GetTempFileName();
            var package = Package.Open(tempFilePath, FileMode.Create);
            var xpsDoc = new XpsDocument(package);

            return xpsDoc;
        }

        #endregion
    }

    /// <summary>
    /// Print preview window for invoices
    /// </summary>
    public class PrintPreviewWindow : Window
    {
        private Button _printButton;
        private Button _cancelButton;
        private ScrollViewer _scrollViewer;

        public FrameworkElement? PreviewContent { get; set; }
        public InvoiceData? InvoiceData { get; set; }
        public IPrintService? PrintService { get; set; }

        public PrintPreviewWindow()
        {
            InitializeWindow();
            CreateLayout();
        }

        private void InitializeWindow()
        {
            Title = "Print Preview - Invoice";
            Width = 900;
            Height = 700;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            WindowState = WindowState.Maximized;
        }

        private void CreateLayout()
        {
            var mainGrid = new Grid();
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // Preview area
            _scrollViewer = new ScrollViewer
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                Background = Brushes.LightGray,
                Padding = new Thickness(20) // FIXED: Single parameter for uniform padding
            };

            Grid.SetRow(_scrollViewer, 0);
            mainGrid.Children.Add(_scrollViewer);

            // Button panel
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(10) // FIXED: Single parameter for uniform margin
            };

            _printButton = new Button
            {
                Content = "Print",
                Padding = new Thickness(20, 10, 20, 10), // FIXED: Four parameters (left, top, right, bottom)
                Margin = new Thickness(0, 0, 10, 0), // FIXED: Four parameters (left, top, right, bottom)
                Background = Brushes.Green,
                Foreground = Brushes.White
            };
            _printButton.Click += PrintButton_Click;

            _cancelButton = new Button
            {
                Content = "Cancel",
                Padding = new Thickness(20, 10, 20, 10) // FIXED: Four parameters (left, top, right, bottom)
            };
            _cancelButton.Click += CancelButton_Click;

            buttonPanel.Children.Add(_printButton);
            buttonPanel.Children.Add(_cancelButton);

            Grid.SetRow(buttonPanel, 1);
            mainGrid.Children.Add(buttonPanel);

            Content = mainGrid;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (PreviewContent != null)
            {
                // Add border and shadow effect for better preview
                var border = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1), // Single parameter for uniform border
                    Margin = new Thickness(10), // Single parameter for uniform margin
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        Color = Colors.Gray,
                        BlurRadius = 10,
                        ShadowDepth = 5,
                        Opacity = 0.5
                    }
                };

                border.Child = PreviewContent;
                _scrollViewer.Content = border;
            }
        }

        private async void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (PrintService != null && InvoiceData != null)
            {
                var success = await PrintService.PrintInvoiceAsync(InvoiceData);
                if (success)
                {
                    DialogResult = true;
                    Close();
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}