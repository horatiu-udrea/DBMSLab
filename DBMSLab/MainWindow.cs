using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Xml.Schema;
using DBMSLab.database;

namespace DBMSLab
{
    public partial class MainWindow : Form
    {
        private DataSet dataSet;
        private ElectronicsDataAdapter dataAdapter;

        public MainWindow()
        {
            InitializeComponent();
            labelParent.Text = ConfigurationManager.AppSettings["parentTable"];
            labelChild.Text = ConfigurationManager.AppSettings["childTable"];
            dataSet = new DataSet();
            dataAdapter = new ElectronicsDataAdapter();
            refreshData();
            ElectronicsDataSet.addDataSetRelation(dataSet, ConfigurationManager.AppSettings["pk_field"],
                ConfigurationManager.AppSettings["fk_field"]);
            var parentBindingSource = new BindingSource(dataSet, ConfigurationManager.AppSettings["parentTable"]);
            var childBindingSource = new BindingSource(parentBindingSource, "ParentChild");
            parentGridView.DataSource = parentBindingSource;
            childGridView.DataSource = childBindingSource;
            changeHeaders();
        }

        private void changeHeaders()
        {
            var headers = ConfigurationManager.AppSettings["parentHeaders"].Split(',');
            for (int i = 0; i < headers.Length; i++)
            {
                parentGridView.Columns[i].HeaderText = headers[i];
            }

            headers = ConfigurationManager.AppSettings["childHeaders"].Split(',');
            for (int i = 0; i < headers.Length; i++)
            {
                childGridView.Columns[i].HeaderText = headers[i];
            }
        }

        private void MainWindow_Load(object sender, System.EventArgs e)
        {
            refreshData();
        }

        private void refreshData()
        {
            dataSet.Clear();
            try
            {
                dataAdapter.ParentAdapter.Fill(dataSet, ConfigurationManager.AppSettings["parentTable"]);
                dataAdapter.ChildAdapter.Fill(dataSet, ConfigurationManager.AppSettings["childTable"]);
            }
            catch (SqlException)
            {
                MessageBox.Show("There was an error communicating with the database", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                dataAdapter.ChildAdapter.Update(dataSet, ConfigurationManager.AppSettings["childTable"]);
                changesLabel.Visible = true;
                refreshData();
            }
            catch (SqlException)
            {
                MessageBox.Show("There was an error communicating with the database", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}