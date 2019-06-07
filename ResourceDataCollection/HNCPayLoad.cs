using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceDataCollection
{
    class HNCPayLoad
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string IP { get; set; }
        public DateTime TimeStamp { get; set; }
        public HNCLoadData LoadDataInfo { get; set; }
        public HNCPositionData PositionInfo { get; set; }

        // 新增 hnc数据库
        public HNCcmdVelData HNCcmdVelData { get; set; }
        public HNCactVelData HNCactVelData { get; set; }
        public string AlarmInfo { set; get; }
        public int AlarmNum { get; set; }
        public int hncCodeLineNum { set; get; } 
        public string posabnormalInfo { set; get; }
        public string loadabnormalInfo { set; get; }
    }
}
