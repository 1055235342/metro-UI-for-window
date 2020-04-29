using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools.utils
{
    public class FileUtils
    {
        public static void writeJsonFile(string path, string json)
        {
            string result1 = @path;//结果保存到桌面
            FileStream fs = new FileStream(result1, FileMode.Truncate, FileAccess.ReadWrite);
            StreamWriter wr = null;
            wr = new StreamWriter(fs);
            wr.WriteLine(json);
            wr.Close();
        }
    }
}
