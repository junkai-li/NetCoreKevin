using Common.IOT.Sensors;
using System;
using System.Device.Gpio;

namespace Common.IOT
{

    /// <summary>
    /// 物联网传感器帮助类
    /// </summary>
    public static class IOTHelper
    {

        public static bool HumanInfraredSensor()
        {

            int outPin = 17;


            using HumanInfraredSensor sensor = new HumanInfraredSensor(outPin, PinNumberingScheme.Logical);


            if (sensor.IsMotionDetected)
            {
                Console.WriteLine("检测到人体");
            }
            else
            {
                Console.WriteLine("未检测到人体");
            }

            return sensor.IsMotionDetected;
        }
    }
}
