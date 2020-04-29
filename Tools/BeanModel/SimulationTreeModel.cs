using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.BeanModel
{
    public class SimulationTreeModel : INotifyPropertyChanged
    {
        public SimulationTreeModel() { }
        private int id;
        public int Id
        {
            get { return id; }
            set { id = value; OnPropertyChanged(new PropertyChangedEventArgs("Id")); }
        }
        private string name;
        public string Name 
        {
            get { return name; }
            set { name = value; OnPropertyChanged(new PropertyChangedEventArgs("Name")); }
        }
        private ObservableCollection<Product> products;
        public ObservableCollection<Product> Products
        {
            get { return products; }
            set { products = value; OnPropertyChanged(new PropertyChangedEventArgs("products")); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        public SimulationTreeModel(int id, string name, ObservableCollection<Product> products)
        {
            Id = id;
            Name = name;
            Products = products;
        }
    }

    public class Product
    {
        private int productId;
        public int ProductId
        {
            get { return productId; }
            set { productId = value; }
        }
        private string productName;
        public string ProductName
        {
            get { return productName; }
            set { productName = value; }
        }
    }

    public class SimulationMenuList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}
