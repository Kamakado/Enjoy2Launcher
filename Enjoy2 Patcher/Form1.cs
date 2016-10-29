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
using System.Diagnostics;

namespace Enjoy2_Patcher
{
    public partial class Form1 : Form
    {
        WebClient webClient;    // Our WebClient that will be doing the downloading for us
        Stopwatch sw = new Stopwatch();    // The stopwatch which we will be using to calculate the download speed
        bool downloaded; //флаг загрузки 
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }
        //"name\"=\"pack\\Zone.epk\" \"CRC32\"=\"ba0a7990\" \"size\"=\"93092608\"\r\n"
        //artpclnt.dll CRC32: 1917343657 MD5: 26B7EF7142E838B886FED8159C891A10 Size: 110592
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            button1.Enabled = false;
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
                file_source[i].filepath = files[i];
                //Запись имени файла в структуру
                file_source[i].filename = files[i].Replace(AppDomain.CurrentDomain.BaseDirectory, "");
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
            //Сравнение и загрузка\удаление файлов
            for (int i = 0; i < file_source.Length; i++)
            {
                for (int j = 0; j < file_cloud.Length; j++)
                {
                    //Если имена файлов равны и не было совпадений, то
                    if ((file_source[i].filename == file_cloud[j].filename) && (!file_cloud[j].check))
                    {
                        file_source[i].check = true;
                        //Проверка на чек сумму и размер
                        if ((file_source[i].crc32 == file_cloud[j].crc32) && (file_source[i].size == file_cloud[j].size))
                        {
                            file_cloud[j].check = true;
                            break;
                        }
                        else
                        {
                            //webClient.DownloadFileAsync(new Uri(link), "sPCK.exe"); //куда сохранить, в данный момент в папку где запущена программа
                            //Удаляем файл
                            File.Delete(file_source[i].filepath);
                            label1.Text = file_cloud[i].filename;
                            //Скачиваем новый
                            downloadFile((@"http://www.kamakado.ru/patcher/" + file_cloud[i].filename.Replace(@"\", @"/")), AppDomain.CurrentDomain.BaseDirectory + file_cloud[i].filename);
                        }
                    }
                }
                if (!file_source[i].check)
                {
                    try
                    {
                        //Удаляем файл
                        File.Delete(file_source[i].filepath);
                    }
                    catch
                    {

                    }
                }
            }
            progressBar1.Value = 0;
            progressBar1.Maximum = file_cloud.Length;
            //Докачка недостающих файлов
            for (int i = 0; i < file_cloud.Length; i++)
            {
                if ((!file_cloud[i].check) && (file_cloud[i].filename != "Enjoy2 Launcher.exe"))
                {
                    try
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(file_cloud[i].filename));
                    }
                    catch
                    {

                    }
                    label1.Text = file_cloud[i].filename;
                    downloaded = false;
                    downloadFile((@"http://www.kamakado.ru/patcher/" + file_cloud[i].filename.Replace(@"\", @"/")), AppDomain.CurrentDomain.BaseDirectory + file_cloud[i].filename);
                    while(!downloaded)
                    {

                    }
                }
            }
            button1.Enabled = true;
            //for()
        }

        public void downloadFile(string urlAddress, string location)
        {
            using (webClient = new WebClient())
            //{
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(webClient_DownloadProgressChanged);
                try
                {
                    // The variable that will be holding the url address
                    Uri URL;
                    // Make sure the url starts with "http://"
                    if (!urlAddress.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
                        URL = new Uri("http://" + urlAddress);
                    else
                        URL = new Uri(urlAddress);
                    // Start the stopwatch which we will be using to calculate the download speed
                    sw.Start();
                    // Start downloading the file
                    webClient.DownloadFileAsync(URL, location);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            //}
        }
        // The event that will trigger when the WebClient is completed
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            progressBar1.Value++;
            downloaded = true;
        }


        // The event that will fire whenever the progress of the WebClient is changed
        private void webClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            try
            {
                // Calculate download speed and output it to label3
                if (label3.Text != (Convert.ToDouble(e.BytesReceived) / 1024 / sw.Elapsed.TotalSeconds).ToString("0"))
                    label2.Text = (Convert.ToDouble(e.BytesReceived) / 1024 / sw.Elapsed.TotalSeconds).ToString("0.00") + " kb/s";

                // Update the progressbar percentage only when the value is not the same (to avoid updating the control constantly)
                if (progressBar2.Value != e.ProgressPercentage)
                    progressBar2.Value = e.ProgressPercentage;

                // Show the percentage on our label (update only if the value isn't the same to avoid updating the control constantly)
                if (label3.Text != e.ProgressPercentage.ToString() + "%")
                    label3.Text = e.ProgressPercentage.ToString() + "%";

                // Update the label with how much data have been downloaded so far and the total size of the file we are currently downloading
                label4.Text = (Convert.ToDouble(e.BytesReceived) / 1024 / 1024).ToString("0.00") + " Mb's" + "  /  " + (Convert.ToDouble(e.TotalBytesToReceive) / 1024 / 1024).ToString("0.00") + " Mb's";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
} 