namespace WarThreadsGUI
{
    public partial class Form1 : Form
    {
        private const int MaxBullets = 3;
        private readonly string[] EnemyImages = { "enemy1", "enemy2", "enemy3" };
        private readonly Random Random = new Random();
        private Mutex screenlock = new(); // ������ ���������� ������

        private readonly Semaphore BulletSemaphore = new Semaphore(MaxBullets, MaxBullets); // ������� ��� ����������� ���������� ����
        private readonly AutoResetEvent StartEvent = new AutoResetEvent(false); // ������� ��� ������ ����

        private int hitCount = 0;
        private int missCount = 0;
        private bool gameStarted = false;
        private int speed = 10;

        private PictureBox cannonPictureBox;

        public Form1()
        {
            InitializeComponent();

            // ������� ������ �����
            cannonPictureBox = new PictureBox
            {
                Image = Properties.Resources.cannon,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(40, 40),
                Location = new Point(panelGame.Width / 2, panelGame.Height - 40),
                Tag = "cannon"
            };

            InitializeGame();
            KeyDown += Form1_KeyDown;
        }

        private void InitializeGame()
        {
            hitCount = 0;
            missCount = 0;
            speed = 10;
            gameStarted = false;

            StartEvent.Reset();

            // ����� �������� ������
            var generateEnemiesThread = new Thread(GenerateEnemies);
            generateEnemiesThread.Start();

            // ����� ���������� �������� ������
            var speedThread = new Thread(IncreaseSpeed);
            speedThread.Start();

            panelGame.Controls.Clear();
            panelGame.Controls.Add(cannonPictureBox);

            UpdateScore();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int step = 20; // ��� ����������� �����

            // ��������� ������� ������� � ���������� ����� ��������������
            if (e.KeyCode == Keys.Left)
            {
                StartEvent.Set();

                // ���������, ����� ����� �� ����� �� ������� ����� ������� ������
                if (cannonPictureBox.Left - step >= 0)
                {
                    cannonPictureBox.Left -= step;
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                StartEvent.Set();

                // ���������, ����� ����� �� ����� �� ������� ������ ������� ������
                if (cannonPictureBox.Right + step <= panelGame.Width)
                {
                    cannonPictureBox.Left += step;
                }
            }
            else if (e.KeyCode == Keys.Space)
            {
                Thread bulletThread = new Thread(FireBullet);
                bulletThread.Start();
            }
        }

        private void IncreaseSpeed()
        {
            while (gameStarted)
            {
                Interlocked.Increment(ref speed);
                Thread.Sleep(100);
            }
        }

        private void GameOver()
        {
            if (screenlock.WaitOne(0))
            {
                MessageBox.Show($"�� ���������! ���������: {hitCount}, ��������: {missCount}", "����� �������",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close();
            }
        }

        private void GenerateEnemies()
        {
            if (StartEvent.WaitOne(15000))
            {
                gameStarted = true;
            }

            // ������� ���������� �����
            while (gameStarted)
            {
                if (Random.Next(50) < (hitCount + missCount) / 25 + 20)
                {
                    Thread enemyThread = new Thread(CreateEnemy);
                    enemyThread.Start();
                }

                Thread.Sleep(400);
            }
        }

        private void CreateEnemy()
        {
            // ����� ���������� ����������� �����
            string randomImage = EnemyImages[Random.Next(EnemyImages.Length)];

            var size = new Size(20, 20);
            var numberLives = 1;

            // ��������� ���������� y
            int y = Random.Next(panelGame.Height - 50);

            // ������� �������
            if (Random.Next(100) < 5)
            {
                size = new Size(panelGame.Width / 2, panelGame.Height / 2);
                y = 10;
                numberLives = 1000;
            }

            // �������� y ���������� �����, ������ y ���������� ������
            int x = y % 2 != 0 ? 0 : panelGame.Width;

            // ���������� ����������� � ����������� �� ��������� �������
            int direction = x == 0 ? 1 : -1;

            PictureBox enemy = new PictureBox
            {
                Image = (Image)Properties.Resources.ResourceManager.GetObject(randomImage),
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = size,
                Location = new Point(x, y),
                Tag = "enemy"
            };

            panelGame.Invoke(new Action(() =>
            {
                panelGame.Controls.Add(enemy);
            }));

            // ���� ��������� ��������� � �������� ������
            while ((direction == 1 && x <= panelGame.Width) || (direction == -1 && x >= 0))
            {
                for (int i = 0; i < 10; i++)
                {
                    Thread.Sleep(Random.Next(1, 10));

                    foreach (Control control in panelGame.Controls)
                    {
                        // ��������� ������������ ����� � �����
                        if (control is PictureBox bullet && bullet.Tag.ToString() == "bullet")
                        {
                            if (bullet.Bounds.IntersectsWith(enemy.Bounds))
                            {
                                numberLives--;

                                if (numberLives <= 0)
                                {
                                    HandleHit(enemy);
                                    DeleteBullet(bullet);
                                    return;
                                }
                            }
                        }
                    }
                }

                x += direction * speed;

                // ��������� ��������� �����
                enemy.Location = new Point(x, enemy.Location.Y);
            }

            HandleMiss(enemy);
        }

        private void HandleHit(PictureBox enemy)
        {
            Interlocked.Increment(ref hitCount);

            panelGame.Invoke(new Action(() =>
            {
                panelGame.Controls.Remove(enemy);
            }));

            UpdateScore();
        }

        private void HandleMiss(PictureBox enemy)
        {
            Interlocked.Increment(ref missCount);

            // ������� ����� � ������
            panelGame.Invoke(new Action(() =>
            {
                panelGame.Controls.Remove(enemy);
            }));

            UpdateScore();

            // �������� ������� ���������� ����
            if (missCount >= 30)
            {
                GameOver();
            }
        }

        private void UpdateScore()
        {
            string title = string.Format($"����� ������� - ���������: {hitCount}, ��������: {missCount}");

            if (InvokeRequired)
            {
                Invoke(new Action(() =>
                {
                    Text = title;
                }));
            }
            else
            {
                Text = title;
            }
        }

        private void FireBullet()
        {
            // ���� ������� ����� 0, �������� �� ����������
            if (!BulletSemaphore.WaitOne(0))
            {
                return;
            }

            // ���������� ����
            PictureBox bullet = new PictureBox
            {
                Image = Properties.Resources.bullet,
                Size = new Size(15, 15),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(cannonPictureBox.Location.X + 14, cannonPictureBox.Location.Y - 10),
                Tag = "bullet"
            };

            // ��������� ���� �� ������
            panelGame.Invoke(new Action(() =>
            {
                panelGame.Controls.Add(bullet);
            }));

            while (bullet.Location.Y >= 0)
            {
                bullet.Top -= 10;
                Thread.Sleep(10);

                if (bullet.Tag.ToString() == "deleted")
                {
                    return;
                }
            }

            DeleteBullet(bullet);
        }

        private void DeleteBullet(PictureBox bullet)
        {
            if (bullet.Tag.ToString() == "deleted")
            {
                return;
            }

            // ������� ���� � ������
            panelGame.Invoke(new Action(() =>
            {
                panelGame.Controls.Remove(bullet);
            }));

            // ������� ������ - �������� 1 � ��������
            BulletSemaphore.Release();

            bullet.Tag = "deleted";
        }
    }
}