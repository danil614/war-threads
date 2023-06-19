using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WarThreads
{
    using System;
    using System.Threading;

    class Program
    {
        static object screenlock = new object(); // объект блокировки экрана
        static Semaphore bulletsem = new Semaphore(3, 3); // семафор для ограничения количества пуль
        static AutoResetEvent startevt = new AutoResetEvent(false); // событие для начала игры
        static ConsoleKeyInfo keyInfo; // информация о нажатой клавише

        static int hit = 0; // количество попаданий
        static int miss = 0; // количество промахов

        // Создание случайного числа от n0 до n1
        static int Random(int n0, int n1)
        {
            Random random = new Random();
            return random.Next(n0, n1);
        }

        // Вывести на экран символ в позицию x и y
        static void WriteAt(int x, int y, char c)
        {
            lock (screenlock) // Блокировать вывод на экран при помощи блокировки
            {
                Console.SetCursorPosition(x, y);
                Console.Write(c);
            }
        }

        // Получить нажатие на клавишу (счетчик повторений в ct)
        static ConsoleKey GetAKey(out int ct)
        {
            ct = 0;
            ConsoleKey key = ConsoleKey.NoName;
            while (true)
            {
                keyInfo = Console.ReadKey(true);
                if (keyInfo.Key == ConsoleKey.Spacebar || keyInfo.Key == ConsoleKey.LeftArrow || keyInfo.Key == ConsoleKey.RightArrow)
                {
                    ct = keyInfo.Modifiers == ConsoleModifiers.Shift ? 10 : 1; // установить счетчик повторений
                    key = keyInfo.Key;
                    break;
                }
            }
            return key;
        }

        // Определить символ в заданной позиции экрана
        static char GetAt(int x, int y)
        {
            char c;
            lock (screenlock) // Блокировать доступ к консоли
            {
                Console.SetCursorPosition(x, y);
                c = Console.ReadKey(true).KeyChar;
            }
            return c;
        }

        // Отобразить очки в заголовке окна и проверить условие завершения игры
        static void Score()
        {
            Console.Title = $"Thread War - Попаданий: {hit}, Промахов: {miss}";
            if (miss >= 30)
            {
                Monitor.Enter(screenlock);
                Thread.CurrentThread.Suspend();
                ConsoleMessageBox.Show("Игра окончена!", "Thread War");
                Environment.Exit(0);
            }
        }

        // Поток противника
        static void BadGuy(object _y)
        {
            int y = (int)_y; // случайная координата y
            int dir;
            int x;
            // Нечетные y появляются слева, четные y появляются справа
            x = y % 2 != 0 ? 0 : Console.WindowWidth - 1;
            // Установить направление в зависимости от начальной позиции
            dir = x == 0 ? 1 : -1;
            // Пока противник находится в пределах экрана
            while ((dir == 1 && x != Console.WindowWidth - 1) || (dir == -1 && x != 0))
            {
                bool hitMe = false;
                WriteAt(x, y, "-\\|/"[x % 4]);

                for (int i = 0; i < 15; i++)
                {
                    Thread.Sleep(40);
                    if (GetAt(x, y) == '*')
                    {
                        hitMe = true;
                        break;
                    }
                }
                WriteAt(x, y, ' ');

                if (hitMe)
                {
                    Console.Beep();
                    Interlocked.Increment(ref hit);
                    Score();
                    return;
                }

                x += dir;
            }
            Interlocked.Increment(ref miss);
            Score();
        }

        // Этот поток занимается созданием потоков противников
        static void BadGuys()
        {
            // Ждем сигнала к началу игры в течение 15 секунд
            if (startevt.WaitOne(15000))
            {
                // Создаем случайного врага каждые 5 секунд с координатами от 1 до 10
                while (true)
                {
                    if (Random(0, 100) < (hit + miss) / 25 + 20)
                    {
                        Thread enemyThread = new Thread(BadGuy);
                        enemyThread.Start(Random(1, 10));
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        // Этот поток представляет пулю, каждая пуля - это отдельный поток
        static void Bullet(object _xy_)
        {
            ConsoleKey key = keyInfo.Key;
            int ct = keyInfo.Modifiers == ConsoleModifiers.Shift ? 10 : 1;
            int x = ((ConsoleKeyInfo)_xy_).KeyChar;
            int y = Console.WindowHeight - 1;

            lock (screenlock) // Проверить семафор и создать пулю
            {
                if (!bulletsem.WaitOne(0))
                    return; // если семафор равен 0, выстрела не происходит
            }

            while (y-- > 0)
            {
                WriteAt(x, y, '*'); // отобразить пулю
                Thread.Sleep(100);
                WriteAt(x, y, ' '); // стереть пулю
            }

            lock (screenlock) // Выстрел сделан - добавить 1 к семафору
            {
                bulletsem.Release();
            }
        }

        // Основная программа
        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            Thread mainThread = Thread.CurrentThread;
            startevt.Reset();

            // Инициализировать отображение информации об очках
            Score();

            // установка начальной позиции пушки
            int y = Console.WindowHeight - 1;
            int x = Console.WindowWidth / 2;

            // Запустить поток BadGuys, не делать ничего до получения сигнала или истечения 15 секунд
            Thread badGuysThread = new Thread(BadGuys);
            badGuysThread.Start();

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Spacebar:
                        Thread bulletThread = new Thread(Bullet);
                        bulletThread.Start(keyInfo.KeyChar);
                        Thread.Sleep(100); // дать пуле время улететь на некоторое расстояние
                        break;

                    case ConsoleKey.LeftArrow: // команда "влево!"
                        startevt.Set(); // поток BadGuys работает
                        WriteAt(x, y, ' '); // убрать с экрана пушку
                        while (keyInfo.Modifiers == ConsoleModifiers.Shift ? --ct >= 0 : --ct >= 0) // переместиться
                        {
                            if (x > 0)
                                x--;
                        }
                        break;

                    case ConsoleKey.RightArrow: // команда "вправо!"; логика та же
                        startevt.Set();
                        WriteAt(x, y, ' ');
                        while (keyInfo.Modifiers == ConsoleModifiers.Shift ? --ct >= 0 : --ct >= 0)
                        {
                            if (x < Console.WindowWidth - 1)
                                x++;
                        }
                        break;
                }
            }
        }
    }

}
