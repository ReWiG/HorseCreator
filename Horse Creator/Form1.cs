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
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(horseName);
            }
        }

        // Проверка обновления
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                WebClient web = new WebClient();
                web.DownloadFile(Properties.Settings.Default.updateUrl + "upd/ver.txt", "ver.txt"); // Качаем файл и сохраняем локально
                String[] fileHashes = File.ReadAllLines("ver.txt");
                File.Delete("ver.txt"); // Удаляем локальную копию

                if (fileHashes[0] == Application.ProductVersion)
                {
                    MessageBox.Show("Обновления отсутствуют!");
                }
                else
                {
                    if(MessageBox.Show("Доступна новая версия! Выполнить обновление?",
                        "Доступно обновление", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        this.Hide();
                        
                        FormUpdater upd = new FormUpdater();
                        upd.Show(this);                        
                    }
                }
            }
            catch
            {     // Если номер версии не можем получить,
                MessageBox.Show("Невозможно проверить обновление!");
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
            pictureBox1.Image = DbMan.SelectImageColor(comboBox3.SelectedItem.ToString());
        }
    }
}
