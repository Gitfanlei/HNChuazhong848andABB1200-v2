using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using System.Configuration;

namespace ResourceDataCollection
{
    class KafkaProducer
    {
        // config  是一个静态只读
        private static readonly Dictionary<string, object> config = new Dictionary<string, object>
        {
            ["bootstrap.servers"] = ConfigurationManager.AppSettings.Get("kafkaIpWithPort"),
            ["retries"] = 0,
            ["batch.num.messages"] = 1,
            ["socket.blocking.max.ms"] = 1,
            ["socket.nagle.disable"] = true,
            ["queue.buffering.max.ms"] = 0,
            ["default.topic.config"] = new Dictionary<string, object>
            {
                ["acks"] = 1
            }
        };

        private static readonly string kafkaTopic = "DeviceData";

        public string ProducerToKafka(string msg)
        {
            // 构建 一个变量 并使用它
            using (var producer= new Producer<Null,string>(config,null,new StringSerializer(Encoding.UTF8)))
            {
                var dr = producer.ProduceAsync(kafkaTopic, null, msg).Result;
                return dr.TopicPartitionOffset.ToString();
            }
        }
    }
}
