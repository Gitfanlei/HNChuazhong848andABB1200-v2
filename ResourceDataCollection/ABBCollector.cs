using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABB.Robotics.Controllers;
using ABB.Robotics.Controllers.Discovery;
using ABB.Robotics.Controllers.RapidDomain;
using ABB.Robotics.Controllers.EventLogDomain;
using System.Configuration;
using System.Timers;

namespace ResourceDataCollection
{
    class ABBCollector
    {
        public Controller ABBController;
        public ControllerInfo ABBControllerInfo;
        public static Timer abbTimer1;

        private bool chooseSocket = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("chooseSocket"));

        private static readonly int abbCollecttime = Convert.ToInt32(ConfigurationManager.AppSettings.Get("collectFrequnencyWithUnitMs"));

        public ABBCollector()
        {
            ABBDiscovery();  // 当前仅扫描一次 可以+timer 设定扫描间隔

            SetTimer1(abbCollecttime);
        }

        private void SetTimer1(int interval)
        {
            abbTimer1 = new Timer(interval);
            abbTimer1.Start();
            abbTimer1.AutoReset = true;
            abbTimer1.Enabled = true;
            abbTimer1.Elapsed += readTextRange;
            abbTimer1.Elapsed += readEventLog;
                        
        }

        private void ABBDiscovery()
        {
            NetworkScanner networkScanner = new NetworkScanner();
            networkScanner.Scan();

            ControllerInfo[] controllerInfos = networkScanner.GetControllers(chooseSocket ? NetworkScannerSearchCriterias.Virtual : NetworkScannerSearchCriterias.Real);

            if (controllerInfos.Length > 0)
            {
                Console.WriteLine($"当前online {controllerInfos.Length} 个ABB机器人");

                ABBControllerInfo = controllerInfos[0];

                ABBController = ControllerFactory.CreateFrom(ABBControllerInfo);  // 利用反射方式创建controller实例

                Console.WriteLine($"Found one ABB.System Name is:{abbSystemName} System ID is:{abbSystemID} System IP is:{abbSystemIP}");
            }
            else
            {
                Console.WriteLine("没有扫描到Online的机器人！");
            }
        }

        // 日志信息读取（报警信息）
        public string WarningText;
        public void readEventLog(object source, ElapsedEventArgs e)
        {

            EventLogCategory[] eventLogCategories = ABBController.EventLog.GetCategories();

            foreach (EventLogCategory cats in eventLogCategories)
            {

                foreach (EventLogMessage mesg in cats.Messages)
                {
                    if (mesg.Type.ToString() == "Warning")
                    {
                        WarningText = mesg.Title.ToString() + "-" + mesg.Type.ToString() + "-" + mesg.Timestamp.ToString();
                        Console.Write(WarningText);
                    }
                }
            }
        }

        // 运行进度
        public Int32 lineNum;                                                                       // 行号
        List<Int32> intList = new List<Int32>();
        public Int32[] lineNumList;
        public float MinLineNum;                                                                    // 最小行号
        public float MaxLineNum;                                                                    // 最大行号
        public float progressBar=0;                                                                   // 进度条

        // MotionPointer / ProgramPointer
        public void readTextRange(object source, ElapsedEventArgs e)
        {
            
            ABB.Robotics.Controllers.RapidDomain.Task[] tasks = ABBController.Rapid.GetTasks();     // 获取当前任务 ****

            foreach (ABB.Robotics.Controllers.RapidDomain.Task task in tasks)
            {
                task.Motion.ToString();                                                             // 验证是否获取了当前运动   ture 则获取成功*****
                task.Cycle.ToString();                                                              // 获取当前 程序的执行次数
                task.ExecutionStatus.ToString();                                                    // 获取当前程序的执行状态 ***** run or stop

                //MessageBox.Show("");

                ProgramPosition motion = task.MotionPointer;                                        // 获取当前运动指针 ******
                lineNum = motion.Range.Begin.Row;                                                   // 获取当前指针所在代码行的位置

                // 构建代码行的 动态列表
                if (lineNum != 0)
                {
                    intList.Add(lineNum);
                    int[] strArray = intList.ToArray();
                }

                // 获取Main 代码行数
                MinLineNum = intList.Min();
                MaxLineNum = intList.Max();

                // 动态计算当前代码运行进度
                if (MaxLineNum != MinLineNum)
                {
                    progressBar = ((lineNum - MinLineNum) / (MaxLineNum - MinLineNum)) * 100;

                    //Console.WriteLine($"{lineNum},{MinLineNum},{MaxLineNum}");
                    Console.WriteLine(lineNum);

                    return;

                }
                else if (progressBar == 100.00)
                {
                    progressBar = 0;
                    return;
                }
                else if (MinLineNum == 0)
                {
                    progressBar = 0;
                    return;
                }

                //Console.WriteLine($"{progressBar}");
                // 当前运动指针的相关参数    补充
                motion.Range.End.Row.ToString();
                motion.Routine.ToString();
                motion.Module.ToString();
                // MessageBox.Show(motion.Range.Begin.Row.ToString());

                // 当前程序指针相关参数  与 运动指针类似  参考
                ProgramPosition programPosition = task.ProgramPointer;                              // 获取程序指针当前的位置 *****
                programPosition.Module.ToString();                                                  // 获取当前运行的模块  ****
                programPosition.Routine.ToString();                                                 // 获取当前程序事务(程序)的位置  ****
                programPosition.Range.End.Column.ToString();                                        // 当前文本范围  列 与 行
                programPosition.Range.Begin.Row.ToString();                                         // 获取当前 程序开始的行
                programPosition.Range.End.Row.ToString();                                           // 获取当前 程序结束的行
            }
        }

        // 相关私有变量的获取
        public string abbreadCurEvent
        {
            get
            {
                return WarningText;
            }
        }

        public float abbProgressBar
        {
            get
            {
                return progressBar;
            }
        }

        public string abbSystemName
        {
            get
            {
                return ABBController.SystemName.ToString();

            }
        }
        public string abbSystemID
        {
            get
            {
                return ABBController.SystemId.ToString();
            }
        }

        public string abbSystemIP
        {
            get
            {
                return ABBController.IPAddress.ToString();
            }
        }

        public string abbSystemDateTime
        {
            get
            {
                return ABBController.DateTime.ToString();
            }
        }

        public RobJoint abbPositionInfo
        {
            get
            {
                return ABBController.MotionSystem.MechanicalUnits[0].GetPosition().RobAx;  // 轴位置
            }
        }

        public string abbAxisNum
        {
            get
            {
                return ABBController.MotionSystem.MechanicalUnits[0].NumberOfAxes.ToString();  // 轴数量
            }
        }

        public int abbSpeedRatio
        {
            get
            {
                return ABBController.MotionSystem.SpeedRatio;   // 速度比

            }
        }

        public string abbRapidValue
        {
            get
            {
                return ABBController.Rapid.ToString(); // RAPID任务 数据

            }
        }

        public string abbrunLevel
        {
            get
            {
                return ABBController.RunLevel.ToString();  // 运行级别
            }
        }

        public string abbCurUser
        {
            get
            {
                return UserInfo.DefaultUser.Name;  //获取用户名
            }
        }



        public float abbTemperature
        {
            get
            {
                return ABBController.MainComputerServiceInfo.Temperature; // 主电脑服务器 温度
            }
        }

        public string abbControllerName
        {
            get
            {
                return ABBController.Name;  // 控制器名称
            }
        }

        public string abbOperationMode
        {
            get
            {
                return ABBController.OperatingMode.ToString();   // 当前操作模式           
            }
        }
    }
}
