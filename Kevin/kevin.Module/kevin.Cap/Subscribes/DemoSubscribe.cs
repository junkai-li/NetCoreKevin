using DotNetCore.CAP;
using System;

namespace kevin.Cap
{
    public class DemoSubscribe : ICapSubscribe
    {

        [CapSubscribe("ShowMessage", Group = "Group1")]
        public void ShowMessage(string message)
        {
            var inst=int.Parse(message);
            Task.Run(() =>
            {
                Console.WriteLine("ShowMessageGroup1" + DateTime.Now + inst.ToString());
            });

        }
        [CapSubscribe("ShowMessage", Group = "Group2")]
        public void ShowMessage2(string message)
        {
            var inst = int.Parse(message);
            Task.Run(() =>
            {
                Console.WriteLine("ShowMessage2Group2" + DateTime.Now + inst.ToString());
            });

        }
    }
}
