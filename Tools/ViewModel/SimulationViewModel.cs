using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Tools.BeanModel;
using Tools.pages;

namespace Tools.ViewModel
{
    public class SimulationViewModel : ViewModelBase
    {
        private ObservableCollection<SimulationTreeModel> simulations;  
        
        public ObservableCollection<SimulationTreeModel> Simulations
        {
            get { return simulations; }
            set { simulations = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<Product> products;

        public ObservableCollection<Product> Products
        {
            get { return products; }
            set { products = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// 第三方接口数据模拟
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public SimulationViewModel()
        {
            InitCommand();
        }

        private void InitCommand()
        {
            var json = File.ReadAllText(@"Resources\json\SimulationMenu.json");
            Simulations = new ObservableCollection<SimulationTreeModel>();
            List<SimulationMenuList> simulationMenuLists = JsonConvert.DeserializeObject<List<SimulationMenuList>>(json);
            foreach (SimulationMenuList simulationMenuList in simulationMenuLists)
            {
                Products = new ObservableCollection<Product>();
                List<Product> products = simulationMenuList.Products;
                foreach (Product product in products)
                {
                    Products.Add(product);
                }
                Simulations.Add(new SimulationTreeModel()
                {
                    Id = simulationMenuList.Id,
                    Name = simulationMenuList.Name,
                    Products = Products
                }) ;
            }

            //Products = new ObservableCollection<Product>() {
            //    new Product() { ProductId = 1, ProductName = "宇视机制登录" },
            //    new Product() { ProductId = 2, ProductName = "速通门登录" },
            //    new Product() { ProductId = 3, ProductName = "人脸白名单" },
            //    new Product() { ProductId = 4, ProductName = "人脸黑名单" },
            //    new Product() { ProductId = 5, ProductName = "室内机" },
            //};

            //Simulations = new ObservableCollection<SimulationTreeModel>
            //{
            //    new SimulationTreeModel() { Id = 1, Name = "外经贸接口模拟", Products = Products },
            //    new SimulationTreeModel() { Id = 2, Name = "第三方接口模拟1", Products = new ObservableCollection<Product>() },
            //    new SimulationTreeModel() { Id = 3, Name = "第三方接口模拟2", Products = new ObservableCollection<Product>() },
            //    new SimulationTreeModel() { Id = 4, Name = "第三方接口模拟3", Products = new ObservableCollection<Product>() }
            //};
        }

    }

    public class SimulationProductInfo
    {
        public string title { get; set; }
        public string annotation { get; set; }
        public int method { get; set; }
        public int protocol { get; set; }
        public string ip { get; set; }
        public int port { get; set; }
        public string uri { get; set; }
        public int contentType { get; set; }
        public string jsonContext { get; set; }
        public string responseJsonContext1 { get; set; }
        public string responseJsonContext2 { get; set; }
    }


}
