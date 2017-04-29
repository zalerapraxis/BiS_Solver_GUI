using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiS_Solver_GUI
{
    class Settings
    {
        public static string GetGamePath()
        {
            return Properties.Settings.Default.gamepath;
        }
    }
}
