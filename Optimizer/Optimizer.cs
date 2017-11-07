using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Optimizer
{
    class Optimizer
    {
        //private static HashSet<model.Parameters> _parameterSet;
        private static HashSet<model.ShortParameters> _parameterSet;
        private static RabbitmqHandler _workerQueue = new RabbitmqHandler();
        private static ConcurrentDictionary<int, string[]> _optimizeSummaries = new ConcurrentDictionary<int, string[]>();

        public static void Main(string[] args)
        {
            // 1. Generete all the possible parameters sets
            _parameterSet = GenerateShortParameterSpace();

            // 2. Concurrently publishing all parameters sets & start worker to handle them
            if (_parameterSet.Count == 0)
            {
                Console.WriteLine("No parameter set to run, Exit!");
                return;
            }
            int threadNum = 0;
            List<Worker> workerList = new List<Worker>();
            foreach (model.ShortParameters parameters in _parameterSet)
            {
                _workerQueue.Publish(parameters, "worker_queue_test");
                Console.WriteLine("Worker Thread " + threadNum + " is init to handle the job");
                Worker worker = new Worker();
                Thread workerThread = new Thread(worker.Run) { IsBackground = true, Name = "WorkerThread" + threadNum++ };
                workerThread.Start();
                workerList.Add(worker);
            }

            // 3. Summarize the statistics
            var ts = Stopwatch.StartNew();
            _workerQueue.Consume("log_queue_test", threadNum, "log.txt");
            while (workerList.Any(w => w.IsActive) && ts.ElapsedMilliseconds < 3000 * 1000)
            {
                Thread.Sleep(5 * 1000);
                Console.WriteLine("Waiting for worker threads to exit...");
            }

            Console.WriteLine("All the workers have finished their job. Ready to quit. Press any key to exit the program.");
            Console.ReadLine();

        }

        private static HashSet<model.Parameters> GenerateParameterSpace()
        {
            var result = new HashSet<model.Parameters>();

            var fastPeriodConfig = "10:100:40";
            var slowPeriodConfig = "20:110:40";
            var fastPeriodRange = fastPeriodConfig.Split(':').Select(x => Convert.ToInt32(x)).ToList();
            var slowPeriodRange = slowPeriodConfig.Split(':').Select(x => Convert.ToInt32(x)).ToList();
            int steps = 0;
            while (fastPeriodRange[0] + steps * fastPeriodRange[2] <= fastPeriodRange[1]
                && slowPeriodRange[0] + steps * slowPeriodRange[2] <= slowPeriodRange[1]
                && fastPeriodRange[0] + steps * fastPeriodRange[2] < slowPeriodRange[0] + steps * slowPeriodRange[2])
            {
                var param = new model.Parameters()
                {
                    FastPeriod = fastPeriodRange[0] + steps * fastPeriodRange[2],
                    SlowPeriod = slowPeriodRange[0] + steps * slowPeriodRange[2]
                };
                result.Add(param);
                steps++;
            }

            return result;
        }

        private static HashSet<model.ShortParameters> GenerateShortParameterSpace()
        {
            var result = new HashSet<model.ShortParameters>();

            var bollWindowConfig = "9:50:10";
            var bollDevConfig = "1.0:5.0:0.1";
            var cciWindowConfig = "5:50:5";
            var atrWindowConfig = "5:50:5";

            var bollWindowRange = bollWindowConfig.Split(':').Select(x => Convert.ToInt32(x)).ToList();
            var bollDevRange = bollDevConfig.Split(':').Select(x => Convert.ToDecimal(x)).ToList();
            var cciWindowRange = cciWindowConfig.Split(':').Select(x => Convert.ToInt32(x)).ToList();
            var atrWindowRange = atrWindowConfig.Split(':').Select(x => Convert.ToInt32(x)).ToList();

            int steps = 0;
            while (bollWindowRange[0] + steps * bollWindowRange[2] <= bollWindowRange[1]
                && bollDevRange[0] + steps * bollDevRange[2] <= bollDevRange[1]
                && cciWindowRange[0] + steps * cciWindowRange[2] <= cciWindowRange[1]
                && atrWindowRange[0] + steps * atrWindowRange[2] <= atrWindowRange[1])
            {
                var param = new model.ShortParameters()
                {
                    bollWindow = bollWindowRange[0] + steps * bollWindowRange[2],
                    bollDev = bollDevRange[0] + steps * bollDevRange[2],
                    cciWindow = cciWindowRange[0] + steps * cciWindowRange[2],
                    atrWindow = atrWindowRange[0] + steps * atrWindowRange[2]
                };
                result.Add(param);
                steps++;
            }

            return result;
        }

    }
}
