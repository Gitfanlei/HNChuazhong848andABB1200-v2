using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum AlarmType
    {
    	ALARM_SY = 0,//	系统报警（System）
    	ALARM_CH,		//	通道报警（Channel）
    	ALARM_AX,		//	轴报警（Axis）
    	ALARM_SV,		//	伺服报警（Servo）
    	ALARM_PC,		//	PLC报警（PLC）
    	ALARM_DV,		//	设备报警（Dev）
    	ALARM_PS,		//	语法报警（Program Syntax）
    	ALARM_UP,		//	用户PLC报警（User PLC）
    	ALARM_HM,		//	HMI报警（HMI）
    	ALARM_TYPE_ALL
    };

    public enum AlarmLevel
    {
    	ALARM_ERR = 0,//	错误（Error）
    	ALARM_MSG,		//	提示（Message）	
    	ALARM_LEVEL_ALL
    };

    public class HNCALARM
    {
       public const Int32 EHNC_INVAL =  -101  ;//  无效的参数 
       public const Int32 EHNC_FUNC =  -102  ;//  功能无法执行 
       public const Int32 ALARM_TXT_LEN =  64  ;//  报警内容文本长度 
   
    }
}
