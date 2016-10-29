using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Enjoy2_Launcher;
using System.Net;

namespace Enjoy2_Patcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Получение патчлиста
            WebClient test = new WebClient();
            string temp = Encoding.ASCII.GetString(test.DownloadData(@"LINK"));
            listBox1.Items.Add(temp);
            //Объявление структуры патчлиста
            CRCTools.File_Info_t[] file_cloud = null;
            label1.Text = temp;
            //progressBar1.Maximum = file_cloud.Length;
            //Парсинг в структуру
            int counter = 0;
            while (temp.IndexOf("\"name\"=\"") != -1)
            {
                //Указание размерности
                Array.Resize(ref file_cloud, (counter + 1));  
                file_cloud[counter].filename = temp.Substring(temp.IndexOf("\"name\"=\"") + 8, (temp.IndexOf("\" \"CRC32\"=\"") - (temp.IndexOf("\"name\"=\"") + 8)));
                file_cloud[counter].crc32 = uint.Parse(temp.Substring(temp.IndexOf("\"CRC32\"=\"") + 9, (temp.IndexOf("\" \"size\"=\"") - (temp.IndexOf("\"CRC32\"=\"") + 9))), System.Globalization.NumberStyles.HexNumber);
                file_cloud[counter].size = Convert.ToInt64(temp.Substring(temp.IndexOf("\"size\"=\"") + 8, (temp.IndexOf("\"\r\n") - (temp.IndexOf("\"size\"=\"") + 8))));
                listBox1.Items.Add("\"name\"=\"" + file_cloud[counter].filename + "\" \"CRC32\"=\"" + Convert.ToString(file_cloud[counter].crc32, 16) + "\" \"size\"=\"" + file_cloud[counter].size + "\"");              
                temp = temp.Remove(0, (temp.IndexOf("\r\n") + 2));
                label1.Text = file_cloud[counter++].filename;                
            }     
            //backgroundWorker1.RunWorkerAsync();
        }
        //"name\"=\"pack\\Zone.epk\" \"CRC32\"=\"ba0a7990\" \"size\"=\"93092608\"\r\n"
        //artpclnt.dll CRC32: 1917343657 MD5: 26B7EF7142E838B886FED8159C891A10 Size: 110592
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            //Получение списка файлов
            string[] files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*", SearchOption.AllDirectories);
            //Создание массива структур описания файлов
            CRCTools.File_Info_t[] file_source = null;
            //Изменение размера массива
            Array.Resize(ref file_source, files.Length);
            //progressBar1.Maximum = file_source.Length;
            //Вычисление контрольной суммы файла
            for (int i = 0; i < files.Length; i++)
            {
                //Открытие файла
                FileStream fsrd = File.OpenRead(files[i]);
                //Получение информации о файле
                FileInfo file_inform = new System.IO.FileInfo(files[i]);
                //Запись полного пути в структуру
                file_source[i].filepath =files[i];
                //Запись имени файла в структуру
                file_source[i].filename =files[i].Replace(AppDomain.CurrentDomain.BaseDirectory,"");
                //Запись контрольной суммы файла в структуру
                file_source[i].crc32 = CRCTools.CalculateCRC(fsrd);
                //Запись размера файла
                file_source[i].size = file_inform.Length;
                //Вывод на экран в listBox
                listBox1.Items.Add("\"name\"=\"" + file_source[i].filename + "\" \"CRC32\"=\"" + Convert.ToString(file_source[i].crc32, 16) + "\" \"size\"=\"" + file_source[i].size + "\"");
                //listBox1.Items.Add(file_source[i].filename + " " + Convert.ToString(file_source[i].crc32,16));
                //Закрытие файла
                fsrd.Close();
            }
            //Получение патчлиста
            WebClient test = new WebClient();
            string temp = Encoding.ASCII.GetString(test.DownloadData(@"http://kamakado.ru/patcher/patchlist"));
            listBox1.Items.Add(temp);
            //Объявление структуры патчлиста
            CRCTools.File_Info_t[] file_cloud = null;
            //Указание размерности
            Array.Resize(ref file_cloud, temp.Length);
            label1.Text = temp;
            //progressBar1.Maximum = file_cloud.Length;
            //Парсинг в структуру
            int counter = 0;
            while (temp.IndexOf("\"name\"=\"") != -1)
            {
                file_cloud[counter].filename = temp.Substring(temp.IndexOf("\"name\"=\"") + 8, (temp.IndexOf("\" \"CRC32\"=\"") - (temp.IndexOf("\"name\"=\"") + 8)));
                file_cloud[counter].crc32 = uint.Parse(temp.Substring(temp.IndexOf("\"CRC32\"=\"") + 9, (temp.IndexOf("\" \"size\"=\"") - (temp.IndexOf("\"CRC32\"=\"") + 9))), System.Globalization.NumberStyles.HexNumber);
                file_cloud[counter].size = Convert.ToInt64(temp.Substring(temp.IndexOf("\"size\"=\"") + 8, (temp.IndexOf("\"\r\n") - (temp.IndexOf("\"size\"=\"") + 8))));
                listBox1.Items.Add("\"name\"=\"" + file_cloud[counter].filename + "\" \"CRC32\"=\"" + Convert.ToString(file_cloud[counter].crc32, 16) + "\" \"size\"=\"" + file_cloud[counter].size + "\"");
                counter++;
                temp = temp.Remove(0, (temp.IndexOf("\r\n") + 2));
                label1.Text = file_cloud[counter].filename;
            }
            //Сравнение и загрузка\удаление файлов
            //for()
        }
    }
}
