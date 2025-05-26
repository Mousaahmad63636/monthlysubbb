using SubscriptionManager.Models;

namespace SubscriptionManager.Services
{
    /// <summary>
    /// Interface for print and document generation services
    /// </summary>
    public interface IPrintService
    {
        /// <summary>
        /// Prints an invoice using the system's default printer
        /// </summary>
        /// <param name="invoiceData">Invoice data to print</param>
        /// <returns>True if printing was successful</returns>
        Task<bool> PrintInvoiceAsync(InvoiceData invoiceData);

        /// <summary>
        /// Shows print preview dialog for an invoice
        /// </summary>
        /// <param name="invoiceData">Invoice data to preview</param>
        /// <returns>True if user proceeded with printing</returns>
        Task<bool> ShowPrintPreviewAsync(InvoiceData invoiceData);

        /// <summary>
        /// Exports invoice to PDF file
        /// </summary>
        /// <param name="invoiceData">Invoice data to export</param>
        /// <param name="filePath">Path where PDF should be saved</param>
        /// <returns>True if export was successful</returns>
        Task<bool> ExportToPdfAsync(InvoiceData invoiceData, string filePath);

        /// <summary>
        /// Opens file dialog and exports invoice to PDF
        /// </summary>
        /// <param name="invoiceData">Invoice data to export</param>
        /// <returns>True if export was successful</returns>
        Task<bool> ExportToPdfWithDialogAsync(InvoiceData invoiceData);

        /// <summary>
        /// Prints customer invoice with all reading history
        /// </summary>
        /// <param name="customer">Customer to print invoice for</param>
        /// <param name="settings">Application settings</param>
        /// <param name="readingHistory">Customer's reading history</param>
        /// <returns>True if printing was successful</returns>
        Task<bool> PrintCustomerInvoiceAsync(CustomerSubscription customer, Settings settings, List<CounterHistory> readingHistory);

        /// <summary>
        /// Prints invoice for a specific reading
        /// </summary>
        /// <param name="customer">Customer information</param>
        /// <param name="settings">Application settings</param>
        /// <param name="reading">Specific reading to print</param>
        /// <returns>True if printing was successful</returns>
        Task<bool> PrintSingleReadingInvoiceAsync(CustomerSubscription customer, Settings settings, CounterHistory reading);

        /// <summary>
        /// Gets list of available printers
        /// </summary>
        /// <returns>List of printer names</returns>
        List<string> GetAvailablePrinters();

        /// <summary>
        /// Sets the default printer for the application
        /// </summary>
        /// <param name="printerName">Name of the printer to set as default</param>
        void SetDefaultPrinter(string printerName);
    }
}