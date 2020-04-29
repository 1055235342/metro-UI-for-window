using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.BeanModel
{
    public class MenuList
    {
        public MenuList() { }
        public int id { get; set; }
        public string name { get; set; }
        public PackIconKind icon { get; set; }
    }
}
