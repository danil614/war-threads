using System.Resources;
using Timer = System.Threading.Timer;

namespace WarThreadsGUI
{
    public partial class Form1 : Form
    {
        private const int MaxBullets = 5;
        private const int InitialEnemySpeed = 500;
        private const int MinEnemySpeed = 100;
        private const int EnemySpeedIncrement = 50;

        private bool gameStarted = false;
        private int bulletCount = 0;
        private int hitCount = 0;
        private int missCount = 0;
        private int enemySpeed = InitialEnemySpeed;

        private Thread enemyThread;
        private Timer enemyTimer;
        private Timer gameTimer;

        public Form1()
        {
            InitializeComponent();

            InitializeGame();

            KeyDown += Form1_KeyDown;
        }

        private void InitializeGame()
        {
            enemyThread = new Thread(GenerateEnemies);
            enemyThread.IsBackground = true;

            enemyTimer = new Timer(UpdateEnemies, null, Timeout.Infinite, 1000);
            gameTimer = new Timer(IncreaseEnemySpeed, null, Timeout.Infinite, 5000);
        }

        private void GenerateEnemies()
        {
            while (true)
            {
                if (gameStarted)
                {
                    PictureBox enemy = new PictureBox
                    {
                        Image = Properties.Resources.enemy,
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        Size = new Size(40, 40),
                        Location = new Point(new Random().Next(panelGame.Width - 40), 0),
                        Tag = "enemy"
                    };

                    panelGame.Invoke(new Action(() =>
                    {
                        panelGame.Controls.Add(enemy);
                    }));

                    Thread.Sleep(enemySpeed);
                }
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
        }

        private void HandleMiss(PictureBox enemy)
        {
            missCount++;

            panelGame.Invoke(new Action(() =>
            {
                panelGame.Controls.Remove(enemy);
            }));
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
            enemyThread.Abort();

            MessageBox.Show("Game Over", "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FireBullet()
        {
            PictureBox bullet = new PictureBox
            {
                Image = Properties.Resources.bullet,
                Size = new Size(5, 10),
                Location = new Point(pbCannon.Location.X + pbCannon.Width / 2, pbCannon.Location.Y - 10),
                Tag = "bullet"
            };

            panelGame.Controls.Add(bullet);

            Timer bulletTimer = new Timer(UpdateBullet, bullet, 0, 50);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            int step = 10; // Шаг перемещения пушки

            // Проверяем нажатую клавишу и перемещаем пушку соответственно
            if (e.KeyCode == Keys.Left)
            {
                // Проверяем, чтобы пушка не вышла за пределы левой границы панели
                if (pbCannon.Left - step >= 0)
                {
                    pbCannon.Left -= step;
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                // Проверяем, чтобы пушка не вышла за пределы правой границы панели
                if (pbCannon.Right + step <= panelGame.Width)
                {
                    pbCannon.Left += step;
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
            else if (e.KeyCode == Keys.Enter)
            {
                if (!gameStarted)
                {
                    StartGame();
                }
            }
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

        private void StartGame()
        {
            gameStarted = true;
            bulletCount = 0;
            hitCount = 0;
            missCount = 0;
            enemySpeed = InitialEnemySpeed;

            panelGame.Controls.Clear();

            enemyTimer.Change(0, 1000);
            gameTimer.Change(0, 5000);

            enemyThread = new Thread(GenerateEnemies);
            enemyThread.Start();
        }
    }
}