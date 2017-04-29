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
            var job = ((ComboBoxItem)cmbJobs.SelectedItem).Name;
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

        private string GetSavagePresetExcludes()
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

            // Clean preset excludes if user has specified IDs to include specifically
            if (!string.IsNullOrWhiteSpace(txtIncludeIDs.Text))
            {
                var includeIds = txtIncludeIDs.Text.Split(' ');
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
            var ids = txtExcludeIDs.Text.Split(' ');
            var output = "";

            if (!string.IsNullOrWhiteSpace(txtExcludeIDs.Text))
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
            var ids = txtIncludeIDs.Text.Split(' ');
            var output = "";

            if (!string.IsNullOrWhiteSpace(txtIncludeIDs.Text))
            {
                var specificIncludesBuilder = new StringBuilder();
                foreach (var id in ids)
                {
                    specificIncludesBuilder.Append($"-R {id} ");
                }
                output = specificIncludesBuilder.ToString();

                //savageExcludeFlags = savageExcludeFlags.Replace(specificIdIncludes, "");
            }
            return output;
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
