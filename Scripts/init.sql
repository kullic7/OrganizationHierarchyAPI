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
CREATE TABLE [Companies] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Code] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
);

CREATE TABLE [Employees] (
    [Id] int NOT NULL IDENTITY,
    [CompanyId] int NOT NULL,
    [Title] nvarchar(max) NULL,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Phone] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Employees] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Employees_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION
);

CREATE TABLE [OrganizationUnits] (
    [Id] int NOT NULL IDENTITY,
    [CompanyId] int NOT NULL,
    [ParentId] int NULL,
    [ManagerId] int NULL,
    [Name] nvarchar(max) NOT NULL,
    [Code] nvarchar(450) NOT NULL,
    [Type] int NOT NULL,
    CONSTRAINT [PK_OrganizationUnits] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_OrganizationUnits_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OrganizationUnits_Employees_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [Employees] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_OrganizationUnits_OrganizationUnits_ParentId] FOREIGN KEY ([ParentId]) REFERENCES [OrganizationUnits] ([Id]) ON DELETE NO ACTION
);

CREATE UNIQUE INDEX [IX_Companies_Code] ON [Companies] ([Code]);

CREATE INDEX [IX_Employees_CompanyId] ON [Employees] ([CompanyId]);

CREATE UNIQUE INDEX [IX_OrganizationUnits_CompanyId_Code] ON [OrganizationUnits] ([CompanyId], [Code]);

CREATE INDEX [IX_OrganizationUnits_ManagerId] ON [OrganizationUnits] ([ManagerId]);

CREATE INDEX [IX_OrganizationUnits_ParentId] ON [OrganizationUnits] ([ParentId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260507173811_InitialCreate', N'10.0.7');

COMMIT;
GO

