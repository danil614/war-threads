using System;
using System.Collections.Generic;
using System.Threading;

class Program
{
    static object screenLock = new object(); // объект блокировки для изменения экрана
    static Semaphore bulletSemaphore = new Semaphore(3, 3); // семафор для ограничения количества выстрелов
    static AutoResetEvent startEvent = new AutoResetEvent(false); // событие для начала игры
    static int hit = 0; // количество попаданий
    static int miss = 0; // количество промахов

    static void Main(string[] args)
    {
        Console.CursorVisible = false;
        Console.Title = "Война потоков - Попаданий: 0, Промахов: 0";
        startEvent.Reset();

        // Запустить поток BadGuys, не делать ничего до получения сигнала или истечения 15 секунд
        Thread badGuysThread = new Thread(BadGuysThread);
        badGuysThread.IsBackground = true;
        badGuysThread.Start();

        // Установка начальной позиции пушки
        int x = Console.WindowWidth / 2;
        int y = Console.WindowHeight - 1;

        while (true)
        {
            lock (screenLock)
            {
                Console.SetCursorPosition(x, y);
                Console.Write('|');
            }

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            switch (keyInfo.Key)
            {
                case ConsoleKey.Spacebar:
                    FireBullet(x, y);
                    break;
                case ConsoleKey.LeftArrow: // Команда "влево!"
                    startEvent.Set(); // Поток BadGuys работает
                    Console.SetCursorPosition(x, y);
                    Console.Write(' '); // Убрать с экрана пушку
                    if (x > 0) x--;
                    break;
                case ConsoleKey.RightArrow: // Команда "вправо!"
                    startEvent.Set();
                    Console.SetCursorPosition(x, y);
                    Console.Write(' ');
                    if (x < Console.WindowWidth - 1) x++;
                    break;
            }
        }
    }

    static void BadGuysThread()
    {
        Random random = new Random();

        startEvent.Reset(); // Сбросить событие перед ожиданием
        startEvent.WaitOne(TimeSpan.FromSeconds(15));

        while (true)
        {
            if (random.Next(100) < (hit + miss) / 25 + 20)
            {
                int y = random.Next(1, 11);
                Thread badGuyThread = new Thread(() => BadGuyThread(y));
                badGuyThread.IsBackground = true;
                badGuyThread.Start();
            }

            Thread.Sleep(1000);
        }
    }

    static void BadGuyThread(int y)
    {
        int x = y % 2 == 0 ? 0 : Console.WindowWidth - 1;
        int direction = x == 0 ? 1 : -1;

        while ((direction == 1 && x != Console.WindowWidth) || (direction == -1 && x != -1))
        {
            lock (screenLock)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(GetBadGuyChar(x));
            }

            bool isHit = false;

            for (int i = 0; i < 15; i++)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                    if (keyInfo.Key == ConsoleKey.Spacebar)
                    {
                        lock (screenLock)
                        {
                            Console.SetCursorPosition(x, y);
                            Console.Write(' ');
                        }
                        isHit = true;
                        break;
                    }
                }

                Thread.Sleep(200);
            }

            lock (screenLock)
            {
                Console.SetCursorPosition(x, y);
                Console.Write(' ');
            }

            if (isHit)
            {
                Interlocked.Increment(ref hit);
                Console.Title = $"Война потоков - Попаданий: {hit}, Промахов: {miss}";
            }
            else
            {
                Interlocked.Increment(ref miss);
                Console.Title = $"Война потоков - Попаданий: {hit}, Промахов: {miss}";
            }

            Thread.Sleep(100);
        }
    }

    static void FireBullet(int x, int y)
    {
        if (bulletSemaphore.WaitOne(TimeSpan.Zero))
        {
            Thread bulletThread = new Thread(() => BulletThread(x, y));
            bulletThread.IsBackground = true;
            bulletThread.Start();
        }
    }

    static void BulletThread(int x, int y)
    {
        while (y > 0)
        {
            lock (screenLock)
            {
                Console.SetCursorPosition(x, y);
                Console.Write('*');
                Console.SetCursorPosition(x, y + 1);
                Console.Write(' ');
            }

            y--;

            Thread.Sleep(100);
        }

        bulletSemaphore.Release();
    }

    static char GetBadGuyChar(int x)
    {
        if (x % 2 == 0)
            return '/';
        else
            return '\\';
    }
}
