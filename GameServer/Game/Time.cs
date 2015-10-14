using ExitGames.Concurrency.Fibers;
using ExitGames.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Space.Game
{
    public static class Time
    {
        //private static float prevTime;
        //private static float curTime;
        //private static float deltaTimer;
        //private static long tickCounter;

        //private static DateTime startDT;

        //private static readonly PoolFiber fiber = new PoolFiber();
        //private static readonly ReaderWriterLockSlim lockObj = new ReaderWriterLockSlim();
        //private static int lockMilliseconds;
        //private static IDisposable updateDisposable;

        ///// <summary>
        ///// Set start time
        ///// </summary>
        //public static void Start(int lockMillis)
        //{
        //    lockMilliseconds = lockMillis;
        //    prevTime = 0f;
        //    curTime = 0f;
        //    deltaTimer = 0f;
        //    startDT = DateTime.UtcNow;

        //    fiber.Start();
        //    updateDisposable = fiber.ScheduleOnInterval(Tick, 20, 20);
        //}

        //private static void Tick() {
        //    prevTime = curTime;
        //    curTime = (float)(DateTime.UtcNow - startDT).TotalSeconds;
        //    deltaTimer = curTime - prevTime;
        //    tickCounter++;
        //}

        //public static void Stop() {
        //    if (updateDisposable != null) {
        //        updateDisposable.Dispose();
        //        updateDisposable = null;
        //    }
        //    fiber.Dispose();
        //}


        private static float currentTime;
        private static float delta;
        private static DateTime referenceDate;
        private static bool firstTick = true;

        public static void Tick() {
            if(firstTick ) {
                referenceDate = DateTime.UtcNow;
                currentTime = 0;
                delta = 0;
                firstTick = false;
            } else {
                float oldTime = currentTime;
                currentTime = (float)(DateTime.UtcNow - referenceDate).TotalSeconds;
                delta = currentTime - oldTime;
            }
        }


        public static float curtime()
        {
            return currentTime;
        }

        public static float deltaTime() {
            return delta;
        }
    }
}
