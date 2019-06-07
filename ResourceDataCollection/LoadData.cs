using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceDataCollection
{
    // 
    class LoadData
    {
        public string ABB_DATA { set; get; }
        public string HNC_DATA { set; get; }
    }

    class HNCLoadData
    {
        public string AxLoad_1 { get; set; }
        public string AxLoad_2 { get; set; }
        public string AxLoad_3 { get; set; }
        public string AxLoad_4 { get; set; }
        public string AxLoad_5 { get; set; }
    }

    class HNCPositionData
    {
        public string Ax_1 { get; set; }
        public string Ax_2 { get; set; }
        public string Ax_3 { get; set; }
        public string Ax_4 { get; set; }
        public string Ax_5 { get; set; }
    }

    class HNCcmdVelData
    {
        public string AxcmdVel_1 { get; set; }
        public string AxcmdVel_2 { get; set; }
        public string AxcmdVel_3 { get; set; }
        public string AxcmdVel_4 { get; set; }
        public string AxcmdVel_5 { get; set; }
    }
    class HNCactVelData
    {
        public string AxactVel_1 { get; set; }
        public string AxactVel_2 { get; set; }
        public string AxactVel_3 { get; set; }
        public string AxactVel_4 { get; set; }
        public string AxactVel_5 { get; set; }
    }
}
