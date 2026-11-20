using System;
using System.Windows.Forms;
using RentalManagementSystem.Controllers;
using RentalManagementSystem.Models;

namespace RentalManagementSystem.Views
{
    public partial class PaymentForm : Form
    {
        private readonly PaymentController _controller = new();
        private int selectedId = 0;

        public PaymentForm()
        {
            InitializeComponent();
        }

        private void PaymentForm_Load(object sender, EventArgs e)
        {
            cboRental.DataSource = _controller.GetRentalChoices();
            cboRental.DisplayMember = "Display";
            cboRental.ValueMember = "RentalID";

            if (cboStatus.Items.Count == 0)
                cboStatus.Items.AddRange(new object[] { "Paid", "Pending", "Late" });
            cboStatus.SelectedIndex = 0;

            dtpPaymentDate.Value = DateTime.Today;
            LoadData();
        }

        private void LoadData()
        {
            dgvPayments.DataSource = null;
            dgvPayments.DataSource = _controller.GetAll();
            if (dgvPayments.Columns["RentalID"] != null)
                dgvPayments.Columns["RentalID"].Visible = false;
        }

        private bool TryReadInputs(out Payment p)
        {
            p = new Payment();
            if (!decimal.TryParse(txtAmount.Text, out decimal amount))
            {
                MessageBox.Show("Amount must be a valid number.");
                return false;
            }
            p.PaymentID = selectedId;
            p.RentalID = cboRental.SelectedValue is int r ? r : 0;
            p.Amount = amount;
            p.PaymentDate = dtpPaymentDate.Value.Date;
            p.Status = cboStatus.Text;
            return true;
        }

        private void ClearInputs()
        {
            selectedId = 0;
            txtAmount.Clear();
            if (cboRental.Items.Count > 0) cboRental.SelectedIndex = 0;
            dtpPaymentDate.Value = DateTime.Today;
            cboStatus.SelectedIndex = 0;
            dgvPayments.ClearSelection();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!TryReadInputs(out var p)) return;
            string result = _controller.Add(p);
            if (result != "OK") { MessageBox.Show(result); return; }
            MessageBox.Show("Payment added.");
            ClearInputs(); LoadData();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a payment from the list first."); return; }
            if (!TryReadInputs(out var p)) return;
            string result = _controller.Update(p);
            if (result != "OK") { MessageBox.Show(result); return; }
            MessageBox.Show("Payment updated.");
            ClearInputs(); LoadData();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == 0) { MessageBox.Show("Select a payment from the list first."); return; }
            if (MessageBox.Show("Delete this payment?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            string result = _controller.Delete(selectedId);
            if (result != "OK") { MessageBox.Show(result); return; }
            ClearInputs(); LoadData();
        }

        private void btnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void dgvPayments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvPayments.Rows[e.RowIndex].DataBoundItem is not Payment p) return;
            selectedId = p.PaymentID;
            cboRental.SelectedValue = p.RentalID;
            txtAmount.Text = p.Amount.ToString();
            dtpPaymentDate.Value = p.PaymentDate;
            cboStatus.Text = p.Status;
        }
    }
}