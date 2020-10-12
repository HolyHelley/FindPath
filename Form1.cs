using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FindPath
{
    public partial class Form1 : Form
    {
        bool onFiled = false;
        bool mouseDown = false;
        bool targetMoving = false;
        public int fieldSize = 500;
        public int cellSize = 25;
        Point playerLocation = new Point(2, 2);
        Point targetLocation = new Point(15, 15);
        List<Point> dots = new List<Point>();
        List<Point> path = new List<Point>();
        Point maxVal;
        CellMatrix targetField;
        Graphics g;

        struct CellMatrix
        {
            public int[,] Cells;
            public int Size;
            public int CellSize;
            public CellMatrix(int size, int cellSize)
            {
                Size = size;
                CellSize = cellSize;
                Cells = new int[Size, Size];
                Zero();
            }
            public void Zero()
            {
                for (int i = 0; i < Size; i++)
                {
                    for (int j = 0; j < Size; j++)
                    {
                        Cells[i, j] = 0;
                    }
                }
            }
            public int Find(int x, int y)
            {
                return Cells[x, y];
            }
            public int FindByCoords(int x, int y)
            {
                return Cells[x / 25, y / 25];
            }
            public void SetByCoords(int x, int y, int val)
            {
                Cells[x / 25, y / 25] = val;
            }
            public void SetByIndex(int x, int y, int val)
            {
                Cells[x, y] = val;
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadingField();
        }

        private void LoadingField()
        {
            targetField = new CellMatrix(20, 25);
            playerLocation = new Point(2,2);
            Bitmap field = new Bitmap(fieldSize, fieldSize);
            pictureBox1.Image = new Bitmap(field);
            g = Graphics.FromImage(pictureBox1.Image);
            SolidBrush brush = new SolidBrush(Color.White);
            g.FillRectangle(brush, 0, 0, fieldSize, fieldSize);

            DrawLines();
            FillNumbers();

            ////
            //////
            /////ГЕНЕРАЦИЯ ПОЛЯ ЦЕЛИ
            /////
            for (int i = 0; i < targetField.Size; i++)
            {
                for (int j = 0; j < targetField.Size; j++)
                {
                    var tempIndex = 40 -(int) Math.Round( Math.Sqrt((targetLocation.X - i) * (targetLocation.X - i) +
                        (targetLocation.Y - j) * (targetLocation.Y - j))) * 2;
                    if (tempIndex > 0)
                    {
                        targetField.Cells[i, j] = tempIndex;
                        PaintCell(i * cellSize, j * cellSize, Color.FromArgb(
                            ((tempIndex * 5)) > 255 ? 255 : 255 - (tempIndex * 5)   ,
                            ((tempIndex * 5)) > 255 ? 255 : 255 - (tempIndex * 5),
                            255),
                             true);
                    }
                }
            }
            //////
            ////
            ///
            PaintCell((targetLocation.X) * cellSize, targetLocation.Y * cellSize, Color.DarkBlue, false);//цель

            PaintCell(playerLocation.X * cellSize, playerLocation.Y * cellSize, Color.Green, false);//игрок
        }

        private void DrawLines()
        {
            Pen pen = new Pen(Color.Black);
            for (int i = 0; i <= fieldSize / cellSize; i++)
            {
                g.DrawLine(pen, i * cellSize, 0, i * cellSize, fieldSize);
                g.DrawLine(pen, 0, i * cellSize, fieldSize, i * cellSize);
            }
        }

        private void FillNumbers()
        {
            Font font = new Font("Arial", 10);
            SolidBrush brushB = new SolidBrush(Color.Black);
            StringFormat frm = new StringFormat();
            frm.Alignment = StringAlignment.Center;
            frm.LineAlignment = StringAlignment.Center;
            for (int i = 0; i < fieldSize / cellSize; i++)
            {
                for (int j = 0; j < fieldSize / cellSize; j++)
                {
                    g.DrawString(targetField.Find(i, j).ToString(), font, brushB, new RectangleF(i * cellSize, j * cellSize, cellSize, cellSize), frm);
                }
            }
        }

        private void SetNumber(int x, int y)
        {
            Font font = new Font("Arial", 10);
            SolidBrush brushB = new SolidBrush(Color.Black);
            StringFormat frm = new StringFormat();
            frm.Alignment = StringAlignment.Center;
            frm.LineAlignment = StringAlignment.Center;
            g.DrawString(targetField.Find(x, y).ToString(), font, brushB,
                new RectangleF(x * cellSize, y * cellSize, cellSize, cellSize), frm);
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            var xloc = e.X / cellSize;
            var yloc = e.Y / cellSize;
            if (onFiled && (xloc < 20 && xloc >= 0) && (yloc < 20 && yloc >= 0))
            {
                if (mouseDown)
                {
                    targetField.SetByIndex(e.X / cellSize, e.Y / cellSize, -10);
                    PaintCell(e.X / cellSize * cellSize, e.Y / cellSize * cellSize, Color.Chocolate, true);
                    dots.Add(new Point(e.X , e.Y));
                }
                if (targetMoving)
                {
                    PaintCell(targetLocation.X*cellSize, targetLocation.Y*cellSize, Color.White, true);
                    targetLocation = new Point(e.X/cellSize,e.Y/cellSize);
                    PaintCell(e.X/cellSize*cellSize,e.Y/cellSize*cellSize,Color.DarkBlue,true);
                    
                }
            }
        }

        private void NegatField(int x,int y)
        {
            for (int i = 0; i < targetField.Size; i++)
            {
                for (int j = 0; j < targetField.Size; j++)
                {
                    var tempIndex = (int)Math.Sqrt((x / cellSize - i) * (x / cellSize - i) +
                        (y / cellSize - j) * (y / cellSize - j)) - 10;
                    if (tempIndex < -7)
                    {
                        targetField.Cells[i, j] = tempIndex + targetField.Cells[i, j];
                        PaintCell(i * cellSize, j * cellSize, Color.FromArgb(255, -tempIndex * 20, -tempIndex * 20), true);
                    }
                }
            }
        }

        /// <summary>
        /// Закраска клетки по индексам
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void PaintCell(int x, int y, Color color, bool drawNumber)
        {
            var rectLoc = new Point(x, y);
            // Console.WriteLine(rectLoc);
            Pen pen = new Pen(Color.Black);
            SolidBrush brush = new SolidBrush(color);
            g.FillRectangle(brush, rectLoc.X, rectLoc.Y, cellSize, cellSize);
            g.DrawRectangle(pen, rectLoc.X, rectLoc.Y, cellSize, cellSize);
            if (drawNumber)
                SetNumber(x / cellSize, y / cellSize);

            pictureBox1.Invalidate();
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            onFiled = false;
            //  Console.WriteLine(onFiled);
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            onFiled = true;
            //  Console.WriteLine(onFiled);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (onFiled)
            {
                if (new Point(e.X / cellSize, e.Y / cellSize)!=targetLocation)
                {
                    mouseDown = true;
                    targetField.SetByIndex(e.X / cellSize, e.Y / cellSize, -10);
                    PaintCell(e.X / cellSize * cellSize, e.Y / cellSize * cellSize, Color.Chocolate, true);
                }
                else
                {
                    targetMoving = true;
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseDown = false;
            if (targetMoving == true)
            {
                targetMoving = false;
                LoadingField();
            }
            //foreach(Point p in dots)
            //{
            //    NegatField(p.X, p.Y);
            //}
            dots.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            timer1.Start();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            PaintCell(playerLocation.X * cellSize, playerLocation.Y * cellSize, Color.Green, true);
            path.Add(new Point(playerLocation.X, playerLocation.Y));

            ///
            ////ПОИСК ПУТИ
            ///4
            ///
            if(playerLocation.X + 1 < targetField.Size-1)
                maxVal = new Point(playerLocation.X + 1, playerLocation.Y);
            else
            {
                maxVal = new Point(playerLocation.X - 1, playerLocation.Y);
            }

            for (int i = (playerLocation.X - 1 < 0 ? 0 : playerLocation.X - 1);
                i <= (playerLocation.X+1 > targetField.Size-1 ? playerLocation.X : playerLocation.X + 1); i++)
            {
                for (int j = (playerLocation.Y - 1 < 0 ? 0 : playerLocation.Y - 1);
                    j <= (playerLocation.Y+1 > targetField.Size-1 ? playerLocation.Y : playerLocation.Y + 1); j++)
                {
                    Console.WriteLine(i+" "+j);
                    if (targetField.Find(i, j) >= targetField.Find(maxVal.X, maxVal.Y))
                    {
                        if (!(i == playerLocation.X && j==playerLocation.Y) && !path.Contains(new Point(i,j)) &&
                            targetField.Find(i,j)>-10)
                        {

                            maxVal = new Point(i, j);
                            //path.Add(maxVal);
                        }
                    }
                }
            }

            path.Add(maxVal);
        
            Console.WriteLine(maxVal);
            playerLocation = maxVal;
            PaintCell(playerLocation.X * cellSize, playerLocation.Y * cellSize, Color.Green, false);
            if (playerLocation == targetLocation)
                timer1.Stop();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            path.Clear();
            LoadingField();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            timer1.Stop();
        }
    }
}
