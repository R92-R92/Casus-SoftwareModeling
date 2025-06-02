using System.IO;
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
        private List<string> selectedPicturePaths = new();
        private string? selectedSketchPath;
        private int currentPictureIndex = -1;

        public MainWindow()
        {
            InitializeComponent();
            RefreshConceptOverviews();
            PropSelectionListBox.ItemsSource = new DAL().GetAllProps();
            ModelSelectionListBox.ItemsSource = new DAL().GetAllContacts();
            ShootSelectionComboBox.ItemsSource = new DAL().GetAllShoots();
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
            EditConceptForm.Visibility = Visibility.Collapsed;
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
            NewConceptNameTextBox.Text = "";
            NewConceptDescriptionTextBox.Text = "";
            PropSelectionListBox.UnselectAll();
            ModelSelectionListBox.UnselectAll();
            ShootSelectionComboBox.SelectedItem = null;

            // Reset sketch
            selectedSketchPath = null;
            SketchPreviewImage.Source = null;
            SketchPreviewImage.Visibility = Visibility.Collapsed;
            SketchAddIcon.Visibility = Visibility.Visible;
            DeleteSketchButton.Visibility = Visibility.Collapsed;

            // Reset pictures
            selectedPicturePaths.Clear();
            currentPictureIndex = -1;

            // Reset picture preview
            PicturePreviewImage.Source = null;
            PicturePreviewImage.Visibility = Visibility.Collapsed;
            PictureAddIcon.Visibility = Visibility.Visible;
            DeletePictureButton.Visibility = Visibility.Collapsed;
            ExtraPictureButton.Visibility = Visibility.Collapsed;
            NextPictureButton.Visibility = Visibility.Collapsed;
            PrevPictureButton.Visibility = Visibility.Collapsed;
            PictureUploadBorder.MouseLeftButtonUp -= UploadPicture_Click;
            PictureUploadBorder.MouseLeftButtonUp += UploadPicture_Click;
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
            // MessageBox.Show("Home address updated successfully.");
            DashboardButton_Click(null, null);
        }






        // CONCEPT
        private void RefreshConceptOverviews()
        {
            List<Concept> allConcepts = new DAL().GetAllConcepts();

            ConceptsDataGrid.ItemsSource = allConcepts;
            DashboardConceptsDataGrid.ItemsSource = allConcepts;
        }

        private void CreateNewConcept_Click(object sender, RoutedEventArgs e)
        {
            string name = NewConceptNameTextBox.Text;
            string description = NewConceptDescriptionTextBox.Text;
            string sketch = "";

            // LATER WEGWERKEN
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a name for the concept before creating it.");
                return;
            }

            if (new DAL().ConceptNameExists(name))
            {
                MessageBox.Show("A concept with this name already exists. Please choose a different name.");
                return;
            }

            List<Prop> props = PropSelectionListBox.SelectedItems.Cast<Prop>().ToList();
            List<Contact> models = ModelSelectionListBox.SelectedItems.Cast<Contact>().ToList();
            Shoot? shoot = ShootSelectionComboBox.SelectedItem as Shoot;

            Concept newConcept = new Concept(0, name, description, sketch, props, shoot);
            newConcept.Models = models;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;
            string conceptFolder = System.IO.Path.Combine(rootPath, "Pictures", name, "Pictures");
            Directory.CreateDirectory(conceptFolder);

            foreach (string originalPath in selectedPicturePaths)
            {
                string fileName = System.IO.Path.GetFileName(originalPath);
                string destPath = System.IO.Path.Combine(conceptFolder, fileName);
                File.Copy(originalPath, destPath, overwrite: true);
                newConcept.AddPictures(destPath);
            }

            string sketchFolder = System.IO.Path.Combine(rootPath, "Pictures", name, "Sketch");
            Directory.CreateDirectory(sketchFolder);

            if (!string.IsNullOrEmpty(selectedSketchPath))
            {
                string sketchFileName = System.IO.Path.GetFileName(selectedSketchPath);
                string sketchDestPath = System.IO.Path.Combine(sketchFolder, sketchFileName);
                File.Copy(selectedSketchPath, sketchDestPath, overwrite: true);
                sketch = sketchDestPath;
            }

            newConcept.Sketch = sketch;
            newConcept.Create();
            // MessageBox.Show("Concept successfully created.");
            RefreshConceptOverviews();
            HidePanels();
            ConceptsView.Visibility = Visibility.Visible;
        }






        private void UploadPicture_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dlg.ShowDialog() == true)
            {
                foreach (var file in dlg.FileNames)
                {
                    selectedPicturePaths.Add(file);
                }

                currentPictureIndex = selectedPicturePaths.Count - 1;
                ShowCurrentPicture();
            }
        }





        private void DeletePicture_Click(object sender, RoutedEventArgs e)
        {
            if (currentPictureIndex >= 0 && currentPictureIndex < selectedPicturePaths.Count)
            {
                selectedPicturePaths.RemoveAt(currentPictureIndex);

                if (selectedPicturePaths.Count == 0)
                {
                    currentPictureIndex = -1;
                }
                else if (currentPictureIndex >= selectedPicturePaths.Count)
                {
                    currentPictureIndex = selectedPicturePaths.Count - 1;
                }

                ShowCurrentPicture();
            }
        }

        private void PrevPicture_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPicturePaths.Count > 0)
            {
                currentPictureIndex = (currentPictureIndex - 1 + selectedPicturePaths.Count) % selectedPicturePaths.Count;
                ShowCurrentPicture();
            }
        }

        private void NextPicture_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPicturePaths.Count > 0)
            {
                currentPictureIndex = (currentPictureIndex + 1) % selectedPicturePaths.Count;
                ShowCurrentPicture();
            }
        }

        private void ShowCurrentPicture()
        {
            if (selectedPicturePaths.Count > 0 && currentPictureIndex >= 0)
            {
                PicturePreviewImage.Source = new BitmapImage(new Uri(selectedPicturePaths[currentPictureIndex]));
                PicturePreviewImage.Visibility = Visibility.Visible;
                PictureAddIcon.Visibility = Visibility.Collapsed;
                PictureUploadBorder.MouseLeftButtonUp -= UploadPicture_Click;
                DeletePictureButton.Visibility = Visibility.Visible;
                ExtraPictureButton.Visibility = Visibility.Visible;
                PrevPictureButton.Visibility = selectedPicturePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
                NextPictureButton.Visibility = selectedPicturePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                PicturePreviewImage.Source = null;
                PicturePreviewImage.Visibility = Visibility.Collapsed;
                PictureAddIcon.Visibility = Visibility.Visible;
                PictureUploadBorder.MouseLeftButtonUp += UploadPicture_Click;
                DeletePictureButton.Visibility = Visibility.Collapsed;
                ExtraPictureButton.Visibility = Visibility.Collapsed;
                PrevPictureButton.Visibility = Visibility.Collapsed;
                NextPictureButton.Visibility = Visibility.Collapsed;
            }
        }


















        private void UploadSketch_Click(object sender, MouseButtonEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (dlg.ShowDialog() == true)
            {
                selectedSketchPath = dlg.FileName;
                SketchPreviewImage.Source = new BitmapImage(new Uri(selectedSketchPath));
                SketchPreviewImage.Visibility = Visibility.Visible;
                SketchAddIcon.Visibility = Visibility.Collapsed;
                DeleteSketchButton.Visibility = Visibility.Visible;
            }
        }

        private void DeleteSketch_Click(object sender, RoutedEventArgs e)
        {
            selectedSketchPath = null;
            SketchPreviewImage.Source = null;
            SketchPreviewImage.Visibility = Visibility.Collapsed;
            SketchAddIcon.Visibility = Visibility.Visible;
            DeleteSketchButton.Visibility = Visibility.Collapsed;
        }








        private Concept? conceptBeingEdited = null;

        private void EditConcept_Click(object sender, RoutedEventArgs e)
        {
            // Haal het geselecteerde concept op vanuit de juiste datagrid
            Concept? selected = null;

            if (DashboardView.Visibility == Visibility.Visible)
                selected = DashboardConceptsDataGrid.SelectedItem as Concept;
            else if (ConceptsView.Visibility == Visibility.Visible)
                selected = ConceptsDataGrid.SelectedItem as Concept;

            if (selected == null)
            {
                MessageBox.Show("Please select a concept to edit.");
                return;
            }

            conceptBeingEdited = selected;

            // ❗ VUL DE LISTS IN
            EditPropSelectionListBox.ItemsSource = new DAL().GetAllProps();
            EditModelSelectionListBox.ItemsSource = new DAL().GetAllContacts();
            EditShootSelectionComboBox.ItemsSource = new DAL().GetAllShoots();

            // Vul de invoervelden
            EditConceptNameTextBox.Text = selected.Name;
            EditConceptDescriptionTextBox.Text = selected.Description;

            // Selecteer de juiste props
            EditPropSelectionListBox.SelectedItems.Clear();
            foreach (var prop in selected.Props)
            {
                var match = (EditPropSelectionListBox.ItemsSource as List<Prop>)?.FirstOrDefault(p => p.Id == prop.Id);
                if (match != null)
                    EditPropSelectionListBox.SelectedItems.Add(match);
            }


            // Selecteer de juiste modellen
            EditModelSelectionListBox.SelectedItems.Clear();
            foreach (var model in selected.Models)
            {
                var match = (EditModelSelectionListBox.ItemsSource as List<Contact>)?.FirstOrDefault(m => m.Id == model.Id);
                if (match != null)
                    EditModelSelectionListBox.SelectedItems.Add(match);
            }


            // Selecteer de juiste shoot
            var shootMatch = (EditShootSelectionComboBox.ItemsSource as List<Shoot>)?.FirstOrDefault(s => s.Id == selected.Shoot?.Id);
            EditShootSelectionComboBox.SelectedItem = shootMatch;


            // Sketch inladen
            if (!string.IsNullOrEmpty(selected.Sketch) && File.Exists(selected.Sketch))
            {
                selectedSketchPath = selected.Sketch;
                EditSketchPreviewImage.Source = new BitmapImage(new Uri(selectedSketchPath));
                EditSketchPreviewImage.Visibility = Visibility.Visible;
                EditSketchAddIcon.Visibility = Visibility.Collapsed;
                EditDeleteSketchButton.Visibility = Visibility.Visible;
            }
            else
            {
                selectedSketchPath = null;
                EditSketchPreviewImage.Source = null;
                EditSketchPreviewImage.Visibility = Visibility.Collapsed;
                EditSketchAddIcon.Visibility = Visibility.Visible;
                EditDeleteSketchButton.Visibility = Visibility.Collapsed;
            }

            // Pictures inladen
            selectedPicturePaths = selected.Pictures.ToList();
            currentPictureIndex = selectedPicturePaths.Count > 0 ? 0 : -1;
            //ShowCurrentPicture_Edit();

            // Toon edit form
            HidePanels();
            EditConceptForm.Visibility = Visibility.Visible;
        }



        private void SaveEditConcept_Click(object sender, RoutedEventArgs e)
        {
            if (conceptBeingEdited == null)
            {
                MessageBox.Show("No concept is currently being edited.");
                return;
            }

            // Update basisvelden
            conceptBeingEdited.Name = EditConceptNameTextBox.Text;
            conceptBeingEdited.Description = EditConceptDescriptionTextBox.Text;
            conceptBeingEdited.Props = EditPropSelectionListBox.SelectedItems.Cast<Prop>().ToList();
            conceptBeingEdited.Models = EditModelSelectionListBox.SelectedItems.Cast<Contact>().ToList();
            conceptBeingEdited.Shoot = EditShootSelectionComboBox.SelectedItem as Shoot;

            // Verwerk sketch
            if (!string.IsNullOrEmpty(selectedSketchPath))
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;
                string sketchFolder = System.IO.Path.Combine(rootPath, "Pictures", conceptBeingEdited.Name, "Sketch");
                Directory.CreateDirectory(sketchFolder);
                string fileName = System.IO.Path.GetFileName(selectedSketchPath);
                string destPath = System.IO.Path.Combine(sketchFolder, fileName);
                if (!string.Equals(selectedSketchPath, destPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(selectedSketchPath, destPath, overwrite: true);
                }
                conceptBeingEdited.Sketch = destPath;
            }

            // Verwerk pictures
            conceptBeingEdited.ClearPictures();
            if (selectedPicturePaths.Count > 0)
            {
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;
                string pictureFolder = System.IO.Path.Combine(rootPath, "Pictures", conceptBeingEdited.Name, "Pictures");
                Directory.CreateDirectory(pictureFolder);

                foreach (string path in selectedPicturePaths)
                {
                    string fileName = System.IO.Path.GetFileName(path);
                    string destPath = System.IO.Path.Combine(pictureFolder, fileName);
                    if (!string.Equals(path, destPath, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(path, destPath, overwrite: true);
                    }
                    conceptBeingEdited.AddPictures(destPath);
                }
            }

            // Sla wijzigingen op in database
            conceptBeingEdited.Update();

            conceptBeingEdited = null;
            RefreshConceptOverviews();
            HidePanels();
            ConceptsView.Visibility = Visibility.Visible;
        }











        private void DeleteConcept_Click(object sender, RoutedEventArgs e)
        {
            Concept? concept = null;

            if (DashboardView.Visibility == Visibility.Visible)
            {
                concept = DashboardConceptsDataGrid.SelectedItem as Concept;
            }
            else if (ConceptsView.Visibility == Visibility.Visible)
            {
                concept = ConceptsDataGrid.SelectedItem as Concept;
            }

            if (concept != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to delete the concept \"{concept.Name}\"?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                {
                    return;
                }

                new DAL().DeleteConcept(concept.Id);

                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;
                string conceptDirectory = System.IO.Path.Combine(rootPath, "Pictures", concept.Name);

                if (Directory.Exists(conceptDirectory))
                {
                    try
                    {
                        Directory.Delete(conceptDirectory, recursive: true);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not delete concept files: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                RefreshConceptOverviews();
            }
            else
            {
                MessageBox.Show("Please select a concept to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}