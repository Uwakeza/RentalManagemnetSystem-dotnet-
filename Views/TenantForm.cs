using System;
using System.Windows.Forms;
using RentalManagementSystem.Controllers;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Views
{
    public partial class TenantForm : Form
    {
        private readonly TenantController _controller = new();
        private int selectedTenantId = 0;
        private int selectedUserId = 0;

        public TenantForm()
        {
            InitializeComponent();
        }

        private void TenantForm_Load(object sender, EventArgs e) => LoadData();

        private void LoadData()
        {
            dgvTenants.DataSource = null;
            dgvTenants.DataSource = _controller.GetAll();
            if (dgvTenants.Columns["UserID"] != null)
                dgvTenants.Columns["UserID"].Visible = false;
        }

        private Tenant ReadInputs() => new Tenant
        {
            TenantID = selectedTenantId,
            UserID = selectedUserId,
            FullName = txtFullName.Text,
            Email = txtEmail.Text,
            PhoneNumber = txtPhone.Text.Trim()
        };

        private void ClearInputs()
        {
            selectedTenantId = 0;
            selectedUserId = 0;
            txtFullName.Clear(); txtEmail.Clear(); txtPhone.Clear();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            string result = _controller.AddTenant(ReadInputs());
            if (result != "OK") { MessageBox.Show(result); return; }
            MessageBox.Show("Tenant added.");
            ClearInputs(); LoadData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedTenantId == 0) { MessageBox.Show("Select a tenant from the list first."); return; }
            string result = _controller.UpdateTenant(ReadInputs());
            if (result != "OK") { MessageBox.Show(result); return; }
            MessageBox.Show("Tenant updated.");
            ClearInputs(); LoadData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedTenantId == 0) { MessageBox.Show("Select a tenant from the list first."); return; }
            if (MessageBox.Show("Delete this tenant?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            string result = _controller.DeleteTenant(selectedTenantId, selectedUserId);
            if (result != "OK") { MessageBox.Show(result); return; }
            ClearInputs(); LoadData();
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void dgvTenants_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvTenants.Rows[e.RowIndex].DataBoundItem is not Tenant t) return;
            selectedTenantId = t.TenantID;
            selectedUserId = t.UserID;
        }
    }
}