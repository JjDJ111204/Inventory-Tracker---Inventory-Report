using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;

namespace Inventory_Tracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string connectionString = "Data Source=MSI\\SQLEXPRESS;Initial Catalog=AvailableItems;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
        public MainWindow()
        {
            InitializeComponent();
        }

        private void View_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM AvailableItems";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Bind data to DataGrid
                    Data_Grid1.ItemsSource = dt.DefaultView;

                    // Ensure DataGrid updates column widths properly
                    Data_Grid1.UpdateLayout();

                    foreach (var column in Data_Grid1.Columns)
                    {
                        column.Width = DataGridLength.Auto; // Auto-fit column content
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Export_Button_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                Title = "Save PDF Report"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                ExportToPDF(saveFileDialog.FileName);
            }
        }

        private void ExportToPDF(string filePath)
        {
            try
            {
                DataTable availableItemsTable = GetAvailableItems();
                DataTable reportedItemsTable = GetReportedItems();

                if (availableItemsTable.Rows.Count == 0 && reportedItemsTable.Rows.Count == 0)
                {
                    MessageBox.Show("No data available to export.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                Document doc = new Document(PageSize.A4.Rotate(), 50, 50, 50, 50);
                PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));
                doc.Open();

                // Export Available Items
                if (availableItemsTable.Rows.Count > 0)
                {
                    AddTableToPDF(doc, availableItemsTable, "Available Items Report");
                }

                // Add space between tables
                doc.Add(new iTextSharp.text.Paragraph("\n\n"));

                // Export Reported Items
                if (reportedItemsTable.Rows.Count > 0)
                {
                    AddTableToPDF(doc, reportedItemsTable, "Reported Items Report");
                }

                doc.Close();
                MessageBox.Show("PDF Exported Successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DataTable GetAvailableItems()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM AvailableItems";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dataTable;
        }

        private DataTable GetReportedItems()
        {
            DataTable dataTable = new DataTable();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM ReportedItems";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return dataTable;
        }

        private void AddTableToPDF(Document doc, DataTable dataTable, string title)
        {
            // Add title
            iTextSharp.text.Paragraph tableTitle = new iTextSharp.text.Paragraph(title, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14))
            {
                Alignment = Element.ALIGN_CENTER,
                SpacingAfter = 10
            };
            doc.Add(tableTitle);

            // Create table
            PdfPTable table = new PdfPTable(dataTable.Columns.Count)
            {
                WidthPercentage = 100
            };

            // Set column widths dynamically
            float[] columnWidths = new float[dataTable.Columns.Count];
            for (int i = 0; i < columnWidths.Length; i++)
            {
                columnWidths[i] = 1f; // Equal column distribution
            }
            table.SetWidths(columnWidths);

            // Add headers
            foreach (DataColumn column in dataTable.Columns)
            {
                PdfPCell cell = new PdfPCell(new Phrase(column.ColumnName, FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10)))
                {
                    BackgroundColor = new BaseColor(200, 200, 200),
                    HorizontalAlignment = Element.ALIGN_CENTER,
                    Padding = 5
                };
                table.AddCell(cell);
            }

            // Add data
            foreach (DataRow row in dataTable.Rows)
            {
                foreach (object item in row.ItemArray)
                {
                    PdfPCell dataCell = new PdfPCell(new Phrase(item.ToString(), FontFactory.GetFont(FontFactory.HELVETICA, 10)))
                    {
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    };
                    table.AddCell(dataCell);
                }
            }

            doc.Add(table);
        }

        private void Reported_Tem(object sender, RoutedEventArgs e)
        {

        }

        private void Reported_Item_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "SELECT * FROM ReportedItems";

                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    // Bind data to DataGrid
                    Data_Grid1.ItemsSource = dt.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}