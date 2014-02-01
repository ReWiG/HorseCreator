using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Security.Cryptography;

namespace Horse_Creator
{
    public partial class FormUpdater : Form
    {
        public FormUpdater(String[] fileHashes)
        {
            InitializeComponent();

            List<String> filesToUpdate = new List<string>();

            for (int i = 1; i < fileHashes.Length; i++)
            {
                string[] arrFile_Hash = fileHashes[i].Trim().Split('|');

                if (!File.Exists(arrFile_Hash[0]) ||
                    ComputeMD5Checksum(arrFile_Hash[0]) != arrFile_Hash[1])
                {
                    filesToUpdate.Add(arrFile_Hash[0]);
                }
            }
            if (filesToUpdate.Count != 0)
            {
                File.WriteAllLines("FileToUpdate.txt", filesToUpdate);

                using (WebClient webClient = new WebClient())
                {
                    // Создаём обработчики событий продвижения прогресса и его окончания
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);

                    try
                    {
                        progressBar2.Maximum = filesToUpdate.Count;
                        foreach (var item in filesToUpdate)
                        {
                            label3.Text = item; // Указываем тукущий скачиваемый файл

                            // А если файла нет на сервере? Если файл скачался неправильно?

                            // Начинаем скачивание
                            webClient.DownloadFileAsync(new Uri(Properties.Settings.Default.updateUrl +
                                "upd/" + item), "new." + item);

                            progressBar2.PerformStep();
                        }
                    }
                    catch(WebException e)
                    {
                        MessageBox.Show("Ошибка загрузки файлов, повторите попытку чуть позже."+ e);
                        Application.Exit();
                    }
                }
            }
            else
            {
                MessageBox.Show("Нет файлов для обновления О_о. Сообщите разработчику");
            }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        public void Completed(object sender, AsyncCompletedEventArgs e)
        {
            // run_program(up_filename, "/u \"" + my_filename + "\"");
            this.Dispose();
        }

        /// <summary>
        /// Высчитывает MD5-хэш файла
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <returns>Строка с хэшем</returns>
        private string ComputeMD5Checksum(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                string result = BitConverter.ToString(checkSum).Replace("-", String.Empty);
                return result;
            }
        }
    }
}
