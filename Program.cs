using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace keyboardled_reverse
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Help();
            }
            else
            {
                switch (args[0].ToLower())
                {
                    case "rgba":
                        if (args.Length != 5)
                        {
                            Console.WriteLine("Wrong number of arguments.");
                        }
                        else
                        {
                            KeyboardBackLight.SetColour(Convert.ToByte(args[1]), Convert.ToByte(args[2]), Convert.ToByte(args[3]));
                            KeyboardBackLight.SetBrightness(Convert.ToByte(args[4]));
                        }
                        break;
                    case "rgb":
                        if (args.Length != 4)
                        {
                            Console.WriteLine("Wrong number of arguments.");
                        }
                        else
                        {
                            KeyboardBackLight.SetColour(Convert.ToByte(args[1]), Convert.ToByte(args[2]), Convert.ToByte(args[3]));
                        }
                        break;
                    case "a":
                        if (args.Length != 2)
                        {
                            Console.WriteLine("Wrong number of arguments.");
                        }
                        else
                        {
                            KeyboardBackLight.SetBrightness(Convert.ToByte(args[1]));
                        }
                        break;
                    case "on":
                        KeyboardBackLight.TurnOn();
                        break;
                    case "off":
                        KeyboardBackLight.TurnOff();
                        break;
                    case "mode":
                        if (args.Length != 2)
                        {
                            Console.WriteLine("Wrong number of arguments.");
                        }
                        else
                        {
                            switch (args[1].ToLower())
                            {
                                case "breath":
                                    KeyboardBackLight.SetMode(KeyboardBackLight.Mode.Breath);
                                    break;
                                case "bycle":
                                    KeyboardBackLight.SetMode(KeyboardBackLight.Mode.Cycle);
                                    break;
                                case "random":
                                    KeyboardBackLight.SetMode(KeyboardBackLight.Mode.Random);
                                    break;
                                case "dance":
                                    KeyboardBackLight.SetMode(KeyboardBackLight.Mode.Dance);
                                    break;
                                case "tempo":
                                    KeyboardBackLight.SetMode(KeyboardBackLight.Mode.Tempo);
                                    break;
                                case "flash":
                                    KeyboardBackLight.SetMode(KeyboardBackLight.Mode.Flash);
                                    break;
                                case "wave":
                                    KeyboardBackLight.SetMode(KeyboardBackLight.Mode.Wave);
                                    break;
                                default:
                                    Console.WriteLine($"Unknown mode '{args[1]}' in Mode");
                                    break;
                            };
                        }
                        break;
                    case "booteffect":
                        if (args.Length != 2)
                        {
                            Console.WriteLine("Wrong number of arguments.");
                        }
                        else
                        {
                            switch (args[1].ToLower())
                            {
                                case "on":
                                    KeyboardBackLight.OverrideBootEffect(false);
                                    break;
                                case "off":
                                    KeyboardBackLight.OverrideBootEffect(true);
                                    break;
                                default:
                                    Console.WriteLine($"Unknown option '{args[1]}' in booteffect");
                                    break;
                            };
                        }
                        break;
                    case "timer":
                        if (args.Length != 2)
                        {
                            Console.WriteLine("Wrong number of arguments.");
                        }
                        else
                        {
                            switch (args[1].ToLower())
                            {
                                case "off":
                                    KeyboardBackLight.TurnSleepTimerOff();
                                    break;
                                default:
                                    KeyboardBackLight.SetSleepTimer(Convert.ToInt16(args[1]));
                                    break;
                            };
                        }
                        break;
                    case "monitor":
                        if (args.Length != 2)
                        {
                            Console.WriteLine("Wrong number of arguments.");
                        }
                        else
                        {
                            switch (args[1].ToLower())
                            {
                                case "on":
                                    MonitorControl.ChangeMonitorState(MonitorControl.MonitorMode.MONITOR_ON);
                                    break;
                                case "off":
                                    MonitorControl.ChangeMonitorState(MonitorControl.MonitorMode.MONITOR_OFF);
                                    break;
                                case "standby":
                                    MonitorControl.ChangeMonitorState(MonitorControl.MonitorMode.MONITOR_STANBY);
                                    break;
                                default:
                                    Console.WriteLine($"Unknown option '{args[1]}' in monitor");
                                    break;
                            };
                        }
                        break;
                    case "touchpad":
                        Touchpad.ToggleTouchpad();
                        break;
                    case "power":
                        if (args.Length != 2)
                        {
                            Console.WriteLine("Wrong number of arguments.");
                        }
                        else
                        {
                            switch (args[1].ToLower())
                            {
                                case "saving":
                                    PowerMode.SetPowerMode(PowerMode.Mode.Powersaving);
                                    break;
                                case "quiet":
                                    PowerMode.SetPowerMode(PowerMode.Mode.Quiet);
                                    break;
                                case "performance":
                                    PowerMode.SetPowerMode(PowerMode.Mode.Performance);
                                    break;
                                case "entertainment":
                                    PowerMode.SetPowerMode(PowerMode.Mode.Entertainment);
                                    break;
                                default:
                                    Console.WriteLine($"Unknown option '{args[1]}' in power");
                                    break;
                            };
                        }
                        break;
                    case "fan":
                        if (args.Length == 2)
                        {
                            switch (args[1].ToLower())
                            {
                                case "antidust":
                                    Fan.AntiDust();
                                    break;
                                case "auto":
                                    Fan.SetMode(Fan.Mode.Auto);
                                    break;
                                case "max":
                                    Fan.SetMode(Fan.Mode.Max);
                                    break;
                                case "maxq":
                                    Fan.SetMode(Fan.Mode.MaxQ);
                                    break;
                                default:
                                    Console.WriteLine($"Unknown option '{args[1]}' in fan");
                                    break;
                            };
                        }
                        else if (args.Length == 3)
                        {
                            if (args[1].ToLower() == "offset")
                            {
                                Fan.SetOffset(Convert.ToByte(args[2]));
                            }
                            else
                            {
                                Console.WriteLine($"Unknown option '{args[1]}' in fan");
                            }
                        }
                        else if (args.Length == 8)
                        {
                            if (args[1].ToLower() == "custom")
                            {
                                Fan.SetCustomFanTable(
                                    Convert.ToByte(args[2]),
                                    Convert.ToByte(args[3]),
                                    Convert.ToByte(args[4]),
                                    Convert.ToByte(args[5]),
                                    Convert.ToByte(args[6]),
                                    Convert.ToByte(args[7]));

                            }
                            else
                            {
                                Console.WriteLine($"Unknown option '{args[1]}' in fan");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Wrong number of arguments.");
                        }
                        break;
                    case "status":
                        Console.WriteLine(KeyboardBackLight.GetStatus());
                        Console.WriteLine(PowerMode.GetStatus());
                        Console.WriteLine(Fan.GetStatus());
                        break;
                    case "features":
                        Console.WriteLine(Capabilities.Check());
                        break;
                    default:
                        Console.WriteLine($"Keyword {args[0]} not recognised.");
                        Help();
                        break;
                }
            }
        }

        static void Help()
        {
            Console.WriteLine(@"Use this program by using one of the following commands:
 * RGBA <R> <G> <B> <A> (all bytes)
 * RGB <R> <G> <B> (all bytes)
 * A <A> (byte) (the brightness)
 * On - turns the backlight on
 * Off - turns the backlight off
 * Mode <mode> - sets one of the following modes:
   * Breath - Breaths with the current colour (cycles from 0 to brightness, around 2 sec)
   * Cycle - Cycles through primary and secondary colours (going from 0 to brightness in between, around 2sec). Blue, Green, Red, Blue?, Cyan, Yellow, Pink
   * Random - Goes every ~0.5 sec to a different random colour (with the current brightness)
   * Dance - Flashes every 0.5 sec with a different random colour (goes from 0 to brightness)
   * Tempo - Flashes a colour, then goes off shortly, then flashes the same colour, then goes off longer ~1sec, then starts over with a different colour (goes from 0 to brightness)
   * Flash - Flashes every 1 sec with a different random colour (goes from 0 to brightness)
   * Wave - Slowely (around every 10 sec) shows a random colour slowly increasing and then decreasing the brightness (goes from 0 to brightness)
 * Booteffect <on|off> - turns on/off the booteffect
 * Timer <Off|value> (value is a short) - sets the times in second or turns it off
 * Monitor <On|Off|Standby> - turns the monitor on or off (will turn on with hte next keystroke).
 * Touchpad - will toggle the state of the touchpad (enabled/disabled).
 * Power <Saving|Quiet|Performance|Entertainment> - sets the powerplan.
 * Fan <cmd>
   * Antidust - will do antidust (if supported).
   * Auto - set the mode to auto.
   * Max - set the mode to maximal (100%).
   * MaxQ - set the mode to MaxQ (if supported).
   * Offset <num> - sets the offset the the specified num [0-100].
   * Custom <T> <D> <T> <D> <T> <D> - sets a custom fan curve, T is the temp [0-100] and D the associated speed [0-100]%.
 * Status - shows the status of the settings.
 * Features - shows which features are supported.");
        }
    }
}
