using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EducationalPlatform.API.Models;

public partial class Entities : DbContext
{
    public Entities()
    {
    }

    public Entities(DbContextOptions<Entities> options)
        : base(options)
    {
    }

    private static Entities _context;

    public static Entities GetContext()
    {
        if (_context == null) _context = new Entities();
        return _context;
    }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<CompletedTask> CompletedTasks { get; set; }

    public virtual DbSet<CompletedTasksComment> CompletedTasksComments { get; set; }

    public virtual DbSet<Group> Groups { get; set; }

    public virtual DbSet<GroupsSubject> GroupsSubjects { get; set; }

    public virtual DbSet<ProgramTrack> ProgramTracks { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<SubjectSection> SubjectSections { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<TeachersSubject> TeachersSubjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=120Education.mssql.somee.com;Database=120Education;User Id=0120;Password=3812IAfr;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.SubjectSectionId).HasColumnName("SubjectSectionID");

            entity.HasOne(d => d.SubjectSection).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.SubjectSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Assignments_SubjectSections");
        });

        modelBuilder.Entity<CompletedTask>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.AssignmentId).HasColumnName("AssignmentID");
            entity.Property(e => e.ExecutionDateTime).HasColumnType("datetime");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Assignment).WithMany(p => p.CompletedTasks)
                .HasForeignKey(d => d.AssignmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompletedTasks_Assignments");

            entity.HasOne(d => d.Student).WithMany(p => p.CompletedTasks)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompletedTasks_Students");
        });

        modelBuilder.Entity<CompletedTasksComment>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.CompletedTaskId).HasColumnName("CompletedTaskID");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Student).WithMany(p => p.CompletedTasksComments)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_CompletedTasksComments_Students");

            entity.HasOne(d => d.Teacher).WithMany(p => p.CompletedTasksComments)
                .HasForeignKey(d => d.TeacherId)
                .HasConstraintName("FK_CompletedTasksComments_Teachers");
        });

        modelBuilder.Entity<Group>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.ProgramTrackId).HasColumnName("ProgramTrackID");

            entity.HasOne(d => d.ProgramTrack).WithMany(p => p.Groups)
                .HasForeignKey(d => d.ProgramTrackId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Groups_ProgramTracks");
        });

        modelBuilder.Entity<GroupsSubject>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");

            entity.HasOne(d => d.Group).WithMany(p => p.GroupsSubjects)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupsSubjects_Groups");

            entity.HasOne(d => d.Subject).WithMany(p => p.GroupsSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GroupsSubjects_Subjects");
        });

        modelBuilder.Entity<ProgramTrack>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.ShortPtname)
                .HasMaxLength(50)
                .HasColumnName("ShortPTName");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Email).HasMaxLength(20);
            entity.Property(e => e.GroupId).HasColumnName("GroupID");

            entity.HasOne(d => d.Group).WithMany(p => p.Students)
                .HasForeignKey(d => d.GroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Students_Groups");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
        });

        modelBuilder.Entity<SubjectSection>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.GroupSubjectId).HasColumnName("GroupSubjectID");

            entity.HasOne(d => d.GroupSubject).WithMany(p => p.SubjectSections)
                .HasForeignKey(d => d.GroupSubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_SubjectSections_GroupsSubjects");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.RoleId).HasColumnName("RoleID");

            entity.HasOne(d => d.Role).WithMany(p => p.Teachers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Teachers_Roles");
        });

        modelBuilder.Entity<TeachersSubject>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.SubjectId).HasColumnName("SubjectID");
            entity.Property(e => e.TeacherId).HasColumnName("TeacherID");

            entity.HasOne(d => d.Subject).WithMany(p => p.TeachersSubjects)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeachersSubjects_Subjects");

            entity.HasOne(d => d.Teacher).WithMany(p => p.TeachersSubjects)
                .HasForeignKey(d => d.TeacherId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TeachersSubjects_Teachers");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
