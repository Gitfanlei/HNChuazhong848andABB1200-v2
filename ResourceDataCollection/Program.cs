using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Configuration;
using Newtonsoft.Json;
using HNCAPI;

namespace ResourceDataCollection
{
    class Program
    {
        private static ABBCollector RunABBCollector;
        private static KafkaProducer RunProducer;
        private static HNCCollector RunHNCCollector;
        private static Timer mainTimer;

        private static readonly int timeInterval = Convert.ToInt32(ConfigurationManager.AppSettings.Get("maininterval"));


        // 主程序入口
        [MTAThread]
        static void Main(string[] args)
        {
            SetUp();

            Console.WriteLine("press any key to exit!");
            Console.ReadKey();
            mainTimer.Stop();
            mainTimer.Dispose();
        }

        private static void SetUp()
        {
            RunHNCCollector = new HNCCollector();
            RunABBCollector = new ABBCollector();
            RunProducer = new KafkaProducer();


            SetTimer(timeInterval);
        }

        private static void SetTimer(int interval)
        {
            mainTimer = new Timer(interval);
            mainTimer.Elapsed += dataTranslateData;
            //mainTimer.Elapsed += readTextRange;
            mainTimer.Start();
            mainTimer.AutoReset = true;
            mainTimer.Enabled = true;
        }

        // 需要测试
        private static void dataTranslateData(object source, ElapsedEventArgs e)
        {
            // 机器人数据
            ABBPayLoad ABBPayLoad = new ABBPayLoad
            {
                ID = RunABBCollector.abbSystemID,
                IP = RunABBCollector.abbSystemIP,
                Name = RunABBCollector.abbSystemName,   // 可以修改为 string
                PositionInfo = RunABBCollector.abbPositionInfo,
                TimeStamp = DateTime.Now,
                axisNum = RunABBCollector.abbAxisNum,
                speedRatio = RunABBCollector.abbSpeedRatio,
                rapidvalue = RunABBCollector.abbRapidValue,
                level = RunABBCollector.abbrunLevel,
                curUser = RunABBCollector.abbCurUser,
                Temperature = RunABBCollector.abbTemperature,
                operationMode = RunABBCollector.abbOperationMode,
                contrllerName = RunABBCollector.abbControllerName,
                progressBar = RunABBCollector.progressBar,
                curEvent = RunABBCollector.abbreadCurEvent
            };

            // 注意ABB 机器人 负载数据转化格式
            var positionInfo = ABBPayLoad.PositionInfo;

            string ABBInfoJson = JsonConvert.SerializeObject(ABBPayLoad);
            // HNC数据

            string HNCInfoJson = JsonConvert.SerializeObject(RunHNCCollector.hncPayload);

            string devicesInfo = JsonConvert.SerializeObject(ABBInfoJson + HNCInfoJson);

            Console.WriteLine(devicesInfo.ToString());

            var kafkaTopicOffset = RunProducer.ProducerToKafka(devicesInfo);
        }
    }
}

