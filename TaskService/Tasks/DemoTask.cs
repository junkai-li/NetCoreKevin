using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Repository.Database;

namespace TaskService.Tasks
{
    class DemoTask
    {

        private readonly dbContext db;

        public DemoTask(dbContext context)
        {
            db = context;
        }

        public void Run()
        {
            var tim = new Timer(1000 * 3);

            tim.Elapsed += TimElapsed;
            tim.Start();
        }



        private void TimElapsed(object sender, ElapsedEventArgs e)
        {
            //周期性执行的方法
            var x = db.TUser.ToList();
            Console.WriteLine("HelloWord");
        }

    }
}
