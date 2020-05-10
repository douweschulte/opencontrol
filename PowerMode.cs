using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace keyboardled_reverse
{
    class PowerMode
    {
        [DllImport("InsydeDCHU.dll")]
        static extern int SetDCHU_Data(int command, byte[] buffer, int length);

        [DllImport("InsydeDCHU.dll")]
        static extern int WriteAppSettings(int page, int offset, int length, ref byte buffer);

        [DllImport("InsydeDCHU.dll")]
        public static extern int ReadAppSettings(int page, int offset, int length, ref byte buffer);

        public static string GetStatus()
        {
            byte mode = 0;
            ReadAppSettings(1, 1, 1, ref mode);
            return $"POWERMODE {LookupMode(mode)}";
        }

        public enum Mode
        {
            Quiet,
            Powersaving,
            Performance,
            Entertainment,
        }

        static string LookupMode(byte mode) =>
        mode switch
        {
            0 => "Quiet",
            1 => "Powersaving",
            2 => "Performance",
            3 => "Entertainment",
            _ => $"Unknown '{mode}'",
        };

        public static void SetPowerMode(Mode mode)
        {
            byte value = (byte)mode;
            SetDCHU_Data(121, new byte[4] { value, 0, 0, 25 }, 4);
            WriteAppSettings(1, 1, 1, ref value);
        }
    }
}