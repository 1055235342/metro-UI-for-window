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
    /// <summary>
    /// </para>
    /// </summary>
    public class IndexViewModel : ViewModelBase
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
        public IndexViewModel()
        {
            ListBoxItems = new ObservableCollection<ContentItems>();
            InitStyle();
            var random = new Random();
            InitCommand(random);

            LeftMenus = new List<MenuItem>();
            //LeftMenus.Add(new MenuItem("智慧校园管理平台", PackIconKind.Image));
            //LeftMenus.Add(new MenuItem("银校通管理平台", PackIconKind.Music));
            //LeftMenus.Add(new MenuItem("外经贸测试", PackIconKind.Video));
            //LeftMenus.Add(new MenuItem("文档", PackIconKind.Folder));
            try
            {
                var json = File.ReadAllText(@"Resources\json\menu.json");
                List<MenuList> menuLists = JsonConvert.DeserializeObject<List<MenuList>>(json);
                foreach (MenuList menulist in menuLists)
                {
                    LeftMenus.Add(new MenuItem(menulist.name, menulist.icon));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void InitCommand(Random random)
        {
            ListBoxItems.Clear();

            var json = File.ReadAllText(@"Resources\json\block.json");
            List<Block> blockLists = JsonConvert.DeserializeObject<List<Block>>(json);
            foreach (Block block in blockLists)
            {
                var contentItems = new ContentItems() { MetorInfos = new ObservableCollection<MetorInfo>() };
                List<MetorInfo> data = block.data;
                foreach (MetorInfo metorInfo in data)
                {
                    //new TransitionEffect().LetterNCircle();
                    var metor = new MetorInfo
                    {
                        Name = metorInfo.Name,
                        Color = metorInfo.Color,
                        Width = metorInfo.Width,
                        Height = metorInfo.Height,
                        View = metorInfo.View,
                        Effect = new TransitionEffect()
                        {
                            Kind = kinds[random.Next(0, kinds.Count)],
                            Duration = new TimeSpan(0, 0, 0, 0, 10)
                        }
                    };
                    contentItems.MetorInfos.Add(metor);
                }
                ListBoxItems.Add(contentItems);
            }

            //for (int i = 0; i < 2; i++)
            //{
            //    var contentItems = new ContentItems() { MetorInfos = new ObservableCollection<MetorInfo>() };
            //    for (int j = 0; j < 5; j++)
            //    {
            //        //var metorInfos = new ObservableCollection<MetorInfo>();
            //        var metor = new MetorInfo
            //        {
            //            Name = "应用" + j,
            //            Color = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colors[random.Next(0, colors.Count)])),
            //            Width = random.Next(0, 8) < 4 ? 314 : 150,
            //            Height = 150,
            //            Effect = new TransitionEffect()
            //            {
            //                Kind = kinds[random.Next(0, kinds.Count)],
            //                Duration = new TimeSpan(0, 0, 0, 0, 900)
            //            }
            //        };
            //        contentItems.MetorInfos.Add(metor);
            //    }
            //    ListBoxItems.Add(contentItems);
            //}
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
                TransitionEffectKind.FadeIn,
                TransitionEffectKind.SlideInFromLeft,
                TransitionEffectKind.SlideInFromBottom,
                TransitionEffectKind.SlideInFromRight,
                TransitionEffectKind.SlideInFromTop
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

    public class Block
    {
        public int id { get; set; }
        public string title { get; set; }
        public int menuid { get; set; }
        public List<MetorInfo> data { get; set; }
    }

    public class MetorInfo
    {
        public int id { get; set; }
        public string Name { get; set; }
        public Brush Color { get; set; }
        public string View { get; set; }
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