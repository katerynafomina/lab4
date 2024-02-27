using System.Diagnostics;
using System.Text;
namespace lab4
{
    public partial class Form1 : Form
    {
        private ThreadExample thread1;
        private ThreadExample thread2;
        private ThreadExample thread3;
        private ThreadExample thread4;
       

        public Form1()
        {
            InitializeComponent();


            // ����������� ��'���� ������ �� �� ��������� ������� �� ������
            thread1 = new ThreadExample(1,panel1, suspendButton1, resumeButton1);
            thread2 = new ThreadExample(2,panel2, suspendButton2, resumeButton2);
            thread3 = new ThreadExample(3,panel3, suspendButton3, resumeButton3);
            thread4 = new ThreadExample(4,panel4, suspendButton4, resumeButton4);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ����� ��������� �������� ��������� �� ������
            thread1.StopThread();
            thread2.StopThread();
            thread3.StopThread();
            thread4.StopThread();
        }
    }

    public class ThreadExample
    {
        private int panelNumber;
        private Panel panel;
        private Button suspendButton;
        private Button resumeButton;
        private Thread thread;
        private bool suspended;
        private bool isDisposed; // ������� �� �����

        public ThreadExample(int count, Panel panel, Button suspendButton, Button resumeButton)
        {
            this.panelNumber = count;
            this.panel = panel;
            this.suspendButton = suspendButton;
            this.resumeButton = resumeButton;

            suspendButton.Click += SuspendButton_Click;
            resumeButton.Click += ResumeButton_Click;

            if (panel.IsHandleCreated)
            {
                StartThread();
            }
            else
            {
                // ���� ���� �� �� ��������, ����������� ��䳿 HandleCreated ����� �������� ������
                panel.HandleCreated += (sender, e) => StartThread();
            }

            // ��������, �� ������ ���� ��������
            panel.Disposed += (sender, e) => isDisposed = true;
        }

        private void StartThread()
        {
            thread = new Thread(Run);
            thread.Start();
        }

        public void StopThread()
        {
            // ���������� ������
            thread?.Abort();
        }

        private void Run()
        {
            if (panelNumber == 1)
            {
                DrawFire();
            }
            // ��������� �������� ��� ������� ������
            else if (panelNumber == 2)
            {
                DrawSineWave();
            }
            // ��������� ����������� ������ ������������ ��� �������� ������
            else if (panelNumber == 3)
            {
                DrawChangingRectangle();
            }
            // ��������� ������ ������ ��� ���������� ������
            else if (panelNumber == 4)
            {
                DrawMovingBall();
            }
        }
        private class Star
        {
            public int X { get; private set; }
            public int Y { get; private set; }
            public int Size { get; private set; }
            private int speed;
            private int width;

            public Star(int x, int y, int size, int width)
            {
                X = x;
                Y = y;
                Size = size;
                speed = size;
                this.width = width;
            }

            public void Move()
            {
                X -= speed;
                if (X < 0)
                {
                    X = width;
                }
            }
        }

        private void DrawFire()
        {
            Random random = new Random();
            Star[] stars = new Star[50];

            for (int i = 0; i < stars.Length; i++)
            {
                int x = random.Next(panel.Width);
                int y = random.Next(panel.Height);
                int size = random.Next(1, 5);
                stars[i] = new Star(x, y, size,panel.Width);
            }

            while (true)
            {
                if (!suspended)
                {
                    panel.Invoke(new Action(() =>
                    {
                        // �������� ������������ �����
                        using (Graphics g = panel.CreateGraphics())
                        {
                            g.Clear(Color.Black);
                        }

                        // ��������� ����
                        using (Graphics g = panel.CreateGraphics())
                        {
                            foreach (Star star in stars)
                            {
                                star.Move();
                                g.FillEllipse(Brushes.White, star.X, star.Y, star.Size, star.Size);
                            }
                        }
                    }));
                }

                Thread.Sleep(100); // �������� ��� �������


            }

        }
        
        private void DrawSineWave()
        {
            double angle = 0;
            int xOffset = 0;
            int yOffset = panel.Height / 2;

            while (true)
            {
                if (!suspended)
                {
                    // ��������, �� ������ ����
                    if (panel != null && !panel.IsDisposed)
                    {
                        panel.Invoke(new Action(() =>
                        {
                            using (Bitmap buffer = new Bitmap(panel.Width, panel.Height))
                            {
                                using (Graphics g = Graphics.FromImage(buffer))
                                {
                                    // �������� ������������ �����
                                    g.Clear(Color.White);

                                    // ��������� ��������
                                    int x = 0;
                                    int y = 0;
                                    while (x < panel.Width)
                                    {
                                        y = (int)(Math.Sin(angle) * 50 + yOffset);
                                        g.FillEllipse(Brushes.Red, x + xOffset, y, 5, 5);
                                        angle += 0.1;
                                        x++;
                                    }
                                }

                                // ��������� ������ �� ������
                                using (Graphics panelGraphics = panel.CreateGraphics())
                                {
                                    panelGraphics.DrawImage(buffer, Point.Empty);
                                }
                            }
                        }));
                    }
                }

                Thread.Sleep(100); // �������� ��� ��������� ������ ������
            }
        }
        private void DrawChangingRectangle()
        {
            int width = 50;
            int height = 50;
            int directionX = 1;
            int directionY = 1;

            while (true)
            {
                if (!suspended)
                {
                    // ��������, �� ������ ����
                    if (panel != null && !panel.IsDisposed)
                    {
                        panel.Invoke(new Action(() =>
                        {
                            using (Bitmap buffer = new Bitmap(panel.Width, panel.Height))
                            {
                                using (Graphics g = Graphics.FromImage(buffer))
                                {
                                    // �������� ������������ �����
                                    g.Clear(Color.White);

                                    // ��������� ������������
                                    g.FillRectangle(Brushes.Green, 0, 0, width, height);
                                }

                                // ��������� ������ �� ������
                                using (Graphics panelGraphics = panel.CreateGraphics())
                                {
                                    panelGraphics.DrawImage(buffer, Point.Empty);
                                }
                            }

                            // ���� ������ ������������
                            width += directionX * 5;
                            height += directionY * 5;

                            // ���� ��������, ���� ����������� ������ ��� �����
                            if (width <= 0 || width >= panel.Width)
                            {
                                directionX *= -1;
                            }

                            if (height <= 0 || height >= panel.Height)
                            {
                                directionY *= -1;
                            }
                        }));
                    }
                }

                Thread.Sleep(100); // �������� ��� ��������� ������ ������
            }
        }
        private void DrawMovingBall()
        {
            int x = 0;
            int y = panel.Height / 2;
            int directionX = 1;
            int ballSize = 50;

            while (true)
            {
                if (!suspended)
                {
                    // ��������, �� ������ ����
                    if (panel != null && !panel.IsDisposed)
                    {
                        panel.Invoke(new Action(() =>
                        {
                            using (Bitmap buffer = new Bitmap(panel.Width, panel.Height))
                            {
                                using (Graphics g = Graphics.FromImage(buffer))
                                {
                                    // �������� ������������ �����
                                    g.Clear(Color.White);

                                    // ��������� ������
                                    g.FillEllipse(Brushes.Black, x, y, ballSize, ballSize);
                                }

                                // ��������� ������ �� ������
                                using (Graphics panelGraphics = panel.CreateGraphics())
                                {
                                    panelGraphics.DrawImage(buffer, Point.Empty);
                                }
                            }

                            // ��������� ��������� ������
                            x += directionX * 5;

                            // ���� ��������, ���� ������ ������ ��� �����
                            if (x <= 0 || x >= panel.Width - ballSize)
                            {
                                directionX *= -1;
                            }
                        }));
                    }
                }

                Thread.Sleep(100); // �������� ��� ��������� ������ ������
            }
        }



        private void SuspendButton_Click(object sender, EventArgs e)
        {
            // ������������ ������
            suspended = true;
            suspendButton.Enabled = false;
            resumeButton.Enabled = true;
        }

        private void ResumeButton_Click(object sender, EventArgs e)
        {
            // ³��������� ������ ������
            suspended = false;
            suspendButton.Enabled = true;
            resumeButton.Enabled = false;
        }
    }
}