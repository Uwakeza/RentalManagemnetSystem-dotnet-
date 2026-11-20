using System;
using System.Windows.Forms;
using RentalManagementSystem.Controllers;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Views
{
    public partial class RentalForm : Form
    {
        private readonly RentalController _controller = new();
        private readonly TenantController _tenantController = new();
        private readonly PropertyController _propertyController = new();
        private int selectedId = 0;

        public RentalForm()
        {
            InitializeComponent();
        }

        private void RentalForm_Load(object sender, EventArgs e)
        {
            cboTenant.DataSource = _tenantController.GetAll();
            cboTenant.DisplayMember = "FullName";
            cboTenant.ValueMember = "TenantID";

            cboProperty.DataSource = _propertyController.GetAll();
            cboProperty.DisplayMember = "Address";
            cboProperty.ValueMember = "PropertyID";

            if (cboStatus.Items.Count == 0)
                cboStatus.Items.AddRange(new object[] { "Active", "Ended", "Cancelled" });
            cboStatus.SelectedIndex = 0;

            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today.AddYears(1);
            LoadData();
        }

        private void LoadData()
        {
            dgvRentals.DataSource = null;
            dgvRentals.DataSource = _controller.GetAll();
            if (dgvRentals.Columns["PropertyID"] != null) dgvRentals.Columns["PropertyID"].Visible = false;
            if (dgvRentals.Columns["TenantID"] != null) dgvRentals.Columns["TenantID"].Visible = false;
        }

        private Rental ReadInputs() => new Rental
        {
            RentalID = selectedId,
            TenantID = cboTenant.SelectedValue is int t ? t : 0,
            PropertyID = cboProperty.SelectedValue is int p ? p : 0,
            StartDate = dtpStart.Value.Date,
            EndDate = dtpEnd.Value.Date,
            Status = cboStatus.Text
        };

        private void ClearInputs()
        {
            selectedId = 0;
            dtpStart.Value = DateTime.Today;
            dtpEnd.Value = DateTime.Today.AddYears(1);
            cboStatus.SelectedIndex = 0;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string result = _controller.Add(ReadInputs());
            if (result != "OK") { MessageBox.Show(result); return; }
            MessageBox.Show("Rental added.");
            ClearInputs(); LoadData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a rental from the list first."); return; }
            string result = _controller.Update(ReadInputs());
            if (result != "OK") { MessageBox.Show(result); return; }
            MessageBox.Show("Rental updated.");
            ClearInputs(); LoadData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a rental from the list first."); return; }
            if (MessageBox.Show("Delete this rental?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            string result = _controller.Delete(selectedId);
            if (result != "OK") { MessageBox.Show(result); return; }
            ClearInputs(); LoadData();
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void dgvRentals_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var r = (Rental)dgvRentals.Rows[e.RowIndex].DataBoundItem;
            selectedId = r.RentalID;
            cboTenant.SelectedValue = r.TenantID;
            cboProperty.SelectedValue = r.PropertyID;
            dtpStart.Value = r.StartDate;
            dtpEnd.Value = r.EndDate;
            cboStatus.Text = r.Status;
        }
    }
}