using System;
using System.Windows;

namespace Login
{
        /// <summary>
        ///     Interaction logic for MainWindow.xaml
        /// </summary>
    //TODO: Handle logging out
    //TODO: Use .NET authentication as more secure approach
    public partial class MainWindow
    {
        private readonly Executioner _executioner = new Executioner();
        private volatile ushort _loginFails;

        public MainWindow()
        {
            InitializeComponent();
            TxtBoxLogin.Focus();
            TxtPassBox.Visibility = Visibility.Hidden;
            TxtPassBox.IsEnabled = false;
        }
      
        private void BtnLogin_OnClick(object sender, RoutedEventArgs e)
        {
            if (_loginFails < 3) {
                try {
                    if (_executioner.LoginSelectQuery(TxtBoxLogin.Text, PassBox.Password)) {
                        if (_executioner.LoginCheckPrivelages(TxtBoxLogin.Text))
                            Application.Current.MainWindow = new PaneAdmin();
                        else
                            Application.Current.MainWindow = new PaneUser();
                        Close();
                        Application.Current.MainWindow.Show();
                    } else {
                        //TODO: Penalty removed after certain passage of time
                        MessageBox.Show("Login and/or password is incorrect!");
                        _loginFails++;
                    }
                } catch (Exception exception) { MessageBox.Show(exception.Message); }
            } else {
                BtnLogin.IsEnabled = false;
                TxtBlockErr.Visibility = Visibility.Visible;
                MessageBox.Show("Action blocked after three or more unsuccesful tries.");
            }
        }

        private void BtnRegister_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow = new RegisterWindow();
            Close();
            Application.Current.MainWindow.Show();
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            //TODO: Make undermentioned, commented line of code work or inspect what's wrong with it
            //PassBox.PasswordChar = CheckBox.IsChecked == true ? default(char): '*';
            if (CheckBox.IsChecked == true) {
                PassBox.Visibility = Visibility.Hidden;
                PassBox.IsEnabled = false;
                TxtPassBox.IsEnabled = true;
                TxtPassBox.Text = PassBox.Password;
                TxtPassBox.Focus();
                TxtPassBox.Visibility = Visibility.Visible;
            } else {
                TxtPassBox.Visibility = Visibility.Hidden;
                TxtPassBox.IsEnabled = false;
                PassBox.IsEnabled = true;
                PassBox.Password = TxtPassBox.Text;
                PassBox.Focus();
                PassBox.Visibility = Visibility.Visible;
            }
        }
    }
}