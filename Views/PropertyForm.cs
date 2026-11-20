using System;
using System.Windows.Forms;
using RentalManagementSystem.Controllers;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Views
{
    public partial class PropertyForm : Form
    {
        private readonly PropertyController _controller = new();
        private int selectedId = 0;

        public PropertyForm()
        {
            InitializeComponent();
        }

        private void PropertyForm_Load(object sender, EventArgs e)
        {
            cboOwner.DataSource = _controller.GetOwners();
            cboOwner.DisplayMember = "FullName";
            cboOwner.ValueMember = "UserID";
            LoadData();
        }

        private void LoadData()
        {
            dgvProperties.DataSource = null;
            dgvProperties.DataSource = _controller.GetAll();
            if (dgvProperties.Columns["OwnerID"] != null) dgvProperties.Columns["OwnerID"].Visible = false;
        }

        private bool TryReadInputs(out Property p)
        {
            p = new Property();
            if (!decimal.TryParse(txtRent.Text, out decimal rent))
            {
                MessageBox.Show("Rent must be a valid number.");
                return false;
            }
            p.PropertyID = selectedId;
            p.OwnerID = cboOwner.SelectedValue is int id ? id : 0;
            p.Address = txtAddress.Text;
            p.City = txtCity.Text;
            p.RentAmount = rent;
            p.Description = txtDescription.Text;
            return true;
        }

        private void ClearInputs()
        {
            selectedId = 0;
            txtAddress.Clear(); txtCity.Clear(); txtRent.Clear(); txtDescription.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!TryReadInputs(out var p)) return;
            string result = _controller.Add(p);
            if (result != "OK") { MessageBox.Show(result); return; }
            MessageBox.Show("Property added.");
            ClearInputs(); LoadData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a property from the list first."); return; }
            if (!TryReadInputs(out var p)) return;
            string result = _controller.Update(p);
            if (result != "OK") { MessageBox.Show(result); return; }
            MessageBox.Show("Property updated.");
            ClearInputs(); LoadData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a property from the list first."); return; }
            if (MessageBox.Show("Delete this property?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            string result = _controller.Delete(selectedId);
            if (result != "OK") { MessageBox.Show(result); return; }
            ClearInputs(); LoadData();
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void dgvProperties_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var p = (Property)dgvProperties.Rows[e.RowIndex].DataBoundItem;
            selectedId = p.PropertyID;
            cboOwner.SelectedValue = p.OwnerID;
            txtAddress.Text = p.Address;
            txtCity.Text = p.City;
            txtRent.Text = p.RentAmount.ToString();
            txtDescription.Text = p.Description;
        }
    }
}