using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;

namespace ExpenseTrackerApp
{
    public class Expense
    {
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }

    public partial class Form1 : Form
    {
        private List<Expense> expenses = new List<Expense>();
        private ListView expenseList;
        private Label totalLabel;
        private ComboBox categoryCombo;
        private TextBox amountInput;
        private DateTimePicker datePicker;
        private TextBox descInput;
        private ComboBox filterCombo;
        private string[] categories = { "Food", "Transport", "Entertainment", "Bills", "Shopping", "Other" };

        public Form1()
        {
            InitializeComponent();
            SetupUI();
            SetupEventHandlers();
        }

        private void SetupUI()
        {
            // Form settings
            this.Text = "Expense Tracker";
            this.Width = 800;
            this.Height = 600;
            this.BackColor = Color.White;
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create input controls
            CreateInputControls();
            CreateExpenseList();
            CreateFilterControls();
            CreateSummaryControls();
        }

        private void CreateInputControls()
        {
            // Category ComboBox
            Label categoryLabel = new Label()
            {
                Text = "Category:",
                Left = 20,
                Top = 23,
                AutoSize = true
            };

            categoryCombo = new ComboBox()
{
    Left = 120,
    Top = 20,
    Width = 150,
    DropDownStyle = ComboBoxStyle.DropDownList
};
categoryCombo.Items.AddRange(categories);  // This is the correct way to add the items

            // Amount Input
            Label amountLabel = new Label()
            {
                Text = "Amount:",
                Left = 20,
                Top = 53,
                AutoSize = true
            };

            amountInput = new TextBox()
            {
                Left = 120,
                Top = 50,
                Width = 150
            };

            // Date Picker
            Label dateLabel = new Label()
            {
                Text = "Date:",
                Left = 20,
                Top = 83,
                AutoSize = true
            };

            datePicker = new DateTimePicker()
            {
                Left = 120,
                Top = 80,
                Width = 150,
                Format = DateTimePickerFormat.Short
            };

            // Description Input
            Label descLabel = new Label()
            {
                Text = "Description:",
                Left = 20,
                Top = 113,
                AutoSize = true
            };

            descInput = new TextBox()
            {
                Left = 120,
                Top = 110,
                Width = 150
            };

            // Add Button
            Button addButton = new Button()
            {
                Text = "Add Expense",
                Left = 300,
                Top = 80,
                Width = 100,
                Height = 30,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            addButton.Click += AddExpense;

            this.Controls.AddRange(new Control[] {
                categoryLabel, categoryCombo,
                amountLabel, amountInput,
                dateLabel, datePicker,
                descLabel, descInput,
                addButton
            });
        }

        private void CreateExpenseList()
        {
            expenseList = new ListView()
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Left = 20,
                Top = 150,
                Width = 740,
                Height = 300
            };

            expenseList.Columns.Add("Date", 100);
            expenseList.Columns.Add("Category", 100);
            expenseList.Columns.Add("Amount", 100);
            expenseList.Columns.Add("Description", 420);

            this.Controls.Add(expenseList);

            // Add context menu for delete
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Delete");
            deleteItem.Click += DeleteSelectedExpense;
            contextMenu.Items.Add(deleteItem);
            expenseList.ContextMenuStrip = contextMenu;
        }

        private void CreateFilterControls()
        {
            Label filterLabel = new Label()
            {
                Text = "Filter by Category:",
                Left = 20,
                Top = 477,
                AutoSize = true
            };

            filterCombo = new ComboBox()
            {
                Items = { "All" },
                Left = 120,
                Top = 473,
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            filterCombo.Items.AddRange(categories);
            filterCombo.SelectedIndex = 0;
            filterCombo.SelectedIndexChanged += (s, e) => UpdateExpenseList(filterCombo.SelectedItem.ToString());

            this.Controls.AddRange(new Control[] { filterLabel, filterCombo });
        }

        private void CreateSummaryControls()
        {
            totalLabel = new Label()
            {
                Text = "Total: R0.00",
                Left = 600,
                Top = 477,
                Font = new Font("Arial", 12, FontStyle.Bold),
                AutoSize = true
            };

            this.Controls.Add(totalLabel);
        }

        private void SetupEventHandlers()
        {
            // Input validation for amount
            amountInput.KeyPress += (s, e) => {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && amountInput.Text.Contains('.'))
                {
                    e.Handled = true;
                }
            };
        }

        private void AddExpense(object sender, EventArgs e)
        {
            if (categoryCombo.SelectedItem == null || string.IsNullOrWhiteSpace(amountInput.Text))
            {
                MessageBox.Show("Please fill in all required fields!", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(amountInput.Text, out decimal amount))
            {
                MessageBox.Show("Please enter a valid amount!", "Validation Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Expense expense = new Expense
            {
                Category = categoryCombo.SelectedItem.ToString(),
                Amount = amount,
                Date = datePicker.Value,
                Description = descInput.Text
            };

            expenses.Add(expense);
            UpdateExpenseList(filterCombo.SelectedItem.ToString());
            ClearInputs();
        }

        private void DeleteSelectedExpense(object sender, EventArgs e)
        {
            if (expenseList.SelectedItems.Count > 0)
            {
                if (MessageBox.Show("Are you sure you want to delete this expense?", "Confirm Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    int index = expenseList.SelectedIndices[0];
                    expenses.RemoveAt(index);
                    UpdateExpenseList(filterCombo.SelectedItem.ToString());
                }
            }
        }

        private void ClearInputs()
        {
            categoryCombo.SelectedIndex = -1;
            amountInput.Clear();
            descInput.Clear();
            datePicker.Value = DateTime.Now;
        }

        private void UpdateExpenseList(string filter = "All")
        {
            expenseList.Items.Clear();
            var filteredExpenses = filter == "All" 
                ? expenses 
                : expenses.Where(e => e.Category == filter);

            foreach (var expense in filteredExpenses)
            {
                ListViewItem item = new ListViewItem(new[] {
                    expense.Date.ToShortDateString(),
                    expense.Category,
                    expense.Amount.ToString("C"),
                    expense.Description
                });
                expenseList.Items.Add(item);
            }

            decimal total = filteredExpenses.Sum(e => e.Amount);
            totalLabel.Text = $"Total: {total:C}";
        }
    }
}