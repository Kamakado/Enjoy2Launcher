using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


namespace Enjoy2_Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Получение списка файлов
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*", SearchOption.AllDirectories);
            //Создание массива структур описания файлов
            CRCTools.File_Info_t[] file_source=null;
            //Изменение размера массива
            Array.Resize(ref file_source, files.Length); 
            //Вычисление контрольной суммы файла
            for (int i = 0; i < files.Length; i++)
            {
                //Открытие файла
                FileStream fsrd = File.OpenRead(files[i]);
                //Запись полного пути в структуру
                file_source[i].filepath =files[i];
                //Запись имени файла в структуру
                file_source[i].filename =files[i].Replace(AppDomain.CurrentDomain.BaseDirectory,"");
                //Запись контрольной суммы файла в структуру
                file_source[i].crc32 = CRCTools.CalculateCRC(fsrd);
                //Вывод на экран в listBox
                listBox1.Items.Add(file_source[i].filename + " " + Convert.ToString(file_source[i].crc32,16));
                //Закрытие файла
                fsrd.Close();
            }
            //Запись в файл
            StreamWriter  fswr = new StreamWriter(@"output.txt");
            for (int i = 0; i < file_source.Length; i++)
            {
                fswr.WriteLine(file_source[i].filename+ ":"+Convert.ToString(file_source[i].crc32,16));
            }
            fswr.Close();
            //File.WriteAllText("C:\\output.txt", Array.ConvertAll(File.ReadAllLines("C:\\input.txt"), Convert.ToInt32).Sum().ToString());

        }
    }
}
