using ServiceStack.Redis;
using ServiceStack.Redis.Generic;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DEMO_REDIS_01
{
    public partial class Form1 : MetroFramework.Forms.MetroForm
    {
        public Form1()
        {
            InitializeComponent();
        }

        bool IsEdit = false;

        void Edit(bool values, bool Edit = false)
        {
            if (values == false && Edit == true)
            {
                txtID.ReadOnly = true;
                IsEdit = true;
            }
            else
            {
                txtID.ReadOnly = values;
            }
            txtManufacturer.ReadOnly = values;
            txtModel.ReadOnly = values;
            btnCancel.Enabled = !values;
            btnSave.Enabled = !values;
        }

        void DeactiveAnotherButton(int index = -1)
        {
            /* 
             0 == btnAdd 
             1 == btnEdit
             2 == btnDelete
             */
            if (index == -1)
            {
                btnAdd.Enabled = true;
                btnEdit.Enabled = true;
                btnDelete.Enabled = true;
                return;
            }
            btnAdd.Enabled = false;
            btnEdit.Enabled = false;
            btnDelete.Enabled = false;
            switch (index)
            {
                case 0:
                    btnAdd.Enabled = true;
                    break;
                case 1:
                    btnEdit.Enabled = true;
                    break;
                case 2:
                    btnDelete.Enabled = true;
                    break;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdateDB();
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            ClearText();
            DeactiveAnotherButton(0);
            phoneBindingSource.Add(new Phone());
            phoneBindingSource.MoveLast();
            Edit(false);
            txtID.Focus();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            DeactiveAnotherButton(1);
            Edit(false, true);
            txtID.Focus();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Edit(true);
            phoneBindingSource.ResetBindings(false);
            UpdateDB();
            DeactiveAnotherButton();
            metroLabel1.Focus();
        }

        void ClearText()
        {
            txtID.Text = string.Empty;
            txtManufacturer.Text = string.Empty;
            txtModel.Text = string.Empty;
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            DeactiveAnotherButton(2);
            if (MetroFramework.MetroMessageBox.Show(this, "Are you sure want to delete this record ?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Phone p = phoneBindingSource.Current as Phone;
                using (RedisClient client = new RedisClient("localhost", 6379))
                {
                    IRedisTypedClient<Phone> phone = client.As<Phone>();
                    phone.DeleteById(p.ID);
                    phoneBindingSource.RemoveCurrent();
                    ClearText();
                    UpdateDB();
                }
                DeactiveAnotherButton();
            }
            else
            {
                DeactiveAnotherButton();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            using (RedisClient client = new RedisClient("localhost", 6379))
            {
                phoneBindingSource.EndEdit();
                IRedisTypedClient<Phone> phone = client.As<Phone>();
                List<Phone> ListWillStore = new List<Phone>();
                foreach (var temp in phoneBindingSource.DataSource as List<Phone>)
                {
                    if (!temp.IsNull())
                    {
                        ListWillStore.Add(temp);
                    }
                }
                phone.StoreAll(ListWillStore);
                client.Save();
                MetroFramework.MetroMessageBox.Show(this, "Your data has been successfully saved.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearText();
                DeactiveAnotherButton();

            }
            UpdateDB();
        }

        private void UpdateDB()
        {
            using (RedisClient client = new RedisClient("localhost", 6379))
            {
                IRedisTypedClient<Phone> phone = client.As<Phone>();
                phoneBindingSource.DataSource = phone.GetAll();
                Edit(true);
            }
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string Keyword = string.Empty;
            Keyword = metroTextBox1.Text;
            List<int> vs = new List<int>();
            using (RedisClient client = new RedisClient("localhost", 6379))
            {
                IRedisTypedClient<Phone> phone = client.As<Phone>();
                IList<Phone> phones = phone.GetAll();
                for (int i = 0; i < phones.Count; i++)
                {
                    if (!phones[i].Model.Contains(Keyword) && !phones[i].Manufacturer.Contains(Keyword))
                    {
                        phones.RemoveAt(i);
                        i--;
                    }
                }
                phoneBindingSource.DataSource = phones;
            }
        }
    }
}
