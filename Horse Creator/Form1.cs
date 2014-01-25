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
        DbManager DbMan = new DbManager();

        public Form1()
        {
            InitializeComponent();

            object[] horseName = DbMan.SelectHorseNames();

            if (horseName != null && horseName.Length != 0)
            {
                ActivateItemsAdded();

                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(horseName);
                comboBox1.Text = comboBox1.Items[0].ToString();
            }
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
            byte[] ImageBytes = (byte[])sqlCommand.ExecuteScalar(); // ОБРАБОТАТЬ ИСКЛЮЧЕНИЕ!!!

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
            // Обновление
        }

        // Открытие картинки
        private void button5_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox2.Text = openFileDialog1.FileName;
            }
        }

        // Добавление породы
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (DbMan.InsertHorse(textBox1.Text, richTextBox1.Text) > 0)
                {
                    object[] obj = DbMan.SelectHorseNames();
                    if (obj != null && obj.Length != 0)
                    {
                        ActivateItemsAdded();

                        comboBox1.Items.Clear();

                        comboBox1.Items.AddRange(obj);
                        comboBox1.Text = comboBox1.Items[0].ToString();
                        comboBox1.DroppedDown = true;
                    }
                }
                else
                {
                    MessageBox.Show("Ошибка БД!");
                    Environment.Exit(0);
                }
            }
            else
            {
                MessageBox.Show("Вы не ввели название породы, повторите попытку");
            }
        }

        // Активируем элементы добавления картинок
        private void ActivateItemsAdded()
        {
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
            textBox2.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }

        // Добавление картинок в базу
        private void button6_Click(object sender, EventArgs e)
        {
            if(comboBox1.Text != "" && textBox2.Text != "" && comboBox2.Text != "")
            {
                if (DbMan.InsertImage(comboBox1.Text, textBox2.Text, comboBox2.Text) > 0)
                {
                    MessageBox.Show("Изображение добавлено!");
                }
                else
                {
                    MessageBox.Show("Ошибка добавления, проверьте правильность ввода.");
                }
            }
            else
            {
                MessageBox.Show("Не все поля заполнены!");
            }
        }
    }
}
