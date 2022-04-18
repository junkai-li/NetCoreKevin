using System;
using System.Device.Gpio;

namespace Common.IOT.Sensors
{


    /// <summary>
    /// 人体红外传感器
    /// </summary>
    public class HumanInfraredSensor : IDisposable
    {


        private GpioController _controller;
        private readonly int _outPin;



        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="pin">OUT Pin</param>
        public HumanInfraredSensor(int outPin, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical)
        {
            _outPin = outPin;

            _controller = new GpioController(pinNumberingScheme);
            _controller.OpenPin(outPin, PinMode.Input);
        }



        /// <summary>
        /// 是否检测到人体
        /// </summary>
        public bool IsMotionDetected => _controller.Read(_outPin) == PinValue.High;



        /// <summary>
        /// Cleanup
        /// </summary>
        public void Dispose()
        {
            _controller?.Dispose();
            _controller = null;
        }

    }
}
