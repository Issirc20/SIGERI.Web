IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [CategoriasAmenaza] (
    [Id] uniqueidentifier NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_CategoriasAmenaza] PRIMARY KEY ([Id])
);

CREATE TABLE [ControlesMadurez] (
    [Id] uniqueidentifier NOT NULL,
    [Codigo] nvarchar(50) NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Funcion] int NOT NULL,
    [NivelActual] int NOT NULL,
    [NivelObjetivo] int NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_ControlesMadurez] PRIMARY KEY ([Id])
);

CREATE TABLE [Organizaciones] (
    [Id] uniqueidentifier NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Siglas] nvarchar(50) NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_Organizaciones] PRIMARY KEY ([Id])
);

CREATE TABLE [Usuarios] (
    [Id] uniqueidentifier NOT NULL,
    [Nombre] nvarchar(150) NOT NULL,
    [Apellido] nvarchar(150) NOT NULL,
    [Correo] nvarchar(256) NOT NULL,
    [PasswordHash] nvarchar(500) NOT NULL,
    [Rol] int NOT NULL,
    [Estado] bit NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_Usuarios] PRIMARY KEY ([Id])
);

CREATE TABLE [Vulnerabilidades] (
    [Id] uniqueidentifier NOT NULL,
    [Codigo] nvarchar(50) NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [SeveridadBase] nvarchar(50) NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_Vulnerabilidades] PRIMARY KEY ([Id])
);

CREATE TABLE [Amenazas] (
    [Id] uniqueidentifier NOT NULL,
    [Codigo] nvarchar(50) NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [CategoriaAmenazaId] uniqueidentifier NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_Amenazas] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Amenazas_CategoriasAmenaza_CategoriaAmenazaId] FOREIGN KEY ([CategoriaAmenazaId]) REFERENCES [CategoriasAmenaza] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Activos] (
    [Id] uniqueidentifier NOT NULL,
    [Codigo] nvarchar(50) NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [Tipo] int NOT NULL,
    [Propietario] nvarchar(150) NOT NULL,
    [Ubicacion] nvarchar(200) NOT NULL,
    [Criticidad] int NOT NULL,
    [Estado] bit NOT NULL,
    [OrganizacionId] uniqueidentifier NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_Activos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Activos_Organizaciones_OrganizacionId] FOREIGN KEY ([OrganizacionId]) REFERENCES [Organizaciones] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [DependenciasActivos] (
    [Id] uniqueidentifier NOT NULL,
    [ActivoOrigenId] uniqueidentifier NOT NULL,
    [ActivoDestinoId] uniqueidentifier NOT NULL,
    [TipoDependencia] int NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [CriticidadOperativa] int NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_DependenciasActivos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_DependenciasActivos_Activos_ActivoDestinoId] FOREIGN KEY ([ActivoDestinoId]) REFERENCES [Activos] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_DependenciasActivos_Activos_ActivoOrigenId] FOREIGN KEY ([ActivoOrigenId]) REFERENCES [Activos] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Evaluaciones] (
    [Id] uniqueidentifier NOT NULL,
    [Fecha] datetime2 NOT NULL,
    [Observaciones] nvarchar(1000) NOT NULL,
    [UsuarioId] uniqueidentifier NOT NULL,
    [ActivoId] uniqueidentifier NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_Evaluaciones] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Evaluaciones_Activos_ActivoId] FOREIGN KEY ([ActivoId]) REFERENCES [Activos] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Evaluaciones_Usuarios_UsuarioId] FOREIGN KEY ([UsuarioId]) REFERENCES [Usuarios] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [PerfilesActivosCriticos] (
    [Id] uniqueidentifier NOT NULL,
    [ActivoId] uniqueidentifier NOT NULL,
    [ImpactoReputacion] int NOT NULL,
    [ImpactoFinanciero] int NOT NULL,
    [ImpactoProductividad] int NOT NULL,
    [ImpactoLegal] int NOT NULL,
    [ImpactoSeguridadSsoma] int NOT NULL,
    [ContenedoresTecnicos] nvarchar(1000) NOT NULL,
    [ContenedoresFisicos] nvarchar(1000) NOT NULL,
    [ContenedoresHumanos] nvarchar(1000) NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_PerfilesActivosCriticos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PerfilesActivosCriticos_Activos_ActivoId] FOREIGN KEY ([ActivoId]) REFERENCES [Activos] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Riesgos] (
    [Id] uniqueidentifier NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [Probabilidad_Valor] int NOT NULL,
    [Probabilidad_Descripcion] nvarchar(200) NOT NULL,
    [Impacto_Valor] int NOT NULL,
    [Impacto_Descripcion] nvarchar(200) NOT NULL,
    [PuntajeRiesgo] int NOT NULL,
    [NivelRiesgo] nvarchar(50) NOT NULL,
    [Estado] int NOT NULL,
    [EvaluacionId] uniqueidentifier NOT NULL,
    [AmenazaId] uniqueidentifier NOT NULL,
    [VulnerabilidadId] uniqueidentifier NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_Riesgos] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Riesgos_Amenazas_AmenazaId] FOREIGN KEY ([AmenazaId]) REFERENCES [Amenazas] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Riesgos_Evaluaciones_EvaluacionId] FOREIGN KEY ([EvaluacionId]) REFERENCES [Evaluaciones] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Riesgos_Vulnerabilidades_VulnerabilidadId] FOREIGN KEY ([VulnerabilidadId]) REFERENCES [Vulnerabilidades] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [HistorialesRiesgoResidual] (
    [Id] uniqueidentifier NOT NULL,
    [RiesgoId] uniqueidentifier NOT NULL,
    [PuntajeOriginal] int NOT NULL,
    [PuntajeResidual] int NOT NULL,
    [FechaCalculo] datetime2 NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    CONSTRAINT [PK_HistorialesRiesgoResidual] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HistorialesRiesgoResidual_Riesgos_RiesgoId] FOREIGN KEY ([RiesgoId]) REFERENCES [Riesgos] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [PlanesTratamiento] (
    [Id] uniqueidentifier NOT NULL,
    [RiesgoId] uniqueidentifier NOT NULL,
    [Estrategia] nvarchar(200) NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [FechaInicio] datetime2 NOT NULL,
    [FechaTermino] datetime2 NOT NULL,
    [Estado] int NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_PlanesTratamiento] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_PlanesTratamiento_Riesgos_RiesgoId] FOREIGN KEY ([RiesgoId]) REFERENCES [Riesgos] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [Controles] (
    [Id] uniqueidentifier NOT NULL,
    [PlanTratamientoId] uniqueidentifier NOT NULL,
    [Codigo] nvarchar(50) NOT NULL,
    [Nombre] nvarchar(200) NOT NULL,
    [Descripcion] nvarchar(1000) NOT NULL,
    [MitigaProbabilidad] bit NOT NULL,
    [MitigaImpacto] bit NOT NULL,
    [FechaCreacion] datetime2 NOT NULL,
    [FechaActualizacion] datetime2 NULL,
    [Activo] bit NOT NULL DEFAULT CAST(1 AS bit),
    [CreadoPor] nvarchar(100) NOT NULL,
    [ActualizadoPor] nvarchar(100) NULL,
    CONSTRAINT [PK_Controles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Controles_PlanesTratamiento_PlanTratamientoId] FOREIGN KEY ([PlanTratamientoId]) REFERENCES [PlanesTratamiento] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Activos_Codigo] ON [Activos] ([Codigo]);

CREATE INDEX [IX_Activos_OrganizacionId] ON [Activos] ([OrganizacionId]);

CREATE INDEX [IX_Amenazas_CategoriaAmenazaId] ON [Amenazas] ([CategoriaAmenazaId]);

CREATE UNIQUE INDEX [IX_Amenazas_Codigo] ON [Amenazas] ([Codigo]);

CREATE INDEX [IX_Controles_PlanTratamientoId] ON [Controles] ([PlanTratamientoId]);

CREATE UNIQUE INDEX [IX_ControlesMadurez_Codigo] ON [ControlesMadurez] ([Codigo]);

CREATE INDEX [IX_DependenciasActivos_ActivoDestinoId] ON [DependenciasActivos] ([ActivoDestinoId]);

CREATE INDEX [IX_DependenciasActivos_ActivoOrigenId] ON [DependenciasActivos] ([ActivoOrigenId]);

CREATE INDEX [IX_Evaluaciones_ActivoId] ON [Evaluaciones] ([ActivoId]);

CREATE INDEX [IX_Evaluaciones_UsuarioId] ON [Evaluaciones] ([UsuarioId]);

CREATE INDEX [IX_HistorialesRiesgoResidual_RiesgoId] ON [HistorialesRiesgoResidual] ([RiesgoId]);

CREATE INDEX [IX_PerfilesActivosCriticos_ActivoId] ON [PerfilesActivosCriticos] ([ActivoId]);

CREATE INDEX [IX_PlanesTratamiento_RiesgoId] ON [PlanesTratamiento] ([RiesgoId]);

CREATE INDEX [IX_Riesgos_AmenazaId] ON [Riesgos] ([AmenazaId]);

CREATE INDEX [IX_Riesgos_EvaluacionId] ON [Riesgos] ([EvaluacionId]);

CREATE INDEX [IX_Riesgos_VulnerabilidadId] ON [Riesgos] ([VulnerabilidadId]);

CREATE UNIQUE INDEX [IX_Usuarios_Correo] ON [Usuarios] ([Correo]);

CREATE UNIQUE INDEX [IX_Vulnerabilidades_Codigo] ON [Vulnerabilidades] ([Codigo]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260708145734_InitialCreate', N'10.0.9');

COMMIT;
GO

BEGIN TRANSACTION;
ALTER TABLE [PlanesTratamiento] ADD [AleBase] decimal(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE [PlanesTratamiento] ADD [CostoSalvaguarda] decimal(18,2) NOT NULL DEFAULT 0.0;

ALTER TABLE [PlanesTratamiento] ADD [PorcentajeMitigacion] decimal(5,2) NOT NULL DEFAULT 0.0;

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260708151739_AddTratamientoFinancialFields', N'10.0.9');

COMMIT;
GO

