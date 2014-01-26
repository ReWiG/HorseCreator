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
                comboBox3.Items.Clear();
                comboBox3.Items.AddRange(horseName);
            }
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
