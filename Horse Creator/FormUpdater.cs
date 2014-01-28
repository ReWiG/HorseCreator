using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace Horse_Creator
{
    public partial class FormUpdater : Form
    {
        public FormUpdater()
        {
            InitializeComponent();

            try
            {
                WebClient webClient = new WebClient();
                // Создаём обработчики событий продвижения прогресса и его окончания
                webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);

                // Начинаем скачивание
                webClient.DownloadFileAsync(new Uri(url_program), up_filename);
            }
            catch (Exception ex)
            {  // В случае ошибки выводим сообщение и предлагаем скачать вручную
                error(ex.Message + " " + filename);
            }
        }
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progress_download.Value = e.ProgressPercentage;
        }
        private void Completed(object sender, AsyncCompletedEventArgs e)
        {
            run_program(up_filename, "/u \"" + my_filename + "\"");
            this.Close();
        }
    }
}
