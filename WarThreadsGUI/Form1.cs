using System;
using Timer = System.Threading.Timer;

namespace WarThreadsGUI
{
    public partial class Form1 : Form
    {
        private const int MaxBullets = 5;
        private const int InitialEnemySpeed = 500;
        private const int MinEnemySpeed = 100;
        private const int EnemySpeedIncrement = 50;
        private readonly string[] EnemyImages = { "enemy1", "enemy2", "enemy3" };
        private readonly Random Random = new Random();

        private bool gameStarted = false;
        private int bulletCount = 0;
        private int hitCount = 0;
        private int missCount = 0;
        private int enemySpeed = InitialEnemySpeed;

        private Thread enemyThread;
        private Timer enemyTimer;
        private Timer gameTimer;
        private PictureBox cannonPictureBox;

        public Form1()
        {
            InitializeComponent();

            InitializeGame();

            KeyDown += Form1_KeyDown;
        }

        private void InitializeGame()
        {
            gameStarted = false;

            enemyTimer = new Timer(UpdateEnemies, null, Timeout.Infinite, 1000);
            gameTimer = new Timer(IncreaseEnemySpeed, null, Timeout.Infinite, 5000);

            cannonPictureBox = new PictureBox
            {
                Image = Properties.Resources.cannon,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(40, 40),
                Location = new Point(panelGame.Width / 2, panelGame.Height - 40),
                Tag = "cannon"
            };

            ClearPanelGame();
            UpdateScore();
        }

        private void ClearPanelGame()
        {
            panelGame.Controls.Clear();
            panelGame.Controls.Add(cannonPictureBox);
        }

        private void StartGame()
        {
            if (!gameStarted)
            {
                gameStarted = true;
                bulletCount = 0;
                hitCount = 0;
                missCount = 0;
                enemySpeed = InitialEnemySpeed;

                enemyTimer.Change(0, 1000);
                gameTimer.Change(0, 5000);

                enemyThread = new Thread(GenerateEnemies);
                enemyThread.Start();

                UpdateScore();
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int step = 10; // Шаг перемещения пушки

            // Проверяем нажатую клавишу и перемещаем пушку соответственно
            if (e.KeyCode == Keys.Left)
            {
                StartGame();

                // Проверяем, чтобы пушка не вышла за пределы левой границы панели
                if (cannonPictureBox.Left - step >= 0)
                {
                    cannonPictureBox.Left -= step;
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                StartGame();

                // Проверяем, чтобы пушка не вышла за пределы правой границы панели
                if (cannonPictureBox.Right + step <= panelGame.Width)
                {
                    cannonPictureBox.Left += step;
                }
            }
            else if (e.KeyCode == Keys.Space)
            {
                if (bulletCount >= MaxBullets)
                {
                    return;
                }

                bulletCount++;
                FireBullet();
            }
        }

        private void GenerateEnemies()
        {
            while (gameStarted)
            {
                // Выбор случайного изображения врага
                string randomImage = EnemyImages[Random.Next(EnemyImages.Length)];

                int x;

                // Случайно выбираем позицию слева или справа на панели
                if (Random.Next(2) == 0)
                {
                    x = 0; // Позиция слева
                }
                else
                {
                    x = panelGame.Width - 30; // Позиция справа
                }

                PictureBox enemy = new PictureBox
                {
                    Image = (Image)Properties.Resources.ResourceManager.GetObject(randomImage),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Size = new Size(20, 20),
                    Location = new Point(x, Random.Next(panelGame.Height - 20)),
                    Tag = "enemy"
                };

                panelGame.Invoke(new Action(() =>
                {
                    panelGame.Controls.Add(enemy);
                }));

                Thread.Sleep(enemySpeed);
            }
        }

        private void UpdateEnemies(object state)
        {
            foreach (Control control in panelGame.Controls)
            {
                if (control is PictureBox enemy && enemy.Tag.ToString() == "enemy")
                {
                    if (enemy.InvokeRequired)
                    {
                        enemy.Invoke(new Action(() =>
                        {
                            enemy.Top += 40;
                        }));
                    }
                    else
                    {
                        enemy.Top += 40;
                    }

                    if (enemy.Location.Y >= panelGame.Height)
                    {
                        HandleMiss(enemy);
                    }
                }
            }
        }

        private void HandleHit(PictureBox enemy)
        {
            hitCount++;

            panelGame.Invoke(new Action(() =>
            {
                panelGame.Controls.Remove(enemy);
            }));

            UpdateScore();
        }

        private void HandleMiss(PictureBox enemy)
        {
            missCount++;

            panelGame.Invoke(new Action(() =>
            {
                panelGame.Controls.Remove(enemy);
            }));

            UpdateScore();

            // Проверка условия завершения игры
            if (missCount >= 30)
            {
                GameOver();
            }
        }

        private void UpdateScore()
        {
            string title = string.Format($"Война потоков - Попаданий: {hitCount}, Промахов: {missCount}");

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

        private void IncreaseEnemySpeed(object state)
        {
            enemySpeed -= EnemySpeedIncrement;

            if (enemySpeed <= MinEnemySpeed)
            {
                enemySpeed = MinEnemySpeed;
                gameTimer.Dispose();
            }
        }

        private void GameOver()
        {
            gameStarted = false;

            enemyTimer.Change(Timeout.Infinite, Timeout.Infinite);
            gameTimer.Change(Timeout.Infinite, Timeout.Infinite);

            ClearPanelGame();

            MessageBox.Show($"Вы проиграли! Попаданий: {hitCount}, Промахов: {missCount}", "Война потоков", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FireBullet()
        {
            PictureBox bullet = new PictureBox
            {
                Image = Properties.Resources.bullet,
                Size = new Size(15, 15),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(cannonPictureBox.Location.X + 14, cannonPictureBox.Location.Y - 10),
                Tag = "bullet"
            };

            panelGame.Controls.Add(bullet);

            Timer bulletTimer = new Timer(UpdateBullet, bullet, 0, 50);
        }

        private void UpdateBullet(object state)
        {
            PictureBox bullet = (PictureBox)state;

            if (bullet.InvokeRequired)
            {
                bullet.Invoke(new Action(() =>
                {
                    bullet.Top -= 20;
                }));
            }
            else
            {
                bullet.Top -= 20;
            }

            if (bullet.Location.Y <= 0)
            {
                panelGame.Invoke(new Action(() =>
                {
                    panelGame.Controls.Remove(bullet);
                }));

                bulletCount--;
            }
            else
            {
                foreach (Control control in panelGame.Controls)
                {
                    if (control is PictureBox enemy && enemy.Tag.ToString() == "enemy")
                    {
                        if (bullet.Bounds.IntersectsWith(enemy.Bounds))
                        {
                            HandleHit(enemy);
                            panelGame.Invoke(new Action(() =>
                            {
                                panelGame.Controls.Remove(bullet);
                            }));

                            bulletCount--;
                        }
                    }
                }
            }
        }
    }
}