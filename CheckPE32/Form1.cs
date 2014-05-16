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

        public bool CheckFile(string fileName)
        {
            byte[] data = System.IO.File.ReadAllBytes(textBox1.Text);
            //необходимо найти сигнатуру 50450000h
            string sData = Encoding.Unicode.GetString(data, 0, data.Length);
            int offset = sData.IndexOf("PE  ");
           
            //offset += 88; //смещение 58h - значение по этому смещению хранит контрольную сумму файла
            offset += 0x98; //98h - первое из значений, определяющих наличие подписи (4 значения наоборот)
                            //Это 4-байтное значение означает смещение, по которому находится начало цифровой подписи
                            //offset += 0x9C; //9Ch - второе из значений, определяющих наличие подписи (4 значения наоборот)
                            //Оно означает размер цифровой подписи
            int i = 0;
            bool isNotSigned = true;
            //далее проверяется последовательность из 8 байтов, если все они равны нулю - файл не подписан
            for (i = 0; i < 7; i++)
            {
                if (data[offset + i] != 0) isNotSigned = false;
            };
            if (isNotSigned)
            {
                return false; //если файл не полписан
            }
            else
            {
                return true; //если файл имеет подпись
            }      
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength < 1)
            {
                MessageBox.Show("Путь к файлу не указан", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
