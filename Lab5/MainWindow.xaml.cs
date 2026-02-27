using Lab5.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Windows;

namespace Lab5
{
    //Scaffold Command:
    //scaffold-dbcontext "Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;TrustServerCertificate=True" Microsoft.EntityFrameworkCore.SqlServer -tables Customers, Products, Employees, orders,"order details",categories,suppliers -outputdir Models\Generated -contextdir Data\Generated -namespace Lab5.Models -contextnamespace Lab5.Data -force

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private NorthwindContext _context = new NorthwindContext();
        private string _queryDescription = string.Empty;

        public string QueryDescription { 
            get { return _queryDescription; }
            set { _queryDescription = value; OnPropertyChanged("QueryDescription"); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            // Load all data into memory from the database
            LoadAllData();
            // Set the DataContext so bindings to this window's properties work
            this.DataContext = this;
        }

        private void LoadAllData()
        {
            try
            {
                _context.Categories.Load();
                _context.Customers.Load();
                _context.Employees.Load();
                _context.OrderDetails.Load();
                _context.Orders.Load();
                _context.Products.Load();
                _context.Suppliers.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private void Query1_Click(object sender, RoutedEventArgs e)
        {
            // Customers from North America, ordered by country 

        }
        private void Query2_Click(object sender, RoutedEventArgs e)
        {
            //Products with more than 10 units in stock, sorted from highest to lowest units in stock, then by product name

        }
        private void Query3_Click(object sender, RoutedEventArgs e)
        {
            //Customers with no fax number, ordered by company, then country

        }
        private void Query4_Click(object sender, RoutedEventArgs e)
        {
            //First 5 customers alphabetically by company name

        }
        private void Query5_Click(object sender, RoutedEventArgs e)
        {
            //Group customers by country and count the number of customers in each country, ordered starting with most per country

        }
        private void Query6_Click(object sender, RoutedEventArgs e)
        {
            //List all products that are desserts (category 3!), ordered by price from highest to lowest

        }
        private void Query7_Click(object sender, RoutedEventArgs e)
        {
            //List the top ten most expensive freight orders to countries in South America in the month of December, ordered by freight cost from highest to lowest

        }
        private void Query8_Click(object sender, RoutedEventArgs e)
        {
            //List all distinct countries for Northwind customers, in alphabetical order, with a customer count per country

        }
        private void Query9_Click(object sender, RoutedEventArgs e)
        {
            // List all employees with their title, full name and the name of their manager (if any).

        }
        private void Query10_Click(object sender, RoutedEventArgs e)
        {
            //List all orders grouped by month and year, showing the total number of orders and total revenue for each month, ordered starting with the most recent month

        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}