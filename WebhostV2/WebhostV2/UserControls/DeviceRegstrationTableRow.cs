using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;

namespace WebhostV2.UserControls
{
    public class DeviceRegstrationTableRow : TableRow
    {
        protected TableCell NameCell;
        protected TableCell DeviceTypeCell;
        protected TableCell MacAddressCell;
        protected TableCell IPAddressCell;
        protected TableCell ControllsCell;

        protected CheckBox CompletedCB;
        protected CheckBox RejectedCB;
        protected CheckBox ProctorCB;

        public int RegistrationId
        {
            get;
            protected set;
        }

        public String StudentName
        {
            get
            {
                return NameCell.Text;
            }
            protected set
            {
                NameCell.Text = value;
            }
        }

        public String DeviceType
        {
            get
            { return DeviceTypeCell.Text; }
            protected set
            {
                DeviceTypeCell.Text = value;
            }
        }

        public String MacAddress
        { get { return MacAddressCell.Text; } protected set { MacAddressCell.Text = value; } }

        public bool Completed { get { return CompletedCB.Checked; } protected set { CompletedCB.Checked = value; } }
        public bool Rejected { get { return RejectedCB.Checked; } protected set { RejectedCB.Checked = value; } }
        public bool isProctor { get { return ProctorCB.Checked; } protected set { ProctorCB.Checked = value; } }

        public String IPAddress
        {
            get;
            set;
        }

        public static TableHeaderRow HeaderRow
        {
            get
            {
                TableHeaderRow headerRow = new TableHeaderRow();
                TableHeaderCell Name = new TableHeaderCell() { Text = "Student" };
                TableHeaderCell Type = new TableHeaderCell() { Text = "Device Type" };
                TableHeaderCell Mac = new TableHeaderCell() { Text = "Mac Address" };
                TableHeaderCell IP = new TableHeaderCell() { Text = "IP Address Assigned?" };
                TableHeaderCell Ctrl = new TableHeaderCell() { Text = "Status" };

                headerRow.Cells.Add(Name);
                headerRow.Cells.Add(Type);
                headerRow.Cells.Add(Mac);
                headerRow.Cells.Add(IP);
                headerRow.Cells.Add(Ctrl);

                return headerRow;
            }
        }

        public DeviceRegstrationTableRow(int id)
        {
            NameCell = new TableCell();
            DeviceTypeCell = new TableCell();
            MacAddressCell = new TableCell();
            IPAddressCell = new TableCell();
            ControllsCell = new TableCell();

            CompletedCB = new CheckBox() { Text = "Request Completed", Checked = false };
            RejectedCB = new CheckBox() { Text = "Request Denied", Checked = false };
            ProctorCB = new CheckBox() { Text = "Is Proctor", Checked = false };

            ControllsCell.Controls.Add(CompletedCB);
            ControllsCell.Controls.Add(RejectedCB);
            ControllsCell.Controls.Add(ProctorCB);

            this.Cells.Add(NameCell);
            this.Cells.Add(DeviceTypeCell);
            this.Cells.Add(MacAddressCell);
            this.Cells.Add(IPAddressCell);
            this.Cells.Add(ControllsCell);

            RegistrationId = id;
            using(WebhostEntities db = new WebhostEntities())
            {
                WebhostMySQLConnection.RegistrationRequest request = db.RegistrationRequests.Where(req => req.id == id).Single();
                StudentName = String.Format("{0} {1} [{2}]", request.Student.FirstName, request.Student.LastName, request.Student.GraduationYear);
                DeviceType = request.DeviceType;
                MacAddress = request.MacAddress;
                Completed = request.RequestCompleted;
                Rejected = request.RequestDenied;
                isProctor = PermissionControl.GetProctors().Contains(request.Student.ID);
                IPAddress = Completed?"Already Assigned":Rejected?"Will Not Be Assigned":"";
            }
        }
    }
}