using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;

namespace MW100_T5_Reese
{
    public partial class PrintGeneralReportForm : Form
    {
        Form1 callingForm1;

        DateTime today = DateTime.Today;

        public PrintGeneralReportForm()
        {
            InitializeComponent();
        }

        public PrintGeneralReportForm(Form1 f) //pass reference to itself
        {
            callingForm1 = f;  //to talk to Form1
            InitializeComponent();
        }

        private void PrintGeneralReportForm_Load(object sender, EventArgs e)
        {
            lblReportDateGeneratedOutput.Text = Convert.ToString(today);
            lblReportTotMemOutput.Text = callingForm1.TransferTotalMembers;
            lblReportMaleOutput.Text = callingForm1.TransferMale;
            lblReportFemaleOutput.Text = callingForm1.TransferFemale;
            lblReportMonthNewMembershipsOutput.Text = callingForm1.TransferMonthMemberships;
            lblReportMonthTotAttendanceOutput.Text = callingForm1.TransferMonthAttendance;
            lblReportTotContributionsOutput.Text = callingForm1.TransferTotContr;
            lblReportBuildingOutput.Text = callingForm1.TransferBuildingF;
            lblReportChurchPlantOutput.Text = callingForm1.TransferPlantF;
            lblReportFoodBankOutput.Text = callingForm1.TransferFoodF;
            lblReportGeneralOutput.Text = callingForm1.TransferGenF;
            lblReportMissionsOutput.Text = callingForm1.TransferMissionsF;
            lblReportYouthOutput.Text = callingForm1.TransferYouthF;
            lblReportTithesOutput.Text = callingForm1.TransferTithesF;
            lblReportAvgContributionsOutput.Text = callingForm1.TransferAvgContr;
            lblReportTotEmployeesOutput.Text = callingForm1.TransferTotEmployees;
        }

        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;
            Bitmap bmp = new Bitmap(this.groupBox1.Width, this.groupBox1.Height);
            this.groupBox1.DrawToBitmap(bmp, new Rectangle(0, 0, this.groupBox1.Width, this.groupBox1.Height));
            e.Graphics.DrawImage((Image)bmp, x, y);
        }

        private void btnPrint_Click_2(object sender, EventArgs e)
        {
            PrintDocument doc = new PrintDocument();
            doc.PrintPage += this.Doc_PrintPage;
            PrintDialog dlgSettings = new PrintDialog();
            dlgSettings.Document = doc;
            if (dlgSettings.ShowDialog() == DialogResult.OK)
            {
                doc.Print();
            }
        }
    }
}