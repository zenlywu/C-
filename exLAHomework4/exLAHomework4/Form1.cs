using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace exLAHomework4
{
    public partial class Form1 : Form
    {
        Bitmap _bmpBG = new Bitmap(800, 600);
        Bitmap _bmp = new Bitmap(800, 600);        
        Point _ptS = new Point();
        Point _ptE = new Point();
        bool _draw = false;
        ArrayList _ptList = new ArrayList();
        private ArrayList _currentPolygon = new ArrayList();

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add("Rectangle");
            comboBox1.Items.Add("Pentagon");
            comboBox1.Items.Add("Ellipse");
            comboBox1.Items.Add("Polygon");

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            _ptS.X = e.X;
            _ptS.Y = e.Y;
            _draw = true;
            _ptList.Add(_ptS); // Add the initial point to the list
        }



        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _ptE.X = e.X;
            _ptE.Y = e.Y;
            _draw = false;

            if (this.comboBox1.Text == "Line") CalculatelLine(_ptS, _ptE);
            if (this.comboBox1.Text == "Triangle")
            {
                DrawTriangle(_ptList);
                CalculateTriangle();
            }
            if (this.comboBox1.Text == "Rectangle")
            {
                DrawRectangle(_ptList);
                CalculateRectangle();
            }
            if (this.comboBox1.Text == "Pentagon" && _ptList.Count >= 5)
            {
                _bmp = (Bitmap)_bmpBG.Clone();
                DrawPentagon(_ptList);
                CalculatePentagon(_ptList);
                _ptList.Clear();
            }
            if (this.comboBox1.Text == "Ellipse")
            {
                DrawEllipse(_ptS, _ptE);
                CalculateEllipse(_ptS, _ptE);
            }
            if (this.comboBox1.Text == "Polygon")
            {
                // 只在滑鼠移動時添加新的點
                if (_draw)
                {
                    _ptList.Add(e.Location);
                }

                if (_ptList.Count >= 3)
                {
                    // 連接最後一點和起始點
                    DrawPolygon(_ptList, (Point)_ptList[0]);
                    CalculatePolygon();
                }
            }

           

        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.comboBox1.Text == "Line")
            {
                if (!_draw) return;
                DrawLine(_ptS, new Point(e.X, e.Y));
            }
            if (_draw && this.comboBox1.Text == "Polygon")
            {
                DrawPolygon(_ptList, e.Location);
            }

        }

        private void CalculatelLine(Point ptS, Point ptE)
        {
            double X2 = Math.Pow(ptS.X - ptE.X, 2);
            double Y2 = Math.Pow(ptS.Y - ptE.Y, 2);
            double dist = Math.Sqrt(X2 + Y2);
            string strLog1 = string.Format("From ({0},{1}) to ({2},{3}) dist={4} \r\n", ptS.X, ptS.Y, ptE.X, ptE.Y, dist);
            string strLog2 = string.Format("Area = 0 \r\n\r\n");
            this.textBox1.Text += strLog1+strLog2 ;
        }
        private void DrawLine(Point ptS,Point ptE)
        {
            _bmp = (Bitmap) _bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            //g.Clear(Color.White);
            Pen pen = new Pen(Color.Purple, 3);
            g.DrawLine(pen, ptS, ptE);
            this.pictureBox1.Image = _bmp;
        }
        private void CalculateTriangle()
        {
            if (_ptList.Count < 3) return;

            double totalDistance = 0;
            double area = 0;

            StringBuilder coordinates = new StringBuilder("座標: ");

            for (int i = 0; i < 3; i++)
            {
                int idxS = i;
                int idxE = (i + 1) % 3;

                Point ptS = (Point)_ptList[idxS];
                Point ptE = (Point)_ptList[idxE];

                double X2 = Math.Pow(ptS.X - ptE.X, 2);
                double Y2 = Math.Pow(ptS.Y - ptE.Y, 2);
                double dist = Math.Sqrt(X2 + Y2);

                totalDistance += dist;
                area += (ptS.X * ptE.Y - ptE.X * ptS.Y);

                coordinates.AppendFormat("({0},{1})", ptS.X, ptS.Y);

                if (i < 2)
                {
                    coordinates.Append(" - ");
                }
            }

            totalDistance = Math.Round(totalDistance, 2);
            area = Math.Abs(area) / 2.0;

            string strLog1 = string.Format("周長 = {0} \r\n", totalDistance);
            string strLog2 = string.Format("面積 = {0} \r\n\r\n", area);

            this.textBox1.Text += coordinates.ToString() + "\r\n" + strLog1 + strLog2;

            DrawTriangle(_ptList);
            _ptList.Clear();
        }

        private void DrawTriangle(ArrayList list)
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);            
            Pen pen = new Pen(Color.Purple, 3);
            for(int i=0;i<list.Count;i++)
            {
                Point pt1 = (Point)list[i];
                g.DrawEllipse(pen,pt1.X,pt1.Y  ,3,3);

                //if (list.Count<=2) continue;
                int idxE = (i + 1) % 3;
                if (idxE >= list.Count) continue;
                Point pt2 = (Point)list[idxE];
                g.DrawLine(pen, pt1,pt2);
            }            
            this.pictureBox1.Image = _bmp;
        }
        private void CalculateRectangle()
        {
            if (_ptList.Count < 2)
            {
                // 至少需要兩個頂點
                return;
            }

            // 取得兩個已點擊的頂點
            Point pt1 = (Point)_ptList[0];
            Point pt2 = (Point)_ptList[1];

            // 計算其他兩個點的座標
            Point pt3 = new Point(pt1.X, pt2.Y);
            Point pt4 = new Point(pt2.X, pt1.Y);

            // 輸出四個頂點的座標
            StringBuilder pointsLog = new StringBuilder("頂點座標: ");
            pointsLog.AppendFormat("({0},{1}) ({2},{3}) ({4},{5}) ({6},{7})",
                pt1.X, pt1.Y, pt2.X, pt2.Y, pt3.X, pt3.Y, pt4.X, pt4.Y);

            int width = Math.Abs(pt2.X - pt1.X);
            int height = Math.Abs(pt2.Y - pt1.Y);

            int area = width * height;
            int perimeter = 2 * (width + height);

            string strLog1 = string.Format("寬度 = {0} \r\n", width);
            string strLog2 = string.Format("高度 = {0} \r\n", height);
            string strLog4 = string.Format("周長 = {0} \r\n\r\n", perimeter);
            string strLog3 = string.Format("面積 = {0} \r\n", area);
            

            this.textBox1.Text += pointsLog.ToString() + "\r\n" + strLog1 + strLog2 + strLog3 + strLog4;

            DrawRectangle(_ptList);
            _ptList.Clear();
        }

        private void DrawRectangle(ArrayList list)
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            Pen pen = new Pen(Color.Blue, 3);

            if (list.Count >= 2)
            {
                Point pt1 = (Point)list[0];
                Point pt2 = (Point)list[1];

                g.DrawRectangle(pen, Math.Min(pt1.X, pt2.X), Math.Min(pt1.Y, pt2.Y),
                    Math.Abs(pt2.X - pt1.X), Math.Abs(pt2.Y - pt1.Y));
            }

            for (int i = 1; i < list.Count; i++)
            {
                Point pt = (Point)list[i];
                g.DrawEllipse(pen, pt.X, pt.Y, 3, 3);

                if (i < list.Count - 1)
                {
                    Point nextPt = (Point)list[i + 1];
                    g.DrawLine(pen, pt, nextPt);
                }
            }

            this.pictureBox1.Image = _bmp;
        }

        private void CalculatePentagon(ArrayList list)
        {
            if (list.Count < 5) return;

            double perimeter = 0;
            double area = 0;

            StringBuilder verticesLog = new StringBuilder("頂點座標: ");
            for (int i = 0; i < 5; i++)
            {
                Point pt = (Point)list[i];
                verticesLog.AppendFormat("({0},{1})", pt.X, pt.Y);

                if (i < 4)
                {
                    verticesLog.Append(" - ");
                }
            }

            for (int i = 0; i < 5; i++)
            {
                Point pt1 = (Point)list[i];
                Point pt2 = (Point)list[(i + 1) % 5];
                perimeter += DistanceBetweenPoints(pt1, pt2);
            }

            // 這裡可以使用適當的方法計算五邊形的面積
            // 這裡僅舉例，實際應用中可能需要使用更複雜的公式
            area = 0.25 * Math.Sqrt(5 * (5 + 2 * Math.Sqrt(5))) * Math.Pow(DistanceBetweenPoints((Point)list[0], (Point)list[1]), 2);

            // 輸出周長和面積
            string strLog1 = string.Format("周長 = {0} \r\n", perimeter);
            string strLog2 = string.Format("面積 = {0} \r\n\r\n", area);
            this.textBox1.Text += verticesLog.ToString() + "\r\n" + strLog1 + strLog2;

            DrawPentagon(list);
        }
        private void DrawPentagon(ArrayList list)
        {
            if (list.Count >= 5)
            {
                _bmp = (Bitmap)_bmpBG.Clone();
                Graphics g = Graphics.FromImage(_bmp);
                Pen pen = new Pen(Color.Green, 3);

                Point[] pointsArray = new Point[5];
                for (int i = 0; i < 5; i++)
                {
                    pointsArray[i] = (Point)list[i];
                }

                g.DrawPolygon(pen, pointsArray);

                // 顯示每個頂點
                foreach (Point pt in list)
                {
                    g.DrawEllipse(pen, pt.X, pt.Y, 3, 3);
                }

                this.pictureBox1.Image = _bmp;
            }
        }
        private double DistanceBetweenPoints(Point pt1, Point pt2)
        {
            double xDiff = pt2.X - pt1.X;
            double yDiff = pt2.Y - pt1.Y;

            return Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }
        private void CalculateEllipse(Point ptS, Point ptE)
        {
            double majorAxis = Math.Max(Math.Abs(ptE.X - ptS.X), Math.Abs(ptE.Y - ptS.Y));
            double minorAxis = Math.Min(Math.Abs(ptE.X - ptS.X), Math.Abs(ptE.Y - ptS.Y));

            double area = Math.PI * majorAxis * minorAxis;
            double perimeter = Math.PI * (3 * (majorAxis + minorAxis) - Math.Sqrt((3 * majorAxis + minorAxis) * (majorAxis + 3 * minorAxis)));

            string strLog1 = string.Format("長軸 = {0} \r\n", majorAxis);
            string strLog2 = string.Format("短軸 = {0} \r\n", minorAxis);
            string strLog3 = string.Format("面積 = {0} \r\n", area);
            string strLog4 = string.Format("周長 = {0} \r\n\r\n", perimeter);

            this.textBox1.Text += strLog1 + strLog2 + strLog3 + strLog4;
        }
        private void DrawEllipse(Point ptS, Point ptE)
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            Pen pen = new Pen(Color.Red, 3);

            int x = Math.Min(ptS.X, ptE.X);
            int y = Math.Min(ptS.Y, ptE.Y);
            int width = Math.Abs(ptE.X - ptS.X);
            int height = Math.Abs(ptE.Y - ptS.Y);

            g.DrawEllipse(pen, x, y, width, height);

            this.pictureBox1.Image = _bmp;
        }
        private void CalculatePolygon()
        {
            if (_ptList.Count < 3) return;

            // 計算 Polygon 頂點座標、周長和面積
            StringBuilder resultBuilder = new StringBuilder();
            double perimeter = 0;
            double area = 0;

            for (int i = 0; i < _ptList.Count; i++)
            {
                Point pt1 = (Point)_ptList[i];
                Point pt2 = (Point)_ptList[(i + 1) % _ptList.Count]; // 下一個點，最後一個點連接回第一個點

                // 計算兩點之間的距離
                double dist = Math.Sqrt(Math.Pow(pt2.X - pt1.X, 2) + Math.Pow(pt2.Y - pt1.Y, 2));

                // 計算周長
                perimeter += dist;

                // 計算面積（這裡的面積可能是多邊形的總面積，取決於多邊形的形狀）
                area += 0.5 * (pt1.X * pt2.Y - pt2.X * pt1.Y);

                // 將頂點座標加入結果中
                //resultBuilder.AppendFormat("頂點{0}: ({1},{2}) \r\n", i + 1, pt1.X, pt1.Y);
            }

            string strLog1 = resultBuilder.ToString();
            string strLog2 = string.Format("周長 = {0} \r\n", perimeter);
            string strLog3 = string.Format("面積 = {0} \r\n\r\n", Math.Abs(area));

            this.textBox1.Text += strLog1 + strLog2 + strLog3;
        }

        private void DrawPolygon(ArrayList list, Point endPoint)
        {
            _bmp = (Bitmap)_bmpBG.Clone();
            Graphics g = Graphics.FromImage(_bmp);
            Pen pen = new Pen(Color.Blue, 3);

            // 繪製 Polygon 邏輯，連接每個邊
            if (list.Count > 1)
            {
                g.DrawLines(pen, list.Cast<Point>().ToArray());
                g.DrawLine(pen, (Point)list[list.Count - 1], endPoint);
            }

            this.pictureBox1.Image = _bmp;
        }


        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() != DialogResult.OK) return;
            string file = dlg.FileName;
            Bitmap bmp = (Bitmap)Image.FromFile(file);
            double ratioX = (double)bmp.Width / 800;
            double ratioY = (double)bmp.Height / 600;
            double scale = Math.Max(ratioX, ratioY);
            _bmpBG = new Bitmap(bmp, new Size((int)(bmp.Width / scale), (int)(bmp.Height / scale)));
            this.pictureBox1.Image = _bmpBG;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _ptList.Clear();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
