using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace CheckPE32
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        /// <summary>
        /// функция возвращает индекс массива arraySource, с которого начинается массив arrayPattern
        /// </summary>
        /// <param name="arraySource">исходный массив</param>
        /// <param name="arrayPattern">искомый массив</param>
        /// <param name="startInd">индекс в исходном массиве, с которого надо начать поиск</param>
        /// <param name="count">кол-во элементов в исходном массиве, которое надо проверить</param>
        /// <returns>индекс элемента в исходном массиве, с которого начинается искомый массив</returns>
        public static int FindArray(byte[] arraySource, byte[] arrayPattern, int startInd, int count)
        {
            if (count >= arrayPattern.Length && (startInd + count) <= arraySource.Length)
            {
                int maxIndNext = count + startInd;
                int delta = maxIndNext - arrayPattern.Length;
                for (int i = startInd; i < maxIndNext; ++i)
                {
                    if (arrayPattern[0] == arraySource[i] && i <= delta)
                    {
                        int k = 1;
                        for (int j = 1; j < arrayPattern.Length; ++j)
                        {
                            if (arrayPattern[j] == arraySource[i + k])
                                k++;
                            else
                                break;
                            if (k == arrayPattern.Length)
                                return i;
                        }
                    }
                }
            }
            return -1;
        }

        public bool CheckFile(string fileName)
        {
            byte[] data = System.IO.File.ReadAllBytes(textBox1.Text);
            //необходимо найти сигнатуру 50450000h
            //string sData = Encoding.Unicode.GetString(data, 0, data.Length);
            byte[] signature = {0x50, 0x45, 0x00, 0x00};
            int offset = FindArray(data, signature, 0, data.Length);
                    
            //offset += 88; //смещение 58h - значение по этому смещению хранит контрольную сумму файла
            offset += 0x98; //98h - первое из значений, определяющих наличие подписи (4 значения наоборот)
                            //Это 4-байтное значение означает смещение, по которому находится начало цифровой подписи
                            //offset += 0x9C; //9Ch - второе из значений, определяющих наличие подписи (4 значения наоборот)
                            //Оно означает размер цифровой подписи
            bool isNotSigned = true;
            //далее проверяется последовательность из 8 байтов, если все они равны нулю - файл не подписан
            for (int i = 0; i < 7; i++)
            {
                if (data[offset + i] != 0) isNotSigned = false;
            };
            if (isNotSigned)
            {
                return false; //если файл не подписан
            }
            else
            {
                return true; //если файл имеет подпись
            }      
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string fileName = textBox1.Text;
            string fileExt = fileName.Substring(fileName.LastIndexOf(".") + 1);
            if (textBox1.TextLength < 1)
            {
                MessageBox.Show("Укажите путь к файлу", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else {
                if (CheckFile(textBox1.Text))
                {
                    MessageBox.Show("Модуль PE32 имеет подпись", "CheckPE32", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                else
                {
                    MessageBox.Show("Модуль PE32 не подписан!", "CheckPE32", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void файлToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                textBox1.Text = fileName;
            }
        }

        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void оПрограммеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Проверка подписи модуля PE32 \n ВятГУ 2014","О программе");
        }

        private void заданиеToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Написать программу, определяющую, пописан ли PE32+ модуль.","Задание");
        }
    }
}
