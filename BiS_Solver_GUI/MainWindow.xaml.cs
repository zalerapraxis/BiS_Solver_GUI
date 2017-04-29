using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms.ComponentModel;

namespace BiS_Solver_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int SolverPid;
        public string SolverOutput;
        public MainWindow()
        {
            InitializeComponent();
            txtGamePath.Text = Properties.Settings.Default.gamepath;
        }

        private void btnChangeGamePath_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"You will now be asked to select the path your game is installed in. It should contain the folders ""game"" and ""boot"".");

            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default["gamepath"] = dialog.SelectedPath;
                Properties.Settings.Default.Save();
                txtGamePath.Text = Properties.Settings.Default.gamepath;
            }
        }

        private void btnXIVDB_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://xivdb.com");
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (((ComboBoxItem)cmbJobs.SelectedItem).Name == null)
            {
                MessageBox.Show("You must specify a class before starting the solver.");
                return;
            }
            KillRunningSolvers();
            var job = ((ComboBoxItem)cmbJobs.SelectedItem).Name;
            var gamepath = Properties.Settings.Default.gamepath;
            var miscFlags = GetMiscFlags();
            var savageExcludeFlags = GetSavageExcludes();
            var specificIdExcludes = txtExcludeIDs.Text;
            var specificIdIncludes = txtIncludeIDs.Text;

            var launchCmdBuilder = new StringBuilder();
            launchCmdBuilder.Append($"{job} ");
            launchCmdBuilder.Append($"-p \"{gamepath}\" ");
            launchCmdBuilder.Append(miscFlags);
            launchCmdBuilder.Append(savageExcludeFlags);
            launchCmdBuilder.Append($"{specificIdExcludes} ");
            launchCmdBuilder.Append($"{specificIdIncludes} ");
            var solverLaunchArgs = launchCmdBuilder.ToString();

            LaunchSolver(solverLaunchArgs);
        }

        private void btnCancelSolver_Click(object sender, RoutedEventArgs e)
        {
            KillRunningSolvers();
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

            Height = 640;
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
                txtSolverOutput.ScrollToEnd();
            });
            
        }

        private string GetMiscFlags()
        {
            var miscFlagsBuilder = new StringBuilder();
            if (chkSSTiers.IsChecked == true)
            {
                miscFlagsBuilder.Append("--use-tiers ");
            }
            if (chkFood.IsChecked == false)
            {
                miscFlagsBuilder.Append("--no-food ");
            }
            if (chkRelic.IsChecked == false)
            {
                miscFlagsBuilder.Append("--no-relic ");
            }
            var miscFlags = miscFlagsBuilder.ToString();
            return miscFlags;
        }

        private string GetSavageExcludes()
        {
            var savageExcludes = "";
            // Build excludes list for Savage fights
            if (optLimitAll.IsChecked == true)
            {
                savageExcludes = Properties.Resources.a9sExcludes + Properties.Resources.a11sExcludes +
                                        Properties.Resources.a10sExcludes + Properties.Resources.a12sExcludes;
            }
            if (optA9S.IsChecked == true)
            {
                savageExcludes = Properties.Resources.a10sExcludes + Properties.Resources.a11sExcludes + 
                                        Properties.Resources.a12sExcludes;
            }
            if (optA10S.IsChecked == true)
            {
                savageExcludes = Properties.Resources.a11sExcludes + Properties.Resources.a12sExcludes;
            }
            if (optA11S.IsChecked == true)
            {
                savageExcludes = Properties.Resources.a12sExcludes;
            }
            return savageExcludes;
        }

        private void KillRunningSolvers()
        {
            Process p = null;
            try
            {
                p = Process.GetProcessById(SolverPid);
                p.Kill();
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            KillRunningSolvers();
        }
    }
}
