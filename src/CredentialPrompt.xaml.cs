/*
 Copyright (c) 2018 Kevin Ott
 Licensed under the MIT License
 See the LICENSE file in the project root for more information. 
*/

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

namespace Win_Info
{
    /// <summary>
    /// Interaction logic for CredentialPrompt.xaml
    /// </summary>
    public partial class CredentialPrompt : Window
    {
        private bool processing;
        public string Test { get; set; }
        public Credential Credential { get; private set; } = new Credential();
        private bool usernameInputValid = false;
        private bool passwordInputValid = false;

        public CredentialPrompt()
        {
            InitializeComponent();
        }

        // Cancel option
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Cancel clears passwordbox and closes
            PasswordBox.Clear();
            this.Close();
        }

        // Username text box event handler
        // Parses the domain and disables the OK button if the username or domain is invalid
        private void UserNameBox_TextChanged(object sender, RoutedEventArgs e)
        {
            if (processing)
                return;
            processing = true;

            // Parse username
            if(UserNameBox.Text == "")
            {
                usernameInputValid = false;
                DomainIndicator.Text = "";
            }
            else if (Credential.ParseDomain(UserNameBox.Text) == true)
            {
                if (Credential.domain != "Localhost")
                {
                    DomainIndicator.Text = Credential.domain.ToUpper();
                }
                else
                {
                    DomainIndicator.Text = Credential.domain;
                }
                usernameInputValid = true;
                if (passwordInputValid == true)
                {
                    OK.IsEnabled = true;
                }
            }
            else
            {
                usernameInputValid = false;
                DomainIndicator.Text = "Invalid Domain";
                OK.IsEnabled = false;
            }
            SetOKStatus();
            processing = false;
        }

        // Password box text change handler
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(PasswordBox.Password == String.Empty)
            {
                passwordInputValid = false;
            }
            else
            {
                passwordInputValid = true;
            }
            SetOKStatus();
        }

        // Enables or disables the OK option
        private void SetOKStatus()
        {
            if(usernameInputValid == false | passwordInputValid == false)
            {
                OK.IsEnabled = false;
            }
            else
            {
                OK.IsEnabled = true;
            }
        }

        // OK option
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if(UserNameBox.Text == "")
            {
                MessageBox.Show("Username cannot be blank.");
                return;
            }
            if (PasswordBox.Password == String.Empty)
            {
                MessageBox.Show("Password cannot be blank.");
                return;
            }
            // Parse the domain from the username, prompt if invalid
            if (Credential.ParseDomain(UserNameBox.Text) == false)
            {
                MessageBox.Show("Invalid Domain. Enter domain as DOMAIN\\USERNAME or USERNAME@DOMAIN");
                return;
            }

            // Parse the domain for invalid characters
            if (Credential.ParseInvalidCharacters(Credential.domain) == false)
            {
                MessageBox.Show("Domain contains invalid characters. AD domains cannot contains the characters \", ~:!#$%^&'(){}_ \"");
                return;
            }

            // Parse the username for invalid charachters
            if (Credential.ParseInvalidCharacters(Credential.userName) == false)
            {
                MessageBox.Show("Username contains invalid characters. AD user names cannot contains the characters \", ~:!#$%^&'(){}_ \"");
                return;
            }
            else
            {
                Credential.securePassword = PasswordBox.SecurePassword;
                PasswordBox.Clear();
                this.DialogResult = true;
            }
        }

        private void PasswordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                OK_Click(this, new RoutedEventArgs());
            }
        }
    }
}
