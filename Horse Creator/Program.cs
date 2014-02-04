using System;
using System.IO;
using System.Windows.Forms;

namespace Horse_Creator
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string[] keys = Environment.GetCommandLineArgs();
            
            if (keys.Length >= 2)
            {
                if (keys[1] == "/u")
                {  // Этап Б. Запущена новая версия из временного файла 
                    do_copy_downloaded_program();
                    
                }
                else if (keys[1] == "/d")  // Этап Ц. Осталось удалить временный файл.
                    do_delete_old_program();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static void do_copy_downloaded_program()
        {
            String[] files = File.ReadAllLines("FileToUpdate.txt");
            foreach (var item in files)
            {
                int loop = 10; // Количество попыток 
                while (--loop > 0 && File.Exists(item))
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(200); // Небольшая задержка
                    }
                }

                try
                {   // Копируем скачанный файл в оригинальное имя файла
                    File.Copy("new." + item, item);
                }
                catch
                {
                    MessageBox.Show("Ошибка копирования Файлов");
                }
            }

            // Запускаем этап «Ц»
            run_program(files[0], "/d");
            Environment.Exit(0);
        }

        static void do_delete_old_program()
        {
            String[] files = File.ReadAllLines("FileToUpdate.txt");
            foreach (var item in files)
            {
                int loop = 10; // Количество попыток 
                while (--loop > 0 && File.Exists(item))
                {
                    try
                    {
                        File.Delete("new." + item);
                    }
                    catch
                    {
                        System.Threading.Thread.Sleep(200); // Небольшая задержка
                    }
                }
            }
            File.Delete("FileToUpdate.txt");
        }

        public static void run_program(string filename, string keys)
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
    }
}
