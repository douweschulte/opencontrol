using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace keyboardled_reverse
{
    class Fan
    {
        [DllImport("InsydeDCHU.dll")]
        static extern int SetDCHU_Data(int command, byte[] buffer, int length);

        [DllImport("InsydeDCHU.dll")]
        static extern int GetDCHU_Data_Buffer(int command, ref byte buffer);

        [DllImport("InsydeDCHU.dll")]
        static extern int WriteAppSettings(int page, int offset, int length, ref byte buffer);

        [DllImport("InsydeDCHU.dll")]
        public static extern int ReadAppSettings(int page, int offset, int length, ref byte buffer);

        public static string GetStatus()
        {
            byte[] offset = new byte[4];
            ReadAppSettings(4, 7, 1, ref offset[0]);
            byte[] mode = new byte[4];
            ReadAppSettings(4, 5, 1, ref mode[0]);

            return $"FANMODE {LookupMode(mode[0])}\nFANOFFSET {offset[0]}\nFANCURVE {Read_FanInfo()}\nFANTEMP {FanSpeed()}";
        }

        static string FanSpeed()
        {
            byte[] numArray = new byte[256];
            GetDCHU_Data_Buffer(12, ref numArray[0]);
            int rpm = (int)numArray[3] + ((int)numArray[2] << 8);
            rpm = (int)Math.Round(60.0 / (5.56521739130435E-05 * rpm) * 2.0, 0);
            int duty = numArray[16];
            int percent = (int)Math.Round((double)duty / (double)byte.MaxValue * 100.0, 0);
            int remote = (int)numArray[18]; //Global.RW_REG.CalCPUTemp(Global.RW_REG.GetTDP(), (int)numArray[18]);
            return $"{rpm}RPM {percent}% {remote}°C";
        }

        public static void AntiDust()
        {
            SetDCHU_Data(121, new byte[4] { 1, 0, 0, 41 }, 4);
        }

        // As a percentage in range [0-255]
        public static void SetOffset(byte offset)
        {
            SetDCHU_Data(121, new byte[4] { offset, 0, 0, 14 }, 4);
            WriteAppSettings(4, 7, 1, ref new byte[4] { offset, 0, 0, 14 }[0]);
        }

        // 0 - auto
        // 1 - max
        // 5 - MaxQ
        // 6 - custom
        public static void SetMode(Mode mode)
        {
            SetDCHU_Data(121, new byte[4] { (byte)mode, 0, 0, 1 }, 4);
            WriteAppSettings(4, 5, 1, ref new byte[4] { (byte)mode, 0, 0, 1 }[0]);
        }

        public enum Mode
        {
            Auto = 0,
            Max = 1,
            MaxQ = 5,
            Custom = 6,
        }

        static string LookupMode(byte mode) =>
        mode switch
        {
            0 => "Auto",
            1 => "Max",
            5 => "MaxQ",
            6 => "Custom",
            _ => $"Unknown '{mode}'",
        };

        public static void SetCustomFanTable(byte T1, byte D1, byte T2, byte D2, byte T3, byte D3)
        {
            Fan.SetMode(Fan.Mode.Custom);
            byte[] buffer = new byte[256];
            buffer[2] = T2;
            buffer[3] = (byte)Math.Round((double)D2 / 100.0 * (double)byte.MaxValue, 0);
            buffer[4] = T3;
            buffer[5] = (byte)Math.Round((double)D3 / 100.0 * (double)byte.MaxValue, 0);
            int R12 = (int)Math.Round((double)((int)D2 - (int)D1) / (double)((int)T2 - (int)T1) * 2.55 * 16.0, 0);
            int R23 = (int)Math.Round((double)((int)D3 - (int)D2) / (double)((int)T3 - (int)T2) * 2.55 * 16.0, 0);
            int R34 = (int)Math.Round((double)((int)100 - (int)D3) / (double)((int)100 - (int)T3) * 2.55 * 16.0, 0);
            buffer[14] = (byte)(R12 >> 8);
            buffer[15] = (byte)(R12 & (int)byte.MaxValue);
            buffer[16] = (byte)(R23 >> 8);
            buffer[17] = (byte)(R23 & (int)byte.MaxValue);
            buffer[18] = (byte)(R34 >> 8);
            buffer[19] = (byte)(R34 & (int)byte.MaxValue);
            SetDCHU_Data(14, buffer, buffer.Length);
            SetFanInfo(T1, D1, T2, D2, T3, D3);
        }

        static void SetFanInfo(byte T1, byte D1, byte T2, byte D2, byte T3, byte D3)
        {
            byte[] offset = new byte[4];
            ReadAppSettings(4, 7, 1, ref offset[0]);
            byte[] mode = new byte[4];
            ReadAppSettings(4, 5, 1, ref mode[0]);

            byte[] buffer = new byte[256];
            buffer[0] = 3; //major
            buffer[1] = 0; //minor
            buffer[2] = 0; //build
            buffer[3] = 0; //revision
            buffer[4] = 8;
            buffer[5] = mode[0];
            buffer[6] = 1;
            buffer[7] = offset[0];
            buffer[16] = D1;
            buffer[17] = D2;
            buffer[18] = D3;
            buffer[19] = (byte)100;
            buffer[22] = T1;
            buffer[23] = T2;
            buffer[24] = T3;
            buffer[25] = (byte)100;
            int R12 = (int)Math.Round((double)((int)D2 - (int)D1) / (double)((int)T2 - (int)T1) * 2.55 * 16.0, 0);
            int R23 = (int)Math.Round((double)((int)D3 - (int)D2) / (double)((int)T3 - (int)T2) * 2.55 * 16.0, 0);
            int R34 = (int)Math.Round((double)((int)100 - (int)D3) / (double)((int)100 - (int)T3) * 2.55 * 16.0, 0);
            buffer[28] = (byte)(R12 & (int)byte.MaxValue);
            buffer[29] = (byte)(R12 >> 8);
            buffer[30] = (byte)(R23 & (int)byte.MaxValue);
            buffer[31] = (byte)(R23 >> 8);
            buffer[32] = (byte)(R34 & (int)byte.MaxValue);
            buffer[33] = (byte)(R34 >> 8);

            WriteAppSettings(4, 0, 256, ref buffer[0]);
        }

        static string Read_FanInfo()
        {
            byte[] numArray2 = new byte[256];
            ReadAppSettings(4, 0, 256, ref numArray2[0]);
            byte D1 = numArray2[16];
            byte D2 = numArray2[17];
            byte D3 = numArray2[18];
            byte T1 = numArray2[22];
            byte T2 = numArray2[23];
            byte T3 = numArray2[24];
            return $"{T1}°C {D1}% {T2}°C {D2}% {T3}°C {D3}% 100°C 100% ";
        }
    }
}