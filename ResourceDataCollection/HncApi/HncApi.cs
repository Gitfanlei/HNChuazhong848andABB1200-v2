using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace HNCAPI
{

    [StructLayout(LayoutKind.Explicit, Size = 8, CharSet = CharSet.Ansi, Pack = 4)]
    public struct SHncData
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.I4)]
        public Int32 i;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.R8)]
        public Double f;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 n;	// 变量偏移地址
    }
    // 系统用全局变量、表达式运算的联合数据类型
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct SDataUnion
    {
        public Byte type;
        public Byte g90;
        [MarshalAs(UnmanagedType.Struct, SizeConst = 1)]
        public SHncData v;
    }

    // 参数值
    [StructLayout(LayoutKind.Explicit, Size = 8, CharSet = CharSet.Ansi, Pack = 4)]
    public struct SParamValue
    {
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public SByte[] s;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct nctime_t
    {
        public Int32 second;	// seconds - [0,59]
        public Int32 minute;	// minutes - [0,59]
        public Int32 hour;	// hours   - [0,23]
        public Int32 hsecond; /* hundredths of seconds */
        public Int32 day;	// [1,31]
        public Int32 month;	// [0,11] (January = 0)
        public Int32 year;	// (current year minus 1900)
        public Int32 wday;	// Day of week, [0,6] (Sunday = 0)
    }
    // 报警历史记录数据
    [StructLayout(LayoutKind.Sequential, Size = 132, CharSet = CharSet.Ansi, Pack = 4)]
    public struct AlarmHisData
    {
        [MarshalAs(UnmanagedType.I4)]
        public Int32 alarmNo;
        [MarshalAs(UnmanagedType.Struct)]
        public nctime_t timeBegin;
        [MarshalAs(UnmanagedType.Struct)]
        public nctime_t timeEnd;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public String text;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct SEventElement
    {
        [MarshalAs(UnmanagedType.I2)]
        public Int16 src;// 事件来源
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 code;// 事件代码
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public Byte[] buf;
    }

    // 文件结构
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct nc_finfo_t
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
        public SByte[] reserved;
        [MarshalAs(UnmanagedType.U4)]
        public UInt32 attrib;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 wr_time;
        [MarshalAs(UnmanagedType.U2)]
        public UInt16 wr_date;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 size;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 48)]
        public String name;
    }
    // 文件查找结构
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    public struct ncfind_t
    {
        [MarshalAs(UnmanagedType.Struct, SizeConst = 1)]
        public nc_finfo_t info;
        [MarshalAs(UnmanagedType.I4)]
        public Int32 handle;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 18)]
        public String time;
    }

    public class HncApi
    {
        private const Int32 byteSize = 1;
        private const Int32 shortSize = 2;
        private const Int32 intSize = 4;
        private const Int32 doubleSize = 8;
        private const Int32 ipSize = 100;
        private const Int32 PAR_PROP_DATA_LEN = 68;//sizeof(SDataProperty)
        public  const Int32 MAX_FILE_NUM_PER_DIR = 128;
        public  const Int32 ALARM_HISTORY_MAX_NUM = 154; //网络消息可存储最大报警历史数

        //NET
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int16 HNC_NetInit(String ip, UInt16 port);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetExit", CallingConvention = CallingConvention.Cdecl)]
        public static extern void HNC_NetExit();

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetIsThreadStartup", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int16 HNC_NetIsThreadStartup();

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetSetIpaddr", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_NetSetIpaddr(String ip, UInt16 port, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetGetIpaddr", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_NetGetIpaddr(IntPtr ip, ref UInt16 port, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetAddIpaddr", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_NetAddIpaddr(String ip, UInt16 port, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetDelIpaddr", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_NetDelIpaddr(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetGetClientNo", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_NetGetClientNo(String ip, UInt16 port, ref Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetFileSend", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_NetFileSend(String localNamme, String dstName, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetFileGet", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_NetFileGet(String localNamme, String dstName, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetFileGetDirInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_NetFileGetDirInfo(String dirname, IntPtr info, ref UInt16 num, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetConnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int16 HNC_NetConnect(String ip, UInt16 port);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_NetIsConnect", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt16 HNC_NetIsConnect(Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_NetFileCheck", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_NetFileCheck(String localNamme, String dstName, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_NetFileRemove", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_NetFileRemove(String dstName, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_NetGetDllVer", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_NetGetDllVer(IntPtr dllVer);

	 	[DllImport("HncNetDll.dll", EntryPoint = "HNC_NetGetSDKVer", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_NetGetSDKVer(IntPtr dllPlusVer);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_NetGetPeriod", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_NetGetPeriod(Int32 type, ref Int32 value);

	 	[DllImport("HncNetDll.dll", EntryPoint = "HNC_NetSetPeriod", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_NetSetPeriod(Int32 type,Int32 value);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_NetMakeDir", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_NetMakeDir(String dir, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_NetRemoveDir", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_NetRemoveDir(String dir, Int16 clientNo);
        //寄存器
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_RegGetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_RegGetValue(Int32 type, Int32 index, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_RegSetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_RegSetValue(Int32 type, Int32 index, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_RegSetBit", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_RegSetBit(Int32 type, Int32 index, Int32 bit, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_RegClrBit", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_RegClrBit(Int32 type, Int32 index, Int32 bit, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_RegGetNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_RegGetNum(Int32 type, ref Int32 num, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_RegGetFGBase", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_RegGetFGBase(Int32 baseType, ref Int32 value, Int16 clientNo);

        //变量
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_VarGetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_VarGetValue(Int32 type, Int32 no, Int32 index, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_VarSetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_VarSetValue(Int32 type, Int32 no, Int32 index, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_VarSetBit", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_VarSetBit(Int32 type, Int32 no, Int32 index, Int32 bit, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_VarClrBit", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_VarClrBit(Int32 type, Int32 no, Int32 index, Int32 bit, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_MacroVarGetValue", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_MacroVarGetValue(Int32 no, ref SDataUnion var, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_MacroVarSetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_MacroVarSetValue(Int32 no, IntPtr index, Int16 clientNo);

        //参数
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanLoad", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanLoad(String lpFileName, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanSave", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanSave(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanSaveAs", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanSaveAs(String lpFileName, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanGetParaPropEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, IntPtr propValue, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanSetParaPropEx", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, IntPtr propValue, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanGetParaProp", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, IntPtr prop_value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanSetParaProp", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, IntPtr prop_value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanGetFileName", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ParamanGetFileName(Int32 fileNo, IntPtr buf, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanGetSubClassProp", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ParamanGetSubClassProp(Int32 fileNo, Byte propType, IntPtr propValue, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanGetTotalRowNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanGetTotalRowNum(ref Int32 rowNum, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanTransRow2Index", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanTransRow2Index(Int32 fileNo, Int32 subNo, Int32 rowNo, ref Int32 index, ref Int16 dupNum, ref Int16 dupNo, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanTransRowx2Row", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanTransRowx2Row(Int32 rowx, ref Int32 fileNo, ref Int32 subNo, ref Int32 row, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanTransId2Rowx", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanTransId2Rowx(Int32 parmId, ref Int32 rowx, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanRewriteSubClass", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanRewriteSubClass(Int32 fileNo, Int32 subNo, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanSaveStrFile", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanSaveStrFile(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanGetI32", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanGetI32(Int32 fileNo, Int32 subNo, Int32 index, ref Int32 value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanSetI32", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanSetI32(Int32 fileNo, Int32 subNo, Int32 index, Int32 value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanGetFloat", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanGetFloat(Int32 fileNo, Int32 subNo, Int32 index, ref Double value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanSetFloat", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanSetFloat(Int32 fileNo, Int32 subNo, Int32 index, Double value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanGetStr", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ParamanGetStr(Int32 fileNo, Int32 subNo, Int32 index, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanSetStr", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ParamanSetStr(Int32 fileNo, Int32 subNo, Int32 index, String value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanGetItem", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ParamanSetItem", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, IntPtr value, Int16 clientNo);

        //系统
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SystemGetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_SystemGetValue(Int32 type, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SystemSetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_SystemSetValue(Int32 type, IntPtr value, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SystemGetUserRealTimeData", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_SystemGetUserRealTimeData(Byte[] info,Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SystemSetUserRealTimeData", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_SystemSetUserRealTimeData(Byte[] info, Int16 clientNo);

        //通道、轴
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ChannelGetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ChannelSetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ChannelSetValue(Int32 type, Int32 ch, Int32 index, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_AxisGetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_AxisGetValue(Int32 type, Int32 ax, IntPtr value, Int16 clientNo);

        //坐标系
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_CrdsGetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_CrdsGetValue(Int32 type, Int32 ax, IntPtr value, Int32 ch, Int32 crds, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_CrdsSetValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_CrdsSetValue(Int32 type, Int32 ax, IntPtr value, Int32 ch, Int32 crds, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_CrdsGetMaxNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_CrdsGetMaxNum(Int32 type, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_CrdsLoad", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_CrdsLoad(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_CrdsSave", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_CrdsSave(Int16 clientNo);

        //刀具、刀库
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolMagSave", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolMagSave(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolGetMaxMagNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolGetMaxMagNum(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolGetMagHeadBase", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolGetMagHeadBase(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolGetPotDataBase", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolGetPotDataBase(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolGetMagBase", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolGetMagBase(Int32 magNo, Int32 index, ref Int32 value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolSetMagBase", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolSetMagBase(Int32 magNo, Int32 index, Int32 value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolMagGetToolNo", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolMagGetToolNo(Int32 magNo, Int32 potNo, ref Int32 toolNo, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolMagSetToolNo", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolMagSetToolNo(Int32 magNo, Int32 potNo, Int32 toolNo, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolGetPotAttri", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolGetPotAttri(Int32 magNo, Int32 potNo, ref Int32 potAttri, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolSetPotAttri", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolSetPotAttri(Int32 magNo, Int32 potNo, Int32 potAttri, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolLoad", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolLoad(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolSave", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolSave(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolGetMaxToolNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_ToolGetMaxToolNum(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolGetToolPara", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ToolGetToolPara(Int32 toolNo, Int32 index, IntPtr value, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ToolSetToolPara", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_ToolSetToolPara(Int32 toolNo, Int32 index, IntPtr value, Int16 clientNo);

        //采样
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplGetPeriod", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplGetPeriod(Int32 ch, ref Int32 tick, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplSetPeriod", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplSetPeriod(Int32 ch, Int32 tick, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplGetLmt", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplGetLmt(Int32 ch, ref Int32 type, ref Int32 n, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplSetLmt", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplSetLmt(Int32 ch, Int32 type, Int32 n, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplGetChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplGetChannel(ref Int32 chnNum, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplSetChannel", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplSetChannel(Int32 chnNum, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplGetPropertyType", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplGetPropertyType(Int32 ch, ref Int16 type, ref Int16 axisNo, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplSetPropertyType", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplSetPropertyType(Int32 ch, Int32 type, Int32 axisNo, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplGetRegType", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplGetRegType(Int32 ch, ref Int32 type, ref Int32 offset, ref Int32 dataLen, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplSetRegType", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplSetRegType(Int32 ch, Int32 type, Int32 offset, Int32 dataLen, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplReset", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplReset(Int32 ch, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplTriggerOn", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplTriggerOn(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplTriggerOff", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplTriggerOff(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplGetNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplGetNum(Int32 ch, ref Int32 num, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplSetNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplSetNum(Int32 ch, Int32 num, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplGetData", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_SamplGetData(Int32 ch, ref Int32 num, IntPtr data, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SamplGetStat", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SamplGetStat(Int16 clientNo);

        //告警
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_AlarmGetNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_AlarmGetNum(Int32 type, Int32 level, ref Int32 num, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_AlarmGetData", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_AlarmGetData(Int32 type, Int32 level, Int32 index, ref Int32 alarmNo, IntPtr alarmText, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_AlarmGetHistoryNum", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_AlarmGetHistoryNum(ref Int32 num, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_AlarmGetHistoryData", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_AlarmGetHistoryData(Int32 index, ref Int32 count, IntPtr data, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_AlarmRefresh", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_AlarmRefresh(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_AlarmSaveHistory", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_AlarmSaveHistory(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_AlarmClrHistory", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_AlarmClrHistory(Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_AlarmClr", CallingConvention = CallingConvention.Cdecl)]
    	public static extern Int32 HNC_AlarmClr(Int32 type, Int32 level,Int16 clientNo);

        //G代码
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_FprogGetFullName", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_FprogGetFullName(Int32 ch, IntPtr progName, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_FprogRandomInit", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_FprogRandomInit(Int32 ch, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_FprogRandomLoad", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_FprogRandomLoad(Int32 line, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_FprogRandomWriteback", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_FprogRandomWriteback(Int32 line, Byte flag, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlSkipToRow", CallingConvention = CallingConvention.Cdecl)]
    	public static extern Int32 HNC_SysCtrlSkipToRow(Int32 ch, Int32 row, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_FprogRandomExit", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_FprogRandomExit(Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlSelectProg", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SysCtrlSelectProg(Int32 ch, String name, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlLoadProg", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SysCtrlLoadProg(Int32 ch, String name, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlResetProg", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SysCtrlResetProg(Int32 ch, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlStopProg", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SysCtrlStopProg(Int32 ch, Int16 clientNo);

        //升级、备份
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SysBackup", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SysBackup(Int32 flag, String PathName, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_SysUpdate", CallingConvention = CallingConvention.Cdecl)]
        public static extern Int32 HNC_SysUpdate(Int32 flag, String PathName, Int16 clientNo);

        //事件
        [DllImport("HncNetDll.dll", EntryPoint = "HNC_EventPut", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_EventPut(IntPtr ev, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_EventGetSysEv", CallingConvention = CallingConvention.Cdecl)]
        private static extern Int32 HNC_EventGetSysEv(IntPtr ev);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_NetDiskMount", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_NetDiskMount(String ip, String progAddr, String name, String pass, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ActivationGetExpDate", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_ActivationGetExpDate(ref Int32 flag, Int32[] pDate, Int16 clientNo);

        [DllImport("HncNetDll.dll", EntryPoint = "HNC_ActivationGetLastday", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_ActivationGetLastday(ref Int32 flag, ref Int32 day, Int16 clientNo);

        //MDI
		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlMdiTry", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_SysCtrlMdiTry(Int32 ch, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlMdiReq", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_SysCtrlMdiReq(Int32 ch, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlMdiOpen", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_SysCtrlMdiOpen(Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlMdiClear", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_SysCtrlMdiClear(Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlMdiStop", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_SysCtrlMdiStop(Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlMdiClose", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_SysCtrlMdiClose(Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_FprogMdiConfirm", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_FprogMdiConfirm(Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_FprogMdiSetContent", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_FprogMdiSetContent(String txt, Int32 txtLen, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlMdiGetBlk", CallingConvention = CallingConvention.Cdecl)]
		private static extern Int16 HNC_SysCtrlMdiGetBlk(IntPtr pos , IntPtr msft, IntPtr ijkr, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_SysCtrlMdiUpdate", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_SysCtrlMdiUpdate(Int32 rowNum, Int16 clientNo);

//verify
		[DllImport("HncNetDll.dll", EntryPoint = "HNC_VerifyGetCurveType", CallingConvention = CallingConvention.Cdecl)]
    	public static extern Int32  HNC_VerifyGetCurveType(Int32 ch, ref Int32 curtype, Int16 clientNo);

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_VerifyGetCurveSpos", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_VerifyGetCurveSpos(Int32 ch,Int32 ax, ref Int32 spos, Int16 clientNo );

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_VerifyGetCurveEpos", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_VerifyGetCurveEpos(Int32 ch,Int32 ax, ref Int32 epos, Int16 clientNo );

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_VerifyGetLinePos", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_VerifyGetLinePos(Int32 ch, Int32[] pos, ref Int32 flag, Int16 clientNo );

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_VerifyClearCurve", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_VerifyClearCurve(Int32 ch, Int16 clientNo );

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_VerifyGetCurvePoint", CallingConvention = CallingConvention.Cdecl)]
		public static extern  Int32 HNC_VerifyGetCurvePoint(Int32 ch, Int32[] pos, ref Int32 vflag, Int16 clientNo );

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_VerifyCalcuCyclePara", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_VerifyCalcuCyclePara(Int32 ch, ref Byte vflag, Int16 clientNo );

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_VerifySetChCmdPos", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_VerifySetChCmdPos(Int32 ch, Int32 ax, Int32 pos, Int16 clientNo );

	    [DllImport("HncNetDll.dll", EntryPoint = "HNC_VerifyGetCmdPos", CallingConvention = CallingConvention.Cdecl)]
		public static extern Int32 HNC_VerifyGetCmdPos(Int32 ax, ref Int32 pos, Int16 clientNo );

		[DllImport("HncNetDll.dll", EntryPoint = "HNC_FprogGetProgPathByIdx", CallingConvention = CallingConvention.Cdecl)]
		private static extern Int32 HNC_FprogGetProgPathByIdx(Int32 pindex, IntPtr progName, Int16  clientNo);
		//net api
		public static  Int32 HNC_NetGetDllVer(ref String dllVer)
		{
		    Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCNET.VERSION_LEN);
            ret = HNC_NetGetDllVer(ptr);
            dllVer = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
			return ret;
        }

		public static  Int32 HNC_NetGetSDKVer(ref String dllPlusVer)
		{
		    Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCNET.VERSION_LEN);
            ret = HNC_NetGetSDKVer(ptr);
            dllPlusVer = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
			return ret;
        }

        //重载函数
        public static Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Byte value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(byteSize);
            ret = HNC_RegGetValue(type, index, ptr, clientNo);
            value = Marshal.ReadByte(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Int16 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(shortSize);
            ret = HNC_RegGetValue(type, index, ptr, clientNo);
            value = Marshal.ReadInt16(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            ret = HNC_RegGetValue(type, index, ptr, clientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_RegSetValue(Int32 type, Int32 index, Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HNC_RegSetValue(type, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_RegSetValue(Int32 type, Int32 index, Int16 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(shortSize);
            Marshal.WriteInt16(ptr, value);
            ret = HNC_RegSetValue(type, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_RegSetValue(Int32 type, Int32 index, Byte value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(byteSize);
            Marshal.WriteByte(ptr, value);
            ret = HNC_RegSetValue(type, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, ref String value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(doubleSize);
            ret = HNC_ChannelGetValue(type, ch, index, ptr, clientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, ref Double value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(doubleSize);
            ret = HNC_ChannelGetValue(type, ch, index, ptr, clientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        public static Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, ref Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            ret = HNC_ChannelGetValue(type, ch, index, ptr, clientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ChannelSetValue(Int32 type, Int32 ch, Int32 index, Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HNC_ChannelSetValue(type, ch, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_AxisGetValue(Int32 type, Int32 ax, ref String value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCAXIS.MOTOR_TYPE_LEN);
            ret = HNC_AxisGetValue(type, ax, ptr, clientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_AxisGetValue(Int32 type, Int32 ax, ref Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            ret = HNC_AxisGetValue(type, ax, ptr, clientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_AxisGetValue(Int32 type, Int32 ax, ref Double value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(doubleSize);
            ret = HNC_AxisGetValue(type, ax, ptr, clientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_CrdsGetValue(Int32 type, Int32 ax, ref Double value, Int32 ch, Int32 crds, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(doubleSize);
            ret = HNC_CrdsGetValue(type, ax, ptr, ch, crds, clientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_CrdsGetValue(Int32 type, Int32 ax, ref Int32 value, Int32 ch, Int32 crds, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            ret = HNC_CrdsGetValue(type, ax, ptr, ch, crds, clientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_CrdsSetValue(Int32 type, Int32 ax, Double value, Int32 ch, Int32 crds, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(doubleSize);
            Marshal.StructureToPtr(value, ptr, true);
            ret = HNC_CrdsSetValue(type, ax, ptr, ch, crds, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_CrdsSetValue(Int32 type, Int32 ax, Int32 value, Int32 ch, Int32 crds, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HNC_CrdsSetValue(type, ax, ptr, ch, crds, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ToolGetToolPara(Int32 toolNo, Int32 index, ref Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            ret = HNC_ToolGetToolPara(toolNo, index, ptr, clientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ToolGetToolPara(Int32 toolNo, Int32 index, ref Double value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(doubleSize);
            ret = HNC_ToolGetToolPara(toolNo, index, ptr, clientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ToolSetToolPara(Int32 toolNo, Int32 index, Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HNC_ToolSetToolPara(toolNo, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ToolSetToolPara(Int32 toolNo, Int32 index, Double value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(doubleSize);
            Marshal.StructureToPtr(value, ptr, true);
            ret = HNC_ToolSetToolPara(toolNo, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_VarGetValue(Int32 type, Int32 no, Int32 index, ref Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            ret = HNC_VarGetValue(type, no, index, ptr, clientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_VarGetValue(Int32 type, Int32 no, Int32 index, ref Double value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(doubleSize);
            ret = HNC_VarGetValue(type, no, index, ptr, clientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_VarSetValue(Int32 type, Int32 no, Int32 index, Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HNC_VarSetValue(type, no, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_VarSetValue(Int32 type, Int32 no, Int32 index, Double value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(doubleSize);
            Marshal.StructureToPtr(value, ptr, true);
            ret = HNC_VarSetValue(type, no, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_SystemGetValue(Int32 type, ref String value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCSYS.MAX_SYS_STR_LEN);
            ret = HNC_SystemGetValue(type, ptr, clientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_SystemGetValue(Int32 type, ref Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            ret = HNC_SystemGetValue(type, ptr, clientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_SystemSetValue(Int32 type, String value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCSYS.MAX_SYS_STR_LEN);
            ptr = Marshal.StringToCoTaskMemAnsi(value);
            ret = HNC_SystemSetValue(type, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_SystemSetValue(Int32 type, Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HNC_SystemSetValue(type, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, ref Int32 propValue, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetParaPropEx(parmId, propType, ptr, clientNo);
            propValue = Marshal.ReadInt32((IntPtr)(ptr.ToInt32() + intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, ref Double propValue, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetParaPropEx(parmId, propType, ptr, clientNo);
            propValue = (Double)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + intSize), typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, SByte[] propValue, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetParaPropEx(parmId, propType, ptr, clientNo);
            Int32 length = propValue.Length < HNCDATATYPE.PARAM_STR_LEN ? propValue.Length : HNCDATATYPE.PARAM_STR_LEN;

            if (ret == 0)
            {
                for (Int32 i = 0; i < length; ++i)
                {
                    propValue[i] = (SByte)Marshal.ReadByte((IntPtr)(ptr.ToInt32() + intSize + i));
                }
            }
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, ref String propValue, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetParaPropEx(parmId, propType, ptr, clientNo);
            propValue = Marshal.PtrToStringAnsi((IntPtr)(ptr.ToInt32() + intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, Int32 propValue, Int16 clientNo)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 1);
            Marshal.WriteInt32((IntPtr)(ptr.ToInt32() + intSize), propValue);
            ret = HNC_ParamanSetParaPropEx(parmId, propType, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, Double propValue, Int16 clientNo)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 2);
            Marshal.StructureToPtr(propValue, (IntPtr)(ptr.ToInt32() + intSize), true);
            ret = HNC_ParamanSetParaPropEx(parmId, propType, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, SByte[] propValue, Int16 clientNo)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 11);
            for (Int32 i = 0; i < propValue.Length; ++i)
            {
                Marshal.WriteByte((IntPtr)(ptr.ToInt32() + intSize + i), (Byte)propValue[i]);
            }

            ret = HNC_ParamanSetParaPropEx(parmId, propType, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, String propValue, Int16 clientNo)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 5);

            Byte[] tempArray = Encoding.Default.GetBytes(propValue);
            Byte[] strArray = new Byte[tempArray.Length + 1];
            tempArray.CopyTo(strArray, 0);
            strArray[strArray.Length - 1] = 0;
            Marshal.Copy(strArray, 0, (IntPtr)(ptr.ToInt32() + intSize), strArray.Length);

            ret = HNC_ParamanSetParaPropEx(parmId, propType, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, ref Int32 prop_value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ptr, clientNo);
            prop_value = Marshal.ReadInt32((IntPtr)(ptr.ToInt32() + intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, ref Double prop_value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ptr, clientNo);
            prop_value = (Double)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + intSize), typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, SByte[] prop_value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ptr, clientNo);
            Int32 length = prop_value.Length < HNCDATATYPE.PARAM_STR_LEN ? prop_value.Length : HNCDATATYPE.PARAM_STR_LEN;

            if (ret == 0)
            {
                for (Int32 i = 0; i < length; ++i)
                {
                    prop_value[i] = (SByte)Marshal.ReadByte((IntPtr)(ptr.ToInt32() + intSize + i));
                }
            }
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, ref String prop_value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ptr, clientNo);
            prop_value = Marshal.PtrToStringAnsi((IntPtr)(ptr.ToInt32() + intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, Int32 prop_value, Int16 clientNo)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 1);
            Marshal.WriteInt32((IntPtr)(ptr.ToInt32() + intSize), prop_value);
            ret = HNC_ParamanSetParaProp(filetype, subid, index, prop_type, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, Double prop_value, Int16 clientNo)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 2);
            Marshal.StructureToPtr(prop_value, (IntPtr)(ptr.ToInt32() + intSize), true);
            ret = HNC_ParamanSetParaProp(filetype, subid, index, prop_type, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, SByte[] prop_value, Int16 clientNo)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 11);
            for (Int32 i = 0; i < prop_value.Length; ++i)
            {
                Marshal.WriteByte((IntPtr)(ptr.ToInt32() + intSize + i), (Byte)prop_value[i]);
            }
            ret = HNC_ParamanSetParaProp(filetype, subid, index, prop_type, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, String prop_value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 5);

            Byte[] tempArray = Encoding.Default.GetBytes(prop_value);
            Byte[] strArray = new Byte[tempArray.Length + 1];
            tempArray.CopyTo(strArray, 0);
            strArray[strArray.Length - 1] = 0;
            Marshal.Copy(strArray, 0, (IntPtr)(ptr.ToInt32() + intSize), strArray.Length);

            ret = HNC_ParamanSetParaProp(filetype, subid, index, prop_type, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetSubClassProp(Int32 fileNo, Byte propType, ref Int32 propValue, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetSubClassProp(fileNo, propType, ptr, clientNo);
            propValue = Marshal.ReadInt32((IntPtr)(ptr.ToInt32() + intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetSubClassProp(Int32 fileNo, Byte propType, ref String propValue, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetSubClassProp(fileNo, propType, ptr, clientNo);
            propValue = Marshal.PtrToStringAnsi((IntPtr)(ptr.ToInt32() + intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_MacroVarSetValue(Int32 no, SDataUnion var, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SDataUnion)));
            Marshal.StructureToPtr(var, ptr, true);
            ret = HNC_MacroVarSetValue(no, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, ref Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            ret = HNC_ParamanGetItem(fileNo, subNo, index, ptr, clientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, ref Double value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            ret = HNC_ParamanGetItem(fileNo, subNo, index, ptr, clientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, SByte[] value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            ret = HNC_ParamanGetItem(fileNo, subNo, index, ptr, clientNo);
            Int32 length = value.Length < HNCDATATYPE.PARAM_STR_LEN ? value.Length : HNCDATATYPE.PARAM_STR_LEN;

            if (ret == 0)
            {
                for (Int32 i = 0; i < length; ++i)
                {
                    value[i] = (SByte)Marshal.ReadByte((IntPtr)(ptr.ToInt32() + i));
                }
            }
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, ref String value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            ret = HNC_ParamanGetItem(fileNo, subNo, index, ptr, clientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            Marshal.WriteInt32(ptr, value);
            ret = HNC_ParamanSetItem(fileNo, subNo, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, Double value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            Marshal.StructureToPtr(value, ptr, true);
            ret = HNC_ParamanSetItem(fileNo, subNo, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, SByte[] value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            for (Int32 i = 0; i < value.Length; ++i)
            {
                Marshal.WriteByte((IntPtr)(ptr.ToInt32() + i), (Byte)value[i]);
            }
            ret = HNC_ParamanSetItem(fileNo, subNo, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, String value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            Byte[] tempArray = Encoding.Default.GetBytes(value);
            Byte[] strArray = new Byte[tempArray.Length + 1];
            tempArray.CopyTo(strArray, 0);
            strArray[strArray.Length - 1] = 0;

            Marshal.Copy(strArray, 0, ptr, strArray.Length);
            ret = HNC_ParamanSetItem(fileNo, subNo, index, ptr, clientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_NetGetIpaddr(ref String ip, ref UInt16 port, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(ipSize);
            ret = HNC_NetGetIpaddr(ptr, ref port, clientNo);
            ip = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_SamplGetData(Int32 ch, ref Int32 num, Int32[] data, Int16 clientNo)
        {
            Int32 ret = -1;
            if (num <= 0)
            {
                return ret;
            }

            Int32 dataLength = num * intSize;
            IntPtr ptr = Marshal.AllocHGlobal(dataLength);
            ret = HNC_SamplGetData(ch, ref num, ptr, clientNo);

            if (ret == 0)
            {
                Marshal.Copy(ptr, data, 0, num);
            }

            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        public static Int32 HNC_ParamanGetStr(Int32 fileNo, Int32 subNo, Int32 index, ref String value, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HNC_ParamanGetStr(fileNo, subNo, index, ptr, clientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_FprogGetFullName(Int32 ch, ref String progName, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCFPROGMAN.PROG_PATH_SIZE);
            ret = HNC_FprogGetFullName(ch, ptr, clientNo);
            progName = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_ParamanGetFileName(Int32 fileNo, ref String buf, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCDATADEF.PARAMAN_LIB_TITLE_SIZE);
            ret = HNC_ParamanGetFileName(fileNo, ptr, clientNo);
            buf = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_AlarmGetData(Int32 type, Int32 level, Int32 index, ref Int32 alarmNo, ref String alarmText, Int16 clientNo)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCALARM.ALARM_TXT_LEN);
            ret = HNC_AlarmGetData(type, level, index, ref alarmNo, ptr, clientNo);
            alarmText = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_NetFileGetDirInfo(String dirname, ncfind_t[] info, ref UInt16 num, Int16 clientNo)
        {
            Int32 size = Marshal.SizeOf(typeof(ncfind_t));
            IntPtr ptr = Marshal.AllocHGlobal(size * info.Length);
            Int32 ret = -1;
            ret = HNC_NetFileGetDirInfo(dirname, ptr, ref num, clientNo);

            if (ret == 0)
            {
                for (Int32 i = 0; i < num; ++i)
                {
                    info[i] = (ncfind_t)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + i * size), typeof(ncfind_t));
                }
            }

            Marshal.FreeHGlobal(ptr);
            return ret;
        }

		public static Int32 HNC_EventPut(SEventElement ev, Int16 clientNo)
		{
		   Int32 ret = -1;
		   IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SEventElement)));
		   Marshal.StructureToPtr(ev, ptr, true);
		   ret = HNC_EventPut(ptr, clientNo);
		   Marshal.FreeHGlobal(ptr);

		   return ret;
		}


        public static Int32 HNC_EventGetSysEv(ref SEventElement ev)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SEventElement)));
            ret = HNC_EventGetSysEv(ptr);
            ev = (SEventElement)Marshal.PtrToStructure(ptr, typeof(SEventElement));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public static Int32 HNC_AlarmGetHistoryData(Int32 index, ref Int32 count, AlarmHisData[] data, Int16 clientNo)
        {
            Int32 MAX_ALARM_HISDATA_LEN = HncApi.ALARM_HISTORY_MAX_NUM * Marshal.SizeOf(typeof(AlarmHisData));
            IntPtr ptr = Marshal.AllocHGlobal(MAX_ALARM_HISDATA_LEN);
            Int32 ret = HNC_AlarmGetHistoryData(index, ref count, ptr, clientNo);
            if (ret == 0)
            {
                for (Int32 i = 0; i < count; i++)
                {
                    data[i] = (AlarmHisData)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + i * Marshal.SizeOf(typeof(AlarmHisData))), typeof(AlarmHisData));
                }
            }

            Marshal.FreeHGlobal(ptr);

            return ret;
        }
        public static Int32 HNC_SysCtrlMdiGetBlk(Int32[] pos, Int32[] msft, Int32[] ijkr, Int16 clientNo)
		{
            Int32 ret = -1;

            IntPtr posPtr = Marshal.AllocHGlobal(HNCDATADEF.CHAN_AXES_NUM * intSize);
            IntPtr msftPtr = Marshal.AllocHGlobal(HNCSYSCTRL.MSFTCOUNT * intSize);
            IntPtr ijkrPtr = Marshal.AllocHGlobal(HNCSYSCTRL.IJKRCOUNT * intSize);
            ret = HNC_SysCtrlMdiGetBlk(posPtr, msftPtr, ijkrPtr,clientNo);

            if (ret == 0)
            {
                Marshal.Copy(posPtr, pos, 0, HNCDATADEF.CHAN_AXES_NUM);
                Marshal.Copy(msftPtr, msft, 0, HNCSYSCTRL.MSFTCOUNT);
                Marshal.Copy(ijkrPtr, ijkr, 0, HNCSYSCTRL.IJKRCOUNT);
            }

            Marshal.FreeHGlobal(posPtr);
			Marshal.FreeHGlobal(msftPtr);
			Marshal.FreeHGlobal(ijkrPtr);

            return ret;
		}


	    public static Int32 HNC_FprogGetProgPathByIdx(Int32 pindex, ref String  progName, Int16  clientNo)
		{
			Int32 ret = -1;
			IntPtr ptr = Marshal.AllocHGlobal(HNCFPROGMAN.PROG_PATH_SIZE);
			ret = HNC_FprogGetProgPathByIdx(pindex, ptr, clientNo);
			progName = Marshal.PtrToStringAnsi(ptr);
			Marshal.FreeHGlobal(ptr);

			return ret;
		}

        internal static int HNC_SystemGetValue(object hNC_SYS_ACTIVE_CHAN, ref int mch, short activeClientNo)
        {
            throw new NotImplementedException();
        }
    }
}
