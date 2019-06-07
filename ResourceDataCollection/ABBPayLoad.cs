using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.Robotics.Controllers.RapidDomain;

namespace ResourceDataCollection
{
    class ABBPayLoad
    {
        public string ID { set; get; }
        public string IP { set; get; }
        public string Name { set; get; }
        public DateTime TimeStamp { set; get; }
        public RobJoint PositionInfo { set; get; }
        public int speedRatio { set; get; }
        public string axisNum { set; get; }
        public string rapidvalue { set; get; }
        public string level { set; get; }
        public string curUser { set; get; }
        public float Temperature { set; get; }
        public string contrllerName { set; get; }
        public string operationMode { set; get; }
        public float progressBar { set; get; }
        public string curEvent { set; get; }
    }
    class ABBPosition
    {
        public string Rax_1 { set; get; }
        public string Rax_2 { set; get; }
        public string Rax_3 { set; get; }
        public string Rax_4 { set; get; }
        public string Rax_5 { set; get; }
        public string Rax_6 { set; get; }

        public static implicit operator RobJoint(ABBPosition v)
        {
            throw new NotImplementedException();
        }
    }
}
