using System;
using System.Windows.Forms;

namespace Tup.Dota2Recipe.Spider
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //TestFile(@"test_qmap\chat_schinese.txt");
            //TestFile(@"test_qmap\default_viper.txt");
            //TestFile(@"test_qmap\dota_schinese.txt");
            //TestFile(@"test_qmap\items_schinese.txt");
            //TestFile(@"test_qmap\gameui_schinese.txt"); 
            //TestFile(@"test_qmap\valve_schinese.txt");
            //Console.Read();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="fileName"></param>
        //static void TestFile(string fileName)
        //{
        //    var file = File.ReadAllText(fileName, Encoding.UTF8);
        //    var qmap = SimpleQMapParser.Parse(file);
        //    Console.WriteLine("{0}:{1}", fileName, qmap);
        //}
    }
}
