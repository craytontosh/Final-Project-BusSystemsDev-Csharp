using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MW100_T5_Reese
{
    public partial class Form1 : Form
    {
        #region FIELDS
        //all fields

        //create lists
        List<Contribution> contributionList = new List<Contribution>();
        List<Contribution> loadedContributions = new List<Contribution>();
        List<Member> memberList = new List<Member>();
        List<Member> loadedMembers = new List<Member>();

        int currMemIndex = 1;
        int currContrIndex = 1;

        string selectedFileName;

        //VALUES TO CALCULATE OUR QUICK STATS
        double totalContributions = 0,
               averageContributions = 0,
               janContributions = 0,
               febContributions = 0,
               marContributions = 0,
               aprContributions = 0,
            //calculates the amount going to a fund
               buildingFund = 0,
               churchPlanting = 0,
               foodBankFund = 0,
               generalFund = 0,
               missionsFund = 0,
               youthFund = 0,
               tithesFund = 0,
            //counts the methods of payment
               countCash = 0,
               countCheck = 0,
               countOnline = 0,
               countOther = 0;

        int age = 0;
        DateTime today = DateTime.Today;
        DateTime month = DateTime.Today;

        #endregion

        #region FORM1-METHODS

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //pre-load form

            //clear all txtboxes and lbls
            txtFName.Text = "";
            txtLName.Text = "";
            dtpBirthdate.Text = "";
            cboGender.Text = "";
            cboMaritalStatus.Text = "";
            txtMemID.Text = "";
            cboMemType.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            cboState.Text = "";
            txtPhNum.Text = "";
            txtEmail.Text = "";

            //Clear contribution history date time picker
            ContrClearDTP();

            //call methods
            LoadContributionData();
            LoadMemberData();
            DisplayMemProfs(currMemIndex);
            DisplayContributions(currContrIndex);
            LoadChartData();

            //Format the DataGridViews
            dgvMembers.BackgroundColor = Color.White;
            dgvMembers.BorderStyle = BorderStyle.None;
            dgvMembers.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvMembers.ScrollBars = System.Windows.Forms.ScrollBars.Both;

            dgvContributionHistory.BackgroundColor = Color.White;
            dgvContributionHistory.BorderStyle = BorderStyle.None;
            dgvContributionHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvContributionHistory.ScrollBars = System.Windows.Forms.ScrollBars.Both;
        }

        private void ContrClearDTP()
        {
            dtpContrHistDate.Format = DateTimePickerFormat.Custom;
            dtpContrHistDate.CustomFormat = " ";
        }

        #endregion

        #region PROCESSING-CSV-METHODS

        //pre-load contribution data from csv files
        private void LoadContributionData()
        {
            //open and read the file
            FileStream inFile = new FileStream("ContributionsFile.csv", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);

            //array to hold the values in the file
            string[] contributionInput = new string[8];
            const char DELIM = ',';
            //read from the file
            string inputStr = reader.ReadLine();
            //count variable
            int count = 0;
            while (inputStr != null)
            {
                //create contribution object
                Contribution contributionObject = new Contribution();

                contributionInput = inputStr.Split(DELIM);

                contributionObject.ContributionNo = Convert.ToInt32(contributionInput[0]);
                contributionObject.MemberID = contributionInput[1];
                contributionObject.ContributionDate = Convert.ToDateTime(contributionInput[2]);
                contributionObject.Amount = Convert.ToDouble(contributionInput[3]);
                contributionObject.PaymentMethod = contributionInput[4];
                contributionObject.CheckNo = Convert.ToInt32(contributionInput[5]);
                contributionObject.DesignatedFund = contributionInput[6];
                contributionObject.Notes = contributionInput[7];

                //calculates the total and average contributions
                totalContributions += contributionObject.Amount;
                ++count;

                //nested if statements to calculate the monthly contributions
                if (contributionObject.ContributionDate.Year == today.Year)
                {
                    if (contributionObject.ContributionDate.Month == 1)
                    {
                        janContributions += contributionObject.Amount;
                    }
                    else if (contributionObject.ContributionDate.Month == 2)
                    {
                        febContributions += contributionObject.Amount;
                    }
                    else if (contributionObject.ContributionDate.Month == 3)
                    {
                        marContributions += contributionObject.Amount;
                    }
                    else if (contributionObject.ContributionDate.Month == 4)
                    {
                        aprContributions += contributionObject.Amount;
                    }
                }

                //if statements that will calculate what funds are being distributed where
                if (contributionObject.DesignatedFund == "Building Fund")
                {
                    buildingFund += contributionObject.Amount;
                }
                else if (contributionObject.DesignatedFund == "Church Planting")
                {
                    churchPlanting += contributionObject.Amount;
                }
                else if (contributionObject.DesignatedFund == "Food Bank")
                {
                    foodBankFund += contributionObject.Amount;
                }
                else if (contributionObject.DesignatedFund == "General Fund")
                {
                    generalFund += contributionObject.Amount;
                }
                else if (contributionObject.DesignatedFund == "Missions")
                {
                    missionsFund += contributionObject.Amount;
                }
                else if (contributionObject.DesignatedFund == "Restricted-Youth")
                {
                    youthFund += contributionObject.Amount;
                }
                else if (contributionObject.DesignatedFund == "Tithes")
                {
                    tithesFund += contributionObject.Amount;
                }

                //hidden labels to display for each fund
                lblHiddenBuildingFundOutput.Text = buildingFund.ToString("C2");
                lblHiddenPlantingFundOutput.Text = churchPlanting.ToString("C2");
                lblHiddenFoodBankFundOutput.Text = foodBankFund.ToString("C2");
                lblHiddenGenFundOutput.Text = generalFund.ToString("C2");
                lblHiddenMissionsFundOutput.Text = missionsFund.ToString("C2");
                lblHiddenYouthFundOutput.Text = youthFund.ToString("C2");
                lblHiddenTithesFundOutput.Text = tithesFund.ToString("C2");

                //if statements that will calculate payment distribution
                if (contributionObject.PaymentMethod == "Check")
                {
                    ++countCheck;
                }
                else if (contributionObject.PaymentMethod == "Cash")
                {
                    ++countCash;
                }
                else if (contributionObject.PaymentMethod == "Online")
                {
                    ++countOnline;
                }
                else if (contributionObject.PaymentMethod == "Other")
                {
                    ++countOther;
                }

                //adds object to the list and reads the next line
                contributionList.Add(contributionObject);
                inputStr = reader.ReadLine();
            }
            reader.Close();
            inFile.Close();

            //calculates the total and average contributions
            averageContributions = totalContributions / count;
            string avgContributionsOutput = averageContributions.ToString("C2");
            string totalContributionsOutput = totalContributions.ToString("C2");
            lblTotContributionsOutput.Text = totalContributionsOutput;
            lblAvgContribution.Text = avgContributionsOutput;


            //calls the method that loads the data to the grid
            DisplayContributionDataGrid(contributionList);
        }

        //display contribution data from csv file to Contribution History tab
        private void DisplayContributionDataGrid(List<Contribution> contributionList)
        {
            //Define the DATASOURCE thru a BINDINGSOURCE 
            BindingSource bindSrc = new BindingSource();
            bindSrc.DataSource = contributionList; //the passed list is the source
            bindingNavigator1.BindingSource = bindSrc;  //attach the bindingsource to the NAV BAR
            dgvContributionHistory.DataSource = null;
            dgvContributionHistory.DataSource = bindSrc; //attach the bindingsource to the DATA GRID

            ////AUTOFIT the DATA GRID
            dgvContributionHistory.AutoResizeColumns();  //This stmt is required!
            dgvContributionHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; //Autofit
            dgvContributionHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; //Re-allow user to chg it

            //Highlight 1st few columns
            dgvContributionHistory.Columns["ContributionNo"].DefaultCellStyle.BackColor = Color.LightSteelBlue;
            dgvContributionHistory.Columns["MemberID"].DefaultCellStyle.BackColor = Color.LightSteelBlue;

            //Freeze 1st few columns 
            dgvContributionHistory.Columns["ContributionNo"].Frozen = true;
            dgvContributionHistory.Columns["MemberID"].Frozen = true;

            //Display total # rows in the List<>
            lblContrRecordsDisplayed.Text = contributionList.Count.ToString();
        }

        //pre-load member data from csv file
        private void LoadMemberData()
        {
            FileStream inFile = new FileStream("MembersFile1.csv", FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(inFile);

            //array to hold the values in the file
            string[] memberInput = new string[17];
            const char DELIM = ',';
            //read from the file
            string inputStr = reader.ReadLine();

            //count variables 
            int count = 0,
                maleCount = 0,
                femaleCount = 0,
                monthlyAttendanceCount = 0,
                monthlyAdmissionsCount = 0,
                staffMembersCount = 0;

            while (inputStr != null)
            {
                //create member object
                Member memberObject = new Member();

                memberInput = inputStr.Split(DELIM);
                //read and puts the data in its respective attribute
                memberObject.MemberID = Convert.ToInt32(memberInput[0]);
                memberObject.LastName = memberInput[1];
                memberObject.FirstName = memberInput[2];
                memberObject.Honorific = memberInput[3];
                memberObject.Gender = memberInput[4];
                memberObject.Birthdate = Convert.ToDateTime(memberInput[5]);
                memberObject.Address = memberInput[6];
                memberObject.City = memberInput[7];
                memberObject.State = memberInput[8];
                memberObject.Zip = Convert.ToInt32(memberInput[9]);
                memberObject.Phone = memberInput[10];
                memberObject.Email = memberInput[11];
                memberObject.MemberType = memberInput[12];
                memberObject.MembershipDate = Convert.ToDateTime(memberInput[13]);
                memberObject.AttendanceBeginDate = Convert.ToDateTime(memberInput[14]);
                memberObject.AttendanceLastDate = Convert.ToDateTime(memberInput[15]);
                memberObject.MaritalStatus = memberInput[16];
                //adds it to the list
                memberList.Add(memberObject);
                //reads the next line
                inputStr = reader.ReadLine();

                //calculates the number of men and women
                if (memberObject.Gender == "M")
                {
                    ++maleCount;
                }
                else if (memberObject.Gender == "F")
                {
                    ++femaleCount;
                }

                //hidden labels to display on general print report
                lblTotMaleOutputHidden.Text = Convert.ToString(maleCount);
                lblTotFemaleOuputHidden.Text = Convert.ToString(femaleCount);

                //calculates the numbers of members this month of april
                if (memberObject.AttendanceLastDate.Month == month.Month)
                {
                    ++monthlyAttendanceCount;
                }

                //calculates the number of new members this month
                if (memberObject.MembershipDate.Month == month.Month)
                {
                    ++monthlyAdmissionsCount;
                }

                //STAFF MEMBERS 
                if (memberObject.MemberType == "Staff")
                {
                    ++staffMembersCount;
                }

                //calculating total age
                age += (today.Year - memberObject.Birthdate.Year);

                ++count;
            }
            reader.Close();
            inFile.Close();

            //calculating average age
            string ageOutput = (age / count).ToString();
            lblAverageMemAgeOutput.Text = ageOutput;

            //monthly attendance count
            string monthlyAttendanceCountOutput = monthlyAttendanceCount.ToString();
            lblMonthlyAttendance.Text = monthlyAttendanceCountOutput;

            //monthly admissions count
            string monthlyAdmissionsCountOutput = monthlyAdmissionsCount.ToString();
            lblThisMonthMembershipsOutput.Text = monthlyAdmissionsCountOutput;

            //number of staff members
            string staffMembersCountOutput = staffMembersCount.ToString();
            lblTotEmployeesOutput.Text = staffMembersCountOutput;

            //membership count 
            string membersCount = count.ToString();
            lblTotalMembersOutput.Text = membersCount;

            //male to female ratio
            maleCount = maleCount / 10;
            femaleCount = femaleCount / 10;
            string maleCountOutput = maleCount.ToString(),
                   femaleCountOutput = femaleCount.ToString();
            lblMaleOutput.Text = maleCountOutput;
            lblFemaleOutput.Text = femaleCountOutput;

            DisplayMemberDataGrid(memberList);
        }

        //display member data from csv file to Membership tab
        private void DisplayMemberDataGrid(List<Member> anyMemberList)
        {
            //Define the DATASOURCE thru a BINDINGSOURCE 
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = anyMemberList; //the passed list is the source
            bindingNavigator2.BindingSource = bindingSource;  //attach the bindingsource to the NAV BAR
            dgvMembers.DataSource = null;
            dgvMembers.DataSource = bindingSource; //attach the bindingsource to the DATA GRID

            //AUTOFIT the DATA GRID
            dgvMembers.AutoResizeColumns();  //This stmt is required!
            dgvMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells; //Autofit
            dgvMembers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None; //Re-allow user to chg it

            //Highlight 1st few columns
            dgvMembers.Columns["MemberID"].DefaultCellStyle.BackColor = Color.LightSteelBlue;
            dgvMembers.Columns["LastName"].DefaultCellStyle.BackColor = Color.LightSteelBlue;
            dgvMembers.Columns["FirstName"].DefaultCellStyle.BackColor = Color.LightSteelBlue;

            //Freeze 1st few columns 
            dgvMembers.Columns["MemberID"].Frozen = true;
            dgvMembers.Columns["LastName"].Frozen = true;
            dgvMembers.Columns["FirstName"].Frozen = true;

            //Align numbers to the middle right/center
            dgvMembers.Columns["MemberID"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;
            dgvMembers.Columns["Birthdate"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dgvMembers.Columns["Zip"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dgvMembers.Columns["Phone"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dgvMembers.Columns["MembershipDate"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dgvMembers.Columns["AttendanceBeginDate"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;
            dgvMembers.Columns["AttendanceLastDate"].DefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleRight;

            //Display total # rows in the List<>
            lblMembershipsRecordsDisplayed.Text = anyMemberList.Count.ToString();

            //Save the passed list to loadedMembers
            loadedMembers = anyMemberList; //what do i do with anyMemberList??

            //Resest member index position
            currMemIndex = 1;
        }

        //saves data to file
        private void WriteContributionsToFile(string anyFileName)
        {
            //Declare FILESTREAM & STREAMWRITER
            FileStream outputDestFile = new FileStream("ContributionsFile.csv",
                FileMode.Create, FileAccess.Write);

            StreamWriter writer = new StreamWriter(outputDestFile);

            //FOREACH: loop to write the entire list to Writer
            foreach (Contribution c in contributionList)
            {
                writer.WriteLine(c.ContributionNo + "," +
                    c.MemberID + "," +
                    c.ContributionDate + "," +
                    c.Amount + "," +
                    c.PaymentMethod + "," +
                    c.CheckNo + "," +
                    c.DesignatedFund + "," +
                    c.Notes);
            }

            //Close STREAMWRITER & FILESTREAM
            writer.Close();
            outputDestFile.Close();
        }

        #endregion

        #region MEM-PROF
        //all Member tab code

        //display member data in Member tab
        private void DisplayMemProfs(int pos)
        {
            txtMemID.Text = memberList[pos].MemberID.ToString();
            txtLName.Text = memberList[pos].LastName;
            txtFName.Text = memberList[pos].FirstName;
            cboHonorific.Text = memberList[pos].Honorific;
            cboGender.Text = memberList[pos].Gender;
            dtpBirthdate.Text = memberList[pos].Birthdate.ToShortDateString();
            txtAddress.Text = memberList[pos].Address;
            txtCity.Text = memberList[pos].City;
            cboState.Text = memberList[pos].State;
            txtZip.Text = memberList[pos].Zip.ToString();
            txtPhNum.Text = memberList[pos].Phone.ToString();
            txtEmail.Text = memberList[pos].Email;
            cboMemType.Text = memberList[pos].MemberType;
            dtpMembershipDate.Text = memberList[pos].MembershipDate.ToShortDateString();
            dtpMemAttendanceBegin.Text = memberList[pos].AttendanceBeginDate.ToShortDateString();
            dtpMemAttendanceLast.Text = memberList[pos].AttendanceLastDate.ToShortDateString();
            cboMaritalStatus.Text = memberList[pos].MaritalStatus;

            lblOutputTotalNumMemProf.Text = memberList.Count.ToString();
            lblOutputMemProfNum.Text = Convert.ToString(currMemIndex);
        }


        //save btn
        private void btnSaveMemProf_Click(object sender, EventArgs e)
        {
            Member memberObject = new Member();
            memberObject.MemberID = Convert.ToInt32(txtMemID.Text);
            memberObject.LastName = txtLName.Text;
            memberObject.FirstName = txtFName.Text;
            memberObject.Honorific = cboHonorific.Text;
            memberObject.Gender = cboGender.Text;
            memberObject.Birthdate = Convert.ToDateTime(dtpBirthdate.Text);
            memberObject.Address = txtAddress.Text;
            memberObject.City = txtCity.Text;
            memberObject.State = cboState.Text;
            memberObject.Zip = Convert.ToInt32(txtZip.Text);
            memberObject.Phone = txtPhNum.Text;
            memberObject.Email = txtEmail.Text;
            memberObject.MemberType = cboMemType.Text;
            memberObject.MembershipDate = Convert.ToDateTime(dtpMembershipDate.Text);
            memberObject.AttendanceBeginDate = Convert.ToDateTime(dtpMemAttendanceBegin.Text);
            memberObject.AttendanceLastDate = Convert.ToDateTime(dtpMemAttendanceLast.Text);
            memberObject.MaritalStatus = cboMaritalStatus.Text;
            currMemIndex++;

            //NEED TO COMPLETE THIS CODE
            ////write new member to csv file
            //DisplayPlayerInReport(memberObject);
            memberList.Add(memberObject);
        }

        //update btn
        private void btnUpdateMemProf_Click(object sender, EventArgs e)
        {
            //NEED TO COMPLETE THIS CODE so that the updated info replaces the data in csv
            Member memberObject = new Member();
            memberObject.MemberID = Convert.ToInt32(txtMemID.Text);
            memberObject.LastName = txtLName.Text;
            memberObject.FirstName = txtFName.Text;
            memberObject.Honorific = cboHonorific.Text;
            memberObject.Gender = cboGender.Text;
            memberObject.Birthdate = Convert.ToDateTime(dtpBirthdate.Text);
            memberObject.Address = txtAddress.Text;
            memberObject.City = txtCity.Text;
            memberObject.State = cboState.Text;
            memberObject.Zip = Convert.ToInt32(txtZip.Text);
            memberObject.Phone = txtPhNum.Text;
            memberObject.Email = txtEmail.Text;
            memberObject.MemberType = cboMemType.Text;
            memberObject.MembershipDate = Convert.ToDateTime(dtpMembershipDate.Text);
            memberObject.AttendanceBeginDate = Convert.ToDateTime(dtpMemAttendanceBegin.Text);
            memberObject.AttendanceLastDate = Convert.ToDateTime(dtpMemAttendanceLast.Text);
            memberObject.MaritalStatus = cboMaritalStatus.Text;
        }

        //clear btn
        private void btnClearMemProf_Click(object sender, EventArgs e)
        {
            ClearMemProf();
        }

        private void ClearMemProf()
        {
            txtMemID.Text = "";
            txtLName.Text = "";
            txtFName.Text = "";
            cboHonorific.Text = "";
            cboGender.Text = "";
            dtpBirthdate.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            cboState.Text = "";
            txtZip.Text = "";
            txtPhNum.Text = "";
            txtEmail.Text = "";
            cboMemType.Text = "";
            dtpMembershipDate.Text = "";
            dtpMemAttendanceBegin.Text = "";
            dtpMemAttendanceLast.Text = "";
            cboMaritalStatus.Text = "";
        }

        //delete btn
        private void btnDeleteMemProf_Click(object sender, EventArgs e)
        {
            //write delete btn code so that the displayed mem profile is deleted from csv
            int pos;

            //Find the position in the List of the contribution to delete
            pos = currMemIndex;

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this record?  " + "\n\n" +
                        "Member ID:  " + currMemIndex + "\n\n",
                        "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                //IF the member was found, remove it
                if (pos > -1 && txtMemID.Text != "")
                {
                    memberList.RemoveAt(pos);
                }

                ClearMemProf();
            }
        }

        //search btn
        private void btnSearchMemProfile_Click(object sender, EventArgs e)
        {
            //NEED TO COMPLETE THIS CODE
            if (txtContrSearchMemID.Text == null)
            {
                MessageBox.Show("Please enter Member ID to complete search.", "Invalid Member Information!");
            }
            else
            {
                //call search method

            }
        }

        //clear search btn
        private void btnMemClearSearch_Click(object sender, EventArgs e)
        {
            txtMemSearchLName.Text = "";
            txtMemSearchFName.Text = "";
            txtMemSearchMemID.Text = "";
        }

        //go to first member record
        private void tsbFirstMember_Click(object sender, EventArgs e)
        {
            if (memberList.Count > 0)
            {
                //NEED TO COMPLETE THIS CODE
                currMemIndex = 0;
                DisplayMemProfs(currMemIndex);
            }
        }

        //go to previous member record
        private void tsbPreviousMember_Click(object sender, EventArgs e)
        {
            if (currMemIndex > 0)
            {
                //NEED TO COMPLETE THIS CODE
                currMemIndex--;
                DisplayMemProfs(currMemIndex);
            }
        }

        //go to next member record
        private void tsbNextMember_Click(object sender, EventArgs e)
        {
            if (currMemIndex < memberList.Count - 1)
            {
                //NEED TO COMPLETE THIS CODE
                currMemIndex++;
                DisplayMemProfs(currMemIndex);
            }
        }

        //go to last member record
        private void tsbLastMember_Click(object sender, EventArgs e)
        {
            if (memberList.Count > 0)
            {
                //NEED TO COMPLETE THIS CODE
                currMemIndex = memberList.Count - 1;
                DisplayMemProfs(currMemIndex);
            }
        }

        #endregion

        #region CONTR
        //all Contribution tab code

        //changed
        //display contribution data in Contribution tab
        private void DisplayContributions(int pos)
        {
            txtContrNo.Text = contributionList[pos].ContributionNo.ToString();
            txtContrMemID.Text = contributionList[pos].MemberID;
            txtContrAmt.Text = contributionList[pos].Amount.ToString("C2");
            cboContrPaymentMethod.Text = contributionList[pos].PaymentMethod.ToString();
            txtContrCheckNo.Text = contributionList[pos].CheckNo.ToString();
            cboContrDesignatedFund.Text = contributionList[pos].DesignatedFund.ToString();
            dtpContrDate.Text = contributionList[pos].ContributionDate.ToString();

            lblOutputContrTotalNum.Text = contributionList.Count.ToString();
            lblOutputContrNum.Text = Convert.ToString(currContrIndex);
        }

        //changed
        private void UpdateContributions(int pos)
        {
            Contribution contributionObject = new Contribution();
            contributionObject.ContributionNo = Convert.ToInt32(txtContrNo.Text);
            contributionObject.MemberID = txtContrMemID.Text;
            contributionObject.ContributionDate = Convert.ToDateTime(dtpContrDate.Text);
            contributionObject.Amount = Convert.ToDouble(txtContrAmt.Text);
            contributionObject.PaymentMethod = cboContrPaymentMethod.Text;
            contributionObject.CheckNo = Convert.ToInt32(txtContrCheckNo.Text);
            contributionObject.DesignatedFund = cboContrDesignatedFund.Text;
            contributionObject.Notes = txtContrNotes.Text;

            contributionList.Add(contributionObject);
            WriteContributionsToFile(selectedFileName);
            DeleteUpdatedContribution();
        }

        //changed
        private void DeleteUpdatedContribution()
        {
            int pos;
            pos = currContrIndex;

            if (pos > -1 && txtContrNo.Text != "")
            {
                contributionList.RemoveAt(pos);
            }
            WriteContributionsToFile(selectedFileName);
        }

        //changed
        //add btn
        private void btnSaveContr_Click(object sender, EventArgs e)
        {
            Contribution contributionObject = new Contribution();
            contributionObject.ContributionNo = Convert.ToInt32(txtContrNo.Text);
            contributionObject.MemberID = txtContrMemID.Text;
            contributionObject.ContributionDate = Convert.ToDateTime(dtpContrDate.Text);
            contributionObject.Amount = Convert.ToDouble(txtContrAmt.Text);
            contributionObject.PaymentMethod = cboContrPaymentMethod.Text;
            contributionObject.CheckNo = Convert.ToInt32(txtContrCheckNo.Text);
            contributionObject.DesignatedFund = cboContrDesignatedFund.Text;
            contributionObject.Notes = txtContrNotes.Text;

            //write new member to csv file
            contributionList.Add(contributionObject);
            WriteContributionsToFile(selectedFileName);
            MessageBox.Show("New record added!");
        }

        //changed
        //update btn
        private void btnUpdateContr_Click(object sender, EventArgs e)
        {
            UpdateContributions(currContrIndex);
        }

        private void ClearContr()
        {
            txtContrNo.Text = "";
            txtContrMemID.Text = "";
            dtpContrDate.Text = "";
            txtContrAmt.Text = "";
            cboContrPaymentMethod.Text = "";
            txtContrCheckNo.Text = "";
            cboContrDesignatedFund.Text = "";
            txtContrNotes.Text = "";
        }

        //clear btn
        private void btnClearContr_Click(object sender, EventArgs e)
        {
            ClearContr();
        }

        //changed
        //delete btn
        private void btnDeleteContr_Click(object sender, EventArgs e)
        {
            DeleteContribution();
        }

        //changed
        //delets record in the contribution tab
        private void DeleteContribution()
        {
            //write delete btn code so that the displayed contribution is deleted from csv
            int pos;
            //Find the position in the List of the contribution to delete
            pos = currContrIndex;

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this record?  " + "\n\n" +
                        "Contribution No:  " + currContrIndex + "\n\n",
                        "Delete Record", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (dialogResult == DialogResult.Yes)
            {
                //IF the contribution was found, remove it
                if (pos > -1 && txtContrNo.Text != "")
                {
                    contributionList.RemoveAt(pos);
                }
            }

            ClearContr();
            WriteContributionsToFile(selectedFileName);
        }

        //search contribution btn
        private void btnContrSearch_Click(object sender, EventArgs e)
        {
            //Need to check for errors

            //data validation
            if (txtContrSearchMemID.Text == "")
            {
                MessageBox.Show("Please enter Member ID to complete search.", "Invalid Member Information!");
            }
            else
            {
                ClearContr();
                //Retrieve the original list
                var filterContrs =
                   from c in contributionList
                   select c;

                filterContrs = filterContrs.Where(c => c.ContributionNo == Convert.ToInt32(txtContrSearchMemID.Text));

                currContrIndex = Convert.ToInt32(filterContrs);
            }

            DisplayContributions(currContrIndex);
        }

        //clear search contribution btn
        private void btnContrClearSearch_Click(object sender, EventArgs e)
        {
            txtContrSearchLName.Text = "";
            txtContrSearchFName.Text = "";
            txtContrSearchMemID.Text = "";
        }

        //go to first member record
        private void tsbFirstContribution_Click_1(object sender, EventArgs e)
        {
            if (currContrIndex >= 0)
            {
                currContrIndex = 0;
                DisplayContributions(currContrIndex);
            }
        }

        //go to previous member record
        private void tsbPreviousContribution_Click_1(object sender, EventArgs e)
        {
            if (currContrIndex > 0)
            {
                currContrIndex--;
                DisplayContributions(currContrIndex);
            }
        }

        //go to next member record
        private void tsbNextContribution_Click_1(object sender, EventArgs e)
        {
            if (currContrIndex < contributionList.Count - 1)
            {
                currContrIndex++;
                DisplayContributions(currContrIndex);
            }
        }

        //go to last member record
        private void tsbLastContribution_Click_1(object sender, EventArgs e)
        {
            if (currContrIndex >= 0)
            {
                currContrIndex = contributionList.Count - 1;
                DisplayContributions(currContrIndex);
            }

        }

        #endregion


        #region MEMBERSHIP
        //ALL MEMBERSHIPS TAB CODE

        //Filter the member data grid based on user selected inputs
        private void btnFilterMembers_Click(object sender, EventArgs e)
        {
            //Retrieve the original list
            var filterMembers =
                from m in memberList
                select m;

            //Filter members by Last Name

            //Filter members by Member Type
            if (cboFilterMemType.Text != "")
            {
                filterMembers = filterMembers.Where(m => m.MemberType == cboFilterMemType.Text);
            }

            //Filter members by Marital Status
            if (cboFilterMarStatus.Text != "")
            {
                filterMembers = filterMembers.Where(m => m.MaritalStatus == cboFilterMarStatus.Text);
            }

            //Filter members by Age Classification


            //Display only the filtered members in the Grid
            DisplayMemberDataGrid(filterMembers.ToList());
        }

        //Clear the selected filters
        private void btnClearFilterMem_Click(object sender, EventArgs e)
        {
            //Clear textBox and comboBoxes
            txtFilterLName.Text = "";
            cboFilterMemType.Text = "";
            cboFilterMarStatus.Text = "";
            cboFilterAge.Text = "";

            //Redisplay the original list
            DisplayMemberDataGrid(memberList);
        }

        private void dgvMembers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //Load the player's profile for the row selected
            if (e.RowIndex > -1)  //ignore the header row
            {
                LoadMemberProfile(loadedMembers, e.RowIndex);
            }
        }

        private void LoadMemberProfile(List<Member> anyMemberList, int pos)
        {
            //Load a single member in the Member Profile
            lblMemberName.Text = anyMemberList[pos].Honorific + ". " +
                anyMemberList[pos].FirstName + " " + anyMemberList[pos].LastName;

            txtMembershipsMemID.Text = anyMemberList[pos].MemberID.ToString();
            txtMembershipsAddress.Text = anyMemberList[pos].Address;
            txtMembershipsCity.Text = anyMemberList[pos].City;
            txtMembershipsState.Text = anyMemberList[pos].State;
            txtMembershipsZip.Text = anyMemberList[pos].Zip.ToString();
            txtMembershipsPhNum.Text = anyMemberList[pos].Phone.ToString();
            txtMembershipsEmail.Text = anyMemberList[pos].Email;
            txtMembershipsMemType.Text = anyMemberList[pos].MemberType;
            dtpMembershipsMemDate.Text = anyMemberList[pos].MembershipDate.ToString();
            dtpMembershipsBeginDate.Text = anyMemberList[pos].AttendanceBeginDate.ToString();
            dtpMembershipsLastDate.Text = anyMemberList[pos].AttendanceLastDate.ToString();

            //Set the current member position
            currMemIndex = pos;
        }



        #endregion

        #region CONTR-HIST
        //CONTRIBUTION HISTORY TAB CODE

        //Filter the contribution data grid based on user selected inputs
        private void btnContrFilter_Click(object sender, EventArgs e)
        {
            //Retrieve the original list
            var filterContributions =
            from c in contributionList
            select c;

            //Filter contributions by Member ID
            if (txtMemIDFilter.Text != "")
            {
                filterContributions = filterContributions.Where(c => c.MemberID == txtMemIDFilter.Text);
            }

            //Filter contributions by Designated Fund
            if (cboFundFilter.Text != "")
            {
                filterContributions = filterContributions.Where(c => c.DesignatedFund == cboFundFilter.Text);
            }

            //Filter contributions by date
            //dtpContrHistDate.ValueChanged += (s, c) => { dtpContrHistDate.CustomFormat = (dtpContrHistDate.Checked && dtpContrHistDate.Value != dtpContrHistDate.MinDate) ? "MM/dd/yyyy" : " "; };

            if (dtpContrHistDate.Text != " ")
            {
                filterContributions = filterContributions.Where(c => c.ContributionDate == Convert.ToDateTime(dtpContrHistDate.Text));
            }

            //Display only the filtered contributions in the Grid
            DisplayContributionDataGrid(filterContributions.ToList());
        }


        //Clear the selected filters
        private void btnContrClearFilters_Click(object sender, EventArgs e)
        {
            //Clear comboBox, textBox, and dateTimePicker
            cboFundFilter.Text = "";
            txtMemIDFilter.Text = "";

            //Redisplay the original list
            DisplayContributionDataGrid(contributionList);

            //clear date
            ContrClearDTP();
        }

        //Sort the contribution data grid based on user selected inputs
        private void btnContrSort_Click(object sender, EventArgs e)
        {
            //Retrieve the original list
            var sortContributions =
                from c in contributionList
                select c;

            //Sort by Member ID *** ASK ABOUT THIS
            if (chkContrSortMemID.Checked)
            {
                sortContributions = sortContributions.OrderBy(c => c.MemberID).ToList();
            }

            if (chkContrSortAmt.Checked)
            {
                sortContributions = sortContributions.OrderBy(c => c.Amount).ToList();
            }

            //Display the sorted list in the grid
            DisplayContributionDataGrid(sortContributions.ToList());
        }

        #endregion

        #region QUICK-STATS
        //all Quick Stats tab code
        private void LoadChartData()
        {
            //this loads the chart data for monthly contributions
            this.chart2016Contributions.Series["Monthly Contributions"].Points.AddXY("Jan", janContributions);
            this.chart2016Contributions.Series["Monthly Contributions"].Points.AddXY("Feb", febContributions);
            this.chart2016Contributions.Series["Monthly Contributions"].Points.AddXY("Mar", marContributions);
            this.chart2016Contributions.Series["Monthly Contributions"].Points.AddXY("Apr", aprContributions);

            //this loads chart data for fund distribution
            double[] yValues = { buildingFund, churchPlanting, foodBankFund, generalFund, missionsFund, youthFund, tithesFund };
            string[] xValues = { "Building Fund", "Church Planting", "Food Bank", "General Fund", "Missions", "Restricted-Youth", "Tithes" };

            this.chartFundDistribution.Series["Series1"].Points.DataBindXY(xValues, yValues);

            //this loads chart data for payment distribution
            double[] yPymtValues = { countCheck, countCash, countOnline, countOther };
            string[] xPymtValues = { "Check", "Cash", "Online", "Other" };

            this.chartPymtDistribution.Series["SeriesPymt"].Points.DataBindXY(xPymtValues, yPymtValues);
        }
        #endregion

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit Application",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm fabout = new AboutForm();
            fabout.ShowDialog();
        }


        #region PRINT_REPORT

        private void btnPrintReports_Click(object sender, EventArgs e)
        {
            PrintGeneralReportForm fprint = new PrintGeneralReportForm(this);
            fprint.ShowDialog();
        }

        public string TransferTotalMembers
        {
            get { return lblTotalMembersOutput.Text; }
        }

        public string TransferMale
        {
            get
            { return lblTotMaleOutputHidden.Text; }
        }

        public string TransferFemale
        {
            get
            { return lblTotFemaleOuputHidden.Text; }
        }

        public string TransferMonthMemberships
        {
            get
            { return lblThisMonthMembershipsOutput.Text; }
        }

        public string TransferMonthAttendance
        {
            get
            { return lblMonthlyAttendance.Text; }
        }

        public string TransferTotEmployees
        {
            get
            { return lblTotEmployeesOutput.Text; }
        }

        public string TransferTotContr
        {
            get
            { return lblTotContributionsOutput.Text; }
        }

        public string TransferAvgContr
        {
            get
            { return lblAvgContribution.Text; }
        }

        public string TransferBuildingF
        {
            get
            { return lblHiddenBuildingFundOutput.Text; }
        }

        public string TransferPlantF
        {
            get
            { return lblHiddenPlantingFundOutput.Text; }
        }

        public string TransferFoodF
        {
            get
            { return lblHiddenFoodBankFundOutput.Text; }
        }

        public string TransferGenF
        {
            get
            { return lblHiddenGenFundOutput.Text; }
        }

        public string TransferMissionsF
        {
            get
            { return lblHiddenMissionsFundOutput.Text; }
        }

        public string TransferYouthF
        {
            get
            { return lblHiddenYouthFundOutput.Text; }
        }

        public string TransferTithesF
        {
            get
            { return lblHiddenTithesFundOutput.Text; }
        }

        #endregion

        private void dtpContrHistDate_ValueChanged(object sender, EventArgs e)
        {
            // if contribution history date time picker is selected, display a date
            if (dtpContrHistDate.Checked)
            {
                dtpContrHistDate.Format = DateTimePickerFormat.Custom;
                dtpContrHistDate.Format = DateTimePickerFormat.Short;
            }
        }

        #region HOME
        private void btnPicMem_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabMaintainMember);
        }

        private void btnPicContr_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabMaintainContributions);
        }

        private void btnPicAllMemberships_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabAllMemberships);
        }

        private void btnContrHist_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabContributionHistory);
        }

        private void btnPicQuickStats_Click(object sender, EventArgs e)
        {
            tabControl.SelectTab(tabQuickStats);
        }

        private void btnPicPrint_Click(object sender, EventArgs e)
        {
            PrintGeneralReportForm fprint = new PrintGeneralReportForm(this);
            fprint.ShowDialog();
        }

        #endregion

    }
}
