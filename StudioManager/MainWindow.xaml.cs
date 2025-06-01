using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudioManager
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RefreshConceptOverviews();
            PropSelectionListBox.ItemsSource = new DAL().GetAllProps();
            ModelSelectionListBox.ItemsSource = new DAL().GetAllContacts();



        }

        // NAVIGATION
        public void HidePanels()
        {
            DashboardView.Visibility = Visibility.Collapsed;
            ProjectsView.Visibility = Visibility.Collapsed;
            ShootsView.Visibility = Visibility.Collapsed;
            ConceptsView.Visibility = Visibility.Collapsed;
            PropsView.Visibility = Visibility.Collapsed;
            ContactsView.Visibility = Visibility.Collapsed;
            HomeAddressView.Visibility = Visibility.Collapsed;
            NewConceptForm.Visibility = Visibility.Collapsed;
        }

        public void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshConceptOverviews();
            HidePanels();
            DashboardView.Visibility = Visibility.Visible;
        }

        public void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            ProjectsView.Visibility = Visibility.Visible;
        }

        public void ShootsButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            ShootsView.Visibility = Visibility.Visible;
        }

        public void ConceptsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshConceptOverviews();
            HidePanels();
            ConceptsView.Visibility = Visibility.Visible;
        }

        private void NewConceptButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            NewConceptForm.Visibility = Visibility.Visible;
        }


        public void PropsButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            PropsView.Visibility = Visibility.Visible;
        }

        public void ContactsButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            ContactsView.Visibility = Visibility.Visible;
        }


        // SEARCHBOXES
        private void SearchBox_Projects(object sender, TextChangedEventArgs e)
        {
            SearchPlaceholder.Visibility = string.IsNullOrEmpty(ProjectsSearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SearchBox_Shoots(object sender, TextChangedEventArgs e)
        {
            ShootsSearchPlaceholder.Visibility = string.IsNullOrEmpty(ShootsSearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SearchBox_Concepts(object sender, TextChangedEventArgs e)
        {
            ConceptsSearchPlaceholder.Visibility = string.IsNullOrEmpty(ConceptsSearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SearchBox_Props(object sender, TextChangedEventArgs e)
        {
            PropsSearchPlaceholder.Visibility = string.IsNullOrEmpty(PropsSearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SearchBox_Contacts(object sender, TextChangedEventArgs e)
        {
            ContactsSearchPlaceholder.Visibility = string.IsNullOrEmpty(ContactsSearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void SearchBox_DashboardConcepts(object sender, TextChangedEventArgs e)
        {
            DashboardConceptSearchPlaceholder.Visibility = string.IsNullOrEmpty(DashboardConceptSearchTextBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        // HOME LOCATION
        private void UpdateMapWithHomeAddress()
        {
            string street = HomeStreetTextBox.Text;
            string houseNumber = HomeHouseNumberTextBox.Text;
            string postalCode = HomePostalCodeTextBox.Text;
            string city = HomeCityTextBox.Text;
            string country = HomeCountryTextBox.Text;

            string fullAddress = $"{street} {houseNumber}, {postalCode} {city}, {country}";
            string url = $"https://www.google.com/maps?q={Uri.EscapeDataString(fullAddress)}";

            HomeMapWebView.Source = new Uri(url);
        }

        private void ChangeLocation_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            var home = new DAL().GetHomeAddress();
            if (home != null)
            {
                HomeStreetTextBox.Text = home.Street;
                HomeHouseNumberTextBox.Text = home.HouseNumber;
                HomePostalCodeTextBox.Text = home.PostalCode;
                HomeCityTextBox.Text = home.City;
                HomeCountryTextBox.Text = home.Country;
                UpdateMapWithHomeAddress();
            }
            HomeAddressView.Visibility = Visibility.Visible;
        }

        private void SaveHomeAddress_Click(object sender, RoutedEventArgs e)
        {
            Address updated = new Address(
                id: 0,
                street: HomeStreetTextBox.Text,
                houseNumber: HomeHouseNumberTextBox.Text,
                postalCode: HomePostalCodeTextBox.Text,
                city: HomeCityTextBox.Text,
                country: HomeCountryTextBox.Text
            );

            new DAL().UpdateHomeAddress(updated);
            MessageBox.Show("Home address updated successfully.");
            DashboardButton_Click(null, null);
        }


        private void RefreshConceptOverviews()
        {
            List<Concept> allConcepts = new DAL().GetAllConcepts();

            ConceptsDataGrid.ItemsSource = allConcepts;
            DashboardConceptsDataGrid.ItemsSource = allConcepts;
        }




        private void CreateNewConcept_Click(object sender, RoutedEventArgs e)
        {
            string description = NewConceptDescriptionTextBox.Text;
            string sketch = NewConceptSketchTextBox.Text;
            string name = NewConceptNameTextBox.Text;
            List<Prop> props = PropSelectionListBox.SelectedItems.Cast<Prop>().ToList();
            List<Contact> models = ModelSelectionListBox.SelectedItems.Cast<Contact>().ToList();

            Concept newConcept = new Concept(0, name, description, sketch, props, null);
            newConcept.Models = models;
            newConcept.Create();

            MessageBox.Show("Concept successfully created.");
            RefreshConceptOverviews();
            HidePanels();
            ConceptsView.Visibility = Visibility.Visible;
        }
    }
}