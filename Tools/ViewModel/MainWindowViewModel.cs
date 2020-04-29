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
using System.Windows.Media;
using Tools.BeanModel;

namespace Tools.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        private string pageUrl;

        public string PageUrl
        {
            get { return pageUrl; }
            set { pageUrl = value; RaisePropertyChanged(); }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainWindowViewModel()
        {
            PageUrl = "pack://application:,,,/pages/Index.xaml";
        }

    }

}
