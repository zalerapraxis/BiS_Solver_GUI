using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
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
using SaintCoinach;
using SaintCoinach.Xiv;
using Action = System.Action;

namespace BiS_Solver_GUI
{
    /// <summary>
    /// Interaction logic for ItemPickerWindow.xaml
    /// </summary>
    public partial class ItemPickerWindow : Window
    {
        public event Action<string> ReturnValue;
        public string idList;

        public string Foo
        {
            get { return idList; }
        }

        public ItemPickerWindow()
        {
            InitializeComponent();
            if (ReturnValue != null)
                ReturnValue("hello world");
            PopulateDatagrid();
        }

        private void PopulateDatagrid()
        {
            var realm = new ARealmReversed(Properties.Settings.Default.gamepath, SaintCoinach.Ex.Language.English);
            var itemDataSheet = realm.GameData.GetSheet<Item>();

            var i = 16465; // 16465 start index of alexandrian items
            var upperLimit = 16540; //end index of alexandrian items

            while (i < upperLimit)
            {
                dgdTest.Items.Add(itemDataSheet[i]);
                i++;
            }
        }

        private void btnSelectItems_Click(object sender, RoutedEventArgs e)
        {
            var rows = dgdTest.SelectedItems.OfType<XivRow>().ToList();

            StringBuilder idListBuilder = new StringBuilder();
            foreach (var row in rows)
            {
                idListBuilder.Append($"{row.Key} ");
            }
            idList = idListBuilder.ToString();
            this.Close();
        }
    }
}
