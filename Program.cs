using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimerConsoleTest
{
    internal class Program
    {
        private static ManualResetEvent @event = new ManualResetEvent(false);

        private static double summ = 0;
        private static double count = 0;

        static void Main(string[] args)
        {
            WriteEmumChoiseToConsole<PollType>("Введите тип потокого опроса");
            var pollType = ReadEmumChoiseFromConsole<PollType>();

            WriteEmumChoiseToConsole<SyncObjType>("Введите тип примитива синхронизации");
            var syncObjType = ReadEmumChoiseFromConsole<SyncObjType>();

            WriteEmumChoiseToConsole<SyncObjLocalization>("Введите тип локализации примитива синхронизации");
            var syncObjLocalization = ReadEmumChoiseFromConsole<SyncObjLocalization>();

            var threadCount = WriteAndReadUInt32GetToConsole("Введите количество потоков", 1000);
            
            var waitMillisec = WriteAndReadUInt32GetToConsole("Введите шаг опроса в миллисекундах", 500);

            var workImmitationTime = WriteAndReadUInt32GetToConsole("Введите время, иммитирующее работу, в миллисекундах", 10);

            Console.WriteLine("Все параметры заданы, введите любой символ, чтобы начать");
            Console.ReadLine();

            for (int i = 0; i < threadCount; i++)
            {
                var Poller = new PollingClass();
                Poller.PollLockHandler += time =>
                {
                    Console.WriteLine($"{DateTime.Now} очередной вызов занял {time.TotalMilliseconds} миллисекунд");
                    summ += time.TotalMilliseconds;
                    count++;
                    Console.WriteLine($"{DateTime.Now} в среденем {summ / count} миллисекунд");

                };
                Poller.Start(pollType, syncObjType, syncObjLocalization, (int)waitMillisec, (int)workImmitationTime);
            }          
            @event.WaitOne();
        }

        private static uint WriteAndReadUInt32GetToConsole(string calling, uint defaultVal)
        {
            Console.WriteLine($"{calling} (по умолчанию {defaultVal})");
            if (!uint.TryParse(Console.ReadLine(), out var output))
            {
                output = defaultVal;
            }
            return output;
        }
        
        private static void WriteEmumChoiseToConsole<TEnum>(string calling)
            where TEnum : struct
        {
            Console.WriteLine(calling);
            foreach (var v in Enum.GetValues(typeof(TEnum)))
            {
                Console.WriteLine($"{v} - {(int)v}");
            }
        }

        private static TEnum ReadEmumChoiseFromConsole<TEnum>()
            where TEnum : struct
        {
            var input = Console.ReadLine();
            if (int.TryParse(input, out var outputNum) && Enum.IsDefined(typeof(TEnum), outputNum))
            { 
                return (TEnum)Enum.ToObject(typeof(TEnum), outputNum);
            }
            else
            {
                return default;
            }
        }
    }

    
}
