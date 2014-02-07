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
using System.Net;

namespace Horse_Creator
{
    public partial class Form1 : Form
    {
        DbManager DbMan = new DbManager(); // Менеджер базы данных
        Image imgHorse; // Картинка текущей выбранной лошади(тело)
        Image defaultImg;
        List<Image> maneImage; // Список с изображениями грив лошади
        List<Image> tailImage; // Список с изображениями хвостов лошади
        Graphics g;
        Int32 maneCount = 0; // Номер текущей гривы
        Int32 tailCount = 0; // Номер текущего хвоста

        public Form1()
        {
            InitializeComponent();

            label7.Text = "Версия " + Application.ProductVersion;

            object[] horseName = DbMan.SelectHorseNames();

            if (horseName != null && horseName.Length != 0)
            {
                ActivateItemsAdded();

                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(horseName);
                comboBox1.Text = comboBox1.Items[0].ToString();
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(horseName);
            }
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
                        comboBox3.Items.Clear();
                        comboBox3.Items.AddRange(obj);
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
            comboBox3.Enabled = true;
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
                    textBox2.Text = "";
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

        private void comboBox3_SelectionChangeCommitted(object sender, EventArgs e)
        {
            // Обнуляем всё
            pictureBox1.Image = null;
            label8.Visible = false;
            label10.Visible = false;
            button1.Visible = false;
            label9.Visible = false;
            label11.Visible = false;
            button3.Visible = false;
            button2.Visible = false;
            label12.Text = "";

            CompileImage();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Сохранение картинки в файл
            Stream myStream;
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    imgHorse.Save(myStream, ImageFormat.Png);
                    // Code to write the stream goes here.
                    myStream.Close();
                }
            }
            
        }

        private void проверитьОбновлениеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // Качаем файл, сохраняем локально
                WebClient web = new WebClient();
                web.DownloadFile(Properties.Settings.Default.updateUrl + "upd/ver.txt", "ver.txt");

                // Читаем первую строку
                String[] fileHashes = File.ReadAllLines("ver.txt");
                File.Delete("ver.txt"); // Удаляем локальную копию

                if (fileHashes[0] == Application.ProductVersion)
                {
                    MessageBox.Show("Обновления отсутствуют!");
                }
                else
                {
                    if (MessageBox.Show("Доступна новая версия! Выполнить обновление?",
                        "Доступно обновление", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        FormUpdater upd = new FormUpdater(fileHashes);
                        upd.ShowDialog(this);

                    }
                }
            }
            catch (Exception ee)
            {     // Если номер версии не можем получить,
                MessageBox.Show("Невозможно проверить обновление!" + ee);
            }
        }

        private void CompileImage()
        {
            // Создаем новый image нужного размера (это будет объединенный image)
            imgHorse = DbMan.SelectImageColor(comboBox3.SelectedItem.ToString());

            // Проверяем, есть ли картинка 
            if (imgHorse != null)
            {
                pictureBox1.Image = imgHorse;
                label12.Text = DbMan.SelectHorseDescription(comboBox3.SelectedItem.ToString());
                button2.Visible = true;

                // Делаем этот image нашим контекстом, куда будем рисовать
                g = Graphics.FromImage(imgHorse);

                maneImage = DbMan.SelectImageManeOrTail(comboBox3.SelectedItem.ToString(), "mane");
                tailImage = DbMan.SelectImageManeOrTail(comboBox3.SelectedItem.ToString(), "tail");

                if ((maneImage != null) && (maneImage.Count != 0))
                {
                    label8.Visible = true;
                    label10.Visible = true;
                    label10.Text = "(" + maneImage.Count.ToString() + ")";
                    button1.Visible = true;

                    // проверка счетчика на корректность
                    if (maneCount >= maneImage.Count)
                        maneCount = 0;

                    // добавляем image
                    g.DrawImage(maneImage[maneCount], new Point(0, 0));
                }

                if ((tailImage != null) && (tailImage.Count != 0))
                {
                    label9.Visible = true;
                    label11.Visible = true;
                    label11.Text = "(" + tailImage.Count.ToString() + ")";
                    button3.Visible = true;

                    // проверка счетчика на корректность
                    if (tailCount >= tailImage.Count)
                        tailCount = 0;

                    // добавляем image
                    g.DrawImage(tailImage[tailCount], new Point(0, 0));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            maneCount++;
            CompileImage();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tailCount++;
            CompileImage();
        }

        private void обАвторахToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Программист: Кирилл RiG Тестин\nХудожник: Анна Katze Hass Викторовна");
        }
    }
}
