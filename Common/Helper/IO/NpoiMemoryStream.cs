using System.IO;

namespace Common.IO
{
    //新建类 重写Npoi流方法(原始方法会在导出2007版本时出现数据流中断的问题)
    public class NpoiMemoryStream : MemoryStream
    {
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}
