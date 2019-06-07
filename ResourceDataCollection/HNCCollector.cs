using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HNCAPI;
using System.Configuration;
using System.Net;

namespace ResourceDataCollection
{
    class HNCCollector
    {
        public HNCPayLoad hncPayload;

        public static Int16 ActiveClientNo = -1;                                                                                // 网络连接号 -1 离线  0 在线
        private UInt16 localPort = 10001;                                                                                       // 本地端口号

        private static readonly string cncIp = ConfigurationManager.AppSettings.Get("hncip");                                   // 配置文件
        private static readonly ushort cncPort = Convert.ToUInt16(ConfigurationManager.AppSettings.Get("hncport"));             // 配置文件

        // 位置 负载电流 变量——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
        public bool State = false;                                                                                              // 机床状态返回值
        public string[] AxisValueInfo = new string[32];
        public string[] AxiscmdVelInfo = new string[32];
        public string[] AxisactVelInfo = new string[32];

        public Int32 AxisNum = -1;
        public string[] axisName = new string[32];   // 传递变量 AxisName
        private const Int32 AxisTypesCount = 9;   // 规定轴的类型数目
        private const Int32 axisDisplayX = 0x00000001; //   unit =  mm  进制
        private Int32[] axisId = new Int32[32];
        public Double[] loadValue = new Double[32];  // 传递变量 LoadValue
        public Int32[] axisValue = new Int32[32];   // 传递变量 axisValue
        public Double[] cmdVel = new Double[32];    // 指令速度
        public Double[] actVel = new Double[32];    // 实际速度

        private Int32 mch = 0;
        public Int32 Ch = 0;

        // 报警 变量——————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
        private const Int32 alarmCount = 0;
        public string errText = "0";
        public Int32 alarmNum = 0;
        public Int32 alarmNo = 0;
        public Int32 indexNo = 0;
        public string posAbnormalRecords = "";
        public string loadAbnormalRecords = "";
        public string alarmInfo = "";

        AlarmHisData[] historyData = new AlarmHisData[HncApi.ALARM_HISTORY_MAX_NUM]; // 获取的历史故障信息最大154个
        Int32 hisAlarmNum = 0;  // 历史报警数量
        Int32 count = 1; // // 设定 需要获取的历史报警数据的数量     

        public HNCCollector()
        {
            IpInitialize();
            Connect_CNC();
            ShowMsgBox(Connect_CNC(), "机床 连接");
        }

        private void IpInitialize()
        {
            ShowMsgBox(HncApi.HNC_NetInit(GetLocalIpAddr(), localPort) == 0 ? true : false, " 本地IP初始化 ");
        }

        // 获取上位机 IP 验证IP 有效性
        public static string GetLocalIpAddr()
        {
            string hostName = Dns.GetHostName();
            IPHostEntry localHost = Dns.GetHostEntry(hostName);
            IPAddress localIpAddr = null;
            foreach (IPAddress ip in localHost.AddressList)
            {
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && ip.ToString().StartsWith("192.168"))
                {
                    localIpAddr = ip;
                    break;
                }
            }
            return localIpAddr.ToString();
        }

        // 机床连接
        public bool Connect_CNC()
        {
            ActiveClientNo = HNCAPI.HncApi.HNC_NetConnect(cncIp, cncPort);
            if ((ActiveClientNo < 0) || (ActiveClientNo > 255)) return false;
            else
            {
                State = true;
                return true;
            }
        }

        // 轴 负载 位置————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
        public void GetAxisInfo()
        {
            // mch 值
            Int32 ret = HncApi.HNC_SystemGetValue((Int32)HncSystem.HNC_SYS_ACTIVE_CHAN, ref mch, ActiveClientNo);
            //MessageBox.Show(Convert.ToString(ret));  // 信息类别查看  ret=0

            UInt32 AxisMask = AcquireAxisType(mch, ref AxisNum, ActiveClientNo);
            AcquireAxisType((Int16)Ch, ref AxisNum, ActiveClientNo);
            AxisInfoGotItems(AxisMask, ref axisName, ref axisId, ref axisValue, ref loadValue, ref cmdVel, ref actVel, ActiveClientNo);  // ref 通过引用传递参数获取值   

            //GetAlarmInfo(ref alarmNum, ref alarmNo, ref errText, ActiveClientNo);
            GetAxisStrInfo(AxisNum, axisId, axisValue, cmdVel, ref AxisValueInfo, ref AxiscmdVelInfo);                     // AxisValueinfo  是单位换算之后的值  AxisInfoGotItems()  GetAxisStrInfo()  这两个方法中的值 都需要 另外声明一个空变量，来获取这个函数方法取得的值

            // kafka 数据输出点 
            hncPayload = new HNCPayLoad
            {
                ID = "1",
                IP = $"{cncIp}:{cncPort}",
                Name = "HNC1",
                TimeStamp = DateTime.Now,
                LoadDataInfo = new HNCLoadData
                {
                    AxLoad_1 = $"{axisName[0]}:{loadValue[0].ToString("f6")}",
                    AxLoad_2 = $"{axisName[1]}:{loadValue[1].ToString("f6")}",
                    AxLoad_3 = $"{axisName[2]}:{loadValue[2].ToString("f6")}",
                    AxLoad_4 = $"{axisName[3]}:{loadValue[3].ToString("f6")}",
                    AxLoad_5 = $"{axisName[4]}:{loadValue[4].ToString("f6")}"
                },
                PositionInfo = new HNCPositionData
                {
                    Ax_1 = $"{axisName[0]}:{AxisValueInfo[0]}",
                    Ax_2 = $"{axisName[1]}:{AxisValueInfo[1]}",
                    Ax_3 = $"{axisName[2]}:{AxisValueInfo[2]}",
                    Ax_4 = $"{axisName[3]}:{AxisValueInfo[3]}",
                    Ax_5 = $"{axisName[4]}:{AxisValueInfo[4]}"
                },
                HNCactVelData = new HNCactVelData
                {
                    AxactVel_1 = $"{actVel[0]}",
                    AxactVel_2 = $"{actVel[1]}",
                    AxactVel_3 = $"{actVel[2]}",
                    AxactVel_4 = $"{actVel[3]}",
                    AxactVel_5 = $"{actVel[4]}"
                },
                HNCcmdVelData = new HNCcmdVelData
                {
                    AxcmdVel_1 = $"{cmdVel[0]}",
                    AxcmdVel_2 = $"{cmdVel[1]}",
                    AxcmdVel_3 = $"{cmdVel[2]}",
                    AxcmdVel_4 = $"{cmdVel[3]}",
                    AxcmdVel_5 = $"{cmdVel[4]}"
                },
                posabnormalInfo = posAbnormalRecords,
                loadabnormalInfo = loadAbnormalRecords,
                AlarmNum = alarmNum,
                AlarmInfo = alarmInfo

            };

            // 异常信息捕获
            foreach (var pos in AxisValueInfo)
            {
                Int32 position = Convert.ToInt32(pos);

                if ((100 <= position) && (position < 120))
                {
                    posAbnormalRecords += DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                    posAbnormalRecords += "二级警告!" + "当前位置" + position;
                    Console.WriteLine(posAbnormalRecords);
                }
                else if (position >= 120)
                {
                    posAbnormalRecords += DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                    posAbnormalRecords += "一级警告!" + "当前位置" + position;
                    Console.WriteLine(posAbnormalRecords);
                    //ret = HncApi.HNC_SysCtrlStopProg((Int16)Ch, HNCCollector.ActiveClientNo);
                    //if (ret != 0)
                    //{
                    //    Console.WriteLine("Can not execute the order to stop the curNC code!");
                    //}
                    //else
                    //{
                    //    Console.WriteLine("First level warning ! Will be colliding, the NC process is stopped!");
                    //}
                    //break;
                }

                foreach (var load in loadValue)
                {
                    if (load > 0.4)
                    {
                        loadAbnormalRecords += DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString();
                        loadAbnormalRecords += "负载过高警告！ 当前负载" + load;
                        Console.WriteLine(loadAbnormalRecords);
                        //ret = HncApi.HNC_SysCtrlStopProg((Int16)Ch, HNCCollector.ActiveClientNo);
                        //if (ret != 0)
                        //{
                        //    Console.WriteLine("Can not execute the order to stop the curNC code!");
                        //}
                        //else
                        //{
                        //    Console.WriteLine("First level warning ! Will be colliding, the NC process is stopped!");
                        //}
                        //break;
                    }
                }
            }
        }

        // 轴的类型的获取  类型-通道号（ch）-索引号- 掩码（mask 实际上应该表示 value）-clientNo 网络连接号
        public UInt32 AcquireAxisType(Int32 ch, ref Int32 axisnum, Int16 clientNo)
        {
            Int32 mask = 0;  //
            HncApi.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_AXES_MASK, ch, 0, ref mask, clientNo); // 类型 - 通道号 - 索引号 - 掩码 - 连接号
            int num = 0;
            for (Int32 i = 0; i < AxisTypesCount; i++)
            {
                if (((axisDisplayX << i) & mask) != 0)
                    num++;
            }
            axisnum = num;
            return (UInt32)mask;
        }

        // 获取轴信息的变量类型  布尔型
        public bool AxisInfoGotItems(UInt32 AxisMask, ref string[] AxisName, ref Int32[] AxisId, ref Int32[] axisValue, ref Double[] loadValue, ref Double[] cmdVel, ref Double[] actVel, Int16 clientNo)
        {
            bool flag = true;
            Int32 index = 0;
            Int32 ret = 0;  // 引用变量
            if (AxisMask == 0)
            {
                flag = false;
                return flag;
            }

            for (Int32 i = 0; i < AxisTypesCount; i++)
            {
                if ((AxisMask >> i & 1) == 1)  // 位与运算   i 与 1 都是 1 时 返回1   >> 右移 符号（转换为2进制向右移动第二个操作数的位数）
                {
                    ret += HncApi.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_NAME, i, ref AxisName[index], clientNo);  // 类型 轴号 轴值 网络号      // 轴名                 
                    AxisId[index] = i;
                    ret += HncApi.HNC_AxisGetValue((Int32)HncAxis.HNC_AXIS_ACT_POS, AxisId[index], ref axisValue[index], ActiveClientNo);       // 位置信息   是米度单位    
                    ret += HncApi.HNC_AxisGetValue((Int32)HncAxis.HNC_AXIS_LOAD_CUR, AxisId[index], ref loadValue[index], ActiveClientNo);      // 负载电流
                    ret += HncApi.HNC_AxisGetValue((Int32)HncAxis.HNC_AXIS_CMD_VEL, AxisId[index], ref cmdVel[index], ActiveClientNo);          // 指令速度   是米度单位    
                    ret += HncApi.HNC_AxisGetValue((Int32)HncAxis.HNC_AXIS_ACT_VEL, AxisId[index], ref actVel[index], ActiveClientNo);          // 实际速度   是毫米每分钟 不用换算

                    index++;
                    if (ret != 0)
                    {
                        flag = false;
                        break;
                    }
                }

            }
            return flag;
        }

        // 位置单位转换
        private void GetAxisStrInfo(int count, Int32[] AxisId, Int32[] axisValue, Double[] cmdVel, ref string[] AxisValueInfo, ref string[] AxiscmdVelInfo)   // 前三个是输入量，由方法 AxisInfoGotItems（）获取的变量作为输入；ref后面的变量 是引用的传递变量
        {
            Int32 ret = 0;
            Int32 lax = 0;
            Int32 axistype = 0;
            Int32 metric = 0;
            Int32 unit = 100000;
            Int32 diameter = 0;
            for (int i = 0; i < count; i++)
            {
                ret = HncApi.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_LAX, mch, AxisId[i], ref lax, ActiveClientNo);
                // 直半径处理
                ret = HncApi.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_DIAMETER, mch, 0, ref diameter, ActiveClientNo);
                if (0 == lax && 1 == diameter)
                {
                    axisValue[i] *= 2;
                }
                ret = HncApi.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_TYPE, lax, ref axistype, ActiveClientNo);

                // 判断轴的类型 获取响应的单位换算参数
                if (axistype == 1)
                {
                    ret = HncApi.HNC_SystemGetValue((int)HncSystem.HNC_SYS_MOVE_UNIT, ref unit, ActiveClientNo);    // 直线轴 长度分辨率
                    ret = HncApi.HNC_SystemGetValue((int)HncSystem.HNC_SYS_METRIC_DISP, ref metric, ActiveClientNo); // 公英制
                    if (0 == metric) // 英制
                        unit = (Int32)(unit * 25.4);
                }
                else
                {
                    ret = HncApi.HNC_SystemGetValue((int)HncSystem.HNC_SYS_TURN_UNIT, ref unit, ActiveClientNo);  // 旋转轴 角度分辨率
                }

                if (Math.Abs(unit) - 0.00001 <= 0.0) //除零保护
                {
                    AxisValueInfo[i] = "0";
                    AxiscmdVelInfo[i] = "0";  // 米度单位换算为 毫米每分钟
                }
                else
                {
                    AxisValueInfo[i] = ((double)axisValue[i] / unit).ToString("F4"); // 实际输出的轴的位置   单位换算
                    AxiscmdVelInfo[i] = (cmdVel[i] / unit).ToString("F4");
                }

                if (axistype == 1)
                {
                    if (0 == metric) // 英制
                    {
                        AxisValueInfo[i] += " inch";
                    }
                    else
                    {
                        AxisValueInfo[i] += " mm";
                    }
                }
                else
                {
                    AxisValueInfo[i] += " D";
                }
            }
        }

        // 报警信息模块————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
        //刷新 
        private void RefreshAlarm()
        {
            Int32 ret = HncApi.HNC_AlarmRefresh(ActiveClientNo);
        }
        
        // 测试获取当前警报信息
        public void GetAlarmInfo()
        {
            Int32 index = 0;
            string errTxt = string.Empty;
            Int32 ret = 0;

            ret = HncApi.HNC_AlarmGetNum((Int32)AlarmType.ALARM_TYPE_ALL, (Int32)AlarmLevel.ALARM_LEVEL_ALL, ref alarmNum, ActiveClientNo);
            if (ret != 0)
            {
                Console.WriteLine("获取警报失败");
            }
            else
            {
                Console.WriteLine($"当前警报数量为{alarmNum}");
            }

            for (int i = 0; i < alarmNum; i++)
            {
                ret = HncApi.HNC_AlarmGetData((int)AlarmType.ALARM_TYPE_ALL, (int)AlarmLevel.ALARM_ERR, index, ref alarmNo, ref errTxt, ActiveClientNo);
                if (ret == 0)
                {
                    alarmInfo = index.ToString() + alarmNo.ToString() + errTxt + "\n";
                    Console.WriteLine(alarmInfo);
                }

                else
                {
                    Console.WriteLine(" 无法读取警报信息!");
                }
            }

        }

        // ————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————————
        // 消息盒子
        private void ShowMsgBox(bool flag, string msg)
        {
            if (flag)
            {
                Console.WriteLine($"{msg} 成功！");
            }
            else
            {
                Console.WriteLine($"{msg} 失败！");
            }

        }
    }
}
