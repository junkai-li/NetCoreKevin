using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Kevin.Common.Helper.UsbCamera
{
    #region 使用方法

    ////获取所有的摄像头
    //string[] devices = UsbCamera.FindDevices();
    ////获取摄像头支持的分辨率
    //int cameraIndex = 0;
    //UsbCamera.VideoFormat[] formats = UsbCamera.GetVideoFormat(cameraIndex);
    //    using var camera = new UsbCamera(cameraIndex, formats[index]);
    //    camera.Start();
    //                //第一次截图不延迟的话，会出现黑屏
    //                System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5));
    //                MyWriteLine("开始拍照", ConsoleColor.Blue);
    //    var bmp = camera.GetBitmap();
    //    var dicimg = Environment.CurrentDirectory + @$"\IMG";
    //                if (!System.IO.Directory.Exists(dicimg))
    //                {
    //                    System.IO.Directory.CreateDirectory(dicimg);//不存在就创建文件夹
    //                }
    //var dicimgDate = Environment.CurrentDirectory + @$"\IMG\" + DateTime.Now.ToString("yyyy-MM-dd");
    //if (!System.IO.Directory.Exists(dicimgDate))
    //{
    //    System.IO.Directory.CreateDirectory(dicimgDate);//不存在就创建文件夹
    //}
    //var fileName = DateTime.Now.ToString("HH-mm-ss") + @$"-shexiaotou.jpg";
    //var imgurl = dicimgDate + @$"\" + fileName; 
    //bmp.Save(imgurl);
    //bmp.Dispose();
    //camera.Dispose();

#endregion

/// <summary>
/// USB相关接口
/// </summary>
public class UsbCamera : IDisposable
{
    /// <summary>Usb camera image size.</summary>
    public Size Size { get; private set; }

    /// <summary>Start.</summary>
    public Action Start { get; private set; }

    /// <summary>Stop.</summary>
    public Action Stop { get; private set; }

    /// <summary>Release resource.</summary>
    public Action Release { get; private set; }

    /// <summary>Get image.</summary>
    /// <remarks>Immediately after starting, fails because image buffer is not prepared yet.</remarks>
    public Func<Bitmap> GetBitmap { get; private set; }

    /// <summary>
    /// Get available USB camera list.
    /// </summary>
    /// <returns>Array of camera name, or if no device found, zero length array.</returns>
    public static string[] FindDevices()
    {
        return DirectShow.GetFiltes(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory).ToArray();
    }

    /// <summary>
    /// Get video formats.
    /// </summary>
    public static VideoFormat[] GetVideoFormat(int cameraIndex)
    {
        var filter = DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory, cameraIndex);
        var pin = DirectShow.FindPin(filter, 0, DirectShow.PIN_DIRECTION.PINDIR_OUTPUT);
        return GetVideoOutputFormat(pin);
    }

    /// <summary>
    /// Create USB Camera. If device do not support the size, default size will applied.
    /// </summary>
    /// <param name="cameraIndex">Camera index in FindDevices() result.</param>
    /// <param name="size">
    /// Size you want to create. Normally use Size property of VideoFormat in GetVideoFormat() result.
    /// </param>
    public UsbCamera(int cameraIndex, Size size) : this(cameraIndex, new VideoFormat() { Size = size })
    {
    }

    /// <summary>
    /// Create USB Camera. If device do not support the format, default format will applied.
    /// </summary>
    /// <param name="cameraIndex">Camera index in FindDevices() result.</param>
    /// <param name="format">
    /// Normally use GetVideoFormat() result.
    /// You can change TimePerFrame value from Caps.MinFrameInterval to Caps.MaxFrameInterval.
    /// TimePerFrame = 10,000,000 / frame duration. (ex: 333333 in case 30fps).
    /// You can change Size value in case Caps.MaxOutputSize > Caps.MinOutputSize and OutputGranularityX/Y is not zero.
    /// Size = any value from Caps.MinOutputSize to Caps.MaxOutputSize step with OutputGranularityX/Y.
    /// </param>
    public UsbCamera(int cameraIndex, VideoFormat format)
    {
        var camera_list = FindDevices();
        if (cameraIndex >= camera_list.Length) throw new ArgumentException("USB camera is not available.", "cameraIndex");
        Init(cameraIndex, format);
    }

    private void Init(int index, VideoFormat format)
    {
        //----------------------------------
        // Create Filter Graph
        //----------------------------------
        // +--------------------+  +----------------+  +---------------+
        // |Video Capture Source|→| Sample Grabber |→| Null Renderer |
        // +--------------------+  +----------------+  +---------------+
        //                                 ↓GetBitmap()

        var graph = DirectShow.CreateGraph();

        //----------------------------------
        // VideoCaptureSource
        //----------------------------------
        var vcap_source = CreateVideoCaptureSource(index, format);
        graph.AddFilter(vcap_source, "VideoCapture");

        //------------------------------
        // SampleGrabber
        //------------------------------
        var grabber = CreateSampleGrabber();
        graph.AddFilter(grabber, "SampleGrabber");
        var i_grabber = (DirectShow.ISampleGrabber)grabber;
        i_grabber.SetBufferSamples(true);

        //---------------------------------------------------
        // Null Renderer
        //---------------------------------------------------
        var renderer = DirectShow.CoCreateInstance(DirectShow.DsGuid.CLSID_NullRenderer) as DirectShow.IBaseFilter;
        graph.AddFilter(renderer, "NullRenderer");

        //---------------------------------------------------
        // Create Filter Graph
        //---------------------------------------------------
        var builder = DirectShow.CoCreateInstance(DirectShow.DsGuid.CLSID_CaptureGraphBuilder2) as DirectShow.ICaptureGraphBuilder2;
        builder.SetFiltergraph(graph);
        var pinCategory = DirectShow.DsGuid.PIN_CATEGORY_CAPTURE;
        var mediaType = DirectShow.DsGuid.MEDIATYPE_Video;
        builder.RenderStream(ref pinCategory, ref mediaType, vcap_source, grabber, renderer);

        // SampleGrabber Format.
        {
            var mt = new DirectShow.AM_MEDIA_TYPE();
            i_grabber.GetConnectedMediaType(mt);
            var header = (DirectShow.VIDEOINFOHEADER)Marshal.PtrToStructure(mt.pbFormat, typeof(DirectShow.VIDEOINFOHEADER));
            var width = header.bmiHeader.biWidth;
            var height = header.bmiHeader.biHeight;
            var stride = width * (header.bmiHeader.biBitCount / 8);
            DirectShow.DeleteMediaType(ref mt);

            Size = new Size(width, height);

            // fix screen tearing problem(issure #2)
            // you can use previous method if you swap the comment line below.
            // GetBitmap = () => GetBitmapFromSampleGrabberBuffer(i_grabber, width, height, stride);
            GetBitmap = GetBitmapFromSampleGrabberCallback(i_grabber, width, height, stride);
        }

        // Assign Delegates.
        Start = () => DirectShow.PlayGraph(graph, DirectShow.FILTER_STATE.Running);
        Stop = () => DirectShow.PlayGraph(graph, DirectShow.FILTER_STATE.Stopped);
        Release = () =>
        {
            Stop();

            DirectShow.ReleaseInstance(ref i_grabber);
            DirectShow.ReleaseInstance(ref builder);
            DirectShow.ReleaseInstance(ref graph);
        };

        // Properties.
        Properties = new PropertyItems(vcap_source);
    }

    public void Dispose()
    {
        Release?.Invoke();
    }

    /// <summary>Properties user can adjust.</summary>
    public PropertyItems Properties { get; private set; }
    public class PropertyItems
    {
        public PropertyItems(DirectShow.IBaseFilter vcap_source)
        {
            // Pan, Tilt, Roll, Zoom, Exposure, Iris, Focus
            this.CameraControl = Enum.GetValues(typeof(DirectShow.CameraControlProperty)).Cast<DirectShow.CameraControlProperty>()
                .Select(item =>
                {
                    PropertyItems.Property prop = null;
                    try
                    {
                        var cam_ctrl = vcap_source as DirectShow.IAMCameraControl;
                        if (cam_ctrl == null) throw new NotSupportedException("no IAMCameraControl Interface."); // will catched.
                        int min = 0, max = 0, step = 0, def = 0, flags = 0;
                        cam_ctrl.GetRange(item, ref min, ref max, ref step, ref def, ref flags); // COMException if not supports.
                        prop = new Property(min, max, step, def, flags, (flag, value) => cam_ctrl.Set(item, value, (int)flag));
                    }
                    catch (Exception) { prop = new Property(); } // available = false
                    return new { Key = item, Value = prop };
                }).ToDictionary(x => x.Key, x => x.Value);

            // Brightness, Contrast, Hue, Saturation, Sharpness, Gamma, ColorEnable, WhiteBalance, BacklightCompensation, Gain
            this.VideoProcAmp = Enum.GetValues(typeof(DirectShow.VideoProcAmpProperty)).Cast<DirectShow.VideoProcAmpProperty>()
                .Select(item =>
                {
                    PropertyItems.Property prop = null;
                    try
                    {
                        var vid_ctrl = vcap_source as DirectShow.IAMVideoProcAmp;
                        if (vid_ctrl == null) throw new NotSupportedException("no IAMVideoProcAmp Interface."); // will catched.
                        int min = 0, max = 0, step = 0, def = 0, flags = 0;
                        vid_ctrl.GetRange(item, ref min, ref max, ref step, ref def, ref flags); // COMException if not supports.
                        prop = new Property(min, max, step, def, flags, (flag, value) => vid_ctrl.Set(item, value, (int)flag));
                    }
                    catch (Exception) { prop = new Property(); } // available = false
                    return new { Key = item, Value = prop };
                }).ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>Camera Control properties.</summary>
        private Dictionary<DirectShow.CameraControlProperty, Property> CameraControl;

        /// <summary>Video Processing Amplifier properties.</summary>
        private Dictionary<DirectShow.VideoProcAmpProperty, Property> VideoProcAmp;

        /// <summary>Get CameraControl Property. Check Available before use.</summary>
        public Property this[DirectShow.CameraControlProperty item] { get { return CameraControl[item]; } }

        /// <summary>Get VideoProcAmp Property. Check Available before use.</summary>
        public Property this[DirectShow.VideoProcAmpProperty item] { get { return VideoProcAmp[item]; } }

        public class Property
        {
            public int Min { get; private set; }
            public int Max { get; private set; }
            public int Step { get; private set; }
            public int Default { get; private set; }
            public DirectShow.CameraControlFlags Flags { get; private set; }
            public Action<DirectShow.CameraControlFlags, int> SetValue { get; private set; }
            public bool Available { get; private set; }
            public bool CanAuto { get; private set; }

            public Property()
            {
                this.SetValue = (flag, value) => { };
                this.Available = false;
            }

            public Property(int min, int max, int step, int @default, int flags, Action<DirectShow.CameraControlFlags, int> set)
            {
                this.Min = min;
                this.Max = max;
                this.Step = step;
                this.Default = @default;
                this.Flags = (DirectShow.CameraControlFlags)flags;
                this.CanAuto = (Flags & DirectShow.CameraControlFlags.Auto) == DirectShow.CameraControlFlags.Auto;
                this.SetValue = set;
                this.Available = true;
            }

            public override string ToString()
            {
                return string.Format("Available={0}, Min={1}, Max={2}, Step={3}, Default={4}, Flags={5}", Available, Min, Max, Step, Default, Flags);
            }
        }
    }

    private class SampleGrabberCallback : DirectShow.ISampleGrabberCB
    {
        private byte[] Buffer;
        private object BufferLock = new object();

        public Bitmap GetBitmap(int width, int height, int stride)
        {
            var result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            if (Buffer == null) return result;

            var bmp_data = result.LockBits(new Rectangle(Point.Empty, result.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            lock (BufferLock)
            {
                // copy from last row.
                for (int y = 0; y < height; y++)
                {
                    var src_idx = Buffer.Length - (stride * (y + 1));
                    var dst = IntPtr.Add(bmp_data.Scan0, stride * y);
                    Marshal.Copy(Buffer, src_idx, dst, stride);
                }
            }
            result.UnlockBits(bmp_data);

            return result;
        }

        // called when each sample completed.
        // The data processing thread blocks until the callback method returns. If the callback does not return quickly, it can interfere with playback.
        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (Buffer == null || Buffer.Length != BufferLen)
            {
                Buffer = new byte[BufferLen];
            }

            lock (BufferLock)
            {
                Marshal.Copy(pBuffer, Buffer, 0, BufferLen);
            }
            return 0;
        }

        // never called.
        public int SampleCB(double SampleTime, DirectShow.IMediaSample pSample)
        {
            throw new NotImplementedException();
        }
    }

    private Func<Bitmap> GetBitmapFromSampleGrabberCallback(DirectShow.ISampleGrabber i_grabber, int width, int height, int stride)
    {
        var sampler = new SampleGrabberCallback();
        i_grabber.SetCallback(sampler, 1); // WhichMethodToCallback = BufferCB
        return () => sampler.GetBitmap(width, height, stride);
    }

    /// <summary>Get Bitmap from Sample Grabber Current Buffer</summary>
    private Bitmap GetBitmapFromSampleGrabberBuffer(DirectShow.ISampleGrabber i_grabber, int width, int height, int stride)
    {
        try
        {
            return GetBitmapFromSampleGrabberBufferMain(i_grabber, width, height, stride);
        }
        catch (COMException ex)
        {
            const uint VFW_E_WRONG_STATE = 0x80040227;
            if ((uint)ex.ErrorCode == VFW_E_WRONG_STATE)
            {
                // image data is not ready yet. return empty bitmap.
                return new Bitmap(width, height);
            }

            throw;
        }
    }

    /// <summary>Get Bitmap from Sample Grabber Current Buffer</summary>
    private Bitmap GetBitmapFromSampleGrabberBufferMain(DirectShow.ISampleGrabber i_grabber, int width, int height, int stride)
    {

        int sz = 0;
        i_grabber.GetCurrentBuffer(ref sz, IntPtr.Zero); // IntPtr.Zeroで呼び出してバッファサイズ取得
        if (sz == 0) return null;

        var ptr = Marshal.AllocCoTaskMem(sz);
        i_grabber.GetCurrentBuffer(ref sz, ptr);

        var data = new byte[sz];
        Marshal.Copy(ptr, data, 0, sz);

        var result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        var bmp_data = result.LockBits(new Rectangle(Point.Empty, result.Size), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

        for (int y = 0; y < height; y++)
        {
            var src_idx = sz - (stride * (y + 1));
            var dst = IntPtr.Add(bmp_data.Scan0, stride * y);
            Marshal.Copy(data, src_idx, dst, stride);
        }
        result.UnlockBits(bmp_data);
        Marshal.FreeCoTaskMem(ptr);

        return result;
    }


    private DirectShow.IBaseFilter CreateSampleGrabber()
    {
        var filter = DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_SampleGrabber);
        var ismp = filter as DirectShow.ISampleGrabber;

        var mt = new DirectShow.AM_MEDIA_TYPE();
        mt.MajorType = DirectShow.DsGuid.MEDIATYPE_Video;
        mt.SubType = DirectShow.DsGuid.MEDIASUBTYPE_RGB24;
        ismp.SetMediaType(mt);
        return filter;
    }

    /// <summary>
    /// Video Capture Sourceフィルタを作成する
    /// </summary>
    private DirectShow.IBaseFilter CreateVideoCaptureSource(int index, VideoFormat format)
    {
        var filter = DirectShow.CreateFilter(DirectShow.DsGuid.CLSID_VideoInputDeviceCategory, index);
        var pin = DirectShow.FindPin(filter, 0, DirectShow.PIN_DIRECTION.PINDIR_OUTPUT);
        SetVideoOutputFormat(pin, format);
        return filter;
    }

    /// <summary>
    /// ビデオキャプチャデバイスの出力形式を選択する。
    /// </summary>
    private static void SetVideoOutputFormat(DirectShow.IPin pin, VideoFormat format)
    {
        var formats = GetVideoOutputFormat(pin);

        for (int i = 0; i < formats.Length; i++)
        {
            var item = formats[i];

            if (item.MajorType != DirectShow.DsGuid.GetNickname(DirectShow.DsGuid.MEDIATYPE_Video)) continue;
            if (string.IsNullOrEmpty(format.SubType) == false && format.SubType != item.SubType) continue;
            if (item.Caps.Guid != DirectShow.DsGuid.FORMAT_VideoInfo) continue;

            if (item.Size.Width == format.Size.Width && item.Size.Height == format.Size.Height)
            {
                SetVideoOutputFormat(pin, i, format.Size, format.TimePerFrame);
                return;
            }
        }

        // Not found fixed size, search for variable size.
        for (int i = 0; i < formats.Length; i++)
        {
            var item = formats[i];



            if (item.MajorType != DirectShow.DsGuid.GetNickname(DirectShow.DsGuid.MEDIATYPE_Video)) continue;
            if (string.IsNullOrEmpty(format.SubType) == false && format.SubType != item.SubType) continue;
            if (item.Caps.Guid != DirectShow.DsGuid.FORMAT_VideoInfo) continue;

            if (item.Caps.OutputGranularityX == 0) continue;
            if (item.Caps.OutputGranularityY == 0) continue;

            for (int w = item.Caps.MinOutputSize.cx; w < item.Caps.MaxOutputSize.cx; w += item.Caps.OutputGranularityX)
            {
                for (int h = item.Caps.MinOutputSize.cy; h < item.Caps.MaxOutputSize.cy; h += item.Caps.OutputGranularityY)
                {
                    if (w == format.Size.Width && h == format.Size.Height)
                    {
                        SetVideoOutputFormat(pin, i, format.Size, format.TimePerFrame);
                        return;
                    }
                }
            }
        }

        // Not found, use default size.
        SetVideoOutputFormat(pin, 0, Size.Empty, 0);
    }


    private static VideoFormat[] GetVideoOutputFormat(DirectShow.IPin pin)
    {
        var config = pin as DirectShow.IAMStreamConfig;
        if (config == null)
        {
            throw new InvalidOperationException("no IAMStreamConfig interface.");
        }

        int cap_count = 0, cap_size = 0;
        config.GetNumberOfCapabilities(ref cap_count, ref cap_size);
        if (cap_size != Marshal.SizeOf(typeof(DirectShow.VIDEO_STREAM_CONFIG_CAPS)))
        {
            throw new InvalidOperationException("no VIDEO_STREAM_CONFIG_CAPS.");
        }

        var result = new VideoFormat[cap_count];

        var cap_data = Marshal.AllocHGlobal(cap_size);

        for (int i = 0; i < cap_count; i++)
        {
            var entry = new VideoFormat();

            DirectShow.AM_MEDIA_TYPE mt = null;
            config.GetStreamCaps(i, ref mt, cap_data);
            entry.Caps = PtrToStructure<DirectShow.VIDEO_STREAM_CONFIG_CAPS>(cap_data);

            entry.MajorType = DirectShow.DsGuid.GetNickname(mt.MajorType);
            entry.SubType = DirectShow.DsGuid.GetNickname(mt.SubType);

            if (mt.FormatType == DirectShow.DsGuid.FORMAT_VideoInfo)
            {
                var vinfo = PtrToStructure<DirectShow.VIDEOINFOHEADER>(mt.pbFormat);
                entry.Size = new Size(vinfo.bmiHeader.biWidth, vinfo.bmiHeader.biHeight);
                entry.TimePerFrame = vinfo.AvgTimePerFrame;
            }
            else if (mt.FormatType == DirectShow.DsGuid.FORMAT_VideoInfo2)
            {
                var vinfo = PtrToStructure<DirectShow.VIDEOINFOHEADER2>(mt.pbFormat);
                entry.Size = new Size(vinfo.bmiHeader.biWidth, vinfo.bmiHeader.biHeight);
                entry.TimePerFrame = vinfo.AvgTimePerFrame;
            }

            // 解放
            DirectShow.DeleteMediaType(ref mt);

            result[i] = entry;
        }

        // 解放
        Marshal.FreeHGlobal(cap_data);

        return result;
    }

    private static void SetVideoOutputFormat(DirectShow.IPin pin, int index, Size size, long timePerFrame)
    {
        var config = pin as DirectShow.IAMStreamConfig;
        if (config == null)
        {
            throw new InvalidOperationException("no IAMStreamConfig interface.");
        }

        int cap_count = 0, cap_size = 0;
        config.GetNumberOfCapabilities(ref cap_count, ref cap_size);
        if (cap_size != Marshal.SizeOf(typeof(DirectShow.VIDEO_STREAM_CONFIG_CAPS)))
        {
            throw new InvalidOperationException("no VIDEO_STREAM_CONFIG_CAPS.");
        }

        var cap_data = Marshal.AllocHGlobal(cap_size);

        DirectShow.AM_MEDIA_TYPE mt = null;
        config.GetStreamCaps(index, ref mt, cap_data);
        var cap = PtrToStructure<DirectShow.VIDEO_STREAM_CONFIG_CAPS>(cap_data);

        if (mt.FormatType == DirectShow.DsGuid.FORMAT_VideoInfo)
        {
            var vinfo = PtrToStructure<DirectShow.VIDEOINFOHEADER>(mt.pbFormat);
            if (!size.IsEmpty) { vinfo.bmiHeader.biWidth = size.Width; vinfo.bmiHeader.biHeight = size.Height; }
            if (timePerFrame > 0) { vinfo.AvgTimePerFrame = timePerFrame; }
            Marshal.StructureToPtr(vinfo, mt.pbFormat, true);
        }
        else if (mt.FormatType == DirectShow.DsGuid.FORMAT_VideoInfo2)
        {
            var vinfo = PtrToStructure<DirectShow.VIDEOINFOHEADER2>(mt.pbFormat);
            if (!size.IsEmpty) { vinfo.bmiHeader.biWidth = size.Width; vinfo.bmiHeader.biHeight = size.Height; }
            if (timePerFrame > 0) { vinfo.AvgTimePerFrame = timePerFrame; }
            Marshal.StructureToPtr(vinfo, mt.pbFormat, true);
        }

        config.SetFormat(mt);

        if (cap_data != System.IntPtr.Zero) Marshal.FreeHGlobal(cap_data);
        if (mt != null) DirectShow.DeleteMediaType(ref mt);
    }

    private static T PtrToStructure<T>(IntPtr ptr)
    {
        return (T)Marshal.PtrToStructure(ptr, typeof(T));
    }

    public class VideoFormat
    {
        public string MajorType { get; set; }
        public string SubType { get; set; }
        public Size Size { get; set; }
        public long TimePerFrame { get; set; }
        public DirectShow.VIDEO_STREAM_CONFIG_CAPS Caps { get; set; }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}, {3}, {4}", MajorType, SubType, Size, TimePerFrame, CapsString());
        }

        private string CapsString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0}, ", DirectShow.DsGuid.GetNickname(Caps.Guid));
            foreach (var info in Caps.GetType().GetFields())
            {
                sb.AppendFormat("{0}={1}, ", info.Name, info.GetValue(Caps));
            }
            return sb.ToString();
        }
    }


    public static class DirectShow
    {
        #region Function

        public static object CoCreateInstance(Guid clsid)
        {
            return Activator.CreateInstance(Type.GetTypeFromCLSID(clsid));
        }

        public static void ReleaseInstance<T>(ref T com) where T : class
        {
            if (com != null)
            {
                Marshal.ReleaseComObject(com);
                com = null;
            }
        }

        public static IGraphBuilder CreateGraph()
        {
            return CoCreateInstance(DsGuid.CLSID_FilterGraph) as IGraphBuilder;
        }

        public static void PlayGraph(IGraphBuilder graph, FILTER_STATE state)
        {
            var mediaControl = graph as IMediaControl;
            if (mediaControl == null) return;

            switch (state)
            {
                case FILTER_STATE.Paused: mediaControl.Pause(); break;
                case FILTER_STATE.Stopped: mediaControl.Stop(); break;
                default: mediaControl.Run(); break;
            }
        }

        public static List<string> GetFiltes(Guid category)
        {
            var result = new List<string>();

            EnumMonikers(category, (moniker, prop) =>
            {
                object value = null;
                prop.Read("FriendlyName", ref value, 0);
                var name = (string)value;

                result.Add(name);

                return false; // 継続。
            });

            return result;
        }

        public static IBaseFilter CreateFilter(Guid clsid)
        {
            return CoCreateInstance(clsid) as IBaseFilter;
        }

        public static IBaseFilter CreateFilter(Guid category, int index)
        {
            IBaseFilter result = null;

            int curr_index = 0;
            EnumMonikers(category, (moniker, prop) =>
            {
                if (index != curr_index++) return false;

                {
                    object value = null;
                    Guid guid = DirectShow.DsGuid.IID_IBaseFilter;
                    moniker.BindToObject(null, null, ref guid, out value);
                    result = value as IBaseFilter;
                    return true;
                }
            });

            if (result == null) throw new ArgumentException("can't create filter.");
            return result;
        }

        private static void EnumMonikers(Guid category, Func<IMoniker, IPropertyBag, bool> func)
        {
            IEnumMoniker enumerator = null;
            ICreateDevEnum device = null;

            try
            {
                device = (ICreateDevEnum)Activator.CreateInstance(Type.GetTypeFromCLSID(DsGuid.CLSID_SystemDeviceEnum));
                device.CreateClassEnumerator(ref category, ref enumerator, 0);

                if (enumerator == null) return;

                var monikers = new IMoniker[1];
                var fetched = IntPtr.Zero;

                while (enumerator.Next(monikers.Length, monikers, fetched) == 0)
                {
                    var moniker = monikers[0];

                    object value = null;
                    Guid guid = DsGuid.IID_IPropertyBag;
                    moniker.BindToStorage(null, null, ref guid, out value);
                    var prop = (IPropertyBag)value;

                    try
                    {
                        var rc = func(moniker, prop);
                        if (rc == true) break;
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(prop);
                        if (moniker != null) Marshal.ReleaseComObject(moniker);
                    }
                }
            }
            finally
            {
                if (enumerator != null) Marshal.ReleaseComObject(enumerator);
                if (device != null) Marshal.ReleaseComObject(device);
            }
        }

        public static IPin FindPin(IBaseFilter filter, string name)
        {
            var result = EnumPins(filter, (info) =>
            {
                return (info.achName == name);
            });

            if (result == null) throw new ArgumentException("can't fild pin.");
            return result;
        }
        public static IPin FindPin(IBaseFilter filter, int index, PIN_DIRECTION direction)
        {
            int curr_index = 0;
            var result = EnumPins(filter, (info) =>
            {
                if (info.dir != direction) return false;

                return (index == curr_index++);
            });

            if (result == null) throw new ArgumentException("can't fild pin.");
            return result;
        }

        private static IPin EnumPins(IBaseFilter filter, Func<PIN_INFO, bool> func)
        {
            IEnumPins pins = null;
            IPin ipin = null;

            try
            {
                filter.EnumPins(ref pins);

                int fetched = 0;
                while (pins.Next(1, ref ipin, ref fetched) == 0)
                {
                    if (fetched == 0) break;

                    var info = new PIN_INFO();
                    try
                    {
                        ipin.QueryPinInfo(info);
                        var rc = func(info);
                        if (rc) return ipin;
                    }
                    finally
                    {
                        if (info.pFilter != null) Marshal.ReleaseComObject(info.pFilter);
                    }
                }
            }
            catch
            {
                if (ipin != null) Marshal.ReleaseComObject(ipin);
                throw;
            }
            finally
            {
                if (pins != null) Marshal.ReleaseComObject(pins);
            }

            return null;
        }

        public static void ConnectFilter(IGraphBuilder graph, IBaseFilter out_flt, int out_no, IBaseFilter in_flt, int in_no)
        {
            var out_pin = FindPin(out_flt, out_no, PIN_DIRECTION.PINDIR_OUTPUT);
            var inp_pin = FindPin(in_flt, in_no, PIN_DIRECTION.PINDIR_INPUT);
            graph.Connect(out_pin, inp_pin);
        }

        public static void DeleteMediaType(ref AM_MEDIA_TYPE mt)
        {
            if (mt.lSampleSize != 0) Marshal.FreeCoTaskMem(mt.pbFormat);
            if (mt.pUnk != IntPtr.Zero) Marshal.FreeCoTaskMem(mt.pUnk);
            mt = null;
        }

        #endregion


        #region Interface

        [ComVisible(true), ComImport(), Guid("56a8689f-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFilterGraph
        {
            int AddFilter([In] IBaseFilter pFilter, [In, MarshalAs(UnmanagedType.LPWStr)] string pName);
            int RemoveFilter([In] IBaseFilter pFilter);
            int EnumFilters([In, Out] ref IEnumFilters ppEnum);
            int FindFilterByName([In, MarshalAs(UnmanagedType.LPWStr)] string pName, [In, Out] ref IBaseFilter ppFilter);
            int ConnectDirect([In] IPin ppinOut, [In] IPin ppinIn, [In, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pmt);
            int Reconnect([In] IPin ppin);
            int Disconnect([In] IPin ppin);
            int SetDefaultSyncSource();
        }

        [ComVisible(true), ComImport(), Guid("56a868a9-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IGraphBuilder : IFilterGraph
        {
            int Connect([In] IPin ppinOut, [In] IPin ppinIn);
            int Render([In] IPin ppinOut);
            int RenderFile([In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFile, [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrPlayList);
            int AddSourceFilter([In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFileName, [In, MarshalAs(UnmanagedType.LPWStr)] string lpcwstrFilterName, [In, Out] ref IBaseFilter ppFilter);
            int SetLogFile(IntPtr hFile);
            int Abort();
            int ShouldOperationContinue();
        }

        [ComVisible(true), ComImport(), Guid("56a868b1-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IMediaControl
        {
            int Run();
            int Pause();
            int Stop();
            int GetState(int msTimeout, out int pfs);
            int RenderFile(string strFilename);
            int AddSourceFilter([In] string strFilename, [In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object ppUnk);
            int get_FilterCollection([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object ppUnk);
            int get_RegFilterCollection([In, Out, MarshalAs(UnmanagedType.IDispatch)] ref object ppUnk);
            int StopWhenReady();
        }

        [ComVisible(true), ComImport(), Guid("93E5A4E0-2D50-11d2-ABFA-00A0C9C6E38D"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ICaptureGraphBuilder2
        {
            int SetFiltergraph([In] IGraphBuilder pfg);
            int GetFiltergraph([In, Out] ref IGraphBuilder ppfg);
            int SetOutputFileName([In] ref Guid pType, [In, MarshalAs(UnmanagedType.LPWStr)] string lpstrFile, [In, Out] ref IBaseFilter ppbf, [In, Out] ref IFileSinkFilter ppSink);
            int FindInterface([In] ref Guid pCategory, [In] ref Guid pType, [In] IBaseFilter pbf, [In] IntPtr riid, [In, Out, MarshalAs(UnmanagedType.IUnknown)] ref object ppint);
            int RenderStream([In] ref Guid pCategory, [In] ref Guid pType, [In, MarshalAs(UnmanagedType.IUnknown)] object pSource, [In] IBaseFilter pfCompressor, [In] IBaseFilter pfRenderer);
            int ControlStream([In] ref Guid pCategory, [In] ref Guid pType, [In] IBaseFilter pFilter, [In] IntPtr pstart, [In] IntPtr pstop, [In] short wStartCookie, [In] short wStopCookie);
            int AllocCapFile([In, MarshalAs(UnmanagedType.LPWStr)] string lpstrFile, [In] long dwlSize);
            int CopyCaptureFile([In, MarshalAs(UnmanagedType.LPWStr)] string lpwstrOld, [In, MarshalAs(UnmanagedType.LPWStr)] string lpwstrNew, [In] int fAllowEscAbort, [In] IAMCopyCaptureFileProgress pFilter);
            int FindPin([In] object pSource, [In] int pindir, [In] ref Guid pCategory, [In] ref Guid pType, [In, MarshalAs(UnmanagedType.Bool)] bool fUnconnected, [In] int num, [Out] out IntPtr ppPin);
        }

        [ComVisible(true), ComImport(), Guid("a2104830-7c70-11cf-8bce-00aa00a3f1a6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IFileSinkFilter
        {
            int SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszFileName, [In, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pmt);
            int GetCurFile([In, Out, MarshalAs(UnmanagedType.LPWStr)] ref string pszFileName, [Out, MarshalAs(UnmanagedType.LPStruct)] out AM_MEDIA_TYPE pmt);
        }

        [ComVisible(true), ComImport(), Guid("670d1d20-a068-11d0-b3f0-00aa003761c5"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAMCopyCaptureFileProgress
        {
            int Progress(int iProgress);
        }


        [ComVisible(true), ComImport(), Guid("C6E13370-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAMCameraControl
        {
            int GetRange([In] CameraControlProperty Property, [In, Out] ref int pMin, [In, Out] ref int pMax, [In, Out] ref int pSteppingDelta, [In, Out] ref int pDefault, [In, Out] ref int pCapsFlag);
            int Set([In] CameraControlProperty Property, [In] int lValue, [In] int Flags);
            int Get([In] CameraControlProperty Property, [In, Out] ref int lValue, [In, Out] ref int Flags);
        }


        [ComVisible(true), ComImport(), Guid("C6E13360-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAMVideoProcAmp
        {
            int GetRange([In] VideoProcAmpProperty Property, [In, Out] ref int pMin, [In, Out] ref int pMax, [In, Out] ref int pSteppingDelta, [In, Out] ref int pDefault, [In, Out] ref int pCapsFlag);
            int Set([In] VideoProcAmpProperty Property, [In] int lValue, [In] int Flags);
            int Get([In] VideoProcAmpProperty Property, [In, Out] ref int lValue, [In, Out] ref int Flags);
        }


        [ComVisible(true), ComImport(), Guid("6A2E0670-28E4-11D0-A18C-00A0C9118956"), System.Security.SuppressUnmanagedCodeSecurity, InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAMVideoControl
        {
            int GetCaps([In] IPin pPin, [Out] out int pCapsFlags);
            int SetMode([In] IPin pPin, [In] int Mode);
            int GetMode([In] IPin pPin, [Out] out int Mode);
            int GetCurrentActualFrameRate([In] IPin pPin, [Out] out long ActualFrameRate);
            int GetMaxAvailableFrameRate([In] IPin pPin, [In] int iIndex, [In] Size Dimensions, [Out] out long MaxAvailableFrameRate);
            int GetFrameRateList([In] IPin pPin, [In] int iIndex, [In] Size Dimensions, [Out] out int ListSize, [Out] out IntPtr FrameRates);
        }

        [ComVisible(true), ComImport(), Guid("56a86895-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IBaseFilter
        {
            // Inherits IPersist
            int GetClassID([Out] out Guid pClassID);

            // Inherits IMediaControl
            int Stop();
            int Pause();
            int Run(long tStart);
            int GetState(int dwMilliSecsTimeout, [In, Out] ref int filtState);
            int SetSyncSource([In] IReferenceClock pClock);
            int GetSyncSource([In, Out] ref IReferenceClock pClock);

            // -----
            int EnumPins([In, Out] ref IEnumPins ppEnum);
            int FindPin([In, MarshalAs(UnmanagedType.LPWStr)] string Id, [In, Out] ref IPin ppPin);
            int QueryFilterInfo([Out] FILTER_INFO pInfo);
            int JoinFilterGraph([In] IFilterGraph pGraph, [In, MarshalAs(UnmanagedType.LPWStr)] string pName);
            int QueryVendorInfo([In, Out, MarshalAs(UnmanagedType.LPWStr)] ref string pVendorInfo);
        }


        [ComVisible(true), ComImport(), Guid("56a86893-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumFilters
        {
            int Next([In] int cFilters, [In, Out] ref IBaseFilter ppFilter, [In, Out] ref int pcFetched);
            int Skip([In] int cFilters);
            void Reset();
            void Clone([In, Out] ref IEnumFilters ppEnum);
        }

        [ComVisible(true), ComImport(), Guid("C6E13340-30AC-11d0-A18C-00A0C9118956"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IAMStreamConfig
        {
            int SetFormat([In, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pmt);
            int GetFormat([In, Out, MarshalAs(UnmanagedType.LPStruct)] ref AM_MEDIA_TYPE ppmt);
            int GetNumberOfCapabilities(ref int piCount, ref int piSize);
            int GetStreamCaps(int iIndex, [In, Out, MarshalAs(UnmanagedType.LPStruct)] ref AM_MEDIA_TYPE ppmt, IntPtr pSCC);
        }

        [ComVisible(true), ComImport(), Guid("56a8689a-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IMediaSample
        {
            int GetPointer(ref IntPtr ppBuffer);
            int GetSize();
            int GetTime(ref long pTimeStart, ref long pTimeEnd);
            int SetTime([In, MarshalAs(UnmanagedType.LPStruct)] UInt64 pTimeStart, [In, MarshalAs(UnmanagedType.LPStruct)] UInt64 pTimeEnd);
            int IsSyncPoint();
            int SetSyncPoint([In, MarshalAs(UnmanagedType.Bool)] bool bIsSyncPoint);
            int IsPreroll();
            int SetPreroll([In, MarshalAs(UnmanagedType.Bool)] bool bIsPreroll);
            int GetActualDataLength();
            int SetActualDataLength(int len);
            int GetMediaType([In, Out, MarshalAs(UnmanagedType.LPStruct)] ref AM_MEDIA_TYPE ppMediaType);
            int SetMediaType([In, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pMediaType);
            int IsDiscontinuity();
            int SetDiscontinuity([In, MarshalAs(UnmanagedType.Bool)] bool bDiscontinuity);
            int GetMediaTime(ref long pTimeStart, ref long pTimeEnd);
            int SetMediaTime([In, MarshalAs(UnmanagedType.LPStruct)] UInt64 pTimeStart, [In, MarshalAs(UnmanagedType.LPStruct)] UInt64 pTimeEnd);
        }

        [ComVisible(true), ComImport(), Guid("89c31040-846b-11ce-97d3-00aa0055595a"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumMediaTypes
        {
            int Next([In] int cMediaTypes, [In, Out, MarshalAs(UnmanagedType.LPStruct)] ref AM_MEDIA_TYPE ppMediaTypes, [In, Out] ref int pcFetched);
            int Skip([In] int cMediaTypes);
            int Reset();
            int Clone([In, Out] ref IEnumMediaTypes ppEnum);
        }

        [ComVisible(true), ComImport(), Guid("56a86891-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPin
        {
            int Connect([In] IPin pReceivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pmt);
            int ReceiveConnection([In] IPin pReceivePin, [In, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pmt);
            int Disconnect();
            int ConnectedTo([In, Out] ref IPin ppPin);
            int ConnectionMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pmt);
            int QueryPinInfo([Out] PIN_INFO pInfo);
            int QueryDirection(ref PIN_DIRECTION pPinDir);
            int QueryId([In, Out, MarshalAs(UnmanagedType.LPWStr)] ref string Id);
            int QueryAccept([In, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pmt);
            int EnumMediaTypes([In, Out] ref IEnumMediaTypes ppEnum);
            int QueryInternalConnections(IntPtr apPin, [In, Out] ref int nPin);
            int EndOfStream();
            int BeginFlush();
            int EndFlush();
            int NewSegment(long tStart, long tStop, double dRate);
        }

        [ComVisible(true), ComImport(), Guid("56a86892-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IEnumPins
        {
            int Next([In] int cPins, [In, Out] ref IPin ppPins, [In, Out] ref int pcFetched);
            int Skip([In] int cPins);
            void Reset();
            void Clone([In, Out] ref IEnumPins ppEnum);
        }

        [ComVisible(true), ComImport(), Guid("56a86897-0ad4-11ce-b03a-0020af0ba770"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IReferenceClock
        {
            int GetTime(ref long pTime);
            int AdviseTime(long baseTime, long streamTime, IntPtr hEvent, ref int pdwAdviseCookie);
            int AdvisePeriodic(long startTime, long periodTime, IntPtr hSemaphore, ref int pdwAdviseCookie);
            int Unadvise(int dwAdviseCookie);
        }

        [ComVisible(true), ComImport(), Guid("29840822-5B84-11D0-BD3B-00A0C911CE86"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ICreateDevEnum
        {
            int CreateClassEnumerator([In] ref Guid pType, [In, Out] ref System.Runtime.InteropServices.ComTypes.IEnumMoniker ppEnumMoniker, [In] int dwFlags);
        }

        [ComVisible(true), ComImport(), Guid("55272A00-42CB-11CE-8135-00AA004BB851"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IPropertyBag
        {
            int Read([MarshalAs(UnmanagedType.LPWStr)] string PropName, ref object Var, int ErrorLog);
            int Write(string PropName, ref object Var);
        }

        [ComVisible(true), ComImport(), Guid("6B652FFF-11FE-4fce-92AD-0266B5D7C78F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISampleGrabber
        {
            int SetOneShot([In, MarshalAs(UnmanagedType.Bool)] bool OneShot);
            int SetMediaType([In, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pmt);
            int GetConnectedMediaType([Out, MarshalAs(UnmanagedType.LPStruct)] AM_MEDIA_TYPE pmt);
            int SetBufferSamples([In, MarshalAs(UnmanagedType.Bool)] bool BufferThem);
            int GetCurrentBuffer(ref int pBufferSize, IntPtr pBuffer);
            int GetCurrentSample(IntPtr ppSample);
            int SetCallback(ISampleGrabberCB pCallback, int WhichMethodToCallback);
        }

        [ComVisible(true), ComImport(), Guid("0579154A-2B53-4994-B0D0-E773148EFF85"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface ISampleGrabberCB
        {
            [PreserveSig()]
            int SampleCB(double SampleTime, IMediaSample pSample);
            [PreserveSig()]
            int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen);
        }

        #endregion


        #region Structure

        [Serializable]
        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public class AM_MEDIA_TYPE
        {
            public Guid MajorType;
            public Guid SubType;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bFixedSizeSamples;
            [MarshalAs(UnmanagedType.Bool)]
            public bool bTemporalCompression;
            public uint lSampleSize;
            public Guid FormatType;
            public IntPtr pUnk;
            public uint cbFormat;
            public IntPtr pbFormat;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), ComVisible(false)]
        public class FILTER_INFO
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string achName;
            [MarshalAs(UnmanagedType.IUnknown)]
            public object pGraph;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode), ComVisible(false)]
        public class PIN_INFO
        {
            public IBaseFilter pFilter;
            public PIN_DIRECTION dir;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string achName;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, Pack = 8), ComVisible(false)]
        public struct VIDEO_STREAM_CONFIG_CAPS
        {
            public Guid Guid;
            public uint VideoStandard;
            public SIZE InputSize;
            public SIZE MinCroppingSize;
            public SIZE MaxCroppingSize;
            public int CropGranularityX;
            public int CropGranularityY;
            public int CropAlignX;
            public int CropAlignY;
            public SIZE MinOutputSize;
            public SIZE MaxOutputSize;
            public int OutputGranularityX;
            public int OutputGranularityY;
            public int StretchTapsX;
            public int StretchTapsY;
            public int ShrinkTapsX;
            public int ShrinkTapsY;
            public long MinFrameInterval;
            public long MaxFrameInterval;
            public int MinBitsPerSecond;
            public int MaxBitsPerSecond;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct VIDEOINFOHEADER
        {
            public RECT SrcRect;
            public RECT TrgRect;
            public int BitRate;
            public int BitErrorRate;
            public long AvgTimePerFrame;
            public BITMAPINFOHEADER bmiHeader;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct VIDEOINFOHEADER2
        {
            public RECT SrcRect;
            public RECT TrgRect;
            public int BitRate;
            public int BitErrorRate;
            public long AvgTimePerFrame;
            public int InterlaceFlags;
            public int CopyProtectFlags;
            public int PictAspectRatioX;
            public int PictAspectRatioY;
            public int ControlFlags; // or Reserved1
            public int Reserved2;
            public BITMAPINFOHEADER bmiHeader;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, Pack = 2), ComVisible(false)]
        public struct BITMAPINFOHEADER
        {
            public int biSize;
            public int biWidth;
            public int biHeight;
            public short biPlanes;
            public short biBitCount;
            public int biCompression;
            public int biSizeImage;
            public int biXPelsPerMeter;
            public int biYPelsPerMeter;
            public int biClrUsed;
            public int biClrImportant;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct WAVEFORMATEX
        {
            public ushort wFormatTag;
            public ushort nChannels;
            public uint nSamplesPerSec;
            public uint nAvgBytesPerSec;
            public short nBlockAlign;
            public short wBitsPerSample;
            public short cbSize;
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential, Pack = 8), ComVisible(false)]
        public struct SIZE
        {
            public int cx;
            public int cy;
            public override string ToString() { return string.Format("{{{0}, {1}}}", cx, cy); } // for debugging.
        }

        [Serializable]
        [StructLayout(LayoutKind.Sequential), ComVisible(false)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
            public override string ToString() { return string.Format("{{{0}, {1}, {2}, {3}}}", Left, Top, Right, Bottom); } // for debugging.
        }
        #endregion


        #region Enum

        [ComVisible(false)]
        public enum PIN_DIRECTION
        {
            PINDIR_INPUT = 0,
            PINDIR_OUTPUT = 1,
        }

        [ComVisible(false)]
        public enum FILTER_STATE : int
        {
            Stopped = 0,
            Paused = 1,
            Running = 2,
        }

        [ComVisible(false)]
        public enum CameraControlProperty
        {
            Pan = 0,
            Tilt = 1,
            Roll = 2,
            Zoom = 3,
            Exposure = 4,
            Iris = 5,
            Focus = 6,
        }

        [ComVisible(false), Flags()]
        public enum CameraControlFlags
        {
            Auto = 0x0001,
            Manual = 0x0002,
        }

        [ComVisible(false)]
        public enum VideoProcAmpProperty
        {
            Brightness = 0,
            Contrast = 1,
            Hue = 2,
            Saturation = 3,
            Sharpness = 4,
            Gamma = 5,
            ColorEnable = 6,
            WhiteBalance = 7,
            BacklightCompensation = 8,
            Gain = 9
        }

        #endregion


        #region Guid

        public static class DsGuid
        {
            // MediaType
            public static readonly Guid MEDIATYPE_Video = new Guid("{73646976-0000-0010-8000-00AA00389B71}");
            public static readonly Guid MEDIATYPE_Audio = new Guid("{73647561-0000-0010-8000-00AA00389B71}");

            // SubType
            public static readonly Guid MEDIASUBTYPE_None = new Guid("{E436EB8E-524F-11CE-9F53-0020AF0BA770}");
            public static readonly Guid MEDIASUBTYPE_YUYV = new Guid("{56595559-0000-0010-8000-00AA00389B71}");
            public static readonly Guid MEDIASUBTYPE_IYUV = new Guid("{56555949-0000-0010-8000-00AA00389B71}");
            public static readonly Guid MEDIASUBTYPE_YVU9 = new Guid("{39555659-0000-0010-8000-00AA00389B71}");
            public static readonly Guid MEDIASUBTYPE_YUY2 = new Guid("{32595559-0000-0010-8000-00AA00389B71}");
            public static readonly Guid MEDIASUBTYPE_YVYU = new Guid("{55595659-0000-0010-8000-00AA00389B71}");
            public static readonly Guid MEDIASUBTYPE_UYVY = new Guid("{59565955-0000-0010-8000-00AA00389B71}");
            public static readonly Guid MEDIASUBTYPE_MJPG = new Guid("{47504A4D-0000-0010-8000-00AA00389B71}");
            public static readonly Guid MEDIASUBTYPE_RGB565 = new Guid("{E436EB7B-524F-11CE-9F53-0020AF0BA770}");
            public static readonly Guid MEDIASUBTYPE_RGB555 = new Guid("{E436EB7C-524F-11CE-9F53-0020AF0BA770}");
            public static readonly Guid MEDIASUBTYPE_RGB24 = new Guid("{E436EB7D-524F-11CE-9F53-0020AF0BA770}");
            public static readonly Guid MEDIASUBTYPE_RGB32 = new Guid("{E436EB7E-524F-11CE-9F53-0020AF0BA770}");
            public static readonly Guid MEDIASUBTYPE_ARGB32 = new Guid("{773C9AC0-3274-11D0-B724-00AA006C1A01}");
            public static readonly Guid MEDIASUBTYPE_PCM = new Guid("{00000001-0000-0010-8000-00AA00389B71}");
            public static readonly Guid MEDIASUBTYPE_WAVE = new Guid("{E436EB8B-524F-11CE-9F53-0020AF0BA770}");

            // FormatType
            public static readonly Guid FORMAT_None = new Guid("{0F6417D6-C318-11D0-A43F-00A0C9223196}");
            public static readonly Guid FORMAT_VideoInfo = new Guid("{05589F80-C356-11CE-BF01-00AA0055595A}");
            public static readonly Guid FORMAT_VideoInfo2 = new Guid("{F72A76A0-EB0A-11d0-ACE4-0000C0CC16BA}");
            public static readonly Guid FORMAT_WaveFormatEx = new Guid("{05589F81-C356-11CE-BF01-00AA0055595A}");

            // CLSID
            public static readonly Guid CLSID_AudioInputDeviceCategory = new Guid("{33D9A762-90C8-11d0-BD43-00A0C911CE86}");
            public static readonly Guid CLSID_AudioRendererCategory = new Guid("{E0F158E1-CB04-11d0-BD4E-00A0C911CE86}");
            public static readonly Guid CLSID_VideoInputDeviceCategory = new Guid("{860BB310-5D01-11d0-BD3B-00A0C911CE86}");
            public static readonly Guid CLSID_VideoCompressorCategory = new Guid("{33D9A760-90C8-11d0-BD43-00A0C911CE86}");

            public static readonly Guid CLSID_NullRenderer = new Guid("{C1F400A4-3F08-11D3-9F0B-006008039E37}");
            public static readonly Guid CLSID_SampleGrabber = new Guid("{C1F400A0-3F08-11D3-9F0B-006008039E37}");

            public static readonly Guid CLSID_FilterGraph = new Guid("{E436EBB3-524F-11CE-9F53-0020AF0BA770}");
            public static readonly Guid CLSID_SystemDeviceEnum = new Guid("{62BE5D10-60EB-11d0-BD3B-00A0C911CE86}");
            public static readonly Guid CLSID_CaptureGraphBuilder2 = new Guid("{BF87B6E1-8C27-11d0-B3F0-00AA003761C5}");

            public static readonly Guid IID_IPropertyBag = new Guid("{55272A00-42CB-11CE-8135-00AA004BB851}");
            public static readonly Guid IID_IBaseFilter = new Guid("{56a86895-0ad4-11ce-b03a-0020af0ba770}");
            public static readonly Guid IID_IAMStreamConfig = new Guid("{C6E13340-30AC-11d0-A18C-00A0C9118956}");

            public static readonly Guid PIN_CATEGORY_CAPTURE = new Guid("{fb6c4281-0353-11d1-905f-0000c0cc16ba}");
            public static readonly Guid PIN_CATEGORY_PREVIEW = new Guid("{fb6c4282-0353-11d1-905f-0000c0cc16ba}");
            public static readonly Guid PIN_CATEGORY_STILL = new Guid("{fb6c428a-0353-11d1-905f-0000c0cc16ba}");

            private static Dictionary<Guid, string> NicknameCache = null;

            public static string GetNickname(Guid guid)
            {
                if (NicknameCache == null)
                {
                    NicknameCache = typeof(DsGuid).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                        .Where(x => x.FieldType == typeof(Guid))
                        .ToDictionary(x => (Guid)x.GetValue(null), x => x.Name);
                }

                if (NicknameCache.ContainsKey(guid))
                {
                    var name = NicknameCache[guid];
                    var elem = name.Split('_');

                    if (elem.Length >= 2)
                    {
                        var text = string.Join("_", elem.Skip(1).ToArray());
                        return string.Format("[{0}]", text);
                    }
                    else
                    {
                        return name;
                    }
                }

                return guid.ToString();
            }
        }
        #endregion
    }
}
}
