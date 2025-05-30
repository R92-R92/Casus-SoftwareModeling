using System;
using System.Collections.Generic;
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

namespace StudioManager
{
    /// <summary>
    /// Interaction logic for ConceptWindow.xaml
    /// </summary>
    public partial class ConceptWindow : Window
    {
        public List<Address> Addressen { get; set; } = new List<Address>();
        public ConceptWindow()
        {
            InitializeComponent();

            Addressen.Add(new Address("Musterstraße" ,"23", "10115", "Berlin", "Deutschland"));
            foreach(var address in Addressen)
            {
                AddressComboBox.Items.Add(address.FullAdress);
            }
            //DataContext = this;
        }
    }
}
