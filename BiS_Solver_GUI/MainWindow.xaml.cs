using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BiS_Solver_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            TxtGamePath.Text = Properties.Settings.Default.gamepath;
        }

        private void BtnChangeGamePath_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(@"You will now be asked to select the path your game is installed in. It should contain the folders ""game"" and ""boot"".");

            var dialog = new System.Windows.Forms.FolderBrowserDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Properties.Settings.Default["gamepath"] = dialog.SelectedPath;
                Properties.Settings.Default.Save();
                TxtGamePath.Text = Properties.Settings.Default.gamepath;
            }
        }

        private void BtnXIVDB_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://xivdb.com");
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (((ComboBoxItem)CmbJobs.SelectedItem).Name == null)
            {
                MessageBox.Show("You must specify a class before starting the solver.");
                return;
            }
            var job = ((ComboBoxItem)CmbJobs.SelectedItem).Name;
            var gamepath = Properties.Settings.Default.gamepath;
            var miscFlags = GetMiscFlags();
            var savageExcludeFlags = GetSavagePresetExcludes();
            var specificIdExcludes = GetSpecificExcludes();
            var specificIdIncludes = GetSpecificIncludes();

            var launchCmdBuilder = new StringBuilder();
            launchCmdBuilder.Append($"{job} ");
            launchCmdBuilder.Append($"-p \"{gamepath}\" ");
            launchCmdBuilder.Append(miscFlags);
            launchCmdBuilder.Append(savageExcludeFlags);
            launchCmdBuilder.Append($"{specificIdExcludes} ");
            launchCmdBuilder.Append($"{specificIdIncludes} ");
            var solverLaunchArgs = launchCmdBuilder.ToString();

            SolverWindow solverWindow = new SolverWindow(solverLaunchArgs);
            solverWindow.Show();
            ToggleStartButtonEnabled(); // temp disable start button to stop rapid launches, prevents solver from crashing
        }

        private string GetMiscFlags()
        {
            var miscFlagsBuilder = new StringBuilder();
            if (ChkSsTiers.IsChecked == true)
                miscFlagsBuilder.Append("--use-tiers ");
            if (ChkFood.IsChecked == false)
                miscFlagsBuilder.Append("--no-food ");
            if (ChkRelic.IsChecked == false)
                miscFlagsBuilder.Append("--no-relic ");
            var miscFlags = miscFlagsBuilder.ToString();
            return miscFlags;
        }

        private string GetSavagePresetExcludes()
        {
            var savageExcludes = "";
            // Build excludes list for Savage fights
            if (OptLimitAll.IsChecked == true)
                savageExcludes = Properties.Resources.a9sExcludes + Properties.Resources.a11sExcludes +
                                        Properties.Resources.a10sExcludes + Properties.Resources.a12sExcludes;
            if (OptA9S.IsChecked == true)
                savageExcludes = Properties.Resources.a10sExcludes + Properties.Resources.a11sExcludes + 
                                        Properties.Resources.a12sExcludes;
            if (OptA10S.IsChecked == true)
                savageExcludes = Properties.Resources.a11sExcludes + Properties.Resources.a12sExcludes;
            if (OptA11S.IsChecked == true)
                savageExcludes = Properties.Resources.a12sExcludes;

            // Clean preset excludes if user has specified IDs to include
            if (!string.IsNullOrWhiteSpace(TxtIncludeIDs.Text))
            {
                var includeIds = TxtIncludeIDs.Text.Split(' ');
                foreach (var id in includeIds)
                {
                    if (savageExcludes.Contains(id))
                    {
                        var idIndex = savageExcludes.IndexOf(id);
                        savageExcludes = savageExcludes.Remove(idIndex - 3, 9);
                    }
                }
            }

            return savageExcludes;
        }

        private string GetSpecificExcludes()
        {
            var ids = TxtExcludeIDs.Text.Split(' ');
            var output = "";

            if (!string.IsNullOrWhiteSpace(TxtExcludeIDs.Text))
            {
                var specificExcludesBuilder = new StringBuilder();
                foreach (var id in ids)
                {
                    specificExcludesBuilder.Append($"-X {id}");
                }
                output = specificExcludesBuilder.ToString();
            }
            return output;
        }

        private string GetSpecificIncludes()
        {
            var ids = TxtIncludeIDs.Text.Split(' ');
            var output = "";

            if (!string.IsNullOrWhiteSpace(TxtIncludeIDs.Text))
            {
                var specificIncludesBuilder = new StringBuilder();
                foreach (var id in ids)
                {
                    specificIncludesBuilder.Append($"-R {id} ");
                }
                output = specificIncludesBuilder.ToString();
            }
            return output;
        }

        private async void ToggleStartButtonEnabled()
        {
            BtnStart.IsEnabled = false;
            await Task.Delay(5000);
            BtnStart.IsEnabled = true;
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            foreach (var process in Process.GetProcessesByName("FFXIVBisSolverCLI"))
            {
                process.Kill();
            }
            Application.Current.Shutdown();
        }
    }
}
