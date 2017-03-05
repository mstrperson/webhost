using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebhostMySQLConnection;
using WebhostV2.UserControls;

namespace WebhostV2
{
    public partial class LibraryCheckIn : BasePage
    {
        protected List<int> LoadedPasses
        {
            get
            {
                if (Session["LibPasses"] == null)
                    Session["LibPasses"] = new List<int>();

                return (List<int>)Session["LibPasses"];
            }
        }

        protected void AddPass(int pid)
        {
            List<int> passes = LoadedPasses;
            passes.Add(pid);
            Session["LibPasses"] = passes;
        }

        new protected void Page_Init(object sender, EventArgs e)
        {
            base.Page_Init(sender, e);
            if (LoadedPasses.Count > 0)
            {
                foreach (int id in LoadedPasses)
                {
                    LibraryPassCheckIn checkin = (LibraryPassCheckIn)LoadControl("~/UserControls/LibraryPassCheckIn.ascx");
                    checkin.PassId = id;
                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    cell.Controls.Add(checkin);
                    row.Cells.Add(cell);
                    PassTable.Rows.AddAt(0, row);
                }
            }
            else
            {
                PassRefreshTimer_Tick(sender, e);
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void PassRefreshTimer_Tick(object sender, EventArgs e)
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                List<int> currentPasses = db.LibraryPasses.Where(p => p.LibraryDay.Equals(DateTime.Today)).Select(p => p.id).ToList();
                bool reload = false;
                foreach(int id in currentPasses)
                {
                    if (!LoadedPasses.Contains(id))
                    {
                        AddPass(id);
                        reload = true;
                    }
                }

                if (reload)
                    Response.Redirect("~/LibraryCheckin.aspx");
            }
        }
    }
}