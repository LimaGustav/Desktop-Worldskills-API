using System;
using System.Collections.Generic;
using ApiProvaMundialSession3.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiProvaMundialSession3.Contexts;

public partial class MainContext : DbContext
{
    public MainContext()
    {
    }

    public MainContext(DbContextOptions<MainContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chamadum> Chamada { get; set; }

    public virtual DbSet<Cordenadum> Cordenada { get; set; }

    public virtual DbSet<Estacao> Estacaos { get; set; }

    public virtual DbSet<Incidente> Incidentes { get; set; }

    public virtual DbSet<IncidenteSubTipo> IncidenteSubTipos { get; set; }

    public virtual DbSet<IncidenteUnidadeAtendimento> IncidenteUnidadeAtendimentos { get; set; }

    public virtual DbSet<IncidenteUnidadeAtribuidum> IncidenteUnidadeAtribuida { get; set; }

    public virtual DbSet<Severidade> Severidades { get; set; }

    public virtual DbSet<StatusChamadum> StatusChamada { get; set; }

    public virtual DbSet<StatusIncidente> StatusIncidentes { get; set; }

    public virtual DbSet<StatusUnidade> StatusUnidades { get; set; }

    public virtual DbSet<TipoIncidente> TipoIncidentes { get; set; }

    public virtual DbSet<TipoUnidade> TipoUnidades { get; set; }

    public virtual DbSet<Unidade> Unidades { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chamadum>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AnsweredAt)
                .HasColumnType("datetime")
                .HasColumnName("answeredAt");
            entity.Property(e => e.CallerName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("callerName");
            entity.Property(e => e.CallerNumber)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("callerNumber");
            entity.Property(e => e.Descricao)
                .IsUnicode(false)
                .HasColumnName("descricao");
            entity.Property(e => e.EndedAt)
                .HasColumnType("datetime")
                .HasColumnName("endedAt");
            entity.Property(e => e.LinkedIncidentId).HasColumnName("linkedIncidentId");
            entity.Property(e => e.LocationId).HasColumnName("locationID");
            entity.Property(e => e.ReportedType)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("reportedType");
            entity.Property(e => e.StartedAt)
                .HasColumnType("datetime")
                .HasColumnName("startedAt");
            entity.Property(e => e.StatusCallId).HasColumnName("statusCallID");

            entity.HasOne(d => d.LinkedIncident).WithMany(p => p.Chamada)
                .HasForeignKey(d => d.LinkedIncidentId)
                .HasConstraintName("FK_Chamada_Incidente");

            entity.HasOne(d => d.Location).WithMany(p => p.Chamada)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK_Chamada_Cordenada");

            entity.HasOne(d => d.StatusCall).WithMany(p => p.Chamada)
                .HasForeignKey(d => d.StatusCallId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Chamada_Status");
        });

        modelBuilder.Entity<Cordenadum>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.X)
                .HasColumnType("decimal(7, 2)")
                .HasColumnName("x");
            entity.Property(e => e.Y)
                .HasColumnType("decimal(7, 2)")
                .HasColumnName("y");
        });

        modelBuilder.Entity<Estacao>(entity =>
        {
            entity.ToTable("Estacao");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AvailableUnitCount).HasColumnName("availableUnitCount");
            entity.Property(e => e.LocationId).HasColumnName("locationID");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("name");
            entity.Property(e => e.TypeUnitId).HasColumnName("typeUnitID");

            entity.HasOne(d => d.Location).WithMany(p => p.Estacaos)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Estacao_Cordenada");

            entity.HasOne(d => d.TypeUnit).WithMany(p => p.Estacaos)
                .HasForeignKey(d => d.TypeUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Estacao_TipoUnidade");
        });

        modelBuilder.Entity<Incidente>(entity =>
        {
            entity.ToTable("Incidente");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CallId).HasColumnName("callID");
            entity.Property(e => e.Descricao)
                .IsUnicode(false)
                .HasColumnName("descricao");
            entity.Property(e => e.LocationId).HasColumnName("locationID");
            entity.Property(e => e.ReportedAt)
                .HasColumnType("datetime")
                .HasColumnName("reportedAt");
            entity.Property(e => e.ResolvedAt)
                .HasColumnType("datetime")
                .HasColumnName("resolvedAt");
            entity.Property(e => e.SeverityId).HasColumnName("severityID");
            entity.Property(e => e.StatusId).HasColumnName("statusID");
            entity.Property(e => e.SubtypeId).HasColumnName("subtypeID");
            entity.Property(e => e.TypeId).HasColumnName("typeID");

            entity.HasOne(d => d.Location).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_Cordenada");

            entity.HasOne(d => d.Severity).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.SeverityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_Severidade");

            entity.HasOne(d => d.Status).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_Status");

            entity.HasOne(d => d.Subtype).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.SubtypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_SubTipo");

            entity.HasOne(d => d.Type).WithMany(p => p.Incidentes)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Incidente_Tipo");
        });

        modelBuilder.Entity<IncidenteSubTipo>(entity =>
        {
            entity.ToTable("IncidenteSubTipo");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.SubType)
                .HasMaxLength(255)
                .IsUnicode(false);
            entity.Property(e => e.TipoIncidenteId).HasColumnName("tipoIncidenteID");

            entity.HasOne(d => d.TipoIncidente).WithMany(p => p.IncidenteSubTipos)
                .HasForeignKey(d => d.TipoIncidenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_IncidenteSubTipo_TipoIncidente");
        });

        modelBuilder.Entity<IncidenteUnidadeAtendimento>(entity =>
        {
            entity.ToTable("IncidenteUnidadeAtendimento");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AtendidoEm)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("atendidoEm");
            entity.Property(e => e.IncidenteId).HasColumnName("incidenteID");
            entity.Property(e => e.UnidadeId).HasColumnName("unidadeID");

            entity.HasOne(d => d.Incidente).WithMany(p => p.IncidenteUnidadeAtendimentos)
                .HasForeignKey(d => d.IncidenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Atendimento_Incidente");

            entity.HasOne(d => d.Unidade).WithMany(p => p.IncidenteUnidadeAtendimentos)
                .HasForeignKey(d => d.UnidadeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Atendimento_Unidade");
        });

        modelBuilder.Entity<IncidenteUnidadeAtribuidum>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AtribuidoEm)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("atribuidoEm");
            entity.Property(e => e.IncidenteId).HasColumnName("incidenteID");
            entity.Property(e => e.UnidadeId).HasColumnName("unidadeID");

            entity.HasOne(d => d.Incidente).WithMany(p => p.IncidenteUnidadeAtribuida)
                .HasForeignKey(d => d.IncidenteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Atribuida_Incidente");

            entity.HasOne(d => d.Unidade).WithMany(p => p.IncidenteUnidadeAtribuida)
                .HasForeignKey(d => d.UnidadeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Atribuida_Unidade");
        });

        modelBuilder.Entity<Severidade>(entity =>
        {
            entity.ToTable("Severidade");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Severidade1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("severidade");
        });

        modelBuilder.Entity<StatusChamadum>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.StatusChamada)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("statusChamada");
        });

        modelBuilder.Entity<StatusIncidente>(entity =>
        {
            entity.ToTable("StatusIncidente");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Incidente)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("incidente");
        });

        modelBuilder.Entity<StatusUnidade>(entity =>
        {
            entity.ToTable("StatusUnidade");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Unidade)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("unidade");
        });

        modelBuilder.Entity<TipoIncidente>(entity =>
        {
            entity.ToTable("TipoIncidente");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TipoIncidente1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tipoIncidente");
        });

        modelBuilder.Entity<TipoUnidade>(entity =>
        {
            entity.ToTable("TipoUnidade");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.TipoUnidade1)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("tipoUnidade");
        });

        modelBuilder.Entity<Unidade>(entity =>
        {
            entity.ToTable("Unidade");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AssignedIncidentId).HasColumnName("assignedIncidentId");
            entity.Property(e => e.CallSign)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("callSign");
            entity.Property(e => e.EstacaoId).HasColumnName("estacaoID");
            entity.Property(e => e.LocationId).HasColumnName("locationID");
            entity.Property(e => e.StatusUnitId).HasColumnName("statusUnitID");
            entity.Property(e => e.TypeUnitId).HasColumnName("typeUnitID");

            entity.HasOne(d => d.AssignedIncident).WithMany(p => p.Unidades)
                .HasForeignKey(d => d.AssignedIncidentId)
                .HasConstraintName("FK_Unidade_Incidente");

            entity.HasOne(d => d.Estacao).WithMany(p => p.Unidades)
                .HasForeignKey(d => d.EstacaoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Unidade_Estacao");

            entity.HasOne(d => d.Location).WithMany(p => p.Unidades)
                .HasForeignKey(d => d.LocationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Unidade_Cordenada");

            entity.HasOne(d => d.StatusUnit).WithMany(p => p.Unidades)
                .HasForeignKey(d => d.StatusUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Unidade_StatusUnidade");

            entity.HasOne(d => d.TypeUnit).WithMany(p => p.Unidades)
                .HasForeignKey(d => d.TypeUnitId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Unidade_TipoUnidade");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
