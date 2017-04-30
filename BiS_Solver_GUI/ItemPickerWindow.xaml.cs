using System.Linq;
using System.Text;
using System.Windows;
using SaintCoinach;
using SaintCoinach.Xiv;

namespace BiS_Solver_GUI
{
    /// <summary>
    /// Interaction logic for ItemPickerWindow.xaml
    /// </summary>
    public partial class ItemPickerWindow
    {
        public string IdList;

        public string Foo => IdList;

        public ItemPickerWindow()
        {
            InitializeComponent();
            PopulateDatagrid();
        }

        private void PopulateDatagrid()
        {
            var realm = new ARealmReversed(Properties.Settings.Default.gamepath, "FFXIVBisSolverCLI\\SaintCoinach.History.zip", SaintCoinach.Ex.Language.English);
            var itemDataSheet = realm.GameData.GetSheet<Item>();

            // Alexandrian
            var i = 16465; // 16465 start index of alexandrian items
            var upperLimit = 16540; //end index of alexandrian items
            while (i < upperLimit)
            {
                DgdTest.Items.Add(itemDataSheet[i]);
                i++;
            }

            // Augmented Shire
            i = 16327;
            upperLimit = 16402;
            while (i < upperLimit)
            {
                DgdTest.Items.Add(itemDataSheet[i]);
                i++;
            }
        }

        private void BtnSelectItems_Click(object sender, RoutedEventArgs e)
        {
            var rows = DgdTest.SelectedItems.OfType<XivRow>().ToList();

            StringBuilder idListBuilder = new StringBuilder();
            foreach (var row in rows)
            {
                idListBuilder.Append(row == rows.Last() ? $"{row.Key}" : $"{row.Key} "); // Don't add a space after the last item so it doesn't break GetSavagePresetExcludes()
            }
            IdList = idListBuilder.ToString();
            Close();
        }
    }
}
