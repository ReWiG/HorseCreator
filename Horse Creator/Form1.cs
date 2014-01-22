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
using System.Drawing.Imaging;
using System.Data.SQLite;

namespace Horse_Creator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            long totalBytes;
            byte[] buffer;
            using (FileStream stream = new FileStream("2r.png", FileMode.Open, FileAccess.Read))
            {
	            BinaryReader br = new BinaryReader(stream);

                totalBytes = new FileInfo("2r.png").Length;
	            buffer = br.ReadBytes((Int32)totalBytes);
	            stream.Close();
	            br.Close();
            }

            SQLiteConnection conn = new SQLiteConnection(@"Data Source=horse.db");

            //Подготовим запрос к базе на сохранение
            SQLiteCommand sqlCommand = new SQLiteCommand();
            sqlCommand.Connection = conn;
            sqlCommand.CommandType = CommandType.Text;
 
            //Текст запроса
            sqlCommand.CommandText = "INSERT INTO Horse (horseName, horseImage) VALUES ('Название2', @Kartinka);";
 
            //Параметр @Kartinka - наш массив байтов
            sqlCommand.Parameters.Add("@Kartinka", DbType.Binary, buffer.Length).Value = buffer;
 
            //Открываем соединение с базой данных
            conn.Open();
 
            //Выполняем запрос на запись
            //и получаем уникальный идентификатор картинки
            int intID = Convert.ToInt32(sqlCommand.ExecuteScalar());
 
            //Завершаем выполнение команды
            sqlCommand.Cancel();
 
            //Закрываем соединение с базой данных
            conn.Close();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Подготовим соединение с базой данных
            SQLiteConnection sqlConnection = new SQLiteConnection(@"Data Source=horse.db");

            //Подготовим запрос к базе на сохранение
            SQLiteCommand sqlCommand = new SQLiteCommand();
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandType = CommandType.Text;

            //Текст запроса
            sqlCommand.CommandText = "SELECT horseImage FROM horse WHERE id = @imageID";

            //Параметр @imageID - наш идентификатор
            sqlCommand.Parameters.Add("@imageID", DbType.Int32).Value = 1;

            //Открываем соединение с базой данных
            sqlConnection.Open();

            //Выполняем запрос на чтение
            byte[] ImageBytes = (byte[])sqlCommand.ExecuteScalar();

            //Завершаем выполнение команды
            sqlCommand.Cancel();

            //Закрываем соединение с базой данных
            sqlConnection.Close();

            MemoryStream ImageStream = new MemoryStream(ImageBytes);

            //Создадим изображение из потока
            Image Image = Image.FromStream(ImageStream);

            //Закроем поток
            ImageStream.Close();

            pictureBox1.Image = Image;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
