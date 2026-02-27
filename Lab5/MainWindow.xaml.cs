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
            var northAmericanCustomers = _context.Customers
                .Where(c => c.Country == "USA" || c.Country == "Canada" || c.Country == "Mexico")
                .OrderBy(c => c.Country)
                .ToList();

            // Display the results in the DataGrid
            ResultsDataGrid.ItemsSource = northAmericanCustomers;

            //Show the query description in the TextBlock
            QueryDescription = "Customers from North America, ordered by country";
        }
        private void Query2_Click(object sender, RoutedEventArgs e)
        {
            //Products with more than 10 units in stock, sorted from highest to lowest units in stock, then by product name
            var productsInStock = _context.Products
                .Where(p => p.UnitsInStock > 10)
                .OrderByDescending(p => p.UnitsInStock)
                .ThenBy(p => p.ProductName)
                .ToList();
            ResultsDataGrid.ItemsSource = productsInStock;
            QueryDescription = "Products with more than 10 units in stock, sorted by units in stock (desc) then product name";
        }
        private void Query3_Click(object sender, RoutedEventArgs e)
        {
            //Customers with no fax number, ordered by company, then country
            var customersWithoutFax = _context.Customers
                .Where(c => string.IsNullOrEmpty(c.Fax))
                .OrderBy(c => c.CompanyName)
                .ThenBy(c => c.Country)
                .ToList();
            ResultsDataGrid.ItemsSource = customersWithoutFax;
            QueryDescription = "Customers with no fax number, ordered by company then country";
        }
        private void Query4_Click(object sender, RoutedEventArgs e)
        {
            //First 5 customers alphabetically by company name
            var first5Customers = _context.Customers
                .OrderBy(c => c.CompanyName)
                .Take(5)
                .ToList();

            ResultsDataGrid.ItemsSource = first5Customers;
            QueryDescription = "First 5 customers alphabetically by company name";
        }
        private void Query5_Click(object sender, RoutedEventArgs e)
        {
            //Group customers by country and count the number of customers in each country, ordered starting with most per country
            var customersByCountry = _context.Customers
                .GroupBy(c => c.Country)
                .Select(g => new { Country = g.Key, CustomerCount = g.Count() })
                .OrderByDescending(g => g.CustomerCount)
                .ToList();

            ResultsDataGrid.ItemsSource = customersByCountry;
            QueryDescription = "Number of customers per country, ordered by highest count";
        }
        private void Query6_Click(object sender, RoutedEventArgs e)
        {
            //List all products that are desserts (category 3!), ordered by price from highest to lowest
            var dessertProducts = _context.Products
                .Where(p => p.CategoryId == 3)
                .OrderByDescending(p => p.UnitPrice)
                .ToList();
            ResultsDataGrid.ItemsSource = dessertProducts;
            QueryDescription = "Dessert products, ordered by price desc";
        }
        private void Query7_Click(object sender, RoutedEventArgs e)
        {
            //List the top ten most expensive freight orders to countries in South America in the month of December, ordered by freight cost from highest to lowest
            var expensiveFreightOrders = _context.Orders
                .Where(o => (o.ShipCountry == "Brazil" || o.ShipCountry == "Argentina" || o.ShipCountry == "Venezuela")
                            && o.OrderDate.HasValue && o.OrderDate.Value.Month == 12)
                .OrderByDescending(o => o.Freight)
                .Take(10)
                .ToList();
            ResultsDataGrid.ItemsSource = expensiveFreightOrders;
            QueryDescription = "Top 10 most expensive freight orders to South America in December, ordered by freight cost desc";
        }
        private void Query8_Click(object sender, RoutedEventArgs e)
        {
            //List all distinct countries for Northwind customers, in alphabetical order, with a customer count per country
            var countriesWithCustomerCount = _context.Customers
                .GroupBy(c => c.Country)
                .Select(g => new { Country = g.Key, CustomerCount = g.Count() })
                .OrderBy(g => g.Country)
                .ToList();
            ResultsDataGrid.ItemsSource = countriesWithCustomerCount;
            QueryDescription = "Distinct countries with customer counts, alphabetical order";
        }
        private void Query9_Click(object sender, RoutedEventArgs e)
        {
            // List all employees with their title, full name and the name of their manager (if any).
            // Order by the employee's last name on the database side (EF can translate this).
            var employeesWithManagers = _context.Employees
                .Include(e => e.ReportsToNavigation) // include manager navigation
                .OrderBy(e => e.ReportsTo)
                .Select(e => new
                {
                    Title = e.Title,
                    FullName = e.FirstName + " " + e.LastName,
                    ManagerName = e.ReportsToNavigation != null ? e.ReportsToNavigation.FirstName + " " + e.ReportsToNavigation.LastName : "No Manager"
                })
                .ToList();

            ResultsDataGrid.ItemsSource = employeesWithManagers;
            QueryDescription = "Employees with title, full name, and manager name, sorted by last name";
        }
        private void Query10_Click(object sender, RoutedEventArgs e)
        {
            //List all orders grouped by month and year, showing the total number of orders and total revenue for each month, ordered starting with the most recent month
            var ordersByMonthYear = _context.Orders
                .GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    OrderCount = g.Count(),
                    TotalRevenue = g.Sum(o => o.OrderDetails.Sum(od => od.UnitPrice * od.Quantity))
                })
                .OrderByDescending(g => g.Year)
                .ThenByDescending(g => g.Month)
                .ToList();
            ResultsDataGrid.ItemsSource = ordersByMonthYear;
            QueryDescription = "Orders grouped by month/year with order count and total revenue, most recent first";
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}