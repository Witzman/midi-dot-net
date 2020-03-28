using System;
using Midi;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;

namespace MidiExamples
{
    class MidiLED : ExampleBase
    {
        public MidiLED () : base("MidiLED", "Arduino LED Controller."){
            
        }

        public class Summarizer
        {
            Dictionary<string, byte> map = new Dictionary<string, byte>();
            // my led stripe is shorter than my piano
            String[] notesOutOfRange = { "A0", "ASharp0", "B0", "C1", "CSharp1", "D1", "DSharp1", "F7", "FSharp7", "G7", "GSharp7", "A7", "ASharp7", "B7", "C8" };
            public static SerialPort serialPort = new SerialPort();
            public Summarizer(InputDevice inputDevice)
            {
                this.inputDevice = inputDevice;
                inputDevice.NoteOn += new InputDevice.NoteOnHandler(this.NoteOn);
                inputDevice.NoteOff += new InputDevice.NoteOffHandler(this.NoteOff);

                // Serial communication
                serialPort.PortName = "COM8"; // Arduino board COM
                serialPort.BaudRate = 9600;
                serialPort.Open();
                CreateMap();
            }

            public void CreateMap ()
            {
                map.Add("E1", 59);
                map.Add("F1", 58);
                map.Add("FSharp1", 58);
                map.Add("G1", 57);
                map.Add("GSharp1", 56);
                map.Add("A1", 55);
                map.Add("ASharp1", 54);
                map.Add("B1", 54);
                map.Add("C2", 53);
                map.Add("CSharp2", 52);
                map.Add("D2", 51);
                map.Add("DSharp2", 50);

                map.Add("E2", 49);
                map.Add("F2", 48);
                map.Add("FSharp2", 48);
                map.Add("G2", 47);
                map.Add("GSharp2", 46);
                map.Add("A2", 45);
                map.Add("ASharp2", 44);
                map.Add("B2", 44);
                map.Add("C3", 43);
                map.Add("CSharp3", 42);
                map.Add("D3", 41);
                map.Add("DSharp3", 40);

                map.Add("E3", 39);
                map.Add("F3", 38);
                map.Add("FSharp3", 38);
                map.Add("G3", 37);
                map.Add("GSharp3", 36);
                map.Add("A3", 35);
                map.Add("ASharp3", 34);
                map.Add("B3", 34);
                map.Add("C4", 33);
                map.Add("CSharp4", 32);
                map.Add("D4", 31);
                map.Add("DSharp4", 30);

                map.Add("E4", 29);
                map.Add("F4", 28);
                map.Add("FSharp4", 28);
                map.Add("G4", 27);
                map.Add("GSharp4", 26);
                map.Add("A4", 25);
                map.Add("ASharp4", 24);
                map.Add("B4", 24);
                map.Add("C5", 23);
                map.Add("CSharp5", 22);
                map.Add("D5", 21);
                map.Add("DSharp5", 20);

                map.Add("E5", 19);
                map.Add("F5", 18);
                map.Add("FSharp5", 18);
                map.Add("G5", 17);
                map.Add("GSharp5", 16);
                map.Add("A5", 15);
                map.Add("ASharp5", 14);
                map.Add("B5", 14);
                map.Add("C6", 13);
                map.Add("CSharp6", 12);
                map.Add("D6", 11);
                map.Add("DSharp6", 10);

                map.Add("E6", 9);
                map.Add("F6", 8);
                map.Add("FSharp6", 8);
                map.Add("G6", 7);
                map.Add("GSharp6", 6);
                map.Add("A6", 5);
                map.Add("ASharp6", 4);
                map.Add("B6", 4);
                map.Add("C7", 3);
                map.Add("CSharp7", 2);
                map.Add("D7", 1);
                map.Add("DSharp7", 0);

                map.Add("E7", 0);
            }

            // because my piano uses NoteOn(0 vel) event as a NoteOff event, I'll send all data to the Arduino here
            public void NoteOn(NoteOnMessage msg)
            {
                // detect if note is in range
                if (!notesOutOfRange.Contains(msg.Pitch.ToString()))
                {
                    lock (this)
                    {
                        if (msg.Velocity == 0)
                        {
                            Console.WriteLine("NoteOff: " + msg.Pitch);
                            byte[] b = BitConverter.GetBytes(map[msg.Pitch.ToString()]);
                            serialPort.Write(b, 0, b.Length);
                        } else
                        {
                            Console.WriteLine("NoteOn: " + msg.Pitch);
                            // I'm adding 100 to distinguish noteOn and noteOff events
                            byte[] b = BitConverter.GetBytes(100 + map[msg.Pitch.ToString()]);
                            serialPort.Write(b, 0, b.Length);
                        }
                    }
                }
            }

            public void NoteOff(NoteOffMessage msg)
            {
                lock (this)
                {
                    Console.WriteLine("NoteOff: " + msg.Pitch);
                }
            }

            private InputDevice inputDevice;
        }


        public override void Run ()
        {
            // Detects input devices if any
            InputDevice inputDevice = ExampleUtil.ChooseInputDeviceFromConsole();
            if (inputDevice == null)
            {
                Console.WriteLine("No input devices, so can't run this example.");
                ExampleUtil.PressAnyKeyToContinue();
                return;
            } else
            {
                Console.WriteLine("Connected to: " + inputDevice.Name);
            }
            inputDevice.Open();
            inputDevice.StartReceiving(null);

            Summarizer summarizer = new Summarizer(inputDevice);
            ExampleUtil.PressAnyKeyToContinue();
            inputDevice.StopReceiving();
            inputDevice.Close();
            Summarizer.serialPort.Close();
            inputDevice.RemoveAllEventHandlers();
        }
    }
}
