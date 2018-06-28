using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;

namespace Login
{
    public partial class RegisterWindow
    {
        public RegisterWindow()
        {
            InitializeComponent();
            TxtBoxName.Focus();
        }

        private void BtnRegisterTrue_OnClick(object sender, RoutedEventArgs e)
        {
            string[] userData = { TxtBoxName.Text.Substring(0, 1).ToUpper() + TxtBoxName.Text.Substring(1).ToLower(), 
                TxtBoxSurname.Text.Substring(0, 1).ToUpper() + TxtBoxSurname.Text.Substring(1).ToLower(), 
                TxtBoxLogin.Text, new Cryptography().SecurePassword(TxtBoxPassword.Password), TxtBoxEmail.Text.ToLower() };
            UInt16 checksum = 0;
            foreach (var data in userData) if (!String.IsNullOrEmpty(data)) checksum++;
            
            // TODO: Improve handling undermentioned errors
            if (checksum == 5)
                if (TxtBoxPassword.Password == TxtBoxPasswordRepeat.Password)
                    if (TxtBoxPassword.Password.Any(c => c > 32 && c < 127 && !(c >= '0' && c <= '9') &&
                        !((c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z'))))
                        if (TxtBoxPassword.Password.Length > 7 || TxtBoxPassword.Password.Length < 20)
                            if (new EmailAddressAttribute().IsValid(TxtBoxEmail.Text))
                                try {
                                    if (new Executioner().RegisterInsertQuery(string.Join("', '", userData))) {
                                        MessageBox.Show("Successfully registered!\nYou can now log in.");
                                        new GenerateEmail().SendEmailRegister(TxtBoxEmail.Text, TxtBoxLogin.Text);
                                        Application.Current.MainWindow = new MainWindow();
                                        Close();
                                        Application.Current.MainWindow.Show();
                                    }
                                } catch (Exception exception) { MessageBox.Show(exception.Message); }
                            else MessageBox.Show("Email address isn't valid!");
                        else MessageBox.Show("Password have to be at least 7 characters long and maximum 20 characters long!");
                    else MessageBox.Show("Password doesn't contain any special character!");
                else MessageBox.Show("Passwords doesn't match each other!");
            else MessageBox.Show("Too few data!");
        }
    }
}
