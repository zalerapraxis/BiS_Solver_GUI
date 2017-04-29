using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BiS_Solver_GUI
{
    /// <summary>
    /// Interaction logic for SolverWindow.xaml
    /// </summary>
    public partial class SolverWindow : Window
    {
        public int SolverPid;
        public string SolverOutput;

        public SolverWindow(string solverLaunchArgs)
        {
            InitializeComponent();
            LaunchSolver(solverLaunchArgs);
        }

        private void LaunchSolver(string solverLaunchArgs)
        {
            txtSolverOutput.Text = "Loading...";
            Process solver = new Process();
            ProcessStartInfo startInfo =
                new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    WorkingDirectory = "FFXIVBisSolverCLI",
                    FileName = @"FFXIVBisSolverCLI\FFXIVBisSolverCLI.exe",
                    Arguments = solverLaunchArgs,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
            solver.StartInfo = startInfo;
            solver.OutputDataReceived += OutputHandler;
            solver.Start();
            solver.BeginOutputReadLine();
            SolverPid = solver.Id;

            Title = solverLaunchArgs;
        }

        private void OutputHandler(object sendingProcess, DataReceivedEventArgs output)
        {
            Dispatcher.Invoke(() =>
            {
                SolverOutput = SolverOutput + output.Data + Environment.NewLine;
                if (output.Data == "INTEGER OPTIMAL SOLUTION FOUND")
                {
                    SolverOutput = "";
                }
                txtSolverOutput.Text = SolverOutput;
            });
        }

        private void KillRunningSolver()
        {
            try
            {
                var process = Process.GetProcessById(SolverPid);
                process.Kill();
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        private void btnKillSolver_Click(object sender, RoutedEventArgs e)
        {
            KillRunningSolver();
            txtSolverOutput.Text = "";
            Close();
        }
    }
}
