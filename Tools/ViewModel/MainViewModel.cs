using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Tools.ViewModel
{
    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainViewModel : ViewModelBase
    {
        #region Property
        public RelayCommand RefCommand { get; set; }

        public List<string> colors;

        public List<TransitionEffectKind> kinds;

        public ObservableCollection<MetorInfo> metorInfos;
        public ObservableCollection<MetorInfo> MetorInfos
        {
            get { return metorInfos; }
            set { metorInfos = value; RaisePropertyChanged(); }
        }

        private ObservableCollection<ContentItems> listBoxItems;
        public ObservableCollection<ContentItems> ListBoxItems
        {
            get { return listBoxItems; }
            set { listBoxItems = value; RaisePropertyChanged(); }
        }
        #endregion

        public List<MenuItem> LeftMenus { get; set; }
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            ListBoxItems = new ObservableCollection<ContentItems>();
            InitStyle();
            var random = new Random();
            InitCommand(random);

            LeftMenus = new List<MenuItem>();
            LeftMenus.Add(new MenuItem("智慧校园管理平台", PackIconKind.Image));
            LeftMenus.Add(new MenuItem("银校通管理平台", PackIconKind.Music));
            LeftMenus.Add(new MenuItem("外经贸测试", PackIconKind.Video));
            LeftMenus.Add(new MenuItem("文档", PackIconKind.Folder));

        }

        private void InitCommand(Random random)
        {
            ListBoxItems.Clear();
            for (int i = 0; i < 9; i++)
            {
                var contentItems = new ContentItems() { MetorInfos = new ObservableCollection<MetorInfo>() };
                for (int j = 0; j < 7; j++)
                {
                    //var metorInfos = new ObservableCollection<MetorInfo>();
                    var metor = new MetorInfo
                    {
                        Name = "应用" + j,
                        Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[random.Next(0, colors.Count)])),
                        Width = random.Next(0, 8) < 4 ? 314 : 150,
                        Height = 150,
                        Effect = new TransitionEffect()
                        {
                            Kind = kinds[random.Next(0, kinds.Count)],
                            Duration = new TimeSpan(0, 0, 0, 0, 900)
                        }
                    };
                    contentItems.MetorInfos.Add(metor);
                }
                ListBoxItems.Add(contentItems);
            }
        }

        private void InitStyle()
        {
            ///颜色
            colors = new List<string>
            {
                "#1784db"
            };
            ///动画
            kinds = new List<TransitionEffectKind>
            {
                TransitionEffectKind.FadeIn
            };
            //kinds.Add(TransitionEffectKind.ExpandIn);
            //kinds.Add(TransitionEffectKind.SlideInFromLeft);
            //kinds.Add(TransitionEffectKind.SlideInFromBottom);
            //kinds.Add(TransitionEffectKind.SlideInFromRight);
            //kinds.Add(TransitionEffectKind.SlideInFromTop);
            //kinds.Add(TransitionEffectKind.None);
        }
    }

    public class ContentItems
    {
        public ObservableCollection<MetorInfo> MetorInfos { get; set; }
    }

    public class MetorInfo
    {
        public string Name { get; set; }
        public Brush Color { get; set; }
        public TransitionEffect Effect { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class MenuItem
    {
        public String Name { get; private set; }
        public PackIconKind Icon { get; private set; }

        public MenuItem(String name, PackIconKind icon)
        {
            Name = name;
            Icon = icon;
        }
    }
}