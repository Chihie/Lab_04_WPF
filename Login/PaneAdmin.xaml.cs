using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Security;
using System.Windows;
using System.Windows.Controls;


namespace Login
{
    public partial class PaneAdmin
    {
        private readonly Executioner _executioner = new Executioner();
        private readonly GenerateEmail _generateEmail = new GenerateEmail();
        private readonly Cryptography _cryptography = new Cryptography();
        
        private readonly ObservableCollection<string> _actionList = new ObservableCollection<string>();
        private readonly ObservableCollection<string> _privelagesList = new ObservableCollection<string>();

        public PaneAdmin()
        {
            InitializeComponent();
            _executioner.FillDataGridPending(DataGridPending, false);
            FillActionComboBox();
            SetBoxesAvailability(true, false);
            SetBoxesAvailability(false, false);
        }

        private void SetBoxesAvailability(Boolean addUser, Boolean isEnabled)
        {
            if (addUser) {
                TxtBoxName.IsEnabled = isEnabled;
                TxtBoxSurname.IsEnabled = isEnabled;
                TxtBoxLogin.IsEnabled = isEnabled;
                TxtBoxEmail.IsEnabled = isEnabled;
                ComboBoxChoosePrivelages.IsEnabled = isEnabled;
            } else ComboBoxChooseUser.IsEnabled = isEnabled;
        }

        private void FillActionComboBox()
        {
            _actionList.Add("Add user");
            _actionList.Add("Delete user");
            _actionList.Add("Reset user's password");
            ComboBoxChooseAction.ItemsSource = _actionList;
        }
        
        private void FillPrivelagesComboBox()
        {
            _privelagesList.Add("user");
            _privelagesList.Add("admin");
            ComboBoxChoosePrivelages.ItemsSource = _privelagesList;
        }

        private void ClearBoxesContent()
        {
            TxtBoxName.Text = string.Empty;
            TxtBoxSurname.Text = string.Empty;
            TxtBoxLogin.Text = string.Empty;
            TxtBoxEmail.Text = string.Empty;
            ComboBoxChoosePrivelages.SelectedIndex = -1;
            ComboBoxChooseUser.SelectedIndex = -1;
            ComboBoxChooseAction.SelectedIndex = -1;
        }

        private void BtnSaveChange_OnClick(object sender, RoutedEventArgs e)
        {
            string generatedPassword;
            switch (ComboBoxChooseAction.SelectedIndex)
            {
                case 0:
                    generatedPassword = Membership.GeneratePassword(7,1);
                    string[] userData = { TxtBoxName.Text.Substring(0, 1).ToUpper() + TxtBoxName.Text.Substring(1).ToLower(), 
                        TxtBoxSurname.Text.Substring(0, 1).ToUpper() + TxtBoxSurname.Text.Substring(1).ToLower(), 
                        TxtBoxLogin.Text, _cryptography.SecurePassword(generatedPassword), TxtBoxEmail.Text.ToLower(), 
                        ComboBoxChoosePrivelages.SelectedItem.ToString() };
                    UInt16 checksum = 0;
                    foreach (string data in userData) if (!String.IsNullOrEmpty(data)) checksum++;
                    if (checksum == 6)
                        if (new EmailAddressAttribute().IsValid(TxtBoxEmail.Text))
                            try {
                                if (_executioner.AddUserInsertQuery(string.Join("', '", userData))) {
                                    MessageBox.Show("Successfully added new user!\nGenerated password will be send via email.");
                                    _generateEmail.SendEmailAddUser(TxtBoxEmail.Text, TxtBoxLogin.Text, generatedPassword);
                                    SetBoxesAvailability(true, false);
                                }
                            } catch (Exception exception) { MessageBox.Show(exception.Message); }
                        else MessageBox.Show("Email address isn't valid!");
                    else MessageBox.Show("Too few data!");
                    break;
                case 1:
                    if (MessageBox.Show("Sure to delete user?", "Delete user confirmation",
                            MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                        _executioner.DeleteUser(ComboBoxChooseUser.SelectedItem.ToString());
                        SetBoxesAvailability(false, false);
                    }
                    break;
                case 2:
                    string login = ComboBoxChooseUser.SelectedItem.ToString();
                    generatedPassword = Membership.GeneratePassword(7,1);
                    _generateEmail.SendEmailResetPassword(_executioner.GetEmailUser(login), login, generatedPassword);
                    _executioner.ResetPassword(login, _cryptography.SecurePassword(generatedPassword));
                    MessageBox.Show("New password will be send via email.");
                    SetBoxesAvailability(false, false);
                    break;
            }
            ClearBoxesContent();
        }

        private void ComboBoxChooseAction_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch ((sender as ComboBox).SelectedIndex)
            {
                case 0:
                    if(ComboBoxChooseUser.IsEnabled) ComboBoxChooseUser.IsEnabled = false;
                    if(!ComboBoxChoosePrivelages.HasItems) FillPrivelagesComboBox();
                    SetBoxesAvailability(true, true);
                    break;
                case 1:
                    if(TxtBoxName.IsEnabled) SetBoxesAvailability(true, false);
                    _executioner.ListAllNonAdminUsers(ComboBoxChooseUser);
                    ComboBoxChooseUser.IsEnabled = true;
                    break;
                case 2:
                    if(TxtBoxName.IsEnabled) SetBoxesAvailability(true, false);
                    _executioner.ListAllNonAdminUsers(ComboBoxChooseUser);
                    ComboBoxChooseUser.IsEnabled = true;
                    break;
            }
        }

        //TODO: Make this one actually work as intendent: send emails only when combobox state has actually changed
        //TODO: Choose a way which will speed up process:
        //TODO: DB call approach: Compare combobox data to data from table participant before updating
        //TODO: Saving state approach: Save combobox data when entering this view/after updating and compare when btn_OnClick()
        private void BtnPending_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (System.Data.DataRowView dr in DataGridPending.ItemsSource) {
                // confirm, id, event, login, type
                _executioner.UpdateParticipant(dr[0].ToString(), dr[1].ToString());
                _generateEmail.SendEmailPartcipantInfo(_executioner.GetEmailUser(dr[3].ToString()), dr[3].ToString(), 
                    dr[2].ToString(), dr[4].ToString(), dr[0].ToString());
                CheckBoxShowAll.IsChecked = true;
            }
        }

        private void CheckBoxShowAll_OnChecked(object sender, RoutedEventArgs e) =>
            _executioner.FillDataGridPending(DataGridPending, true);

        private void CheckBoxShowAll_OnUnchecked(object sender, RoutedEventArgs e) =>
            _executioner.FillDataGridPending(DataGridPending, false);
    }
}
