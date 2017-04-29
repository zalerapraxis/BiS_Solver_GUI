using System;
using System.Diagnostics;
using System.Windows;

namespace BiS_Solver_GUI
{
    /// <summary>
    /// Interaction logic for SolverWindow.xaml
    /// </summary>
    public partial class SolverWindow
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
            TxtSolverOutput.Text = "Loading...";
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
                if (output.Data == "INTEGER OPTIMAL SOLUTION FOUND") // Gearset found, clear the output and post just the gearset
                {
                    SolverOutput = "";
                }
                TxtSolverOutput.Text = SolverOutput;
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
            TxtSolverOutput.Text = "";
            Close();
        }
    }
}
