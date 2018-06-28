using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;


namespace Login
{
    //TODO: Make all artwork more readable, especially comboboxes
    public partial class PaneUser
    {
        readonly Executioner _executioner = new Executioner();
        readonly ObservableCollection<string> _attendantTypeList = new ObservableCollection<string>();
        readonly ObservableCollection<string> _cateringTypeList = new ObservableCollection<string>();
        private string _eventName;
        
        public PaneUser()
        {
            InitializeComponent();
            _executioner.FillComboboxWithEvents(ComboBoxEventName);
            FillPersonalizedComboBoxes();
        }

        //TODO: Add it as separate tables into db to make changes less painful to implement, more relational (and possibly less code)?
        private void FillPersonalizedComboBoxes()
        {
            ComboBoxAttendantType.IsEnabled = false;
            ComboBoxCateringType.IsEnabled = false;
            
            _attendantTypeList.Add("listener");
            _attendantTypeList.Add("author");
            _attendantTypeList.Add("sponsor");
            _attendantTypeList.Add("arranger");
            ComboBoxAttendantType.ItemsSource = _attendantTypeList;
            
            _cateringTypeList.Add("no-preferences");
            _cateringTypeList.Add("vegetarian");
            _cateringTypeList.Add("gluten-free");
            ComboBoxCateringType.ItemsSource = _cateringTypeList;
        }
        
        /* TODO: Consider clearing data inside boxes (would it be a good thing having in mind that one participant
         can attend one event as more than one participant type? */
        private void BtnSign_OnClick(object sender, RoutedEventArgs e) =>
            _executioner.AddParticipant(_eventName, ComboBoxAttendantType.SelectedItem.ToString(), 
                ComboBoxCateringType.SelectedItem.ToString());

        private void ComboBoxEventName_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
//            if (e.AddedItems != null && e.AddedItems.Count > 0)
//                object newValue = e.AddedItems[0];
            _eventName = (sender as ComboBox).SelectedItem as string;
            _executioner.FillTextBlocks(TextBlockAgenda, TextBlockDate, _eventName);
            ComboBoxAttendantType.IsEnabled = true;
            ComboBoxCateringType.IsEnabled = true;
        }
    }
}
