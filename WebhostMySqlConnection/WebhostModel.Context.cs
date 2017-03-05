﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebhostMySQLConnection
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class WebhostEntities : DbContext
    {
        public WebhostEntities()
            : base("name=WebhostEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AcademicYear> AcademicYears { get; set; }
        public virtual DbSet<AttendanceMarking> AttendanceMarkings { get; set; }
        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<CommentHeader> CommentHeaders { get; set; }
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<DutyTeam> DutyTeams { get; set; }
        public virtual DbSet<Faculty> Faculties { get; set; }
        public virtual DbSet<GradeTable> GradeTables { get; set; }
        public virtual DbSet<GradeTableEntry> GradeTableEntries { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<StudentComment> StudentComments { get; set; }
        public virtual DbSet<Term> Terms { get; set; }
        public virtual DbSet<Weekend> Weekends { get; set; }
        public virtual DbSet<WeekendActivity> WeekendActivities { get; set; }
        public virtual DbSet<StudentSignup> StudentSignups { get; set; }
        public virtual DbSet<Dorm> Dorms { get; set; }
        public virtual DbSet<SeatingChart> SeatingCharts { get; set; }
        public virtual DbSet<SeatingChartSeat> SeatingChartSeats { get; set; }
        public virtual DbSet<Permission> Permissions { get; set; }
        public virtual DbSet<WebPage> WebPages { get; set; }
        public virtual DbSet<ApiRequestHeader> ApiRequestHeaders { get; set; }
        public virtual DbSet<ExternalApi> ExternalApis { get; set; }
        public virtual DbSet<ServiceAccount> ServiceAccounts { get; set; }
        public virtual DbSet<WebPageTag> WebPageTags { get; set; }
        public virtual DbSet<Variable> Variables { get; set; }
        public virtual DbSet<WeekendDiscipline> WeekendDisciplines { get; set; }
        public virtual DbSet<LibraryPass> LibraryPasses { get; set; }
        public virtual DbSet<TimeStampedSignature> TimeStampedSignatures { get; set; }
        public virtual DbSet<CourseRequestComment> CourseRequestComments { get; set; }
        public virtual DbSet<APRequest> APRequests { get; set; }
        public virtual DbSet<RegistrationRequest> RegistrationRequests { get; set; }
        public virtual DbSet<GoogleCalendar> GoogleCalendars { get; set; }
        public virtual DbSet<StudentCalendarSpecialPermission> StudentCalendarSpecialPermissions { get; set; }
        public virtual DbSet<Credit> Credits { get; set; }
        public virtual DbSet<WeekendDuty> WeekendDuties { get; set; }
        public virtual DbSet<ActivityCategory> ActivityCategories { get; set; }
        public virtual DbSet<CourseRequest> CourseRequests { get; set; }
        public virtual DbSet<RequestableCourse> RequestableCourses { get; set; }
        public virtual DbSet<Preference> Preferences { get; set; }
        public virtual DbSet<Fingerprint> Fingerprints { get; set; }
        public virtual DbSet<WednesdaySchedule> WednesdaySchedules { get; set; }
        public virtual DbSet<AttendanceStatus> AttendanceStatuses { get; set; }
        public virtual DbSet<AttendanceSubmissionStatus> AttendanceSubmissionStatuses { get; set; }
        public virtual DbSet<ApiPermissionGroup> ApiPermissionGroups { get; set; }
        public virtual DbSet<ApiPermission> ApiPermissions { get; set; }
    }
}
