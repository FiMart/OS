//เรียงเลขกัน ตั้งแต่ 1-150
using System;
using System.Threading;

namespace OS_Problem_02
{
    class Thread_safe_buffer
    {
        static int[] TSBuffer = new int[10];
        static int Front = 0;
        static int Back = 0;
        static int Count = 0;
        static object lockObject = new object();
        static bool th01Done = false;
        static bool th011Done = false;

        static void EnQueue(int eq)
        {
            lock (lockObject)
            {
                while (Count >= TSBuffer.Length) // รอให้มีพื้นที่ใน Buffer
                {
                    Monitor.Wait(lockObject);
                }

                TSBuffer[Back] = eq;
                Back++;
                Back %= 10;
                Count += 1;

                Monitor.PulseAll(lockObject); // สั่งให้กระบวนการอื่นๆ ที่รอรับสัญญาณทำงาน
            }
        }

        static int DeQueue()
        {
            int x = 0;
            lock (lockObject)
            {
                while (Count == 0) // รอให้มีข้อมูลใน Buffer
                {
                    Monitor.Wait(lockObject);
                }

                x = TSBuffer[Front];
                Front++;
                Front %= 10;
                Count -= 1;

                Monitor.PulseAll(lockObject); // สั่งให้กระบวนการอื่นๆ ที่รอรับสัญญาณทำงาน
            }
            return x;
        }

        static void th01()
        {
            int i;

            for (i = 1; i < 51; i++)
            {
                lock (lockObject)
                {
                    EnQueue(i);
                    //Console.WriteLine("Enqueue {0}", i);
                    Thread.Sleep(5);

                    if (i == 50)
                    {
                        th01Done = true; // บอกว่า Enqueue th1 เสร็จสิ้น
                    }
                }
            }
        }

        static void th011()
        {
            int i;

            for (i = 100; i < 151; i++)
            {
                lock (lockObject)
                {
                    while (!th01Done)
                    {
                        Monitor.Wait(lockObject);
                    }

                    EnQueue(i);
                    //Console.WriteLine("Enqueue {0}", i);
                    Thread.Sleep(5);

                    if (i == 150)
                    {
                        th011Done = true; // บอกว่า Enqueue th11 เสร็จสิ้น
                    }
                }
            }
        }

        static void th02(object t)
        {
            int i;
            int j;
            for (i = 0; i < 60; i++)
            {
                if (Count == 0 && th01Done && th011Done)
                {
                    break;
                }

                j = DeQueue();
                Console.WriteLine("j={0}, thread:{1}", j, t);
                Thread.Sleep(100);
            }
        }

        static void Main(string[] args)
        {
            Thread t1 = new Thread(th01);
            Thread t11 = new Thread(th011);
            Thread t2 = new Thread(th02);
            Thread t21 = new Thread(th02);
            Thread t22 = new Thread(th02);

            t1.Start();
            t11.Start();
            t2.Start(1);
            t21.Start(2);
            t22.Start(3);


        }
    }
}