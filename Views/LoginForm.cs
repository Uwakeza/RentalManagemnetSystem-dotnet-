using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic.ApplicationServices;
using RentalManagementSystem.Controllers;
using RentalManagementSystem.Models;   // <-- the missing lineusing System;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;


namespace RentalManagementSystem.Views
{
    public partial class LoginForm : Form
    {
        private readonly AuthController _auth = new();

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Click(object sender, EventArgs e)
        {
            // leave empty (created by accident when you clicked the form)
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            var user = _auth.Login(txtEmail.Text, txtPassword.Text);
            if (user == null)
            {
                lblError.Text = "Invalid email or password.";
                return;
            }

            this.Hide();
            var main = new MainForm(user);
            main.FormClosed += (s, args) => { txtPassword.Clear(); this.Show(); };
            main.Show();
        }
    }
}
