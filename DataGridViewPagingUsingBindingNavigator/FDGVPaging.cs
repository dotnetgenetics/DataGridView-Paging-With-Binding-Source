using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;

namespace DataGridViewPagingUsingBindingNavigator
{
    public partial class FDGVPaging : Form
    {
        public static int totalRecords { get; set; }
        public const int pageSize = 10;
        private List<string> sourceData = new List<string>();
        private DataTable dtSource = new DataTable();

        public FDGVPaging()
        {
            InitializeComponent();
            bindingNav.BindingSource = bindingProducts;
            bindingProducts.CurrentChanged += BindingProducts_CurrentChanged; ;
            SetSource();
            bindingProducts.DataSource = new PageOffsetList();
        }

        private void BindingProducts_CurrentChanged(object sender, EventArgs e)
        {
            // The desired page has changed, so fetch the page of records using the "Current" offset 
            int offset = (int)bindingProducts.Current;
            var records = new List<Record>();
            for (int i = offset; i < offset + pageSize && i < totalRecords; i++)
            {
                try
                {
                    records.Add(new Record() { ProductName = sourceData[i].ToString() });
                }
                catch (Exception ex)
                {

                }
            }

            dgvProductNames.DataSource = records;
        }

        private void SetSource()
        {
            string sql = string.Format("select [Name] as ProductName from [Production].[Product] order by [Name] asc;");
            SqlConnection conn = null;
            try
            {
                string connection = ConfigurationManager.AppSettings["AdventureWorks"];
                conn = new SqlConnection(connection);
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dtSource);

                if (dtSource.Rows.Count > 0)
                {
                    foreach (DataRow item in dtSource.Rows)
                    {
                        if (!string.IsNullOrEmpty(item[0].ToString()))
                        {
                            sourceData.Add(item[0].ToString());
                        }
                    }
                }

                totalRecords = sourceData.Count;

                conn.Close();
            }
            catch (Exception e)
            {
                conn.Close();
                conn.Dispose();
            }
        }

        private void FDGVPaging_Load(object sender, EventArgs e)
        {
            dgvProductNames.AutoGenerateColumns = false;
        }
    }
}
