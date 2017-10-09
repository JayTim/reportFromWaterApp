using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace reportFromWaterApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        //Заполняем таблицу в приложении
        void dataGridViewContainer()
        {
            dataGridView1.RowCount = 4;
            dataGridView1.RowHeadersVisible = false;

            dataGridView1.Rows[0].Cells[0].Value = "СВД-15";
            dataGridView1.Rows[1].Cells[0].Value = "СГБ - 1,8";
            dataGridView1.Rows[2].Cells[0].Value = "СГБ - 3,2";
            dataGridView1.Rows[3].Cells[0].Value = "СГБ - 4,0";

            dataGridView1.Rows[0].Cells[1].Value = getLastSN_SVD15();
            dataGridView1.Rows[1].Cells[1].Value = getLastSN_SGB18();
            dataGridView1.Rows[2].Cells[1].Value = getLastSN_SGB32();
            dataGridView1.Rows[3].Cells[1].Value = getLastSN_SGB40();

            dataGridView1.Rows[0].Cells[2].Value = getCount_SVD15();
            dataGridView1.Rows[1].Cells[2].Value = getCount_SGB18();
            dataGridView1.Rows[2].Cells[2].Value = getCount_SGB32();
            dataGridView1.Rows[3].Cells[2].Value = getCount_SGB40();

            dataGridView1.Rows[0].Cells[3].Value = getMissed_SVD15();
            dataGridView1.Rows[1].Cells[3].Value = getMissed_SGB18();
            dataGridView1.Rows[2].Cells[3].Value = getMissed_SGB32();
            dataGridView1.Rows[3].Cells[3].Value = getMissed_SGB40();
        }

        //Ищем последний номер счетчика
        string getLastSN_SVD15()
        {

            //Вызываем метод парсинга Txt файла
            convertTxtFile();

            filtredList();

            //Сортируем список по серийному номеру
            itemsFiltred.Sort((a, b) => Int32.Parse(b["SN"]) - Int32.Parse(a["SN"]));

            if (itemsFiltred.Count > 0)
            {
                return itemsFiltred[0]["SN"];
            }
            else
            {
                return "Не поверялись";
            }
        }
        string getLastSN_SGB18()
        {
            return getLastSN_SGB("\\1.8");
        }
        string getLastSN_SGB32()
        {
            return getLastSN_SGB("\\3.2");
        }
        string getLastSN_SGB40()
        {
            return getLastSN_SGB("\\4.0");
        }
        string getLastSN_SGB(string pathType)
        {
            string lastSN = "0";

            string path = "C:\\поверка\\Установка\\" + dataPicker.Value.ToString("yyyy") + "\\" + dataPicker.Value.ToString("MM") + "\\" + dataPicker.Value.ToString("dd") + pathType;
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                char[] splitSeparators1 = { '\\' };
                char[] splitSeparators2 = { '.' };


                for (int i = 0; i < files.Count(); i++)
                {
                    string[] spliFiles = files[i].Split(splitSeparators1, StringSplitOptions.RemoveEmptyEntries);
                    string[] getSN = spliFiles[spliFiles.Length - 1].Split(splitSeparators2, StringSplitOptions.RemoveEmptyEntries);

                    if (int.Parse(lastSN) < int.Parse(getSN[0]))
                    {
                        lastSN = getSN[0];
                    }
                }

                return lastSN;
            }
            else
            {
                return "не поверялись";
            }
        }

        //Ищем количество счетчиков
        string getCount_SVD15()
        {
            int spaceCount = 0;

            filtredList();

            //Сортируем список по серийному номеру
            itemsFiltred.Sort((a, b) => Int32.Parse(a["SN"]) - Int32.Parse(b["SN"]));

            for (int i = 0; i < itemsFiltred.Count; i++)
            {
                if (i != 0 && (itemsFiltred[i - 1]["SN"] == itemsFiltred[i]["SN"]))
                {
                    spaceCount++;
                }
            }

            items.Clear();

            return (itemsFiltred.Count() - spaceCount).ToString();
        }
        string getCount_SGB18()
        {
            return getCount_SGB("\\1.8");
        }
        string getCount_SGB32()
        {
            return getCount_SGB("\\3.2");
        }
        string getCount_SGB40()
        {
            return getCount_SGB("\\4.0");
        }
        string getCount_SGB(string pathType)
        {
            string path = "C:\\поверка\\Установка\\" + dataPicker.Value.ToString("yyyy") + "\\" + dataPicker.Value.ToString("MM") + "\\" + dataPicker.Value.ToString("dd") + pathType;
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                return (files.Length).ToString();
            }
            else
            {
                return "0";
            }
        }

        //Ищем пропущенные номера
        string getMissed_SVD15()
        {
            string missedSN = "нет пропусков";
            return missedSN;
        }
        string getMissed_SGB18()
        {
            return getMissed_SGB("\\1.8");
        }
        string getMissed_SGB32()
        {
            return getMissed_SGB("\\3.2");
        }
        string getMissed_SGB40()
        {
            return getMissed_SGB("\\4.0");
        }
        string getMissed_SGB(string pathType)
        {
            string path = "C:\\поверка\\Установка\\" + dataPicker.Value.ToString("yyyy") + "\\" + dataPicker.Value.ToString("MM") + "\\" + dataPicker.Value.ToString("dd") + pathType;
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                char[] splitSeparators1 = { '\\' };
                char[] splitSeparators2 = { '.' };

                int[] listSN = new int[files.Length];

                for (int i = 0; i < files.Count(); i++)
                {
                    string[] spliFiles = files[i].Split(splitSeparators1, StringSplitOptions.RemoveEmptyEntries);
                    string[] getSN = spliFiles[spliFiles.Length - 1].Split(splitSeparators2, StringSplitOptions.RemoveEmptyEntries);

                    listSN[i] = int.Parse(getSN[0]);
                }

                int[] missingNumbers = Enumerable.Range(listSN[0], listSN.Length - 1).Except(listSN).ToArray();
                return string.Join(",", missingNumbers);
            }
            else
            {
                return "не поверялись";
            }
        }

        // заполняем и сохраняем файл в Excel
        void DisplayInExcel()
        {
            var excelApp = new Excel.Application();

            //очищаем список
            items.Clear();

            //Вызываем метод парсинга Txt файла
            convertTxtFile();

            //Получаем набор ссылок на объекты Workbook
            var excelAppWorkBooks = excelApp.Workbooks;

            //открываем файл Excel
            excelAppWorkBooks.Open(@"C:\report\reportFromWaterApp.xlsx");
            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;

            // Заполняем таблицу
            int numberStartCell = 22; //строчка с которой начинается талица
            int spaceCount = 0;

            // Записываем дату       
            string checkDate = dataPicker.Value.ToString("dd.MM.yyyy");

            // Фильтрум спсисок
            filtredList();

            //Сортируем список по серийному номеру
            itemsFiltred.Sort((a, b) => Int32.Parse(a["SN"]) - Int32.Parse(b["SN"]));

            //Инициализируем Прогрессбар
            progressBarReport.Maximum = itemsFiltred.Count();

            // Записываем данные в ячейки Excel
            workSheet.Cells[19, "D"] = dataPicker.Value.ToString("dd.MM.yyyy") + "-" + personalNumber.Text + "-" + protocolWaterNumber.Text;
            for (int i = 0; i < itemsFiltred.Count; i++)
            {
                progressBarReport.Value = i + 1;

                if (i != 0 && (itemsFiltred[i - 1]["SN"] == itemsFiltred[i]["SN"]))
                {
                    numberStartCell--;
                    spaceCount++;
                    continue;
                }

                workSheet.Cells[i + numberStartCell, "A"] = itemsFiltred[i]["SN"];
                workSheet.Cells[i + numberStartCell, "B"] = "СВД-15";
                workSheet.Cells[i + numberStartCell, "C"] = "15";

                workSheet.Cells[i + numberStartCell, "D"] = itemsFiltred[i].ContainsKey("Qmin") == true ? (float.Parse(itemsFiltred[i]["Qmin"]) / 1000).ToString() : "NoVal";
                workSheet.Cells[i + numberStartCell, "E"].Formula = "=(D" + (i + numberStartCell).ToString() + "-(D" + (i + numberStartCell).ToString() + "*G" + (i + numberStartCell).ToString() + "*0.01))*360/3600";
                workSheet.Cells[i + numberStartCell, "F"].Formula = "=(D" + (i + numberStartCell).ToString() + " )*360/3600";
                workSheet.Cells[i + numberStartCell, "G"] = itemsFiltred[i].ContainsKey("dVmin") == true ? itemsFiltred[i]["dVmin"] : "NoVal";

                workSheet.Cells[i + numberStartCell, "H"] = itemsFiltred[i].ContainsKey("Qmid") == true ? (float.Parse(itemsFiltred[i]["Qmid"]) / 1000).ToString() : "NoVal";
                workSheet.Cells[i + numberStartCell, "I"].Formula = "=(H" + (i + numberStartCell).ToString() + "-(H" + (i + numberStartCell).ToString() + "*K" + (i + numberStartCell).ToString() + "*0.01))*160/3600";
                workSheet.Cells[i + numberStartCell, "J"].Formula = "=(H" + (i + numberStartCell).ToString() + " )*160/3600";
                workSheet.Cells[i + numberStartCell, "K"] = itemsFiltred[i].ContainsKey("dVmid") == true ? itemsFiltred[i]["dVmid"] : "NoVal";

                workSheet.Cells[i + numberStartCell, "L"] = itemsFiltred[i].ContainsKey("Qmax") == true ? (float.Parse(itemsFiltred[i]["Qmax"]) / 1000).ToString() : "NoVal";
                workSheet.Cells[i + numberStartCell, "M"].Formula = "=(L" + (i + numberStartCell).ToString() + "-(L" + (i + numberStartCell).ToString() + "*O" + (i + numberStartCell).ToString() + "*0.01))*60/3600";
                workSheet.Cells[i + numberStartCell, "N"].Formula = "=(L" + (i + numberStartCell).ToString() + " )*60/3600";
                workSheet.Cells[i + numberStartCell, "O"] = itemsFiltred[i].ContainsKey("dVmax") == true ? itemsFiltred[i]["dVmax"] : "NoVal";

            }

            workSheet.Cells[numberStartCell + 2 + itemsFiltred.Count, "A"] = "Всего счетчиков";
            workSheet.Cells[numberStartCell + 2 + itemsFiltred.Count, "C"] = itemsFiltred.Count() - spaceCount;

            workSheet.Cells[numberStartCell + 4 + itemsFiltred.Count, "A"] = "Исполнитель";

            if (personalNumber.Text == "162")
            {
                workSheet.Cells[numberStartCell + 4 + itemsFiltred.Count, "C"] = "Нуждин Д.С";
            }
            if (personalNumber.Text == "145")
            {
                workSheet.Cells[numberStartCell + 4 + itemsFiltred.Count, "C"] = "Ильченко А.Ю";
            }
            if (personalNumber.Text == "196")
            {
                workSheet.Cells[numberStartCell + 4 + itemsFiltred.Count, "C"] = "Воробьев Д.А.";
            }

            // Сохраняем полученный документ
            string date = DateTime.Now.ToString("yy-MM-dd");

            if (!Directory.Exists(@"C:\report\reportFromWaterApp"))
            {
                Directory.CreateDirectory(@"C:\report\reportFromWaterApp");
            }

            workSheet.SaveAs(@"C:\report\reportFromWaterApp\" + dataPicker.Value.ToString("dd.MM.yyyy") + "-" + personalNumber.Text + "-" + protocolWaterNumber.Text + ".xlsx");

            // Отображаем файл Excel
            excelApp.Visible = true;

            //Закрываем процессы
            excelAppWorkBooks.Close();
            excelApp.Quit();
            GC.Collect();
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);

            // Отправляем отчет по почте
            try
            {
                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress("tzaynashev@yandex.ru", "Elehant");
                // кому отправляем
                MailAddress to = new MailAddress("csm55@mail.ru");
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = "Отчет по счетчикам воды за " + date + " число";
                // текст письма
                m.Body = "<h2> Отчет в приложенном файле </h2>";
                // письмо представляет код html
                m.IsBodyHtml = true;
                // добавление файла отчета
                m.Attachments.Add(new Attachment(@"C:\report\reportFromWaterApp\" + dataPicker.Value.ToString("dd.MM.yyyy") + "-" + personalNumber.Text + "-" + protocolWaterNumber.Text + ".xlsx"));
                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                SmtpClient smtp = new SmtpClient("smtp.yandex.ru");
                // логин и пароль
                smtp.Credentials = new NetworkCredential("tzaynashev@yandex.ru", "sinkorswim1");
                smtp.EnableSsl = true;
                smtp.Send(m);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //Сообщение о создании отчета
            MessageBox.Show("Отчет по счетчикам воды готов");

        }

        // заполняем и сохраняем файл в Excel
        void DisplayInExcel2()
        {
            var excelApp = new Excel.Application();

            // Проверяем наличие каталога с данной датой
            if (!Directory.Exists("C:\\поверка\\Установка\\" + dataPicker.Value.ToString("yyyy") + "\\" + dataPicker.Value.ToString("MM") + "\\" + dataPicker.Value.ToString("dd")))
            {
                MessageBox.Show("Каталог с этой датой  не существует");
                return;
            }

            //Очищаем список
            items2.Clear();

            //Вызываем метод парсинга Csv файла
            convertCsvFile();

            //Получаем набор ссылок на объекты Workbook
            var excelAppWorkBooks = excelApp.Workbooks;

            //открываем файл Excel
            excelAppWorkBooks.Open(@"C:\report\reportFromGasApp.xlsx");
            Excel._Worksheet workSheet = (Excel.Worksheet)excelApp.ActiveSheet;

            //Const для заполнения
            const int numberStartCell = 22; //строчка с которой начинается таблица          
            const string relativeHumidity = "38";
            const string airPressure = "99.8";

            //Инициализируем Прогрессбар
            progressBarReport.Maximum = items2.Count();

            // Записываем данные в ячейки Excel
            workSheet.Cells[19, "D"] = dataPicker.Value.ToString("dd.MM.yyyy") + "-" + personalNumber.Text + "-" + protocolGasNumber.Text;
            for (int i = 0; i < items2.Count; i++)
            {
                progressBarReport.Value = i + 1;

                workSheet.Cells[i + numberStartCell, "A"] = items2[i][0].Substring(19).TrimEnd('"');
                workSheet.Cells[i + numberStartCell, "B"] = items2[i][38];
                workSheet.Cells[i + numberStartCell, "C"] = items2[i][28].Trim('"');
                workSheet.Cells[i + numberStartCell, "D"] = relativeHumidity;
                workSheet.Cells[i + numberStartCell, "E"] = airPressure;
                workSheet.Cells[i + numberStartCell, "F"] = float.Parse(items2[i][33].Trim('"')) < 1.9 ? float.Parse(items2[i][33].Trim('"')) : 1.9;
                workSheet.Cells[i + numberStartCell, "G"] = items2[i][35].Trim('"');
                workSheet.Cells[i + numberStartCell, "H"] = items2[i][36].Trim('"');
                workSheet.Cells[i + numberStartCell, "I"] = items2[i][37].Trim('"');
            }

            workSheet.Cells[numberStartCell + 2 + items2.Count, "A"] = "Всего счетчиков 1.8";
            workSheet.Cells[numberStartCell + 2 + items2.Count, "D"] = gasCountType_1_8;

            workSheet.Cells[numberStartCell + 3 + items2.Count, "A"] = "Всего счетчиков 3.2";
            workSheet.Cells[numberStartCell + 3 + items2.Count, "D"] = gasCountType_3_2;

            workSheet.Cells[numberStartCell + 4 + items2.Count, "A"] = "Всего счетчиков 4.0";
            workSheet.Cells[numberStartCell + 4 + items2.Count, "D"] = gasCountType_4_0;

            workSheet.Cells[numberStartCell + 6 + items2.Count, "A"] = "Исполнитель";

            if (personalNumber.Text == "162")
            {
                workSheet.Cells[numberStartCell + 6 + items2.Count, "C"] = "Нуждин Д.С";
            }
            if (personalNumber.Text == "145")
            {
                workSheet.Cells[numberStartCell + 6 + items2.Count, "C"] = "Ильченко А.Ю";
            }
            if (personalNumber.Text == "196")
            {
                workSheet.Cells[numberStartCell + 6 + items2.Count, "C"] = "Воробьев Д.А.";
            }

            // Сохраняем полученный документ
            string date = DateTime.Now.ToString("yy-MM-dd");

            if (!Directory.Exists(@"C:\report\reportFromGasApp"))
            {
                Directory.CreateDirectory(@"C:\report\reportFromGasApp");
            }

            workSheet.SaveAs(@"C:\report\reportFromGasApp\" + dataPicker.Value.ToString("dd.MM.yyyy") + "-" + personalNumber.Text + "-" + protocolGasNumber.Text + ".xlsx");

            // Отображаем файл Excel
            excelApp.Visible = true;

            //Закрываем процессы
            excelAppWorkBooks.Close();
            excelApp.Quit();
            GC.Collect();
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelApp);

            // Отправляем отчет по почте
            try
            {
                // отправитель - устанавливаем адрес и отображаемое в письме имя
                MailAddress from = new MailAddress("tzaynashev@yandex.ru", "Elehant");
                // кому отправляем
                MailAddress to = new MailAddress("csm55@mail.ru");
                // создаем объект сообщения
                MailMessage m = new MailMessage(from, to);
                // тема письма
                m.Subject = "Отчет по газовым счетчикам за " + date + " число";
                // текст письма
                m.Body = "<h2> Отчет в приложенном файле </h2>";
                // письмо представляет код html
                m.IsBodyHtml = true;
                // добавление файла отчета
                m.Attachments.Add(new Attachment(@"C:\report\reportFromGasApp\" + dataPicker.Value.ToString("dd.MM.yyyy") + "-" + personalNumber.Text + "-" + protocolGasNumber.Text + ".xlsx"));
                // адрес smtp-сервера и порт, с которого будем отправлять письмо
                SmtpClient smtp = new SmtpClient("smtp.yandex.ru");
                // логин и пароль
                smtp.Credentials = new NetworkCredential("tzaynashev@yandex.ru", "sinkorswim1");
                smtp.EnableSsl = true;
                smtp.Send(m);
            }

            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            //Сообщение о создании отчета
            MessageBox.Show("Отчет по газовым счетчикам готов");

        }

        //Парсим текстовый файл
        List<Dictionary<string, string>> items = new List<Dictionary<string, string>>();
        void convertTxtFile()
        {
            char[] splitSeparators1 = { ';' };
            char[] splitSeparators2 = { '=' };

            string path = @"\\Kb\db\WaterMeter.Meters.txt";
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        Dictionary<string, string> itemsDictionary = new Dictionary<string, string>();
                        string[] strVals = line.Split(splitSeparators1, StringSplitOptions.RemoveEmptyEntries);

                        try
                        {
                            for (int i = 0; i < strVals.Length; i++)
                            {
                                string[] strKeyAndVal = strVals[i].Split(splitSeparators2, StringSplitOptions.RemoveEmptyEntries);
                                itemsDictionary.Add(strKeyAndVal[0], strKeyAndVal[1]);
                            }
                        }

                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message + "from parcer txt file");
                        }

                        items.Add(itemsDictionary);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "нет доступа в kb");
            }
        }

        // Вызываем по разу метод конвертирования файла для разных счетчиков
        void convertCsvFile()
        {
            ConvertCsvFileThisType("\\1.8");
            ConvertCsvFileThisType("\\3.2");
            ConvertCsvFileThisType("\\4.0");
        }
        //Парсим csv файл и заносим информацию в список
        List<List<string>> items2 = new List<List<string>>();
        int gasCountType_1_8 = 0;
        int gasCountType_3_2 = 0;
        int gasCountType_4_0 = 0;
        void ConvertCsvFileThisType(string pathType)
        {
            string path = "C:\\поверка\\Установка\\" + dataPicker.Value.ToString("yyyy") + "\\" + dataPicker.Value.ToString("MM") + "\\" + dataPicker.Value.ToString("dd");

            char[] splitSeparators = { '=' };

            if (Directory.Exists(path + pathType))
            {
                string[] files = Directory.GetFiles(path + pathType);
                for (int i = 0; i < files.Count(); i++)
                {
                    List<string> itemsList = new List<string>();
                    try
                    {
                        using (StreamReader sr = new StreamReader(files[i], System.Text.Encoding.Default))
                        {
                            string line;
                            while ((line = sr.ReadLine()) != null)
                            {
                                var parmChars = line.ToCharArray();
                                bool isDoubleQuote = false;

                                for (int index = 0; index < parmChars.Length; index++)
                                {
                                    if (parmChars[index] == '"')
                                    {
                                        isDoubleQuote = !isDoubleQuote;
                                    }

                                    if (parmChars[index] == ',' && !isDoubleQuote)
                                    {
                                        parmChars[index] = '=';
                                    }
                                }

                                string strCsv = new string(parmChars);

                                string[] strVals = strCsv.Split(splitSeparators, StringSplitOptions.RemoveEmptyEntries);

                                itemsList.AddRange(strVals);
                            }

                            itemsList.Add(pathType.Trim('\\'));
                            items2.Add(itemsList);
                        }
                        if (pathType == "\\1.8") gasCountType_1_8++;
                        if (pathType == "\\3.2") gasCountType_3_2++;
                        if (pathType == "\\4.0") gasCountType_4_0++;
                    }

                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }
        }

        //Отфильтровываем массив по наличию серийного номера
        List<Dictionary<string, string>> itemsFiltred = new List<Dictionary<string, string>>();
        void filtredList()
        {
            itemsFiltred.Clear();
            for (int j = 0; j < (items.Count); j++)
            {
                if (items[j].ContainsKey("SN") && items[j].ContainsKey("CheckDate") && items[j]["CheckDate"] == dataPicker.Value.ToString("dd.MM.yyyy") && (int.Parse(items[j]["SN"]) > 100))
                {
                    itemsFiltred.Add(items[j]);
                }
            }
        }

        //создание отчета по счетчикам воды
        private void button1_Click(object sender, EventArgs e)
        {
            DisplayInExcel();
        }

        //создание отчета по счетчикам газа
        private void button2_Click(object sender, EventArgs e)
        {
            DisplayInExcel2();
        }

        //событие по смене даты
        private void dataPicker_ValueChanged(object sender, EventArgs e)
        {
            dataGridViewContainer();
        }

        //событие загрузки формы
        private void Form1_Load(object sender, EventArgs e)
        {
            dataPicker.Format = DateTimePickerFormat.Custom;

            dataGridViewContainer();

            // добавляем набор элементов в окнах выбора
            personalNumber.Items.AddRange(new string[] { "162", "145", "196" });
            protocolGasNumber.Items.AddRange(new string[] { "1", "2", "3", "4", "5", "6" });
            protocolWaterNumber.Items.AddRange(new string[] { "1", "2", "3", "4", "5", "6" });

            //по умолчанию
            personalNumber.Text = "162";
            protocolGasNumber.Text = "1";
            protocolWaterNumber.Text = "1";
        }

        //автоматическое обновление 
        private void timer1_Tick(object sender, EventArgs e)
        {
            //если датагридвью == текущей дате то:
            if (dataPicker.Value.Date == DateTime.Now.Date)
            {
                dataGridViewContainer();
            }
        }
    }
}
