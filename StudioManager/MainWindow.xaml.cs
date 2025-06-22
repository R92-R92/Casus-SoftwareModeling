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
        private Concept? conceptBeingEdited = null;
        private Prop? propBeingEdited = null;
        private bool editModeHasDeletions = false;


        private List<Prop> selectedProps = new();
        private List<Prop> editSelectedProps = new();
        private string? returnToView = null;

        private List<Contact> selectedModels = new();
        private List<Contact> editSelectedModels = new();


        private Shoot? editSelectedShoot = null;

        private List<string> detailPicturePaths = new();
        private int detailCurrentPictureIndex = -1;
        private int overlayPictureIndex = -1;

        private Address? selectedNewConceptAddress = null;
        private Address? selectedEditConceptAddress = null;
        private string? addressReturnToView = null;
        private Address? addressBeingEdited = null;

        private Address? selectedNewShootAddress = null;
        private string? shootAddressReturnToView = null;

        private Contact? selectedNewShootContact = null;

        private string? quickShootContractPath = null;
        private string? quickShootReturnToView = null;

        private Address? selectedQuickShootAddress = null;
        private Contact? selectedQuickShootContact = null;

        private string? selectedContractPath;

        private Shoot? shootBeingEdited = null;
        private Address? selectedEditShootAddress = null;
        private Contact? selectedEditShootContact = null;
        private string? selectedEditContractPath = null;
        private bool editShootContractReplaced = false;

        private string? selectedContractPathForDetail;
        private Contact? contactBeingEdited = null;

        private List<Concept> contactLinkedConcepts = new();
        private List<Shoot> contactLinkedShoots = new();
        private List<Concept> shootLinkedConcepts = new();

        private string? quickContactReturnToView = null;
        private string? quickShootContactReturnToView = null;



        public MainWindow()
        {
            InitializeComponent();
            RefreshConceptOverview();
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
            NewPropForm.Visibility = Visibility.Collapsed;
            EditPropForm.Visibility = Visibility.Collapsed;
            StartUpWindow.Visibility = Visibility.Collapsed;
            AddressesView.Visibility = Visibility.Collapsed;
            NewAddressForm.Visibility = Visibility.Collapsed;
            NewShootForm.Visibility = Visibility.Collapsed;
            EditShootForm.Visibility= Visibility.Collapsed;
            NewContactForm.Visibility = Visibility.Collapsed;
            EditContactForm.Visibility= Visibility.Collapsed;
        }


































        public void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshConceptOverview();
            HidePanels();
            DashboardView.Visibility = Visibility.Visible;
        }

        public void ProjectsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshProjectOverview();
            HidePanels();
            ProjectsView.Visibility = Visibility.Visible;
        }

        public void ShootsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshShootOverview();
            HidePanels();
            ShootsView.Visibility = Visibility.Visible;
        }

        public void ConceptsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshConceptOverview();
            HidePanels();
            ConceptsView.Visibility = Visibility.Visible;
        }

        private void CancelEditConcept_Click(object sender, RoutedEventArgs e)
        {
            if (editModeHasDeletions)
            {
                MessageBox.Show("You have deleted one or more photos or a sketch. You can no longer cancel this operation. Please choose 'Save Changes' to continue.", "Cancellation Not Allowed", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            conceptBeingEdited = null;
            HidePanels();
            ConceptsView.Visibility = Visibility.Visible;
        }


        private void NewConceptButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            NewConceptNameTextBox.Text = "";
            NewConceptDescriptionTextBox.Text = "";
            selectedNewConceptAddress = null;
            NewConceptAddressToggleList.ItemsSource = new DAL().GetAllAddresses();
            selectedProps.Clear();
            PropToggleList.ItemsSource = new DAL().GetAllProps().Where(p => p.IsAvailable).ToList();
            selectedModels.Clear();
            ModelToggleList.ItemsSource = new DAL().GetAllContacts();

            selectedSketchPath = null;
            SketchPreviewImage.Source = null;
            SketchPreviewImage.Visibility = Visibility.Collapsed;
            SketchAddIcon.Visibility = Visibility.Visible;
            DeleteSketchButton.Visibility = Visibility.Collapsed;
            NewConceptForm.Visibility = Visibility.Visible;
        }

        public void PropsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshPropOverview();
            HidePanels();
            PropsView.Visibility = Visibility.Visible;

            PropsDataGrid.SelectedItem = null;
            DetailPropName.Text = "";
            DetailPropDescription.Text = "";
            DetailPropAvailable.Text = "";
        }

        private void NewPropButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            NewPropNameTextBox.Text = "";
            NewPropDescriptionTextBox.Text = "";
            NewPropAvailableCheckBox.IsChecked = false;
            NewPropForm.Visibility = Visibility.Visible;
        }

        public void ContactsButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshContactOverview();
            HidePanels();
            ContactsView.Visibility = Visibility.Visible;

            DetailContactName.Text = "";
            DetailContactEmail.Text = "";
            DetailContactPhone.Text = "";
            DetailContactSocial.Text = "";
            DetailContactRole.Text = "";
            DetailContactPayment.Text = "";
            DetailContactConceptList.ItemsSource = null;
            DetailContactShootList.ItemsSource = null;
        }

        public void AddressesButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAddressOverview();
            HidePanels();
            AddressesView.Visibility = Visibility.Visible;

            AddressesDataGrid.SelectedItem = null;
            DetailAddressLocation.Text = "";
            DetailAddressStreet.Text = "";
            DetailAddressHouseNumber.Text = "";
            DetailAddressPostalCode.Text = "";
            DetailAddressCity.Text = "";
            DetailAddressCountry.Text = "";
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

        private void SearchBox_Addresses(object sender, TextChangedEventArgs e)
        {
            AddressesSearchPlaceholder.Visibility = string.IsNullOrEmpty(AddressesSearchTextBox.Text)
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
                locationName: null,
                isLocationOnly: false,
                street: HomeStreetTextBox.Text,
                houseNumber: HomeHouseNumberTextBox.Text,
                postalCode: HomePostalCodeTextBox.Text,
                city: HomeCityTextBox.Text,
                country: HomeCountryTextBox.Text
            );

            new DAL().UpdateHomeAddress(updated);
            DashboardButton_Click(null, null);
        }

        // CONCEPT
        private void RefreshConceptOverview()
        {
            List<Concept> allConcepts = new DAL().GetAllConcepts();

            ConceptsDataGrid.ItemsSource = allConcepts;
            DashboardConceptsDataGrid.ItemsSource = allConcepts;
        }

        private void CreateNewConcept_Click(object sender, RoutedEventArgs e)
        {
            string name = NewConceptNameTextBox.Text;
            string description = NewConceptDescriptionTextBox.Text;
            Address? newAddress = selectedNewConceptAddress;

            string sketch = "";

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

            List<Prop> props = selectedProps.ToList();
            List<Contact> models = selectedModels.ToList();
            Shoot? shoot = null;

            Concept newConcept = new Concept(0, name, newAddress, description, sketch, props, shoot);
            newConcept.Models = models;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;
            string conceptFolder = System.IO.Path.Combine(rootPath, "Pictures", name, "Pictures");
            Directory.CreateDirectory(conceptFolder);

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
            RefreshConceptOverview();
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
                    string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + System.IO.Path.GetExtension(file));
                    File.Copy(file, tempPath, overwrite: true);
                    selectedPicturePaths.Add(tempPath);
                }

                currentPictureIndex = selectedPicturePaths.Count - 1;
                ShowCurrentPicture(EditConceptForm.Visibility == Visibility.Visible);
            }
        }

        private void DeletePicture_Click(object sender, RoutedEventArgs e)
        {


            if (currentPictureIndex >= 0 && currentPictureIndex < selectedPicturePaths.Count)
            {

                string fileToDelete = selectedPicturePaths[currentPictureIndex];

                EditPicturePreviewImage.Source = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();

                try
                {
                    if (File.Exists(fileToDelete)) File.Delete(fileToDelete);
                    editModeHasDeletions = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kon foto '{fileToDelete}' niet verwijderen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                selectedPicturePaths.RemoveAt(currentPictureIndex);

                if (selectedPicturePaths.Count == 0)
                {
                    currentPictureIndex = -1;
                }
                else if (currentPictureIndex >= selectedPicturePaths.Count)
                {
                    currentPictureIndex = selectedPicturePaths.Count - 1;
                }

                ShowCurrentPicture(EditConceptForm.Visibility == Visibility.Visible);
            }
        }

        private void PrevPicture_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPicturePaths.Count > 0)
            {
                currentPictureIndex = (currentPictureIndex - 1 + selectedPicturePaths.Count) % selectedPicturePaths.Count;
                ShowCurrentPicture(EditConceptForm.Visibility == Visibility.Visible);

            }
        }

        private void NextPicture_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPicturePaths.Count > 0)
            {
                currentPictureIndex = (currentPictureIndex + 1) % selectedPicturePaths.Count;
                ShowCurrentPicture(EditConceptForm.Visibility == Visibility.Visible);
            }
        }

        private void ShowCurrentPicture(bool isEditMode = false)
        {
            if (selectedPicturePaths.Count > 0 && currentPictureIndex >= 0)
            {
                var bitmap = LoadImageWithoutLock(selectedPicturePaths[currentPictureIndex]);

                if (isEditMode)
                {
                    EditPicturePreviewImage.Source = bitmap;
                    EditPicturePreviewImage.Visibility = Visibility.Visible;
                    EditPictureAddIcon.Visibility = Visibility.Collapsed;
                    EditDeletePictureButton.Visibility = Visibility.Visible;
                    EditExtraPictureButton.Visibility = Visibility.Visible;
                    EditPrevPictureButton.Visibility = selectedPicturePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
                    EditNextPictureButton.Visibility = selectedPicturePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    return;
                }
            }
            else
            {
                if (isEditMode)
                {
                    EditPicturePreviewImage.Source = null;
                    EditPicturePreviewImage.Visibility = Visibility.Collapsed;
                    EditPictureAddIcon.Visibility = Visibility.Visible;
                    EditDeletePictureButton.Visibility = Visibility.Collapsed;
                    EditExtraPictureButton.Visibility = Visibility.Collapsed;
                    EditPrevPictureButton.Visibility = Visibility.Collapsed;
                    EditNextPictureButton.Visibility = Visibility.Collapsed;
                }
                else
                {
                    return;
                }
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
                if (!string.IsNullOrEmpty(selectedSketchPath) && File.Exists(selectedSketchPath))
                {
                    try
                    {
                        File.Delete(selectedSketchPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Kon oude sketch-bestand niet verwijderen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + System.IO.Path.GetExtension(dlg.FileName));
                File.Copy(dlg.FileName, tempPath, overwrite: true);
                selectedSketchPath = tempPath;

                var bitmap = LoadImageWithoutLock(selectedSketchPath);
                SketchPreviewImage.Source = bitmap;
                EditSketchPreviewImage.Source = bitmap;

                SketchPreviewImage.Visibility = Visibility.Visible;
                EditSketchPreviewImage.Visibility = Visibility.Visible;
                SketchAddIcon.Visibility = Visibility.Collapsed;
                EditSketchAddIcon.Visibility = Visibility.Collapsed;
                DeleteSketchButton.Visibility = Visibility.Visible;
                EditDeleteSketchButton.Visibility = Visibility.Visible;
            }
        }

        private void DeleteSketch_Click(object sender, RoutedEventArgs e)
        {

            EditSketchPreviewImage.Source = null;
            SketchPreviewImage.Source = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();

            if (!string.IsNullOrEmpty(selectedSketchPath) && File.Exists(selectedSketchPath))
            {
                try
                {
                    File.Delete(selectedSketchPath);
                    editModeHasDeletions = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kon sketch-bestand niet verwijderen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            selectedSketchPath = null;

            SketchPreviewImage.Source = null;
            SketchPreviewImage.Visibility = Visibility.Collapsed;
            SketchAddIcon.Visibility = Visibility.Visible;
            DeleteSketchButton.Visibility = Visibility.Collapsed;

            EditSketchPreviewImage.Source = null;
            EditSketchPreviewImage.Visibility = Visibility.Collapsed;
            EditSketchAddIcon.Visibility = Visibility.Visible;
            EditDeleteSketchButton.Visibility = Visibility.Collapsed;
        }

        private void EditConcept_Click(object sender, RoutedEventArgs e)
        {
            OverlayPictureImage.Source = null;
            OverlaySketchImage.Source = null;
            DetailPictureOverlay.Visibility = Visibility.Collapsed;
            DetailSketchOverlay.Visibility = Visibility.Collapsed;

            editModeHasDeletions = false;
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

            editSelectedProps = selected.Props.ToList();
            EditPropToggleList.ItemsSource = new DAL().GetAllProps().Where(p => p.IsAvailable).ToList();

            editSelectedModels = selected.Models.ToList();
            EditModelToggleList.ItemsSource = new DAL().GetAllContacts();

            editSelectedShoot = selected.Shoot;
            EditShootToggleList.ItemsSource = new DAL().GetAllShoots();

            EditConceptNameTextBox.Text = selected.Name;
            EditConceptDescriptionTextBox.Text = selected.Description;

            selectedEditConceptAddress = selected.Address;
            EditConceptAddressToggleList.ItemsSource = new DAL().GetAllAddresses();

            if (!string.IsNullOrEmpty(selected.Sketch) && File.Exists(selected.Sketch))
            {
                selectedSketchPath = selected.Sketch;
                EditSketchPreviewImage.Source = LoadImageWithoutLock(selected.Sketch);

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

            selectedPicturePaths = selected.Pictures.ToList();
            currentPictureIndex = selectedPicturePaths.Count > 0 ? 0 : -1;
            ShowCurrentPicture(true);

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

            string oldName = conceptBeingEdited.Name;
            string newName = EditConceptNameTextBox.Text;

            if (!string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
            {
                if (new DAL().ConceptNameExists(newName))
                {
                    MessageBox.Show("Er bestaat al een concept met deze naam. Kies een andere naam.");
                    return;
                }
            }

            conceptBeingEdited.Name = newName;
            conceptBeingEdited.Description = EditConceptDescriptionTextBox.Text;
            Address? updatedAddress = selectedEditConceptAddress;

            conceptBeingEdited.Address = updatedAddress;
            conceptBeingEdited.Props = editSelectedProps.ToList();
            conceptBeingEdited.Models = editSelectedModels.ToList();
            conceptBeingEdited.Shoot = editSelectedShoot;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;

            if (!string.IsNullOrEmpty(selectedSketchPath))
            {
                string sketchFolder = System.IO.Path.Combine(rootPath, "Pictures", newName, "Sketch");
                Directory.CreateDirectory(sketchFolder);
                string fileName = System.IO.Path.GetFileName(selectedSketchPath);
                string destPath = System.IO.Path.Combine(sketchFolder, fileName);

                if (!string.Equals(selectedSketchPath, destPath, StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(selectedSketchPath, destPath, overwrite: true);
                }

                conceptBeingEdited.Sketch = destPath;
            }

            conceptBeingEdited.ClearPictures();

            if (selectedPicturePaths.Count > 0)
            {
                string pictureFolder = System.IO.Path.Combine(rootPath, "Pictures", newName, "Pictures");
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

            conceptBeingEdited.Update();
            editModeHasDeletions = false;

            if (!string.Equals(oldName, newName, StringComparison.OrdinalIgnoreCase))
            {
                EditSketchPreviewImage.Source = new BitmapImage();
                EditPicturePreviewImage.Source = new BitmapImage();


                string oldConceptPath = System.IO.Path.Combine(rootPath, "Pictures", oldName);
                try
                {
                    if (Directory.Exists(oldConceptPath))
                    {
                        Directory.Delete(oldConceptPath, recursive: true);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kon oude map '{oldConceptPath}' niet verwijderen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            conceptBeingEdited = null;
            RefreshConceptOverview();
            HidePanels();
            ConceptsView.Visibility = Visibility.Visible;
        }

        private BitmapImage LoadImageWithoutLock(string path)
        {
            using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
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

                concept.Delete();

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

                RefreshConceptOverview();
            }
            else
            {
                MessageBox.Show("Please select a concept to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ConceptsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConceptsDataGrid.SelectedItem is Concept selected)
            {
                DetailName.Text = $"Name: {selected.Name}";
                DetailDescription.Text = $"Description: {selected.Description}";
                DetailProps.Text = $"Props: {selected.PropsText}";

                DetailConceptModelList.ItemsSource = selected.Models;
                ConceptLinkedModelsPanel.Visibility = Visibility.Visible;


                DetailAddress.Text = selected.Address != null
                    ? $"Location: {(string.IsNullOrWhiteSpace(selected.Address.LocationName) ? $"{selected.Address.Street} {selected.Address.HouseNumber}, {selected.Address.PostalCode} {selected.Address.City}, {selected.Address.Country}" : selected.Address.LocationName)}"
                    : "Location: –";


                ConceptLinkedShootPanel.Visibility = Visibility.Visible;
                DetailConceptShootList.ItemsSource = selected.Shoot != null
                    ? new List<Shoot> { selected.Shoot }
                    : new List<Shoot>(); 


                DetailShootAddress.Text = selected.Shoot?.Location != null
                    ? $"Shoot Address: {(string.IsNullOrWhiteSpace(selected.Shoot.Location.LocationName)
                        ? $"{selected.Shoot.Location.Street} {selected.Shoot.Location.HouseNumber}, {selected.Shoot.Location.PostalCode} {selected.Shoot.Location.City}, {selected.Shoot.Location.Country}"
                        : selected.Shoot.Location.LocationName)}"
                    : "Shoot Address: –";


                if (!string.IsNullOrEmpty(selected.Sketch) && File.Exists(selected.Sketch))
                {
                    DetailSketchPreviewImage.Source = LoadImageWithoutLock(selected.Sketch);
                    DetailSketchBorder.Visibility = Visibility.Visible;
                }
                else
                {
                    DetailSketchPreviewImage.Source = null;
                    DetailSketchBorder.Visibility = Visibility.Collapsed;
                }

                if (selected.Pictures != null && selected.Pictures.Count > 0)
                {
                    string firstPicture = selected.Pictures.FirstOrDefault(p => File.Exists(p));
                    if (firstPicture != null)
                    {
                        detailPicturePaths = selected.Pictures.Where(File.Exists).ToList();
                        detailCurrentPictureIndex = 0;
                        ShowDetailPicture();
                    }
                    else
                    {
                        detailPicturePaths.Clear();
                        detailCurrentPictureIndex = -1;
                        ShowDetailPicture();
                    }
                }
                else
                {
                    detailPicturePaths.Clear();
                    detailCurrentPictureIndex = -1;
                    ShowDetailPicture();
                }
            }
            else
            {
                DetailName.Text = "";
                DetailShootAddress.Text = "";
                DetailDescription.Text = "";
                DetailProps.Text = "";
                DetailConceptModelList.ItemsSource = null;
                ConceptLinkedModelsPanel.Visibility = Visibility.Collapsed;

                DetailAddress.Text = "";
                DetailConceptShootList.ItemsSource = null;
                ConceptLinkedShootPanel.Visibility = Visibility.Collapsed;


                DetailSketchPreviewImage.Source = null;
                DetailSketchBorder.Visibility = Visibility.Collapsed;

                DetailPicturePreviewImage.Source = null;
                DetailPictureBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void LinkedModel_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is Contact contact)
            {
                HidePanels();
                ContactsView.Visibility = Visibility.Visible;
                RefreshContactOverview();

                var matchingContact = (ContactsDataGrid.ItemsSource as IEnumerable<Contact>)?
                    .FirstOrDefault(c => c.Id == contact.Id);

                if (matchingContact != null)
                {
                    ContactsDataGrid.SelectedItem = null;
                    ContactsDataGrid.Items.Refresh();
                    ContactsDataGrid.SelectedItem = matchingContact;
                    ContactsDataGrid.ScrollIntoView(matchingContact);
                    ContactsDataGrid_SelectionChanged(ContactsDataGrid, null);
                }
            }
        }


        private void DetailSketchPreviewImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (DetailSketchPreviewImage.Source != null)
            {
                OverlaySketchImage.Source = DetailSketchPreviewImage.Source;
                DetailSketchOverlay.Visibility = Visibility.Visible;
                ConceptsDataGrid.IsEnabled = false;
            }
        }

        private void OverlaySketchClose_Click(object sender, RoutedEventArgs e)
        {
            DetailSketchOverlay.Visibility = Visibility.Collapsed;
            ConceptsDataGrid.IsEnabled = true;
        }

        private void ShowDetailPicture()
        {
            if (detailPicturePaths.Count > 0 && detailCurrentPictureIndex >= 0)
            {
                string currentPath = detailPicturePaths[detailCurrentPictureIndex];
                if (File.Exists(currentPath))
                {
                    DetailPicturePreviewImage.Source = LoadImageWithoutLock(currentPath);
                    DetailPictureBorder.Visibility = Visibility.Visible;
                    DetailPrevPictureButton.Visibility = detailPicturePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
                    DetailNextPictureButton.Visibility = detailPicturePaths.Count > 1 ? Visibility.Visible : Visibility.Collapsed;
                }
                else
                {
                    DetailPicturePreviewImage.Source = null;
                    DetailPictureBorder.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                DetailPicturePreviewImage.Source = null;
                DetailPictureBorder.Visibility = Visibility.Collapsed;
                DetailPrevPictureButton.Visibility = Visibility.Collapsed;
                DetailNextPictureButton.Visibility = Visibility.Collapsed;
            }
        }

        private void DetailPrevPicture_Click(object sender, RoutedEventArgs e)
        {
            if (detailPicturePaths.Count > 0)
            {
                detailCurrentPictureIndex = (detailCurrentPictureIndex - 1 + detailPicturePaths.Count) % detailPicturePaths.Count;
                ShowDetailPicture();
            }
        }

        private void DetailNextPicture_Click(object sender, RoutedEventArgs e)
        {
            if (detailPicturePaths.Count > 0)
            {
                detailCurrentPictureIndex = (detailCurrentPictureIndex + 1) % detailPicturePaths.Count;
                ShowDetailPicture();
            }
        }

        private void DetailPicturePreviewImage_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (detailPicturePaths.Count > 0 && detailCurrentPictureIndex >= 0)
            {
                overlayPictureIndex = detailCurrentPictureIndex;
                ShowOverlayPicture();
            }
        }


        private void ShowOverlayPicture()
        {
            if (overlayPictureIndex >= 0 && overlayPictureIndex < detailPicturePaths.Count)
            {
                string imagePath = detailPicturePaths[overlayPictureIndex];
                if (File.Exists(imagePath))
                {
                    OverlayPictureImage.Source = LoadImageWithoutLock(imagePath);

                    DetailPictureOverlay.Visibility = Visibility.Visible;
                    ConceptsDataGrid.IsEnabled = false;
                }
            }
        }

        private void OverlayPrev_Click(object sender, RoutedEventArgs e)
        {
            if (detailPicturePaths.Count > 0)
            {
                overlayPictureIndex = (overlayPictureIndex - 1 + detailPicturePaths.Count) % detailPicturePaths.Count;
                ShowOverlayPicture();
            }
        }

        private void OverlayNext_Click(object sender, RoutedEventArgs e)
        {
            if (detailPicturePaths.Count > 0)
            {
                overlayPictureIndex = (overlayPictureIndex + 1) % detailPicturePaths.Count;
                ShowOverlayPicture();
            }
        }

        private void OverlayClose_Click(object sender, RoutedEventArgs e)
        {
            DetailPictureOverlay.Visibility = Visibility.Collapsed;
            ConceptsDataGrid.IsEnabled = true;
            overlayPictureIndex = -1;
        }

        private void EditAddShoot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Shoot shoot && editSelectedShoot == null)
            {
                editSelectedShoot = shoot;
                EditShootToggleList.Items.Refresh();
            }
        }

        private void EditRemoveShoot_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Shoot shoot && editSelectedShoot?.Id == shoot.Id)
            {
                editSelectedShoot = null;
                EditShootToggleList.Items.Refresh();
            }
        }

        private void EditUpdateAddShootButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Shoot shoot)
            {
                bool isEnabled = editSelectedShoot == null;
                btn.IsEnabled = isEnabled;
                btn.Visibility = Visibility.Visible;
            }
        }


        private void EditUpdateRemoveShootButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Shoot shoot)
            {
                bool isSelected = editSelectedShoot?.Id == shoot.Id;
                btn.IsEnabled = isSelected;
                btn.Visibility = Visibility.Visible;
            }
        }

        private void QuickAddPropFromNewConcept_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            NewQuickPropNameTextBox.Text = "";
            NewQuickPropDescriptionTextBox.Text = "";
            NewQuickPropAvailableCheckBox.IsChecked = true;
            returnToView = "NewConcept";
            QuickAddPropForm.Visibility = Visibility.Visible;
        }

        private void QuickAddPropFromEditConcept_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            NewQuickPropNameTextBox.Text = "";
            NewQuickPropDescriptionTextBox.Text = "";
            NewQuickPropAvailableCheckBox.IsChecked = true;
            returnToView = "EditConcept";
            QuickAddPropForm.Visibility = Visibility.Visible;
        }

        private void CreateQuickProp_Click(object sender, RoutedEventArgs e)
        {
            var name = NewQuickPropNameTextBox.Text;
            var desc = NewQuickPropDescriptionTextBox.Text;
            var avail = NewQuickPropAvailableCheckBox.IsChecked == true;

            if (string.IsNullOrWhiteSpace(name) || new DAL().PropNameExists(name))
            {
                MessageBox.Show("Prop name is empty or already exists.");
                return;
            }

            var prop = new Prop(0, name, desc, avail);
            prop.Create();

            if (returnToView == "NewConcept")
            {
                PropToggleList.ItemsSource = new DAL().GetAllProps().Where(p => p.IsAvailable).ToList();
                NewConceptForm.Visibility = Visibility.Visible;
            }
            else if (returnToView == "EditConcept")
            {
                EditPropToggleList.ItemsSource = new DAL().GetAllProps().Where(p => p.IsAvailable).ToList();
                EditConceptForm.Visibility = Visibility.Visible;
            }

            returnToView = null;
            QuickAddPropForm.Visibility = Visibility.Collapsed;
        }

        private void CancelQuickAddProp_Click(object sender, RoutedEventArgs e)
        {
            if (returnToView == "NewConcept")
                NewConceptForm.Visibility = Visibility.Visible;
            else if (returnToView == "EditConcept")
                EditConceptForm.Visibility = Visibility.Visible;

            returnToView = null;
            QuickAddPropForm.Visibility = Visibility.Collapsed;
        }



        private void QuickAddContactFromNewConcept_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            quickContactReturnToView = "NewConcept";
            ResetQuickContactForm();
            QuickAddContactForm.Visibility = Visibility.Visible;
        }

        private void QuickAddContactFromEditConcept_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            quickContactReturnToView = "EditConcept";
            ResetQuickContactForm();
            QuickAddContactForm.Visibility = Visibility.Visible;
        }


        private void ResetQuickContactForm()
        {
            QuickContactFirstNameTextBox.Text = "";
            QuickContactLastNameTextBox.Text = "";
            QuickContactPhoneTextBox.Text = "";
            QuickContactEmailTextBox.Text = "";
            QuickContactSocialTextBox.Text = "";
            QuickContactRoleComboBox.SelectedIndex = -1;
            QuickContactPayCheckBox.IsChecked = false;
        }

        private void CreateQuickContact_Click(object sender, RoutedEventArgs e)
        {
            string firstName = QuickContactFirstNameTextBox.Text;
            string lastName = QuickContactLastNameTextBox.Text;
            string phone = QuickContactPhoneTextBox.Text;
            string email = QuickContactEmailTextBox.Text;
            string social = QuickContactSocialTextBox.Text;
            string role = (QuickContactRoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            bool payment = QuickContactPayCheckBox.IsChecked == true;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please fill in at least First Name, Last Name and Role.");
                return;
            }

            if (new DAL().ContactNameExists(firstName, lastName))
            {
                MessageBox.Show("A contact with this name already exists.");
                return;
            }

            Contact newContact = new Contact(0, firstName, lastName, phone, email, social, null, role, payment, null);
            newContact.Create();

            if (quickContactReturnToView == "NewConcept")
            {
                ModelToggleList.ItemsSource = new DAL().GetAllContacts();
                NewConceptForm.Visibility = Visibility.Visible;
            }
            else if (quickContactReturnToView == "EditConcept")
            {
                EditModelToggleList.ItemsSource = new DAL().GetAllContacts();
                EditConceptForm.Visibility = Visibility.Visible;
            }

            quickContactReturnToView = null;
            QuickAddContactForm.Visibility = Visibility.Collapsed;
        }

        private void CancelQuickContact_Click(object sender, RoutedEventArgs e)
        {
            if (quickContactReturnToView == "NewConcept")
                NewConceptForm.Visibility = Visibility.Visible;
            else if (quickContactReturnToView == "EditConcept")
                EditConceptForm.Visibility = Visibility.Visible;

            quickContactReturnToView = null;
            QuickAddContactForm.Visibility = Visibility.Collapsed;
        }



        private void AddProp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Prop prop && !selectedProps.Contains(prop))
            {
                selectedProps.Add(prop);
                PropToggleList.Items.Refresh();
            }
        }

        private void RemoveProp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Prop prop && selectedProps.Contains(prop))
            {
                selectedProps.Remove(prop);
                PropToggleList.Items.Refresh();
            }
        }

        private void UpdateAddButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Prop prop)
            {
                btn.IsEnabled = !selectedProps.Any(p => p.Id == prop.Id);
            }
        }

        private void UpdateRemoveButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Prop prop)
            {
                btn.IsEnabled = selectedProps.Any(p => p.Id == prop.Id);
            }
        }

        private void EditAddProp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Prop prop && !editSelectedProps.Any(p => p.Id == prop.Id))
            {
                editSelectedProps.Add(prop);
                EditPropToggleList.Items.Refresh();
            }
        }

        private void EditRemoveProp_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Prop prop)
            {
                var match = editSelectedProps.FirstOrDefault(p => p.Id == prop.Id);
                if (match != null)
                {
                    editSelectedProps.Remove(match);
                    EditPropToggleList.Items.Refresh();
                }
            }
        }

        private void EditUpdateAddButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Prop prop)
            {
                btn.IsEnabled = !editSelectedProps.Any(p => p.Id == prop.Id);
            }
        }

        private void EditUpdateRemoveButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Prop prop)
            {
                btn.IsEnabled = editSelectedProps.Any(p => p.Id == prop.Id);
            }
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Contact model && !selectedModels.Any(m => m.Id == model.Id))
            {
                selectedModels.Add(model);
                ModelToggleList.Items.Refresh();
            }
        }

        private void RemovContact_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Contact model && selectedModels.Any(m => m.Id == model.Id))
            {
                selectedModels.Remove(model);
                ModelToggleList.Items.Refresh();
            }
        }

        private void UpdateAddContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact model)
            {
                btn.IsEnabled = !selectedModels.Any(m => m.Id == model.Id);
            }
        }

        private void UpdateRemoveContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact model)
            {
                btn.IsEnabled = selectedModels.Any(m => m.Id == model.Id);
            }
        }

        private void EditAddContact_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Contact model && !editSelectedModels.Any(m => m.Id == model.Id))
            {
                editSelectedModels.Add(model);
                EditModelToggleList.Items.Refresh();
            }
        }

        private void EditRemoveContact_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is Contact model)
            {
                var match = editSelectedModels.FirstOrDefault(m => m.Id == model.Id);
                if (match != null)
                {
                    editSelectedModels.Remove(match);
                    EditModelToggleList.Items.Refresh();
                }
            }
        }

        private void EditUpdateAddContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact model)
            {
                btn.IsEnabled = !editSelectedModels.Any(m => m.Id == model.Id);
            }
        }

        private void EditUpdateRemoveContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact model)
            {
                btn.IsEnabled = editSelectedModels.Any(m => m.Id == model.Id);
            }
        }

        private void AddNewConceptAddress_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
            {
                selectedNewConceptAddress = address;
                NewConceptAddressToggleList.Items.Refresh();
            }
        }

        private void RemoveNewConceptAddress_Click(object sender, RoutedEventArgs e)
        {
            selectedNewConceptAddress = null;
            NewConceptAddressToggleList.Items.Refresh();
        }

        private void UpdateAddAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address)
            {
                btn.IsEnabled = selectedNewConceptAddress == null;
            }
        }

        private void UpdateRemoveAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
                btn.IsEnabled = selectedNewConceptAddress != null && selectedNewConceptAddress.Id == address.Id;
        }

        private void AddEditConceptAddress_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
            {
                selectedEditConceptAddress = address;
                EditConceptAddressToggleList.Items.Refresh();
            }
        }

        private void RemoveEditConceptAddress_Click(object sender, RoutedEventArgs e)
        {
            selectedEditConceptAddress = null;
            EditConceptAddressToggleList.Items.Refresh();
        }

        private void EditUpdateAddAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address)
                btn.IsEnabled = selectedEditConceptAddress == null;
        }

        private void EditUpdateRemoveAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
                btn.IsEnabled = selectedEditConceptAddress != null && selectedEditConceptAddress.Id == address.Id;
        }

        private void QuickAddAddressFromNewConcept_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            addressReturnToView = "NewConcept";
            ResetQuickAddressForm();
            QuickAddAddressForm.Visibility = Visibility.Visible;
        }

        private void QuickAddAddressFromEditConcept_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            addressReturnToView = "EditConcept";
            ResetQuickAddressForm();
            QuickAddAddressForm.Visibility = Visibility.Visible;
        }

        private void ResetQuickAddressForm()
        {
            QuickIsLocationOnlyCheckBox.IsChecked = false;
            QuickAddressLocationNameTextBox.Text = "";
            QuickAddressStreetTextBox.Text = "";
            QuickAddressHouseNumberTextBox.Text = "";
            QuickAddressPostalCodeTextBox.Text = "";
            QuickAddressCityTextBox.Text = "";
            QuickAddressCountryTextBox.Text = "";
            ToggleQuickLocationFields(null, null);
        }

        private void ToggleQuickLocationFields(object sender, RoutedEventArgs e)
        {
            bool isLocationOnly = QuickIsLocationOnlyCheckBox.IsChecked == true;
            QuickAddressLocationNameTextBox.IsEnabled = isLocationOnly;

            QuickAddressStreetTextBox.IsEnabled = !isLocationOnly;
            QuickAddressHouseNumberTextBox.IsEnabled = !isLocationOnly;
            QuickAddressPostalCodeTextBox.IsEnabled = !isLocationOnly;
            QuickAddressCityTextBox.IsEnabled = !isLocationOnly;
            QuickAddressCountryTextBox.IsEnabled = !isLocationOnly;
        }

        private void CreateQuickAddress_Click(object sender, RoutedEventArgs e)
        {
            bool isLocationOnly = QuickIsLocationOnlyCheckBox.IsChecked == true;

            string? locationName = QuickIsLocationOnlyCheckBox.IsChecked == true
                ? QuickAddressLocationNameTextBox.Text
                : null;

            Address newAddress = new Address(
                id: 0,
                locationName: locationName,
                isLocationOnly: isLocationOnly,
                street: QuickAddressStreetTextBox.Text,
                houseNumber: QuickAddressHouseNumberTextBox.Text,
                postalCode: QuickAddressPostalCodeTextBox.Text,
                city: QuickAddressCityTextBox.Text,
                country: QuickAddressCountryTextBox.Text
            );

            newAddress.Create();

            if (addressReturnToView == "NewConcept")
            {
                NewConceptAddressToggleList.ItemsSource = new DAL().GetAllAddresses();
                NewConceptForm.Visibility = Visibility.Visible;
            }
            else if (addressReturnToView == "EditConcept")
            {
                EditConceptAddressToggleList.ItemsSource = new DAL().GetAllAddresses();
                EditConceptForm.Visibility = Visibility.Visible;
            }

            addressReturnToView = null;
            QuickAddAddressForm.Visibility = Visibility.Collapsed;
        }

        private void CancelQuickAddAddress_Click(object sender, RoutedEventArgs e)
        {
            if (addressReturnToView == "NewConcept")
                NewConceptForm.Visibility = Visibility.Visible;
            else if (addressReturnToView == "EditConcept")
                EditConceptForm.Visibility = Visibility.Visible;

            addressReturnToView = null;
            QuickAddAddressForm.Visibility = Visibility.Collapsed;
        }

        private void QuickAddShootFromEditConcept_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();

            QuickShootDatePicker.SelectedDate = null;
            selectedQuickShootAddress = null;
            selectedQuickShootContact = null;

            QuickShootAddressToggleList.ItemsSource = new DAL().GetAllAddresses();
            QuickShootContactToggleList.ItemsSource = new DAL().GetAllContacts();

            QuickShootIsSignedCheckBox.IsChecked = false;
            QuickShootSignedOnDatePicker.SelectedDate = null;
            QuickSelectedContractTextBlock.Text = "No file selected.";
            quickShootContractPath = null;

            quickShootReturnToView = "EditConcept";
            QuickAddShootForm.Visibility = Visibility.Visible;
        }

        private void UploadContractForQuickShoot_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|Word documents (*.doc;*.docx)|*.doc;*.docx|All files (*.*)|*.*",
                FilterIndex = 3
            };

            if (dlg.ShowDialog() == true)
            {
                quickShootContractPath = dlg.FileName;
                QuickSelectedContractTextBlock.Text = System.IO.Path.GetFileName(quickShootContractPath);
            }
        }

        private void CreateQuickShoot_Click(object sender, RoutedEventArgs e)
        {
            DateTime? shootDate = QuickShootDatePicker.SelectedDate;
            Address? location = selectedQuickShootAddress;
            Contact? signee = selectedQuickShootContact;
            bool isSigned = QuickShootIsSignedCheckBox.IsChecked == true;
            DateTime? signedOn = QuickShootSignedOnDatePicker.SelectedDate;

            if (shootDate == null || location == null)
            {
                MessageBox.Show("Please select both a date and a location.");
                return;
            }

            Shoot newShoot = new Shoot(0, shootDate, location);
            newShoot.Create();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;

            string locationName = !string.IsNullOrWhiteSpace(location.LocationName)
                ? location.LocationName
                : $"{location.Street}_{location.HouseNumber}";

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                locationName = locationName.Replace(c, '_');
            locationName = locationName.Replace(" ", "_");

            string shootFolder = System.IO.Path.Combine(rootPath, "ShootContract", $"{shootDate:yyyy-MM-dd}_{newShoot.Id}_{locationName}");
            Directory.CreateDirectory(shootFolder);

            Contract newContract = new Contract(0, "", signee, isSigned, signedOn, newShoot);

            if (!string.IsNullOrEmpty(quickShootContractPath) && File.Exists(quickShootContractPath))
            {
                string fileName = System.IO.Path.GetFileName(quickShootContractPath);
                string destPath = System.IO.Path.Combine(shootFolder, fileName);
                File.Copy(quickShootContractPath, destPath, true);
                newContract.Body = destPath;
            }

            newContract.Create();

            EditShootToggleList.ItemsSource = new DAL().GetAllShoots();
            EditConceptForm.Visibility = Visibility.Visible;
            QuickAddShootForm.Visibility = Visibility.Collapsed;
            quickShootReturnToView = null;
        }

        private void CancelQuickAddShoot_Click(object sender, RoutedEventArgs e)
        {
            if (quickShootReturnToView == "EditConcept")
                EditConceptForm.Visibility = Visibility.Visible;

            QuickAddShootForm.Visibility = Visibility.Collapsed;
            quickShootReturnToView = null;
        }


        private void AddQuickShootAddress_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
            {
                selectedQuickShootAddress = address;
                QuickShootAddressToggleList.Items.Refresh();
            }
        }

        private void RemoveQuickShootAddress_Click(object sender, RoutedEventArgs e)
        {
            selectedQuickShootAddress = null;
            QuickShootAddressToggleList.Items.Refresh();
        }

        private void UpdateAddQuickShootAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
            {
                btn.IsEnabled = selectedQuickShootAddress == null;
            }
        }

        private void UpdateRemoveQuickShootAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
            {
                btn.IsEnabled = selectedQuickShootAddress?.Id == address.Id;
            }
        }

        private void AddQuickShootContact_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact contact)
            {
                selectedQuickShootContact = contact;
                QuickShootContactToggleList.Items.Refresh();
            }
        }

        private void RemoveQuickShootContact_Click(object sender, RoutedEventArgs e)
        {
            selectedQuickShootContact = null;
            QuickShootContactToggleList.Items.Refresh();
        }

        private void UpdateAddQuickShootContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact contact)
            {
                btn.IsEnabled = selectedQuickShootContact == null;
            }
        }

        private void UpdateRemoveQuickShootContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact contact)
            {
                btn.IsEnabled = selectedQuickShootContact?.Id == contact.Id;
            }
        }

        // PROPS

        private void RefreshPropOverview()
        {
            PropsDataGrid.ItemsSource = new DAL().GetAllProps();
        }

        private void CreateNewProp_Click(object sender, RoutedEventArgs e)
        {
            string name = NewPropNameTextBox.Text;
            string description = NewPropDescriptionTextBox.Text;
            bool isAvailable = NewPropAvailableCheckBox.IsChecked == true;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a name for the prop.");
                return;
            }

            if (new DAL().PropNameExists(name))
            {
                MessageBox.Show("A prop with this name already exists. Please choose a different name.");
                return;
            }

            Prop newProp = new Prop(0, name, description, isAvailable);
            newProp.Create();

            RefreshPropOverview();
            HidePanels();
            PropsView.Visibility = Visibility.Visible;
        }

        private void EditProp_Click(object sender, RoutedEventArgs e)
        {
            Prop? selected = PropsDataGrid.SelectedItem as Prop;

            if (selected == null)
            {
                MessageBox.Show("Please select a prop to edit.");
                return;
            }

            propBeingEdited = selected;

            EditPropNameTextBox.Text = selected.Name;
            EditPropDescriptionTextBox.Text = selected.Description;
            EditPropAvailableCheckBox.IsChecked = selected.IsAvailable;

            HidePanels();
            EditPropForm.Visibility = Visibility.Visible;
        }

        private void SaveEditProp_Click(object sender, RoutedEventArgs e)
        {
            if (propBeingEdited == null)
            {
                MessageBox.Show("No prop selected to edit.");
                return;
            }

            string newName = EditPropNameTextBox.Text;

            if (string.IsNullOrWhiteSpace(newName))
            {
                MessageBox.Show("Please enter a name for the prop.");
                return;
            }

            if (new DAL().PropNameExists(newName, propBeingEdited.Id))
            {
                MessageBox.Show("A prop with this name already exists. Please choose a different name.");
                return;
            }

            propBeingEdited.Name = newName;
            propBeingEdited.Description = EditPropDescriptionTextBox.Text;
            propBeingEdited.IsAvailable = EditPropAvailableCheckBox.IsChecked == true;

            propBeingEdited.Update();

            propBeingEdited = null;
            RefreshPropOverview();
            HidePanels();
            PropsView.Visibility = Visibility.Visible;
        }

        private void DeleteProp_Click(object sender, RoutedEventArgs e)
        {
            Prop? selected = PropsDataGrid.SelectedItem as Prop;

            if (selected == null)
            {
                MessageBox.Show("Please select a prop to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the prop \"{selected.Name}\"?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                selected.Delete();
                RefreshPropOverview();
            }
        }

        private void PropsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PropsDataGrid.SelectedItem is Prop selected)
            {
                DetailPropName.Text = $"Name: {selected.Name}";
                DetailPropDescription.Text = $"Description: {(string.IsNullOrWhiteSpace(selected.Description) ? "–" : selected.Description)}";
                DetailPropAvailable.Text = $"Available: {(selected.IsAvailable ? "Yes" : "No")}";
            }
            else
            {
                DetailPropName.Text = "Name:";
                DetailPropDescription.Text = "Description:";
                DetailPropAvailable.Text = "Available:";
            }
        }



        // SHOOTS

        private void RefreshShootOverview()
        {
            ShootsDataGrid.ItemsSource = new DAL().GetAllShoots();
        }

        private void NewShootButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            selectedNewShootAddress = null;
            NewShootAddressToggleList.ItemsSource = new DAL().GetAllAddresses();
            NewShootDatePicker.SelectedDate = null;
            NewShootForm.Visibility = Visibility.Visible;
            selectedNewShootContact = null;
            NewShootContactToggleList.ItemsSource = new DAL().GetAllContacts();
            NewShootIsSignedCheckBox.IsChecked = false;
            NewShootSignedOnDatePicker.SelectedDate = null;
            selectedContractPath = null;
            SelectedContractFileNameTextBlock.Text = "No file selected.";

        }

        private void AddNewShootAddress_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
            {
                selectedNewShootAddress = address;
                NewShootAddressToggleList.Items.Refresh();
            }
        }

        private void RemoveNewShootAddress_Click(object sender, RoutedEventArgs e)
        {
            selectedNewShootAddress = null;
            NewShootAddressToggleList.Items.Refresh();
        }

        private void UpdateAddShootAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address)
            {
                btn.IsEnabled = selectedNewShootAddress == null;
            }
        }


        private void UpdateRemoveShootAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
                btn.IsEnabled = selectedNewShootAddress != null && selectedNewShootAddress.Id == address.Id;
        }


        private void UploadContractForNewShoot_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|Word documents (*.doc;*.docx)|*.doc;*.docx|All files (*.*)|*.*",
                FilterIndex = 3
            };

            if (dlg.ShowDialog() == true)
            {
                selectedContractPath = dlg.FileName;
                SelectedContractFileNameTextBlock.Text = System.IO.Path.GetFileName(selectedContractPath);
            }
        }


        private void CancelNewShoot_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            RefreshShootOverview();
            ShootsView.Visibility = Visibility.Visible;
        }


        private void CreateNewShoot_Click(object sender, RoutedEventArgs e)
        {
            DateTime? selectedDate = NewShootDatePicker.SelectedDate;
            if (selectedDate == null)
            {
                MessageBox.Show("Please select a date.");
                return;
            }

            if (selectedNewShootAddress == null)
            {
                MessageBox.Show("Please select a location.");
                return;
            }

            Shoot newShoot = new Shoot(0, selectedDate, selectedNewShootAddress);
            newShoot.Create();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;

            string locationName = !string.IsNullOrWhiteSpace(newShoot.Location?.LocationName)
                ? newShoot.Location.LocationName
                : $"{newShoot.Location?.Street}_{newShoot.Location?.HouseNumber}";

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                locationName = locationName.Replace(c, '_');
            }
            locationName = locationName.Replace(" ", "_");

            string shootFolder = System.IO.Path.Combine(rootPath, "ShootContract", $"{newShoot.Date:yyyy-MM-dd}_{newShoot.Id}_{locationName}");

            Directory.CreateDirectory(shootFolder);

            Contract newContract = new Contract(
                id: 0,
                body: "",
                signee: selectedNewShootContact,
                isSigned: NewShootIsSignedCheckBox.IsChecked == true,
                signedOn: NewShootSignedOnDatePicker.SelectedDate,
                shoot: newShoot
            );

            if (!string.IsNullOrEmpty(selectedContractPath) && File.Exists(selectedContractPath))
            {
                string fileName = System.IO.Path.GetFileName(selectedContractPath);
                string destPath = System.IO.Path.Combine(shootFolder, fileName);

                File.Copy(selectedContractPath, destPath, overwrite: true);
                newContract.Body = destPath;
            }

            newContract.Create();
            ShootsButton_Click(null, null);
        }





        private void AddNewShootContact_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact contact)
            {
                selectedNewShootContact = contact;
                NewShootContactToggleList.Items.Refresh();
            }
        }

        private void RemoveNewShootContact_Click(object sender, RoutedEventArgs e)
        {
            selectedNewShootContact = null;
            NewShootContactToggleList.Items.Refresh();
        }

        private void UpdateAddShootContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact)
                btn.IsEnabled = selectedNewShootContact == null;
        }

        private void UpdateRemoveShootContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact contact)
                btn.IsEnabled = selectedNewShootContact != null && selectedNewShootContact.Id == contact.Id;
        }

        private void DeleteShoot_Click(object sender, RoutedEventArgs e)
        {
            Shoot? selected = ShootsDataGrid.SelectedItem as Shoot;

            if (selected == null)
            {
                MessageBox.Show("Please select a shoot to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the shoot on {selected.Date:yyyy-MM-dd}?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;

            string locationName = !string.IsNullOrWhiteSpace(selected.Location?.LocationName)
                ? selected.Location.LocationName
                : $"{selected.Location?.Street}_{selected.Location?.HouseNumber}";

            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                locationName = locationName.Replace(c, '_');
            }
            locationName = locationName.Replace(" ", "_");

            string shootFolder = System.IO.Path.Combine(rootPath, "ShootContract", $"{selected.Date:yyyy-MM-dd}_{selected.Id}_{locationName}");

            if (Directory.Exists(shootFolder))
            {
                try
                {
                    Directory.Delete(shootFolder, recursive: true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not delete contract folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            foreach (var contract in new DAL().GetAllContracts().Where(c => c.Shoot?.Id == selected.Id))
            {
                contract.Delete();
            }

            selected.Delete();
            RefreshShootOverview();
        }



        private void EditShoot_Click(object sender, RoutedEventArgs e)
        {
            if (ShootsDataGrid.SelectedItem is not Shoot selected) return;

            shootBeingEdited = selected;
            selectedEditShootAddress = selected.Location;

            var contract = new DAL().GetContractByShootId(selected.Id);

            selectedEditShootContact = contract?.Signee;
            selectedEditContractPath = contract?.Body;
            editShootContractReplaced = false;

            EditShootDatePicker.SelectedDate = selected.Date;
            EditShootIsSignedCheckBox.IsChecked = contract?.IsSigned ?? false;
            EditShootSignedOnDatePicker.SelectedDate = contract?.SignedOn;

            EditShootAddressToggleList.ItemsSource = new DAL().GetAllAddresses();
            EditShootContactToggleList.ItemsSource = new DAL().GetAllContacts();

            EditSelectedContractFileNameTextBlock.Text = string.IsNullOrEmpty(selectedEditContractPath)
                ? "No file selected."
                : System.IO.Path.GetFileName(selectedEditContractPath);

            HidePanels();
            EditShootForm.Visibility = Visibility.Visible;
        }



        private void UploadContractForEditShoot_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "PDF files (*.pdf)|*.pdf|Word documents (*.doc;*.docx)|*.doc;*.docx|All files (*.*)|*.*",
                FilterIndex = 3
            };

            if (dlg.ShowDialog() == true)
            {
                if (!string.IsNullOrEmpty(selectedEditContractPath) && File.Exists(selectedEditContractPath))
                {
                    try
                    {
                        File.Delete(selectedEditContractPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not delete old contract: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                string tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), Guid.NewGuid().ToString() + System.IO.Path.GetExtension(dlg.FileName));
                File.Copy(dlg.FileName, tempPath, true);
                selectedEditContractPath = tempPath;

                EditSelectedContractFileNameTextBlock.Text = System.IO.Path.GetFileName(tempPath);
                editShootContractReplaced = true;
            }
        }


        private void SaveEditShoot_Click(object sender, RoutedEventArgs e)
        {
            if (shootBeingEdited == null) return;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string rootPath = Directory.GetParent(baseDir)!.Parent!.Parent!.Parent!.Parent!.FullName;

            string oldLocationName = !string.IsNullOrWhiteSpace(shootBeingEdited.Location?.LocationName)
                ? shootBeingEdited.Location.LocationName
                : $"{shootBeingEdited.Location?.Street}_{shootBeingEdited.Location?.HouseNumber}";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) oldLocationName = oldLocationName.Replace(c, '_');
            oldLocationName = oldLocationName.Replace(" ", "_");
            string oldFolder = System.IO.Path.Combine(rootPath, "ShootContract", $"{shootBeingEdited.Date:yyyy-MM-dd}_{shootBeingEdited.Id}_{oldLocationName}");

            string? oldContractPath = new DAL().GetContractByShootId(shootBeingEdited.Id)?.Body;

            shootBeingEdited.Date = EditShootDatePicker.SelectedDate;
            shootBeingEdited.Location = selectedEditShootAddress;
            shootBeingEdited.Update();

            string newLocationName = !string.IsNullOrWhiteSpace(shootBeingEdited.Location?.LocationName)
                ? shootBeingEdited.Location.LocationName
                : $"{shootBeingEdited.Location?.Street}_{shootBeingEdited.Location?.HouseNumber}";
            foreach (char c in System.IO.Path.GetInvalidFileNameChars()) newLocationName = newLocationName.Replace(c, '_');
            newLocationName = newLocationName.Replace(" ", "_");

            string newFolder = System.IO.Path.Combine(rootPath, "ShootContract", $"{shootBeingEdited.Date:yyyy-MM-dd}_{shootBeingEdited.Id}_{newLocationName}");
            Directory.CreateDirectory(newFolder);

            Contract? contract = new DAL().GetContractByShootId(shootBeingEdited.Id);
            if (contract != null)
            {
                contract.Signee = selectedEditShootContact;
                contract.IsSigned = EditShootIsSignedCheckBox.IsChecked == true;
                contract.SignedOn = EditShootSignedOnDatePicker.SelectedDate;

                string? filename = selectedEditContractPath != null ? System.IO.Path.GetFileName(selectedEditContractPath) : null;

                if (!string.IsNullOrEmpty(selectedEditContractPath))
                {
                    string destPath = System.IO.Path.Combine(newFolder, filename!);

                    if (!string.Equals(selectedEditContractPath, destPath, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Copy(selectedEditContractPath, destPath, true);
                    }

                    contract.Body = destPath;

                    if (editShootContractReplaced && !string.IsNullOrEmpty(oldContractPath) && File.Exists(oldContractPath))
                    {
                        try { File.Delete(oldContractPath); } catch { }
                    }
                }
                else if (!string.IsNullOrEmpty(oldContractPath) && oldFolder != newFolder && File.Exists(oldContractPath))
                {
                    string newPath = System.IO.Path.Combine(newFolder, System.IO.Path.GetFileName(oldContractPath));
                    try
                    {
                        File.Copy(oldContractPath, newPath, true);
                        File.Delete(oldContractPath);
                        contract.Body = newPath;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Kon contractbestand verplaatsen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }

                contract.Update();
            }

            if (oldFolder != newFolder && Directory.Exists(oldFolder))
            {
                try { Directory.Delete(oldFolder, true); }
                catch (Exception ex)
                {
                    MessageBox.Show($"Kon oude map niet verwijderen: {ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            shootBeingEdited = null;
            selectedEditShootAddress = null;
            selectedEditShootContact = null;
            selectedEditContractPath = null;
            editShootContractReplaced = false;

            RefreshShootOverview();
            HidePanels();
            ShootsView.Visibility = Visibility.Visible;
        }

        private void CancelEditShoot_Click(object sender, RoutedEventArgs e)
        {
            if (editShootContractReplaced)
            {
                MessageBox.Show("You replaced the contract file. You must press 'Save' to finalize this change. Cancel is no longer allowed.", "Action Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            shootBeingEdited = null;
            selectedEditShootAddress = null;
            selectedEditShootContact = null;
            selectedEditContractPath = null;
            editShootContractReplaced = false;

            HidePanels();
            ShootsView.Visibility = Visibility.Visible;
        }



        private void AddEditShootAddress_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
            {
                selectedEditShootAddress = address;
                EditShootAddressToggleList.Items.Refresh();
            }
        }

        private void RemoveEditShootAddress_Click(object sender, RoutedEventArgs e)
        {
            selectedEditShootAddress = null;
            EditShootAddressToggleList.Items.Refresh();
        }

        private void UpdateAddEditShootAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address)
            {
                btn.IsEnabled = selectedEditShootAddress == null;
            }
        }

        private void UpdateRemoveEditShootAddressButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Address address)
            {
                btn.IsEnabled = selectedEditShootAddress?.Id == address.Id;
            }
        }


        private void AddEditShootContact_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact contact)
            {
                selectedEditShootContact = contact;
                EditShootContactToggleList.Items.Refresh();
            }
        }

        private void RemoveEditShootContact_Click(object sender, RoutedEventArgs e)
        {
            selectedEditShootContact = null;
            EditShootContactToggleList.Items.Refresh();
        }

        private void UpdateAddEditShootContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact)
            {
                btn.IsEnabled = selectedEditShootContact == null;
            }
        }

        private void UpdateRemoveEditShootContactButtonState(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Contact contact)
            {
                btn.IsEnabled = selectedEditShootContact?.Id == contact.Id;
            }
        }

        private void QuickAddAddressFromNewShoot_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            addressReturnToView = "NewShoot";
            ResetQuickAddressFormFromShoot();
            QuickAddAddressFormFromShoot.Visibility = Visibility.Visible;
        }

        private void QuickAddAddressFromEditShoot_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            addressReturnToView = "EditShoot";
            ResetQuickAddressFormFromShoot();
            QuickAddAddressFormFromShoot.Visibility = Visibility.Visible;
        }

        private void CreateQuickAddressFromShoot_Click(object sender, RoutedEventArgs e)
        {
            bool isLocationOnly = ShootIsLocationOnlyCheckBox.IsChecked == true;

            string? locationName = isLocationOnly ? ShootLocationNameTextBox.Text : null;

            Address newAddress = new Address(
                id: 0,
                locationName: locationName,
                isLocationOnly: isLocationOnly,
                street: ShootStreetTextBox.Text,
                houseNumber: ShootHouseNumberTextBox.Text,
                postalCode: ShootPostalCodeTextBox.Text,
                city: ShootCityTextBox.Text,
                country: ShootCountryTextBox.Text
            );

            newAddress.Create();

            if (addressReturnToView == "NewShoot")
            {
                NewShootAddressToggleList.ItemsSource = new DAL().GetAllAddresses();
                NewShootForm.Visibility = Visibility.Visible;
            }
            else if (addressReturnToView == "EditShoot")
            {
                EditShootAddressToggleList.ItemsSource = new DAL().GetAllAddresses();
                EditShootForm.Visibility = Visibility.Visible;
            }

            addressReturnToView = null;
            ResetQuickAddressFormFromShoot();
            QuickAddAddressFormFromShoot.Visibility = Visibility.Collapsed;
        }

        private void CancelQuickAddressFromShoot_Click(object sender, RoutedEventArgs e)
        {
            if (addressReturnToView == "NewShoot")
                NewShootForm.Visibility = Visibility.Visible;
            else if (addressReturnToView == "EditShoot")
                EditShootForm.Visibility = Visibility.Visible;

            addressReturnToView = null;
            ResetQuickAddressFormFromShoot();
            QuickAddAddressFormFromShoot.Visibility = Visibility.Collapsed;
        }

        private void ToggleQuickLocationFieldsFromShoot(object sender, RoutedEventArgs e)
        {
            bool isLocationOnly = ShootIsLocationOnlyCheckBox.IsChecked == true;

            ShootLocationNameTextBox.IsEnabled = isLocationOnly;

            ShootStreetTextBox.IsEnabled = !isLocationOnly;
            ShootHouseNumberTextBox.IsEnabled = !isLocationOnly;
            ShootPostalCodeTextBox.IsEnabled = !isLocationOnly;
            ShootCityTextBox.IsEnabled = !isLocationOnly;
            ShootCountryTextBox.IsEnabled = !isLocationOnly;
        }


        private void ResetQuickAddressFormFromShoot()
        {
            ShootIsLocationOnlyCheckBox.IsChecked = false;
            ShootLocationNameTextBox.Text = "";
            ShootStreetTextBox.Text = "";
            ShootHouseNumberTextBox.Text = "";
            ShootPostalCodeTextBox.Text = "";
            ShootCityTextBox.Text = "";
            ShootCountryTextBox.Text = "";
            ToggleQuickLocationFieldsFromShoot(null, null);
        }

        private void ShootsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ShootsDataGrid.SelectedItem is Shoot selected)
            {
                DetailShootDate.Text = $"Date: {selected.Date?.ToString("yyyy-MM-dd") ?? "–"}";

                DetailShootLocation.Text = "Location: " + (
                    selected.Location?.IsLocationOnly == true
                        ? selected.Location.LocationName
                        : $"{selected.Location?.Street} {selected.Location?.HouseNumber}, {selected.Location?.PostalCode} {selected.Location?.City}, {selected.Location?.Country}"
                );

                var contract = new DAL().GetContractByShootId(selected.Id);

                if (contract?.Signee != null)
                {
                    DetailShootSigneeName.Text = contract.Signee.FullName;
                    DetailShootSigneeName.DataContext = contract.Signee;
                }
                else
                {
                    DetailShootSigneeName.Text = "";
                    DetailShootSigneeName.DataContext = null;
                }

                ShootSigneePanel.Visibility = Visibility.Visible;

                DetailShootSigned.Text = "Contract Signed: " + (contract?.IsSigned == true ? "Yes" : "No");
                DetailShootSignedOn.Text = "Signed On: " + (contract?.SignedOn?.ToString("yyyy-MM-dd") ?? "–");

                selectedContractPathForDetail = contract?.Body;

                ShootContractPanel.Visibility = Visibility.Visible;
                DetailShootContract.Text = !string.IsNullOrWhiteSpace(contract?.Body)
                    ? System.IO.Path.GetFileName(contract.Body)
                    : "";



                shootLinkedConcepts = new DAL().GetAllConcepts()
                    .Where(c => c.Shoot?.Id == selected.Id)
                    .ToList();

                DetailShootConceptList.ItemsSource = shootLinkedConcepts;
                ShootLinkedConceptsPanel.Visibility = Visibility.Visible;

            }
            else
            {
                DetailShootDate.Text = "";
                DetailShootLocation.Text = "";
                DetailShootSigneeName.Text = "";
                DetailShootSigneeName.DataContext = null;
                ShootSigneePanel.Visibility = Visibility.Collapsed;
                DetailShootSigned.Text = "";
                DetailShootSignedOn.Text = "";
                DetailShootContract.Text = "";
                ShootContractPanel.Visibility = Visibility.Collapsed;
                DetailShootConceptList.ItemsSource = null;
                ShootLinkedConceptsPanel.Visibility = Visibility.Collapsed;
                selectedContractPathForDetail = null;
            }
        }

        private void LinkedSignee_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is Contact contact)
            {
                HidePanels();
                ContactsView.Visibility = Visibility.Visible;
                RefreshContactOverview();

                var matchingContact = (ContactsDataGrid.ItemsSource as IEnumerable<Contact>)?
                    .FirstOrDefault(c => c.Id == contact.Id);

                if (matchingContact != null)
                {
                    ContactsDataGrid.SelectedItem = null;
                    ContactsDataGrid.Items.Refresh();
                    ContactsDataGrid.SelectedItem = matchingContact;
                    ContactsDataGrid.ScrollIntoView(matchingContact);
                    ContactsDataGrid_SelectionChanged(ContactsDataGrid, null);
                }
            }
        }


        private void DetailShootContract_Click(object sender, MouseButtonEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(selectedContractPathForDetail) && File.Exists(selectedContractPathForDetail))
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = selectedContractPathForDetail,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DetailShootConcepts_Click(object sender, MouseButtonEventArgs e)
        {
            if (ShootsDataGrid.SelectedItem is Shoot shoot)
            {
                var concepts = new DAL().GetAllConcepts().Where(c => c.Shoot?.Id == shoot.Id).ToList();

                if (concepts.Count == 0)
                {
                    return;
                }

                var selectedConceptId = concepts.First().Id;

                HidePanels();
                ConceptsView.Visibility = Visibility.Visible;

                var matchingConcept = (ConceptsDataGrid.ItemsSource as IEnumerable<Concept>)?
                    .FirstOrDefault(c => c.Id == selectedConceptId);

                if (matchingConcept != null)
                {
                    ConceptsDataGrid.SelectedItem = null;
                    ConceptsDataGrid.Items.Refresh();
                    ConceptsDataGrid.SelectedItem = matchingConcept;
                    ConceptsDataGrid.ScrollIntoView(matchingConcept);

                    ConceptsDataGrid_SelectionChanged(ConceptsDataGrid, null);
                }
            }
        }

        private void QuickAddContactFromNewShoot_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            quickShootContactReturnToView = "NewShoot";
            ResetQuickShootContactForm();
            QuickAddContactFromShootForm.Visibility = Visibility.Visible;
        }

        private void QuickAddContactFromEditShoot_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            quickShootContactReturnToView = "EditShoot";
            ResetQuickShootContactForm();
            QuickAddContactFromShootForm.Visibility = Visibility.Visible;
        }

        private void ResetQuickShootContactForm()
        {
            QuickShootContactFirstNameTextBox.Text = "";
            QuickShootContactLastNameTextBox.Text = "";
            QuickShootContactPhoneTextBox.Text = "";
            QuickShootContactEmailTextBox.Text = "";
            QuickShootContactSocialTextBox.Text = "";
            QuickShootContactRoleComboBox.SelectedIndex = -1;
            QuickShootContactPayCheckBox.IsChecked = false;
        }

        private void CreateQuickShootContact_Click(object sender, RoutedEventArgs e)
        {
            string firstName = QuickShootContactFirstNameTextBox.Text;
            string lastName = QuickShootContactLastNameTextBox.Text;
            string phone = QuickShootContactPhoneTextBox.Text;
            string email = QuickShootContactEmailTextBox.Text;
            string social = QuickShootContactSocialTextBox.Text;
            string role = (QuickShootContactRoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            bool payment = QuickShootContactPayCheckBox.IsChecked == true;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please fill in at least First Name, Last Name and Role.");
                return;
            }

            if (new DAL().ContactNameExists(firstName, lastName))
            {
                MessageBox.Show("A contact with this name already exists.");
                return;
            }

            Contact newContact = new Contact(0, firstName, lastName, phone, email, social, null, role, payment, null);
            newContact.Create();

            if (quickShootContactReturnToView == "NewShoot")
            {
                NewShootContactToggleList.ItemsSource = new DAL().GetAllContacts();
                NewShootForm.Visibility = Visibility.Visible;
            }
            else if (quickShootContactReturnToView == "EditShoot")
            {
                EditShootContactToggleList.ItemsSource = new DAL().GetAllContacts();
                EditShootForm.Visibility = Visibility.Visible;
            }

            quickShootContactReturnToView = null;
            QuickAddContactFromShootForm.Visibility = Visibility.Collapsed;
        }

        private void CancelQuickShootContact_Click(object sender, RoutedEventArgs e)
        {
            if (quickShootContactReturnToView == "NewShoot")
                NewShootForm.Visibility = Visibility.Visible;
            else if (quickShootContactReturnToView == "EditShoot")
                EditShootForm.Visibility = Visibility.Visible;

            quickShootContactReturnToView = null;
            QuickAddContactFromShootForm.Visibility = Visibility.Collapsed;
        }













        // ADDRESSES

        private void RefreshAddressOverview()
        {
            AddressesDataGrid.ItemsSource = new DAL().GetAllAddresses();
        }

        private void NewAddressButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            NewAddressForm.Visibility = Visibility.Visible;

            IsLocationOnlyCheckBox.IsChecked = false;
            NewAddressLocationNameTextBox.Text = "";
            NewAddressStreetTextBox.Text = "";
            NewAddressHouseNumberTextBox.Text = "";
            NewAddressPostalCodeTextBox.Text = "";
            NewAddressCityTextBox.Text = "";
            NewAddressCountryTextBox.Text = "";

            ToggleLocationFields(null, null);
        }



        private void ToggleLocationFields(object sender, RoutedEventArgs e)
        {
            bool isLocationOnly = IsLocationOnlyCheckBox.IsChecked == true;

            NewAddressLocationNameTextBox.IsEnabled = isLocationOnly;

            NewAddressStreetTextBox.IsEnabled = !isLocationOnly;
            NewAddressHouseNumberTextBox.IsEnabled = !isLocationOnly;
            NewAddressPostalCodeTextBox.IsEnabled = !isLocationOnly;
            NewAddressCityTextBox.IsEnabled = !isLocationOnly;
            NewAddressCountryTextBox.IsEnabled = !isLocationOnly;
        }

        private void CreateNewAddress_Click(object sender, RoutedEventArgs e)
        {
            bool isLocationOnly = IsLocationOnlyCheckBox.IsChecked == true;

            string? locationName = isLocationOnly
                ? NewAddressLocationNameTextBox.Text
                : null;


            Address newAddress = new Address(
                id: 0,
                locationName: locationName,
                isLocationOnly: isLocationOnly,
                street: NewAddressStreetTextBox.Text,
                houseNumber: NewAddressHouseNumberTextBox.Text,
                postalCode: NewAddressPostalCodeTextBox.Text,
                city: NewAddressCityTextBox.Text,
                country: NewAddressCountryTextBox.Text
            );

            newAddress.Create();

            RefreshAddressOverview();
            HidePanels();
            AddressesView.Visibility = Visibility.Visible;
        }

        private void CancelNewAddress_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            AddressesView.Visibility = Visibility.Visible;
        }

        private void EditAddress_Click(object sender, RoutedEventArgs e)
        {
            Address? selected = AddressesDataGrid.SelectedItem as Address;

            if (selected == null)
            {
                MessageBox.Show("Please select an address to edit.");
                return;
            }

            EditIsLocationOnlyCheckBox.IsChecked = selected.IsLocationOnly;
            addressBeingEdited = selected;

            EditAddressLocationNameTextBox.Text = selected.LocationName;
            EditAddressStreetTextBox.Text = selected.Street;
            EditAddressHouseNumberTextBox.Text = selected.HouseNumber;
            EditAddressPostalCodeTextBox.Text = selected.PostalCode;
            EditAddressCityTextBox.Text = selected.City;
            EditAddressCountryTextBox.Text = selected.Country;

            EditAddressForm.Visibility = Visibility.Visible;
            AddressesView.Visibility = Visibility.Collapsed;
            ToggleEditLocationFields(null, null);

        }

        private void SaveEditAddress_Click(object sender, RoutedEventArgs e)
        {
            if (addressBeingEdited == null) return;

            addressBeingEdited.IsLocationOnly = EditIsLocationOnlyCheckBox.IsChecked == true;
            addressBeingEdited.LocationName = EditAddressLocationNameTextBox.Text;
            addressBeingEdited.Street = EditAddressStreetTextBox.Text;
            addressBeingEdited.HouseNumber = EditAddressHouseNumberTextBox.Text;
            addressBeingEdited.PostalCode = EditAddressPostalCodeTextBox.Text;
            addressBeingEdited.City = EditAddressCityTextBox.Text;
            addressBeingEdited.Country = EditAddressCountryTextBox.Text;

            addressBeingEdited.Update();
            addressBeingEdited = null;

            RefreshAddressOverview();
            EditAddressForm.Visibility = Visibility.Collapsed;
            AddressesView.Visibility = Visibility.Visible;
        }

        private void CancelEditAddress_Click(object sender, RoutedEventArgs e)
        {
            addressBeingEdited = null;
            EditAddressForm.Visibility = Visibility.Collapsed;
            AddressesView.Visibility = Visibility.Visible;
        }

        private void ToggleEditLocationFields(object sender, RoutedEventArgs e)
        {
            bool isLocationOnly = EditIsLocationOnlyCheckBox.IsChecked == true;

            EditAddressLocationNameTextBox.IsEnabled = isLocationOnly;

            EditAddressStreetTextBox.IsEnabled = !isLocationOnly;
            EditAddressHouseNumberTextBox.IsEnabled = !isLocationOnly;
            EditAddressPostalCodeTextBox.IsEnabled = !isLocationOnly;
            EditAddressCityTextBox.IsEnabled = !isLocationOnly;
            EditAddressCountryTextBox.IsEnabled = !isLocationOnly;
        }



        private void DeleteAddress_Click(object sender, RoutedEventArgs e)
        {
            Address? selected = AddressesDataGrid.SelectedItem as Address;

            if (selected == null)
            {
                MessageBox.Show("Please select an address to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the address \"{(string.IsNullOrWhiteSpace(selected.LocationName) ? selected.Street + " " + selected.HouseNumber : selected.LocationName)}\"?\n\nAll references from contacts, shoots, and concepts will be removed.",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                new DAL().UnlinkAddressReferences(selected.Id);
                selected.Delete();
                RefreshAddressOverview();
            }
        }

        private void AddressesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DetailAddressLocation.Text = "";
            DetailAddressStreet.Text = "";
            DetailAddressHouseNumber.Text = "";
            DetailAddressPostalCode.Text = "";
            DetailAddressCity.Text = "";
            DetailAddressCountry.Text = "";

            if (AddressesDataGrid.SelectedItem is Address selected)
            {
                if (selected.IsLocationOnly)
                {
                    DetailAddressLocation.Text = $"Location: {selected.LocationName}";
                }
                else
                {
                    DetailAddressStreet.Text = $"Street: {selected.Street}";
                    DetailAddressHouseNumber.Text = $"House Number: {selected.HouseNumber}";
                    DetailAddressPostalCode.Text = $"Postal Code: {selected.PostalCode}";
                    DetailAddressCity.Text = $"City: {selected.City}";
                    DetailAddressCountry.Text = $"Country: {selected.Country}";
                }
            }

        }

















        // CONTACTS

        private void RefreshContactOverview()
        {
            ContactsDataGrid.ItemsSource = new DAL().GetAllContacts();
        }

        private void NewContactButton_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();

            NewContactFirstNameTextBox.Text = "";
            NewContactLastNameTextBox.Text = "";
            NewContactPhoneTextBox.Text = "";
            NewContactEmailTextBox.Text = "";
            NewContactSocialTextBox.Text = "";
            NewContactRoleComboBox.SelectedIndex = -1;
            NewContactPayCheckBox.IsChecked = false;

            NewContactForm.Visibility = Visibility.Visible;
        }



        private void CreateNewContact_Click(object sender, RoutedEventArgs e)
        {
            string firstName = NewContactFirstNameTextBox.Text;
            string lastName = NewContactLastNameTextBox.Text;
            string phone = NewContactPhoneTextBox.Text;
            string email = NewContactEmailTextBox.Text;
            string social = NewContactSocialTextBox.Text;
            string role = (NewContactRoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            bool payment = NewContactPayCheckBox.IsChecked == true;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Please enter at least a first name, last name, and select a role.");
                return;
            }

            if (new DAL().ContactNameExists(firstName, lastName))
            {
                MessageBox.Show("A contact with this name already exists.");
                return;
            }


            Contact newContact = new Contact(0, firstName, lastName, phone, email, social, null, role, payment, null);
            newContact.Create();

            RefreshContactOverview();
            HidePanels();
            ContactsView.Visibility = Visibility.Visible;
        }


        private void CancelNewContact_Click(object sender, RoutedEventArgs e)
        {
            HidePanels();
            ContactsView.Visibility = Visibility.Visible;
        }

        private void EditContact_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsDataGrid.SelectedItem is not Contact selected)
            {
                MessageBox.Show("Please select a contact to edit.");
                return;
            }

            contactBeingEdited = selected;

            EditContactFirstNameTextBox.Text = selected.FirstName;
            EditContactLastNameTextBox.Text = selected.LastName;
            EditContactPhoneTextBox.Text = selected.Phone;
            EditContactEmailTextBox.Text = selected.Email;
            EditContactSocialTextBox.Text = selected.SocialMedia;
            EditContactRoleComboBox.SelectedIndex = selected.Role == "Model" ? 0 : 1;
            EditContactPayCheckBox.IsChecked = selected.Payment;

            HidePanels();
            EditContactForm.Visibility = Visibility.Visible;
        }

        private void SaveEditContact_Click(object sender, RoutedEventArgs e)
        {
            if (contactBeingEdited == null)
            {
                MessageBox.Show("No contact selected.");
                return;
            }

            contactBeingEdited.FirstName = EditContactFirstNameTextBox.Text;
            contactBeingEdited.LastName = EditContactLastNameTextBox.Text;
            contactBeingEdited.Phone = EditContactPhoneTextBox.Text;
            contactBeingEdited.Email = EditContactEmailTextBox.Text;
            contactBeingEdited.SocialMedia = EditContactSocialTextBox.Text;
            contactBeingEdited.Role = (EditContactRoleComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            contactBeingEdited.Payment = EditContactPayCheckBox.IsChecked == true;

            contactBeingEdited.Update();

            contactBeingEdited = null;
            RefreshContactOverview();
            HidePanels();
            ContactsView.Visibility = Visibility.Visible;
        }

        private void CancelEditContact_Click(object sender, RoutedEventArgs e)
        {
            contactBeingEdited = null;
            HidePanels();
            ContactsView.Visibility = Visibility.Visible;
        }



        private void DeleteContact_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsDataGrid.SelectedItem is not Contact selected)
            {
                MessageBox.Show("Please select a contact to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show(
                $"Are you sure you want to delete the contact \"{selected.FullName}\"?\n\n" +
                "This contact will also be removed from any linked concepts and contracts. The shoots, concepts and addresses themselves will not be deleted.",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            new DAL().UnlinkContactReferences(selected.Id);
            selected.Delete();

            RefreshContactOverview();
        }

        private void ContactsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContactsDataGrid.SelectedItem is Contact selected)
            {
                DetailContactName.Text = $"Name: {selected.FullName}";
                DetailContactEmail.Text = $"Email: {selected.Email}";
                DetailContactPhone.Text = $"Phone: {selected.Phone}";
                DetailContactSocial.Text = $"Social Media: {selected.SocialMedia}";
                DetailContactRole.Text = $"Role: {selected.Role}";
                DetailContactPayment.Text = $"Payment: {(selected.Payment ? "Yes" : "No")}";

                contactLinkedConcepts = new DAL().GetAllConcepts()
                    .Where(c => c.Models.Any(m => m.Id == selected.Id)).ToList();

                DetailContactConceptList.ItemsSource = contactLinkedConcepts;
                
                contactLinkedShoots = contactLinkedConcepts
                    .Where(c => c.Shoot != null)
                    .Select(c => c.Shoot!)
                    .DistinctBy(s => s.Id)
                    .ToList();

                DetailContactShootList.ItemsSource = contactLinkedShoots;
                ContactLinkedShootsPanel.Visibility = Visibility.Visible;
                ContactLinkedConceptsPanel.Visibility = Visibility.Visible;
            }
            else
            {
                DetailContactName.Text = "";
                DetailContactEmail.Text = "";
                DetailContactPhone.Text = "";
                DetailContactSocial.Text = "";
                DetailContactRole.Text = "";
                DetailContactPayment.Text = "";
                DetailContactConceptList.ItemsSource = null;
                DetailContactShootList.ItemsSource = null;
                ContactLinkedShootsPanel.Visibility = Visibility.Collapsed;

                ContactLinkedConceptsPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void LinkedConcept_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is Concept concept)
            {
                HidePanels();
                ConceptsView.Visibility = Visibility.Visible;

                var matching = (ConceptsDataGrid.ItemsSource as IEnumerable<Concept>)?
                    .FirstOrDefault(c => c.Id == concept.Id);

                if (matching != null)
                {
                    ConceptsDataGrid.SelectedItem = null;
                    ConceptsDataGrid.Items.Refresh();
                    ConceptsDataGrid.SelectedItem = matching;
                    ConceptsDataGrid.ScrollIntoView(matching);
                    ConceptsDataGrid_SelectionChanged(ConceptsDataGrid, null);
                }
            }
        }

        private void LinkedShoot_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock tb && tb.DataContext is Shoot shoot)
            {
                HidePanels();
                ShootsView.Visibility = Visibility.Visible;
                RefreshShootOverview();

                var matchingShoot = (ShootsDataGrid.ItemsSource as IEnumerable<Shoot>)?
                    .FirstOrDefault(s => s.Id == shoot.Id);

                if (matchingShoot != null)
                {
                    ShootsDataGrid.SelectedItem = null;
                    ShootsDataGrid.Items.Refresh();
                    ShootsDataGrid.SelectedItem = matchingShoot;
                    ShootsDataGrid.ScrollIntoView(matchingShoot);
                    ShootsDataGrid_SelectionChanged(ShootsDataGrid, null);
                }
            }
        }



        // PROJECTS

        private void RefreshProjectOverview()
        {
            ProjectsDataGrid.ItemsSource = new DAL().GetAllProjects();
        }
    }
}