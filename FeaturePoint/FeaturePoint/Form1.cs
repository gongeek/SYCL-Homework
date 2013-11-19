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

namespace FeaturePoint
{
    public partial class Form1 : Form
    {
        Bitmap inputImage;
        int w, h;
        int[,] g;
        StreamWriter sw1,sw2;

        public Form1()
        {
            InitializeComponent();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG|*.jpg|PNG|*.png|GIF |*.gif|BMP|*.bmp|所有文件|*.*";
            openFileDialog.ShowDialog();
            string fileName = openFileDialog.FileName;
            inputImage = (Bitmap)Image.FromFile(fileName);
            w=inputImage.Width;
            h=inputImage.Height;
            pictureBox1.Width = w;
            pictureBox1.Height = h;
            pictureBox1.Image = inputImage;
            g = grayValue(inputImage);
            
        }

        public int[,] grayValue(Bitmap bitmap) {
            int[,] a=new int[w,h];
            for (int i = 0; i < w; i++) {
                for (int j = 0; j < h; j++) {
                    Color color = inputImage.GetPixel(i, j);
                    a[i, j] = (color.R + color.G + color.B) / 3;
                }
            }
            return a;
        }


        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            
            int count=0;
            Graphics g = Graphics.FromImage(inputImage);
            Font font = new Font("宋体",24);
            SolidBrush brush = new SolidBrush(Color.Red);
            int[,] v = new int[w, h];
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    v[i, j] = 0;
                }
            }
               int[] sv=new int[w*h];
               for (int i = 0; i < w * h; i++) {
                   sv[i] = 0;
               }
               int k = 0;
            for (int i = 2; i <= w - 3; i++)
            {
                for (int j = 2; j <= h - 3; j++)
                {
                    v[i, j] = interestValueM(i,j);
                    sv[k] = v[i, j];
                    k++;
                }
            }
            sw1 = new StreamWriter("特征点坐标数据(Moravec算子).txt");
            Array.Sort(sv);
            int yu = sv[w * h - 180];
            for (int i = 2; i <= w - 3; i++)
            {
                for (int j = 2; j <= h - 3; j++)
                {
                    if (v[i, j] >yu) {
                        g.DrawString("+",font,brush,new Point(i,j));
                        count++;
                        sw1.WriteLine("("+i+","+j+")");
                    }
                }
            }
            Cursor.Current = Cursors.Default;
            sw1.Close();

        }

        public int interestValueM(int c, int r)
        {
            
            int v1=0, v2=0, v3=0, v4=0,v=0;
            for (int i = -2; i <= 1; i++) {
                v1 += (int)Math.Pow(g[c + i, r] - g[c + i + 1, r],2);
                v2 += (int)Math.Pow(g[c + i, r+i] - g[c + i + 1, r+i+1],2);
                v3 += (int)Math.Pow(g[c, r + i] - g[c, r + i + 1],2);
                v4 += (int)Math.Pow(g[c + i, r-i] - g[c + i + 1, r-i-1],2);
            }
           v= Math.Min(Math.Min(v1, v2),Math.Min(v3,v4));
        return v;
        }

     

        public float interestValueQF(int c, int r) {
            float q=0,gu=0,gv=0,guv=0;

            for (int i = c - 2; i <= c + 1; i++) {
                for (int j = r - 2; j <= r + 1; j++) {
                    gu += (float)Math.Pow(g[i + 1, j + 1] - g[i, j],2);
                    gv += (float)Math.Pow(g[i, j + 1] - g[i+1, j],2);
                    guv += (g[i+1,j+1]-g[i,j])*(g[i,j+1]-g[i+1,j]);
                }
                q = 4 * (gu * gv - guv * guv) / ((gu + gv) * (gu + gv));
            }
                return q;
            
        
        }
        public float interestValueWF(int c, int r)
        {
            float gu=0, gv=0, guv=0, m=0;

            for (int i = c - 2; i <= c + 1; i++)
            {
                for (int j = r - 2; j <= r + 1; j++)
                {
                    gu += (float)Math.Pow(g[i + 1, j + 1] - g[i, j],2 );
                    gv += (float)Math.Pow(g[i, j + 1] - g[i + 1, j],2);
                    guv += (g[i + 1, j + 1] - g[i, j]) * (g[i, j + 1] - g[i + 1, j]);
                }
                m = (gu * gv - guv * guv) / (gu + gv);
            }
            return m;


        } 

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Graphics g = Graphics.FromImage(inputImage);
            Font font = new Font("宋体", 24);
            SolidBrush brush = new SolidBrush(Color.Red);
            int count = 0,k=0;
            sw2 = new StreamWriter("特征点坐标数据(Forstner算子).txt");
             double sum = 0;
            float[,] QArray=new float[w,h], WArray=new float[w,h];

            for (int i=0; i < w; i++)
            {
                for (int j = 0; j < h; j++) {
                    QArray[i, j] = 0;
                    WArray[i, j] = 0;
                }
            }

            for (int i = 3; i <= w - 4; i++)
            {
                for (int j = 3; j <= h - 4; j++)
                {
                    QArray[i, j] = interestValueQF(i, j);
                    WArray[i, j] = interestValueWF(i, j);
                    if (!float.IsNaN(WArray[i,j]))
                    {
                        sum += WArray[i, j];
                        k++;
                    }
                    
                }
            }
            double TW=0, TQ = 0;
            TW = 1.5 * sum/k;
            TQ = (float)0.75;
            for (int i = 3; i <= w - 4; i++)
            {
                for (int j = 3; j <= h - 4; j++)
                {
                    if (QArray[i, j] >TQ&&WArray[i,j]>TW)
                    {
                        g.DrawString("+", font, brush, new Point(i, j));
                        sw2.WriteLine("(" + i + "," + j + ")");
                        count++;
                    }
                }
            }
            Cursor.Current = Cursors.Default;
            sw2.Close();
        }

        private void moravecToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("notepad", "特征点坐标数据(Moravec算子).txt");
        }

        private void forsterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("notepad", "特征点坐标数据(Forstner算子).txt");

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            StreamWriter ss = new StreamWriter("说明.txt");
            ss.WriteLine("1,图像每个像素的灰度值由R,G,B的灰度值除3得到");
            ss.WriteLine("2.Moravaec算子和Forster算子特征点提取，均采用5*5的窗口。其中Moravec算子通过对兴趣值排序取最大的180个点，Forster阈值由经验阈值给出，无法控制提取点数。");
            ss.WriteLine("20113210龚巍");
            ss.Close();
            Process.Start("notepad", "说明.txt");
        }


    }
}
