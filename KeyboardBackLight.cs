using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace keyboardled_reverse
{
    class KeyboardBackLight
    {
        [DllImport("InsydeDCHU.dll")]
        static extern int SetDCHU_Data(int command, byte[] buffer, int length);

        [DllImport("InsydeDCHU.dll")]
        static extern int WriteAppSettings(int page, int offset, int length, ref byte buffer);

        [DllImport("InsydeDCHU.dll")]
        public static extern int ReadAppSettings(int page, int offset, int length, ref byte buffer);

        public static string GetStatus()
        {
            byte brightness = 0;
            ReadAppSettings(2, 35, 1, ref brightness);
            byte[] colour = new byte[3];
            ReadAppSettings(2, 81, 3, ref colour[0]);
            byte mode = 0;
            ReadAppSettings(2, 32, 1, ref mode);
            byte status = 0;
            ReadAppSettings(2, 84, 1, ref status);
            string state = status == 0 ? "Off" : "On";
            byte booteffect = 0;
            ReadAppSettings(2, 7, 1, ref booteffect);
            string boot = booteffect == 0 ? "Default" : "Overridden";
            byte[] sleep = new byte[3];
            ReadAppSettings(2, 37, 3, ref sleep[0]);
            int sleepsec = sleep[0] * 3600 + sleep[1] * 60 + sleep[2];
            byte sleepstatus = 0;
            ReadAppSettings(2, 36, 1, ref sleepstatus);
            string sleepstate = sleepstatus == 0 ? "Off" : "On";

            return $@"RGBA {colour[0]} {colour[1]} {colour[2]} {brightness}
MODE {LookupMode(mode)}
LEDSTATUS {state}
BOOTEFFECT {boot}
SLEEP {sleepsec} SEC
SLEEPSTATUS {sleepstate}";
        }

        /// <summary>
        /// Sets the brightness of the LEDs to the given value
        /// </summary>
        /// <param name="value">The value [0-255]</param>
        public static void SetBrightness(byte value)
        {
            SetDCHU_Data(103, new byte[4] { value, 0, 0, 244 }, 4);
            //TODO: express the setting as a value in [0-4] as they do in their code
            WriteAppSettings(2, 35, 1, ref new byte[1] { value }[0]);
        }

        /// <summary>
        /// Sets the color of the LEDs to the given colour
        /// </summary>
        /// <param name="R">The red component [0-255]</param>
        /// <param name="G">The green component [0-255]</param>
        /// <param name="B">The blue component [0-255]</param>
        public static void SetColour(byte R, byte G, byte B)
        {
            SetDCHU_Data(103, new byte[4] { G, R, B, 240 }, 4);
            WriteAppSettings(2, 81, 3, ref new byte[3] { R, G, B }[0]);
            WriteAppSettings(2, 32, 1, ref new byte[1] { 8 }[0]); //All colour mode
        }

        /// <summary>
        /// Sets the LEDs to the given Mode
        /// </summary>
        /// <param name="mode">The mode</param>
        public static void SetMode(Mode mode)
        {
            SetDCHU_Data(103, new byte[4] { 0, 0, 0, (byte)mode }, 4);
            WriteAppSettings(2, 32, 1, ref new byte[1] { GetModeNum(mode) }[0]);
        }

        /// <summary>
        /// Represents different modes for the LEDs
        /// </summary>
        public enum Mode
        {
            /// <summary>
            /// Breaths with the current colour (cycles from 0 to brightness, around 2 sec)
            /// </summary>
            Breath = 16,
            /// <summary>
            /// Cycles through primary and secondary colours (going from 0 to brightness in between, around 2sec). Blue, Green, Red, Blue?, Cyan, Yellow, Pink  
            /// </summary>
            Cycle = 51,
            /// <summary>
            /// Goes every ~0.5 sec to a different random colour (with the current brightness)
            /// </summary>
            Random = 112,
            /// <summary>
            /// Flashes every 0.5 sec with a different random colour (goes from 0 to brightness)
            /// </summary>
            Dance = 128,
            /// <summary>
            /// Flashes a colour, then goes off shortly, then flashes the same colour, then goes off longer ~1sec, then starts over with a different colour (goes from 0 to brightness)
            /// </summary>
            Tempo = 144,
            /// <summary>
            /// Flashes every 1 sec with a different random colour (goes from 0 to brightness)
            /// </summary>
            Flash = 160,
            /// <summary>
            /// Slowely (around every 10 sec) shows a random colour slowly increasing and then decreasing the brightness (goes from 0 to brightness)
            /// </summary>
            Wave = 176
        }

        static string LookupMode(byte mode) =>
            mode switch
            {
                0 => "Random",
                1 => "Custom",
                2 => "Breath",
                3 => "Cycle",
                4 => "Wave",
                5 => "Dance",
                6 => "Tempo",
                7 => "Flash",
                8 => "All Colour",
                _ => $"Unknown '{mode}'",
            };

        static byte GetModeNum(Mode mode) =>
        mode switch
        {
            Mode.Random => 0,
            Mode.Breath => 2,
            Mode.Cycle => 3,
            Mode.Wave => 4,
            Mode.Dance => 5,
            Mode.Tempo => 6,
            Mode.Flash => 7,
            _ => throw new Exception("Mode does not exist"),
        };

        /// <summary>
        /// Turns the LEDs off
        /// </summary>
        public static void TurnOff()
        {
            SetDCHU_Data(103, new byte[4] { 0, 0, 0, 224 }, 4);
            WriteAppSettings(2, 84, 1, ref new byte[1] { 0 }[0]);
        }

        /// <summary>
        /// Turn the LEDs on
        /// </summary>
        public static void TurnOn()
        {
            SetDCHU_Data(103, new byte[4] { 0, 0, 1, 224 }, 4);
            WriteAppSettings(2, 84, 1, ref new byte[1] { 1 }[0]);
        }

        /// <summary>
        /// Whether or not the boot effect (cycling through colours) should be overridden
        /// </summary>
        /// <param name="value">True = no boot effect, False = default boot effect</param>
        public static void OverrideBootEffect(bool value)
        {
            byte num = (byte)(value ? 1 : 0);
            SetDCHU_Data(121, new byte[4] { num, 0, 0, 30 }, 4);
            WriteAppSettings(2, 7, 1, ref new byte[1] { num }[0]);
        }

        /// <summary>
        /// Sets the time before the keyboard LEDs switch off (also sets the trigger on if it was switched off)
        /// </summary>
        /// <param name="value"></param>
        public static void SetSleepTimer(int value)
        {
            WriteAppSettings(2, 36, 1, ref new byte[1] { 1 }[0]);
            WriteAppSettings(2, 37, 3, ref new byte[3] { (byte)(value / 3600), (byte)(value / 60 % 60), (byte)(value % 60) }[0]);
            byte[] bytes = BitConverter.GetBytes(402653184 + (value << 8) | (int)byte.MaxValue);
            SetDCHU_Data(121, bytes, bytes.Length);
        }

        public static void WriteAcpiSleepTimer(int value)
        {
            WriteAppSettings(2, 37, 3, ref new byte[3]
            {
        (byte) (value / 3600),
        (byte) (value / 60 % 60),
        (byte) (value % 60)
            }[0]);
        }

        /// <summary>
        /// Removes the trigger of the keyboard LED off switching, effectively setting it to infinity.
        /// </summary>
        public static void TurnSleepTimerOff()
        {
            SetDCHU_Data(121, new byte[4] { 0, 0, 0, 24 }, 4);
            WriteAppSettings(2, 36, 1, ref new byte[1] { 0 }[0]);
        }
    }
}