using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ImageRGB
{
    public partial class Form1 : Form
    {
        Bitmap inputImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG|*.jpg|PNG|*.png|GIF |*.gif|BMP|*.bmp|所有文件|*.*";
            openFileDialog.ShowDialog();
            string fileName = openFileDialog.FileName;
            inputImage = (Bitmap)Image.FromFile(fileName);
            pictureBox1.Image = inputImage;

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (inputImage != null)
            {

                Color color = inputImage.GetPixel(e.X, e.Y);
                String reb = color.R.ToString() + "," + color.G.ToString() + "," + color.B.ToString();
                textBox1.Text = "("+reb+")";
               
            }

        }


     
    }
}

