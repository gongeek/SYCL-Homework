using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace NeiDingXiang
{
    
    public partial class Form1 : Form
    {
        
        public Form1()
        {
            InitializeComponent();
        }

        public double[,] data=new double[8,4];

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "TXT|*.txt|BAT|*.bat|所有文件|*.*";
            openFileDialog.ShowDialog();
            string fileName = openFileDialog.FileName;
            FileStream fs = new FileStream(fileName,FileMode.Open);
            StreamReader br = new StreamReader(fs);
            int i = 0;
            while (br.EndOfStream != true) {
                string s = br.ReadLine();
                string[] sarray=s.Replace("         	", "#").Split('#');
                for (int j = 0; j < sarray.Length; j++) {
                    data[i,j]= Double.Parse(sarray[j]);
                }
                i++;
            }
            fs.Close();
            br.Close();
            /*String textBoxString = "";
            for (int m= 0; m < 8; m++) {
                for (int n = 0; n < 4; n++) {
                    textBoxString += data[m, n] + "     ";
                }
                this.textBox1.Text += (textBoxString + "\n");
            }*/
            textBox1.Text = Encoding.Default.GetString(File.ReadAllBytes(fileName));

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Matrix X = new Matrix(3,1);
            Matrix Y = new Matrix(3,1);
            Matrix Ly = new Matrix(8,1);
            Matrix Lx= new Matrix(8, 1);
            Matrix B = new Matrix(8,3);
            int i = 0;
           for(i=0;i<8;i++){
               Lx.SetNum(i, 0, data[i, 2]);
               Ly.SetNum(i, 0, data[i, 3]);
               B.SetNum(i, 0, 1);
               B.SetNum(i, 1, data[i, 0]);
               B.SetNum(i, 2, data[i, 1]);
            }
           Matrix BT = B.Transpose();
           Matrix BTB = Matrix.Mutiply(BT,B);
           Matrix BTB1 = Matrix.Inverse(BTB);
           Matrix BTB1BT = Matrix.Mutiply(BTB1,BT);
          X = Matrix.Mutiply(BTB1BT, Lx);
          Y = Matrix.Mutiply(BTB1BT, Ly);

          double rms;
          double sumx=0,sumy=0;
          for (int p = 0; p < 8; p++) {
             sumx+=Math.Pow(((X.getNum(0, 0) + X.getNum(1, 0) * data[p, 0] + X.getNum(2, 0) * data[p, 1]) - data[p, 2]),2);
             sumy += Math.Pow(((Y.getNum(0, 0) + Y.getNum(1, 0) * data[p, 0] + Y.getNum(2, 0) * data[p, 1]) - data[p, 3]), 2);
          }
          rms = Math.Sqrt(sumx + sumy);
          this.textBox4.Text = rms.ToString();
          this.textBox2.Text = X.getNum(0, 0) + "    " + X.getNum(1, 0) + "   " + X.getNum(2, 0);
          this.textBox3.Text = Y.getNum(0, 0) +"    "+ Y.getNum(1, 0) +"   "+ Y.getNum(2, 0) ;
        }

    }
    //矩阵类
    public class Matrix
    {
        double[,] matrix;
        public int row = 0, col = 0;
        //定义构造函数
        public Matrix()
        {
        }
        public Matrix(int row)
        {
            matrix = new double[row, row];
        }
        public Matrix(int row, int col)
        {
            this.row = row;
            this.col = col;
            matrix = new double[row, col];
        }
        //复制构造函数
        public Matrix(Matrix m)
        {
            int row = m.row;
            int col = m.col;
            matrix = new double[row, col];
            for (int i = 0; i < row; i++)
                for (int j = 0; j < col; j++)
                    matrix[i, j] = m.getNum(i, j);
        }
        //输入相应的值，对矩阵进行设置
        public void SetNum(int i, int j, double num)
        {
            this.matrix[i, j] = num;
        }
        //得到相应的矩阵某个数
        public double getNum(int i, int j)
        {
            return matrix[i, j];
        }
        //输出矩阵
        public void OutputM()
        {
            Console.WriteLine("矩阵为：");
            for (int p = 0; p < row; p++)
            {
                for (int q = 0; q < col; q++)
                {
                    Console.Write("\t" + matrix[p, q]);
                }
                Console.Write("\n");
            }
        }
        //输入矩阵具体数字实现
        public void InputM(int Row, int Col)
        {
            for (int a = 0; a < Col; a++)
            {
                for (int b = 0; b < Col; b++)
                {
                    Console.WriteLine("第{0}行，第{1}列", a + 1, b + 1);
                    double value = Convert.ToDouble(Console.ReadLine());
                    this.SetNum(a, b, value);
                }
            }
        }

        //得到matrix
        public double[,] Detail
        {
            get { return matrix; }
            set { matrix = value; }
        }
        //矩阵转置实现
        public Matrix Transpose()
        {
            Matrix another = new Matrix(col,row);
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    another.SetNum(j, i, matrix[i, j]);
                }
            }
            return another;
        }
        //矩阵相加实现
        public static Matrix Add(Matrix lm, Matrix rm)
        {
            //行出错
            if (lm.row != rm.row)
            {
                System.Exception e = new Exception("相加的两个矩阵的行数不等");
                throw e;
            }
            //列出错
            if (lm.col != rm.col)
            {
                System.Exception e = new Exception("相加的两个矩阵的列数不等");
                throw e;
            }
            Matrix another = new Matrix(lm.row, lm.col);
            for (int i = 0; i < lm.row; i++)
            {
                for (int j = 0; j < lm.col; j++)
                {
                    double temp = lm.getNum(i, j) + rm.getNum(i, j);
                    another.SetNum(i, j, temp);
                }
            }
            return another;
        }
        //矩阵相乘实现
        public static Matrix Mutiply(Matrix m1, Matrix m2)
        {
            Matrix   ret = new Matrix(m1.row, m2.col);
            double[,] retArray=ret.Detail;
            if (m1.col != m2.row)
            {
                System.Exception e = new Exception("前者列数不等于后者行数，无法相乘");
                throw e;
            }
          

            for (int i = 0; i < ret.row; i++)
            {
                for (int j = 0; j < ret.col; j++)
                {
                    retArray[i, j] = 0;
                    for (int p = 0; p < m1.col; p++)
                    {
                        retArray[i,j] += (m1.getNum(i, p)*m2.getNum(p, j));
                        
                    }
                    
                }
            }
            return ret;
        }
        


 
        //矩阵求逆实现
        public static Matrix Inverse(Matrix M)
        {
            int m = M.row;
            int n = M.col;
            if (m != n)
            {
                Exception myException = new Exception("求逆的矩阵不是方阵");
                throw myException;
            }
            Matrix ret = new Matrix(m, n);
     
            double[,] a = M.Detail;
            double[,] b = ret.Detail;
            int i, j, row, k;
            double max, temp;

            //单位矩阵
            for (i = 0; i < n; i++)
            {
                b[i, i] = 1;
            }
            for (k = 0; k < n; k++)
            {
                max = 0; row = k;
                //找最大元，其所在行为row
                for (i = k; i < n; i++)
                {
                    temp = Math.Abs(a[i, k]);
                    if (max < temp)
                    {
                        max = temp;
                        row = i;
                    }
                }
                if (max == 0)
                {
                    MessageBox.Show("没有逆矩阵");
                }
                //交换k与row行
                if (row != k)
                {
                    for (j = 0; j < m; j++)
                    {
                        temp = a[row, j];
                        a[row, j] = a[k, j];
                        a[k, j] = temp;

                        temp = b[row, j];
                        b[row, j] = b[k, j];
                        b[k, j] = temp;
                    }

                }

                //首元化为1
                for (j = k + 1; j < m; j++) a[k, j] /= a[k, k];
                for (j = 0; j < m; j++) b[k, j] /= a[k, k];

                a[k, k] = 1;

                //k列化为0
                //对a
                for (j = k + 1; j < m; j++)
                {
                    for (i = 0; i < k; i++) a[i, j] -= a[i, k] * a[k, j];
                    for (i = k + 1; i < m; i++) a[i, j] -= a[i, k] * a[k, j];
                }
                //对b
                for (j = 0; j < m; j++)
                {
                    for (i = 0; i < k; i++) b[i, j] -= a[i, k] * b[k, j];
                    for (i = k + 1; i < m; i++) b[i, j] -= a[i, k] * b[k, j];
                }
                for (i = 0; i < m; i++) a[i, k] = 0;
                a[k, k] = 1;
            }

            return ret;
        }


    }
}
