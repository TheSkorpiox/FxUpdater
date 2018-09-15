using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FxUpdater
{
    public static class Utils
    {
        public static string ToMo(this double size, string format)
        {
            return ((size / 1000000).ToString(format));
        }

        public static void CopyServerFiles(this string path, string copyTo)
        {
            Directory.CreateDirectory($"{path}/{copyTo}");

            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                string fileName = file.Split('\\').Last();
                File.Move($"{path}/{fileName}", $"{path}/{copyTo}/{fileName}");
            }

            if (Directory.Exists($"{path}/citizen"))
                Directory.Move($"{path}/citizen", $"{path}/{copyTo}/citizen");
        }
    }
}
