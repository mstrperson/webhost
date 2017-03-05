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
    public partial class CourseRequest : BasePage
    {
        protected int SelectedStudentId
        {
            get
            {
                if (Session["CR_SID"] != null)
                    return (int)Session["CR_SID"];
                else
                    return -1;
            }
            set
            {
                Session["CR_SID"] = value;
            }
        }

        protected int termId = DateRange.GetNextTerm();

        protected void LoadData()
        {
            int tid = this.AdminMasqueradeTeacherId > -1 ? this.AdminMasqueradeTeacherId : this.user.ID;
            using (WebhostEntities db = new WebhostEntities())
            {
                Term theTerm = db.Terms.Where(t => t.id == termId).Single();
                Faculty faculty = db.Faculties.Where(f => f.ID == tid).Single();
                StudentSelectDDL.DataSource = StudentListItem.GetDataSource(faculty.Students.Where(s => s.isActive && s.GraduationYear >= theTerm.AcademicYearID).Select(s => s.ID).ToList());
                StudentSelectDDL.DataTextField = "Text";
                StudentSelectDDL.DataValueField = "ID";
                StudentSelectDDL.DataBind();
                SeniorProjectPanel.Visible = false;
                SeniorProjectCB.Checked = false;

                English1DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 2 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                English1DDL.DataTextField = "Text";
                English1DDL.DataValueField = "CourseId";
                English1DDL.DataBind();

                English2DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 2 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                English2DDL.DataTextField = "Text";
                English2DDL.DataValueField = "CourseId";
                English2DDL.DataBind();


                History1DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 3 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                History1DDL.DataTextField = "Text";
                History1DDL.DataValueField = "CourseId";
                History1DDL.DataBind();

                History2DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 3 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                History2DDL.DataTextField = "Text";
                History2DDL.DataValueField = "CourseId";
                History2DDL.DataBind();

                Science1DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 0 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Science1DDL.DataTextField = "Text";
                Science1DDL.DataValueField = "CourseId";
                Science1DDL.DataBind();

                Science2DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 0 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Science2DDL.DataTextField = "Text";
                Science2DDL.DataValueField = "CourseId";
                Science2DDL.DataBind();

                Science3DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 0 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Science3DDL.DataTextField = "Text";
                Science3DDL.DataValueField = "CourseId";
                Science3DDL.DataBind();

                Math1DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 4 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Math1DDL.DataTextField = "Text";
                Math1DDL.DataValueField = "CourseId";
                Math1DDL.DataBind();

                Math2DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 4 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Math2DDL.DataTextField = "Text";
                Math2DDL.DataValueField = "CourseId";
                Math2DDL.DataBind();


                Art1DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 9 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Art1DDL.DataTextField = "Text";
                Art1DDL.DataValueField = "CourseId";
                Art1DDL.DataBind();

                Art2DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 9 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Art2DDL.DataTextField = "Text";
                Art2DDL.DataValueField = "CourseId";
                Art2DDL.DataBind();


                Art3DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 9 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Art3DDL.DataTextField = "Text";
                Art3DDL.DataValueField = "CourseId";
                Art3DDL.DataBind();

                WorldLang1DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 5 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                WorldLang1DDL.DataTextField = "Text";
                WorldLang1DDL.DataValueField = "CourseId";
                WorldLang1DDL.DataBind();

                WorldLang2DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 5 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                WorldLang2DDL.DataTextField = "Text";
                WorldLang2DDL.DataValueField = "CourseId";
                WorldLang2DDL.DataBind();



                Tech1DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 1 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Tech1DDL.DataTextField = "Text";
                Tech1DDL.DataValueField = "CourseId";
                Tech1DDL.DataBind();

                Tech2DDL.DataSource = CourseListItem.GetDataSource(theTerm.AcademicYear.Courses.Where(c => c.DepartmentID == 1 && (theTerm.Name.Equals("Fall") || c.LengthInTerms == 1)).OrderBy(c => c.Name).Select(c => c.id).ToList());
                Tech2DDL.DataTextField = "Text";
                Tech2DDL.DataValueField = "CourseId";
                Tech2DDL.DataBind();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/MidYearCourseRequest.aspx");
            if(!Page.IsPostBack)
            {
                CourseRequestAdmin1.Visible = this.user.Permissions.Contains(0) || this.user.Permissions.Contains(4);
                LoadData();
            }
        }

        protected void LoadPreviousRequests()
        {
            using(WebhostEntities db = new WebhostEntities())
            {
                Student student = db.Students.Where(s => s.ID == SelectedStudentId).Single();
                Term term = db.Terms.Where(t => t.id == termId).Single();
                SeniorProjectPanel.Visible = student.GraduationYear == term.AcademicYearID;
                if(!SeniorProjectPanel.Visible)
                {
                    SeniorProjectCB.Checked = false;
                }

                List<WebhostMySQLConnection.CourseRequest> coursereqs = student.CourseRequests.Where(cr => cr.TermId == termId).ToList();

                if (student.CourseRequestComments.Where(crn => crn.TermId == termId).Count() > 0)
                    NotesInput.Text = student.CourseRequestComments.Where(crn => crn.TermId == termId).Single().Notes;
                else
                    NotesInput.Text = "";

                int eng = 0, hist = 0, sci = 0, math = 0, wl = 0, art = 0, tech = 0;

                foreach(WebhostMySQLConnection.CourseRequest cr in coursereqs)
                {
                    switch (cr.RequestableCourse.Course.DepartmentID)
                    {
                        case 0: // Science
                            if (cr.IsGlobalAlternate)
                            {       
                                Science3DDL.ClearSelection();
                                Science3DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                sci++;
                            }
                            else if (cr.IsSecondary || sci > 0)
                            {
                                Science2DDL.ClearSelection();
                                Science2DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                sci++;
                                SciencePriority.ClearSelection();
                                if (cr.IsSecondary)
                                {
                                    SciencePriority.SelectedIndex = 0;
                                }
                                else
                                {
                                    SciencePriority.SelectedIndex = 1;
                                }
                            }
                            else
                            {
                                Science1DDL.ClearSelection();
                                Science1DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                sci++;
                            }
                            break;
                        case 1: // Tech
                            if (cr.IsGlobalAlternate || cr.IsSecondary || tech > 0)
                            {
                                if (!cr.IsGlobalAlternate && !cr.IsSecondary)
                                {
                                    TechPriority.ClearSelection();
                                    TechPriority.SelectedIndex = 1;
                                }
                                else
                                {
                                    TechPriority.ClearSelection();
                                    TechPriority.SelectedIndex = 0;
                                }
                                Tech2DDL.ClearSelection();
                                Tech2DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                tech++;
                            }
                            else
                            {
                                Tech1DDL.ClearSelection();
                                Tech1DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                tech++;
                            }
                            break;
                        case 2: // English
                            if (cr.IsGlobalAlternate || cr.IsSecondary || eng > 0)
                            {
                                if (!cr.IsGlobalAlternate && !cr.IsSecondary)
                                {
                                    EnglishSelectionPriority.ClearSelection();
                                    EnglishSelectionPriority.SelectedIndex = 1;
                                }
                                else
                                {
                                    EnglishSelectionPriority.ClearSelection();
                                    EnglishSelectionPriority.SelectedIndex = 0;
                                }
                                English2DDL.ClearSelection();
                                English2DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                eng++;
                            }
                            else
                            {
                                English1DDL.ClearSelection();
                                English1DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                eng++;
                            }
                            break;
                        case 3: // History
                            if (cr.IsGlobalAlternate || cr.IsSecondary || hist > 0)
                            {
                                if (!cr.IsGlobalAlternate && !cr.IsSecondary)
                                {
                                    HistorySelectionPriority.ClearSelection();
                                    HistorySelectionPriority.SelectedIndex = 1;
                                }
                                else
                                {
                                    HistorySelectionPriority.ClearSelection();
                                    HistorySelectionPriority.SelectedIndex = 0;
                                }
                                History2DDL.ClearSelection();
                                History2DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                hist++;
                            }
                            else
                            {
                                History1DDL.ClearSelection();
                                History1DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                hist++;
                            }
                            break;
                        case 4: // Math
                            if (cr.IsGlobalAlternate || cr.IsSecondary || math > 0)
                            {
                                if (!cr.IsGlobalAlternate && !cr.IsSecondary)
                                {
                                    MathPriority.ClearSelection();
                                    MathPriority.SelectedIndex = 1;
                                }
                                else
                                {
                                    MathPriority.ClearSelection();
                                    MathPriority.SelectedIndex = 0;
                                }
                                Math2DDL.ClearSelection();
                                Math2DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                math++;
                            }
                            else
                            {
                                Math1DDL.ClearSelection();
                                Math1DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                math++;
                            }
                            break;
                        case 5: // World Languages
                            if (cr.RequestableCourse.Course.Name.Contains("ESL"))
                            {
                                ESL.Checked = true;
                            }
                            else if (cr.IsGlobalAlternate || cr.IsSecondary || wl > 0)
                            {
                                if (!cr.IsGlobalAlternate && !cr.IsSecondary)
                                {
                                    WorldLangPriority.ClearSelection();
                                    WorldLangPriority.SelectedIndex = 1;
                                }
                                else
                                {
                                    WorldLangPriority.ClearSelection();
                                    WorldLangPriority.SelectedIndex = 2;
                                }
                                WorldLang2DDL.ClearSelection();
                                WorldLang2DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                wl++;
                            }
                            else
                            {
                                WorldLang1DDL.ClearSelection();
                                WorldLang1DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                wl++;
                            }
                            break;
                        case 6: // LSP
                            LSP.Checked = true;
                            break;
                        case 9: // Art
                            if (cr.IsGlobalAlternate)
                            {
                                Art3DDL.ClearSelection();
                                Art3DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                Art3Priority.ClearSelection();
                                Art3Priority.SelectedIndex = 0;
                                art++;
                            }
                            else if (cr.IsSecondary || art > 0)
                            {
                                if (art > 1)
                                {
                                    Art3DDL.ClearSelection();
                                    Art3DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                    art++;
                                    Art3Priority.ClearSelection();
                                    if (cr.IsSecondary)
                                    {
                                        Art3Priority.SelectedIndex = 0;
                                    }
                                    else
                                    {
                                        Art3Priority.SelectedIndex = 1;
                                    }
                                }
                                else
                                {
                                    Art2DDL.ClearSelection();
                                    Art2DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                    art++;
                                    ArtPriority.ClearSelection();
                                    if (cr.IsSecondary)
                                    {
                                        ArtPriority.SelectedIndex = 0;
                                    }
                                    else
                                    {
                                        ArtPriority.SelectedIndex = 1;
                                    }
                                }
                            }
                            else
                            {
                                Art1DDL.ClearSelection();
                                Art1DDL.SelectedValue = Convert.ToString(cr.CourseId);
                                art++;
                            }
                            break;
                        case 8:
                            SeniorProjectCB.Checked = true;
                            break;
                        default: break;
                    }
                }

            }
        }

        protected void StudentSelectDDL_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected void SubmitBtn_Click(object sender, EventArgs e)
        {
            using (WebhostEntities db = new WebhostEntities())
            {
                int newCRN = db.CourseRequests.Count() > 0? db.CourseRequests.OrderBy(c => c.id).ToList().Last().id : -1;
                Student student = db.Students.Where(s => s.ID == SelectedStudentId).Single();
                List<WebhostMySQLConnection.CourseRequest> coursereqs = student.CourseRequests.Where(cr => cr.TermId == termId).ToList();
                foreach(WebhostMySQLConnection.CourseRequest cr in coursereqs)
                {
                    if(cr.APRequests.Count > 0)
                    {
                        foreach(APRequest req in cr.APRequests)
                        {
                            req.Sections.Clear();
                        }
                        db.APRequests.RemoveRange(cr.APRequests);
                        cr.APRequests.Clear();
                    }
                }
                db.CourseRequests.RemoveRange(coursereqs);

                if(SeniorProjectCB.Checked)
                {
                    WebhostMySQLConnection.CourseRequest sp = new WebhostMySQLConnection.CourseRequest()
                    {
                        id = ++newCRN,
                        StudentId = SelectedStudentId,
                        CourseId = 1995,
                        IsSecondary = false,
                        IsGlobalAlternate = false,
                        TermId = termId
                    };

                    db.CourseRequests.Add(sp);
                }

                if(LSP.Checked)
                {
                    WebhostMySQLConnection.CourseRequest sp = new WebhostMySQLConnection.CourseRequest()
                    {
                        id = ++newCRN,
                        StudentId = SelectedStudentId,
                        CourseId = 2057,
                        IsSecondary = false,
                        IsGlobalAlternate = false,
                        TermId = termId
                    };

                    db.CourseRequests.Add(sp);
                }

                if(ESL.Checked)
                {
                    WebhostMySQLConnection.CourseRequest sp = new WebhostMySQLConnection.CourseRequest()
                    {
                        id = ++newCRN,
                        StudentId = SelectedStudentId,
                        CourseId = student.GraduationYear > 2018 ? 2058 : 2059,
                        IsSecondary = false,
                        IsGlobalAlternate = false,
                        TermId = termId
                    };

                    db.CourseRequests.Add(sp);
                }

                // Ignore if already enrolled in English Full Year class.
                if (!EnglishFullYearCB.Checked)
                {
                    WebhostMySQLConnection.CourseRequest eng1 = new WebhostMySQLConnection.CourseRequest()
                    {
                        id = ++newCRN,
                        StudentId = SelectedStudentId,
                        CourseId = Convert.ToInt32(English1DDL.SelectedValue),
                        IsSecondary = false,
                        IsGlobalAlternate = false,
                        TermId = termId
                    };

                    db.CourseRequests.Add(eng1);

                    if (English2DDL.SelectedIndex >= 0)
                    {
                        WebhostMySQLConnection.CourseRequest eng2 = new WebhostMySQLConnection.CourseRequest()
                        {
                            id = ++newCRN,
                            StudentId = SelectedStudentId,
                            CourseId = Convert.ToInt32(English2DDL.SelectedValue),
                            IsSecondary = EnglishSelectionPriority.SelectedIndex == 0,
                            IsGlobalAlternate = false,
                            TermId = termId
                        };

                        db.CourseRequests.Add(eng2);
                    }
                }

                if (!HistoryFullYearCB.Checked)
                {
                    if (History1DDL.SelectedIndex >= 0)
                    {
                        WebhostMySQLConnection.CourseRequest hist1 = new WebhostMySQLConnection.CourseRequest()
                        {
                            id = ++newCRN,
                            StudentId = SelectedStudentId,
                            CourseId = Convert.ToInt32(History1DDL.SelectedValue),
                            IsSecondary = false,
                            IsGlobalAlternate = false,
                            TermId = termId
                        };

                        db.CourseRequests.Add(hist1);

                        if (History2DDL.SelectedIndex >= 0)
                        {
                            WebhostMySQLConnection.CourseRequest hist2 = new WebhostMySQLConnection.CourseRequest()
                            {
                                id = ++newCRN,
                                StudentId = SelectedStudentId,
                                CourseId = Convert.ToInt32(History2DDL.SelectedValue),
                                IsSecondary = HistorySelectionPriority.SelectedIndex == 0,
                                IsGlobalAlternate = false,
                                TermId = termId
                            };

                            db.CourseRequests.Add(hist2);
                        }
                    }
                }

                if (!MathFullYear.Checked)
                {
                    if (Math1DDL.SelectedIndex >= 0)
                    {
                        WebhostMySQLConnection.CourseRequest math1 = new WebhostMySQLConnection.CourseRequest()
                        {
                            id = ++newCRN,
                            StudentId = SelectedStudentId,
                            CourseId = Convert.ToInt32(Math1DDL.SelectedValue),
                            IsSecondary = false,
                            IsGlobalAlternate = false,
                            TermId = termId
                        };

                        db.CourseRequests.Add(math1);

                        if (Math2DDL.SelectedIndex >= 0)
                        {
                            WebhostMySQLConnection.CourseRequest math2 = new WebhostMySQLConnection.CourseRequest()
                            {
                                id = ++newCRN,
                                StudentId = SelectedStudentId,
                                CourseId = Convert.ToInt32(Math2DDL.SelectedValue),
                                IsSecondary = MathPriority.SelectedIndex == 0,
                                IsGlobalAlternate = MathPriority.SelectedIndex == 2,
                                TermId = termId
                            };

                            db.CourseRequests.Add(math2);
                        }
                    }
                }

                if (!WorldLanguageSkip.Checked)
                {
                    if (WorldLang1DDL.SelectedIndex >= 0)
                    {
                        WebhostMySQLConnection.CourseRequest wl1 = new WebhostMySQLConnection.CourseRequest()
                        {
                            id = ++newCRN,
                            StudentId = SelectedStudentId,
                            CourseId = Convert.ToInt32(WorldLang1DDL.SelectedValue),
                            IsSecondary = false,
                            IsGlobalAlternate = false,
                            TermId = termId
                        };

                        db.CourseRequests.Add(wl1);

                        if (WorldLang2DDL.SelectedIndex >= 0)
                        {
                            WebhostMySQLConnection.CourseRequest wl2 = new WebhostMySQLConnection.CourseRequest()
                            {
                                id = ++newCRN,
                                StudentId = SelectedStudentId,
                                CourseId = Convert.ToInt32(WorldLang2DDL.SelectedValue),
                                IsSecondary = MathPriority.SelectedIndex == 0,
                                IsGlobalAlternate = MathPriority.SelectedIndex == 2,
                                TermId = termId
                            };

                            db.CourseRequests.Add(wl2);
                        }
                    }
                }

                if (Tech1DDL.SelectedIndex >= 0)
                {
                    WebhostMySQLConnection.CourseRequest tech1 = new WebhostMySQLConnection.CourseRequest()
                    {
                        id = ++newCRN,
                        StudentId = SelectedStudentId,
                        CourseId = Convert.ToInt32(Tech1DDL.SelectedValue),
                        IsSecondary = false,
                        IsGlobalAlternate = false,
                        TermId = termId
                    };

                    db.CourseRequests.Add(tech1);
                    if (Tech2DDL.SelectedIndex >= 0)
                    {
                        WebhostMySQLConnection.CourseRequest tech2 = new WebhostMySQLConnection.CourseRequest()
                        {
                            id = ++newCRN,
                            StudentId = SelectedStudentId,
                            CourseId = Convert.ToInt32(Tech2DDL.SelectedValue),
                            IsSecondary = TechPriority.SelectedIndex == 0,
                            IsGlobalAlternate = TechPriority.SelectedIndex == 2,
                            TermId = termId
                        };

                        db.CourseRequests.Add(tech2);
                    }
                }

                if (!ScienceFullYearCB.Checked)
                {
                    if (Science1DDL.SelectedIndex >= 0)
                    {
                        WebhostMySQLConnection.CourseRequest sci1 = new WebhostMySQLConnection.CourseRequest()
                        {
                            id = ++newCRN,
                            StudentId = SelectedStudentId,
                            CourseId = Convert.ToInt32(Science1DDL.SelectedValue),
                            IsSecondary = false,
                            IsGlobalAlternate = false,
                            TermId = termId
                        };

                        db.CourseRequests.Add(sci1);
                        if (Science2DDL.SelectedIndex >= 0)
                        {
                            WebhostMySQLConnection.CourseRequest sci2 = new WebhostMySQLConnection.CourseRequest()
                            {
                                id = ++newCRN,
                                StudentId = SelectedStudentId,
                                CourseId = Convert.ToInt32(Science2DDL.SelectedValue),
                                IsSecondary = SciencePriority.SelectedIndex == 0,
                                IsGlobalAlternate = SciencePriority.SelectedIndex == 2,
                                TermId = termId
                            };

                            db.CourseRequests.Add(sci2);
                        }

                        if (Science3DDL.SelectedIndex >= 0)
                        {
                            WebhostMySQLConnection.CourseRequest sci3 = new WebhostMySQLConnection.CourseRequest()
                            {
                                id = ++newCRN,
                                StudentId = SelectedStudentId,
                                CourseId = Convert.ToInt32(Science3DDL.SelectedValue),
                                IsSecondary = true,
                                IsGlobalAlternate = true,
                                TermId = termId
                            };

                            db.CourseRequests.Add(sci3);
                        }
                    }
                }

                if (Art1DDL.SelectedIndex >= 0)
                {
                    WebhostMySQLConnection.CourseRequest art1 = new WebhostMySQLConnection.CourseRequest()
                    {
                        id = ++newCRN,
                        StudentId = SelectedStudentId,
                        CourseId = Convert.ToInt32(Art1DDL.SelectedValue),
                        IsSecondary = false,
                        IsGlobalAlternate = false,
                        TermId = termId
                    };

                    db.CourseRequests.Add(art1);

                    if (Art2DDL.SelectedIndex >= 0)
                    {
                        WebhostMySQLConnection.CourseRequest art2 = new WebhostMySQLConnection.CourseRequest()
                        {
                            id = ++newCRN,
                            StudentId = SelectedStudentId,
                            CourseId = Convert.ToInt32(Art2DDL.SelectedValue),
                            IsSecondary = ArtPriority.SelectedIndex == 0,
                            IsGlobalAlternate = ArtPriority.SelectedIndex == 2,
                            TermId = termId
                        };

                        db.CourseRequests.Add(art2);
                    }
                    if(Art3DDL.SelectedIndex >= 0)
                    {
                        WebhostMySQLConnection.CourseRequest art2 = new WebhostMySQLConnection.CourseRequest()
                        {
                            id = ++newCRN,
                            StudentId = SelectedStudentId,
                            CourseId = Convert.ToInt32(Art3DDL.SelectedValue),
                            IsSecondary = Art3Priority.SelectedIndex == 1,
                            IsGlobalAlternate = Art3Priority.SelectedIndex == 0,
                            TermId = termId
                        };

                        db.CourseRequests.Add(art2);
                    }
                }

                if (student.CourseRequestComments.Where(crn => crn.TermId == termId).Count() > 0)
                {
                    CourseRequestComment crn = student.CourseRequestComments.Where(c => c.TermId == termId).Single();
                    crn.Notes = NotesInput.Text;
                }
                else
                {
                    int crnid = db.CourseRequestComments.Count() > 0?db.CourseRequestComments.OrderBy(c => c.id).ToList().Last().id + 1:0;
                    CourseRequestComment crn = new CourseRequestComment()
                    {
                        id = crnid,
                        Notes = NotesInput.Text,
                        StudentId = SelectedStudentId,
                        TermId = termId
                    };

                    db.CourseRequestComments.Add(crn);
                }

                db.SaveChanges();
                Response.Redirect("~/CourseRequestConfirmed.aspx");
            }
        }

        protected void SelectStudentbtn_Click(object sender, EventArgs e)
        {

            SelectedStudentId = Convert.ToInt32(StudentSelectDDL.SelectedValue);
            LoadPreviousRequests();
            SignupPanel.Visible = true;
        }

        protected void CourseRequestAdmin1_Masquerade(object sender, EventArgs e)
        {
            this.AdminMasqueradeTeacherId = CourseRequestAdmin1.MasqeradeId;
            LoadData();
        }

        protected void HistoryFullYearCB_CheckedChanged(object sender, EventArgs e)
        {
            History1DDL.Enabled = !HistoryFullYearCB.Checked;
            History2DDL.Enabled = !HistoryFullYearCB.Checked;
        }

        protected void EnglishFullYearCB_CheckedChanged(object sender, EventArgs e)
        {
            English1DDL.Enabled = !EnglishFullYearCB.Checked;
            English2DDL.Enabled = !EnglishFullYearCB.Checked;
        }

        protected void WorldLanguageSkip_CheckedChanged(object sender, EventArgs e)
        {
            WorldLang1DDL.Enabled = !WorldLanguageSkip.Checked;
            WorldLang2DDL.Enabled = !WorldLanguageSkip.Checked;
        }

        protected void ScienceFullYearCB_CheckedChanged(object sender, EventArgs e)
        {
            Science1DDL.Enabled = !ScienceFullYearCB.Checked;
            Science2DDL.Enabled = !ScienceFullYearCB.Checked;
            Science3DDL.Enabled = !ScienceFullYearCB.Checked;
        }

        protected void MathFullYear_CheckedChanged(object sender, EventArgs e)
        {
            Math1DDL.Enabled = !MathFullYear.Checked;
            Math2DDL.Enabled = !MathFullYear.Checked;
        }

    }
}