using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Drawing;

namespace Horse_Creator
{
    class DbManager
    {
        SQLiteConnection connect = new SQLiteConnection(@"Data Source=horse.db"); // Коннектор

        /// <summary>
        /// Добавляет породу лошади в БД
        /// </summary>
        /// <param name="Name">Название породы</param>
        /// <param name="Desc">Описание породы</param>
        /// <returns>ID добавленной записи или -1 в случае ошибки</returns>
        public Int32 InsertHorse(String Name, String Desc)
        {
            try
            {
                //Подготовим запрос к базе на сохранение
                SQLiteCommand sqlCommand = new SQLiteCommand();
                sqlCommand.Connection = connect;
                sqlCommand.CommandType = CommandType.Text;

                //Текст запроса
                sqlCommand.CommandText = "INSERT INTO horse (horseName, horseDescription) VALUES (@Name, @Desc); SELECT last_insert_rowid();";

                //Параметры
                sqlCommand.Parameters.Add("@Name", DbType.String).Value = Name;
                sqlCommand.Parameters.Add("@Desc", DbType.String).Value = Desc;

                //Открываем соединение с базой данных
                connect.Open();

                //Выполняем запрос на запись
                //и получаем уникальный идентификатор картинки, либо 0 в случае ошибки
                int intID = Convert.ToInt32(sqlCommand.ExecuteScalar());

                //Завершаем выполнение команды
                sqlCommand.Cancel();

                //Закрываем соединение с базой данных
                connect.Close();

                return intID;
            }
            catch (SQLiteException e)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка работы с базой данный. Текст ошибки(сообщите разработчику):"+e);
                return -1;
            }
        }

        /// <summary>
        /// Получает из базы все имена пород лошадей
        /// </summary>
        /// <returns>Массив имён</returns>
        public Object[] SelectHorseNames()
        {
            try
            {
                //Подготовим запрос к базе на сохранение
                SQLiteCommand sqlCommand = new SQLiteCommand();
                sqlCommand.Connection = connect;
                sqlCommand.CommandType = CommandType.Text;

                //Текст запроса
                sqlCommand.CommandText = "SELECT count(*) FROM horse;";

                //Открываем соединение с базой данных
                connect.Open();

                //определяем кол-во записей
                int countHorse = Convert.ToInt32(sqlCommand.ExecuteScalar());

                sqlCommand.CommandText = "SELECT horseName FROM horse;";

                //Выполняем запрос на чтение
                SQLiteDataReader reader = sqlCommand.ExecuteReader();

                Object[] value = new Object[countHorse];

                int i = 0;

                while (reader.Read())//а здесь собственно записи полей
                {
                    value[i] = reader[0].ToString();
                    i++;
                }

                //Завершаем выполнение команды
                sqlCommand.Cancel();

                //Закрываем соединение с базой данных
                connect.Close();

                //Закроем поток
                reader.Close();

                return value;
            }
            catch(SQLiteException e)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка работы с базой данный. Текст ошибки(сообщите разработчику):" + e);
                return null;
            }
        }

        /// <summary>
        /// Добавляет картинку в базу данных
        /// </summary>
        /// <param name="horseName">Имя лошади, которой добавляется картинка</param>
        /// <param name="imagePath">Путь до картинки</param>
        /// <param name="imageType">Тип добавляемой картинки (Тело, грива, хвост)</param>
        /// <returns>ID добавленной записи, либо 0 или -1 в случае ошибки</returns>
        public Int32 InsertImage(String horseName, String imagePath, String imageType)
        {
            long totalBytes;
            byte[] buffer;

            try
            {
                using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
                {
                    BinaryReader br = new BinaryReader(stream);

                    totalBytes = new FileInfo(imagePath).Length;
                    buffer = br.ReadBytes((Int32)totalBytes);
                    br.Close();
                }
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка открытия файла. Текст ошибки(сообщите разработчику):" + e);
                return -1;
            }
                

            //Подготовим запрос к базе на сохранение
            SQLiteCommand sqlCommand = new SQLiteCommand();
            sqlCommand.Connection = connect;
            sqlCommand.CommandType = CommandType.Text;

            // Выбор варианта запроса
            switch (imageType)
            {
                case "Тело":
                    sqlCommand.CommandText = "INSERT INTO horseColorImg (fkHorse, colorImg) SELECT horseId, @img FROM horse WHERE horseName = @horseName LIMIT 1; SELECT last_insert_rowid();";
                    break;
                case "Грива":
                    sqlCommand.CommandText = "INSERT INTO horseManeImg (fkHorse, maneImg) SELECT horseId, @img FROM horse WHERE horseName = @horseName LIMIT 1; SELECT last_insert_rowid();";
                    break;
                case "Хвост":
                    sqlCommand.CommandText = "INSERT INTO horseTailImg (fkHorse, tailImg)  SELECT horseId, @img FROM horse WHERE horseName = @horseName LIMIT 1; SELECT last_insert_rowid();";
                    break;
                default:
                    System.Windows.Forms.MessageBox.Show("Неверный тип добавляемого изображения: " + imageType);
                    return -1;
            }
            
            //Добавляем параметры
            sqlCommand.Parameters.Add("@img", DbType.Binary, buffer.Length).Value = buffer;
            sqlCommand.Parameters.Add("@horseName", DbType.String).Value = horseName;

            //Открываем соединение с базой данных
            connect.Open();

            //Выполняем запрос на запись
            //и получаем уникальный идентификатор картинки, либо 0 в случае ошибки
            int intID = Convert.ToInt32(sqlCommand.ExecuteScalar());

            //Завершаем выполнение команды
            sqlCommand.Cancel();

            //Закрываем соединение с базой данных
            connect.Close();

            return intID;
        }

        /// <summary>
        /// Получает картинку тела лошади
        /// </summary>
        /// <param name="horseName">Имя лошади</param>
        /// <returns>Картинка тела или null в случае ошибки</returns>
        public Image SelectImageColor(String horseName)
        {
            try
            {
                //Подготовим запрос к базе на сохранение
                SQLiteCommand sqlCommand = new SQLiteCommand();
                sqlCommand.Connection = connect;
                sqlCommand.CommandType = CommandType.Text;

                //Текст запроса
                sqlCommand.CommandText = "SELECT colorImg FROM horseColorImg WHERE fkHorse = (SELECT horseId FROM horse WHERE horseName = @horseName) LIMIT 1;";

                //Параметр Имя лошади
                sqlCommand.Parameters.Add("@horseName", DbType.String).Value = horseName;

                //Открываем соединение с базой данных
                connect.Open();

                //Выполняем запрос на чтение
                byte[] ImageBytes = (byte[])sqlCommand.ExecuteScalar();

                //Завершаем выполнение команды
                sqlCommand.Cancel();

                //Закрываем соединение с базой данных
                connect.Close();

                if (ImageBytes != null)
                {
                    using (MemoryStream ImageStream = new MemoryStream(ImageBytes))
                    {
                        //Создадим изображение из потока
                        return Image.FromStream(ImageStream);
                    }
                }
                else
                {
                    return null; // Если нет картинки в базе
                }
            }
            catch (SQLiteException e)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка работы с базой данный. Текст ошибки(сообщите разработчику):" + e);
                return null;
            }
        }

        /// <summary>
        /// Получает из БД списки с изображениями хвостов или грив, в зависимости от параметра option
        /// </summary>
        /// <param name="horseName">Имя лошади</param>
        /// <param name="option">"mane" или "tail"</param>
        /// <returns>List с изображениями или null в случае ошибки</returns>
        public List<Image> SelectImageManeOrTail(String horseName, String option)
        {
            try
            {
                //Подготовим запрос к базе на сохранение
                SQLiteCommand sqlCommand = new SQLiteCommand();
                sqlCommand.Connection = connect;
                sqlCommand.CommandType = CommandType.Text;

                //Текст запроса
                switch(option){
                    case "mane":
                        sqlCommand.CommandText = "SELECT maneImg FROM horseManeImg WHERE fkHorse = (SELECT horseId FROM horse WHERE horseName = @horseName);";
                        break;
                    case "tail":
                        sqlCommand.CommandText = "SELECT tailImg FROM horseTailImg WHERE fkHorse = (SELECT horseId FROM horse WHERE horseName = @horseName);";
                        break;
                    default:
                        System.Windows.Forms.MessageBox.Show("Неверный тип атрибутов(картинок)");
                        return null;
                }
                //Параметр Имя лошади
                sqlCommand.Parameters.Add("@horseName", DbType.String).Value = horseName;

                //Открываем соединение с базой данных
                connect.Open();

                //Выполняем запрос на чтение
                SQLiteDataReader reader = sqlCommand.ExecuteReader();

                List<Image> lImg = new List<Image>();

                while (reader.Read())
                {
                    using (MemoryStream ImageStream = new MemoryStream((byte[])reader[0]))
                    {
                        //Создадим изображение из потока
                        lImg.Add(Image.FromStream(ImageStream));
                    }
                }

                //Завершаем выполнение команды
                sqlCommand.Cancel();

                //Закрываем соединение с базой данных
                connect.Close();

                reader.Close();

                return lImg; // Если нет картинки в базе
            }
            catch (SQLiteException e)
            {
                System.Windows.Forms.MessageBox.Show("Ошибка работы с базой данный. Текст ошибки(сообщите разработчику):" + e);
                return null;
            }
        }
    }
}
