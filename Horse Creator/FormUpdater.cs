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
        List<String> filesToUpdate;
        String programName;

        public FormUpdater(String[] fileHashes)
        {
            InitializeComponent();

            filesToUpdate = new List<string>(); // Список файлов для загрузки
            programName = "new." + fileHashes[1].Trim().Split('|')[0]; // Имя обновлённой программы

            // В цикле разделяем названия файлов и хэши, проверяем хэши
            for (int i = 1; i < fileHashes.Length; i++)
            {
                string[] arrFile_Hash = fileHashes[i].Trim().Split('|'); // Разделяем

                // Проверяем наличие файлов и хэши
                if (!File.Exists(arrFile_Hash[0]) ||
                    ComputeMD5Checksum(arrFile_Hash[0]) != arrFile_Hash[1])
                {
                    filesToUpdate.Add(arrFile_Hash[0]); // Добавляем файл в список для загрузки
                }
            }
            if (filesToUpdate.Count != 0)
            {
                File.WriteAllLines("FileToUpdate.txt", filesToUpdate); // Записываем файл с именами файлов

                progressBar2.Maximum = filesToUpdate.Count; // Задаем максимальное значение общего прогрессбара

                DownloadFile(); // Начинаем рекурсивную загрузку
            }
            else
            {
                MessageBox.Show("Нет файлов для обновления О_о. Сообщите разработчику");
            }
        }

        // Рекурсивный метод для последовательной загрузки файлов
        private void DownloadFile()
        {
            if (filesToUpdate.Count != 0)
            {
                using (WebClient webClient = new WebClient())
                {
                    // Создаём обработчики событий продвижения прогресса и его окончания
                    webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);

                    // Если файл скачался неправильно?

                        // Начинаем скачивание
                    webClient.DownloadFileAsync(new Uri(Properties.Settings.Default.updateUrl +
                            "upd/" + filesToUpdate[0]), "new." + filesToUpdate[0]);
                    
                    label3.Text = filesToUpdate[0]; // Указываем тукущий скачиваемый файл

                    progressBar2.PerformStep();

                    filesToUpdate.RemoveAt(0); // Удаляем скачанный элемент

                }
            }
            else
            {
                run_program(programName, "/u"); // Запускаем обновленную программу
                Environment.Exit(0);
            }
        }

        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        public void Completed(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Ошибка загрузки файлов, повторите попытку чуть позже. " + e.Error.Message);
                this.Close();
                this.Dispose();
            }
            else
            {
                DownloadFile();
            }
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

        private void run_program(string filename, string keys)
        {
            try
            {   // Использование системных методов для запуска программы
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.WorkingDirectory = Application.StartupPath;
                proc.StartInfo.FileName = filename;
                proc.StartInfo.Arguments = keys; // Аргументы командной строки
                proc.Start(); // Запускаем!
            }
            catch (Exception ex)
            {
                MessageBox.Show("Невозможно запустить обновлённую программу!" + ex);
            }
        }

        private void FormUpdater_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
