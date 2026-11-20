using System;
using System.Windows.Forms;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Views
{
    public partial class MainForm : Form
    {
        private readonly User _user;

        public MainForm(User user)
        {
            InitializeComponent();
            _user = user;
            this.Text = "Rental Management System - Welcome " + _user.FullName;
        }

        private void tenantsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new TenantForm().ShowDialog();
        }

        private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PropertyForm().ShowDialog();
        }

        private void rentalsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new RentalForm().ShowDialog();
        }

        private void paymentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new PaymentForm().ShowDialog();
        }

        private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}