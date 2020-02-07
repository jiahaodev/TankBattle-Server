using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankBattle_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!DbManager.Connect("tankbattle", "127.0.0.1", 3306, "root", ""))
            {
                return;
            }

            NetManager.StartNetLoop(8888);
        }
    }
}
