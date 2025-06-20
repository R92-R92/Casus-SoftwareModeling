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
        }


        private void ConceptsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ConceptsDataGrid.SelectedItem is Concept selected)
            {
                DetailName.Text = $"Name: {selected.Name}";
                DetailDescription.Text = $"Description: {selected.Description}";
                DetailProps.Text = $"Props: {selected.PropsText}";
                DetailModels.Text = $"Models: {selected.ModelText}";

                DetailAddress.Text = selected.Address != null
                    ? $"Address: {selected.Address.Street} {selected.Address.HouseNumber}, {selected.Address.PostalCode} {selected.Address.City}, {selected.Address.Country}"
                    : "Address: –";

                DetailShoot.Text = selected.Shoot != null
                    ? $"Shoot: {selected.Shoot.Date?.ToString("yyyy-MM-dd")}"
                    : "Shoot: –";

                // Sketch preview tonen of verbergen
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

                // Eerste geldige afbeelding uit pictures tonen of verbergen
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
                DetailDescription.Text = "";
                DetailProps.Text = "";
                DetailModels.Text = "";
                DetailAddress.Text = "";
                DetailShoot.Text = "";

                DetailSketchPreviewImage.Source = null;
                DetailSketchBorder.Visibility = Visibility.Collapsed;

                DetailPicturePreviewImage.Source = null;
                DetailPictureBorder.Visibility = Visibility.Collapsed;
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
            NewAddressStreetTextBox.Text = "";
            NewAddressHouseNumberTextBox.Text = "";
            NewAddressPostalCodeTextBox.Text = "";
            NewAddressCityTextBox.Text = "";
            NewAddressCountryTextBox.Text = "";
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
            Address newAddress = new Address( id: 0, street: NewAddressStreetTextBox.Text, houseNumber: NewAddressHouseNumberTextBox.Text, postalCode: NewAddressPostalCodeTextBox.Text, city: NewAddressCityTextBox.Text, country: NewAddressCountryTextBox.Text);

            //newAddress.Create(); 

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

            //EditModelSelectionListBox.ItemsSource = new DAL().GetAllContacts();
            //EditShootSelectionComboBox.ItemsSource = new DAL().GetAllShoots();
            editSelectedShoot = selected.Shoot;
            EditShootToggleList.ItemsSource = new DAL().GetAllShoots();

            EditConceptNameTextBox.Text = selected.Name;
            EditConceptDescriptionTextBox.Text = selected.Description;
            if (selected.Address != null)
            {
                EditAddressStreetTextBox.Text = selected.Address.Street;
                EditAddressHouseNumberTextBox.Text = selected.Address.HouseNumber;
                EditAddressPostalCodeTextBox.Text = selected.Address.PostalCode;
                EditAddressCityTextBox.Text = selected.Address.City;
                EditAddressCountryTextBox.Text = selected.Address.Country;
            }
            else
            {
                EditAddressStreetTextBox.Text = "";
                EditAddressHouseNumberTextBox.Text = "";
                EditAddressPostalCodeTextBox.Text = "";
                EditAddressCityTextBox.Text = "";
                EditAddressCountryTextBox.Text = "";
            }

            //EditModelSelectionListBox.SelectedItems.Clear();
            //foreach (var model in selected.Models)
            //{
            //    var match = (EditModelSelectionListBox.ItemsSource as List<Contact>)?.FirstOrDefault(m => m.Id == model.Id);
            //    if (match != null)
            //        EditModelSelectionListBox.SelectedItems.Add(match);
            //}


            //var shootMatch = (EditShootSelectionComboBox.ItemsSource as List<Shoot>)?.FirstOrDefault(s => s.Id == selected.Shoot?.Id);
            //EditShootSelectionComboBox.SelectedItem = shootMatch;


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
            Address updatedAddress = new Address(
    id: conceptBeingEdited.Address?.Id ?? 0,
    street: EditAddressStreetTextBox.Text,
    houseNumber: EditAddressHouseNumberTextBox.Text,
    postalCode: EditAddressPostalCodeTextBox.Text,
    city: EditAddressCityTextBox.Text,
    country: EditAddressCountryTextBox.Text
);

            // Als het een nieuw address is, maak hem aan. Anders update.
            if (updatedAddress.Id == 0)
                updatedAddress.Create();
            else
                updatedAddress.Update();

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



        // SHOOTS - R

        private void RefreshShootOverview()
        {
            ShootsDataGrid.ItemsSource = new DAL().GetAllShoots();
        }


        // CONTACTS

        private void RefreshContactOverview()
        {
            ContactsDataGrid.ItemsSource = new DAL().GetAllContacts();
        }

        // PROJECTS

        private void RefreshProjectOverview()
        {
            ProjectsDataGrid.ItemsSource = new DAL().GetAllProjects();
        }
    }
}