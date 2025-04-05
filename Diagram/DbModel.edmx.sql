
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 04/04/2025 19:51:41
-- Generated from EDMX file: C:\Users\User\Desktop\ModelSecurity\Diagram\DbModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [SecurityPQRDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'forms'
CREATE TABLE [dbo].[forms] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Active] bit  NOT NULL,
    [CreateAt] datetime  NOT NULL,
    [DeleteAt] datetime  NOT NULL
);
GO

-- Creating table 'Models'
CREATE TABLE [dbo].[Models] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Active] nvarchar(max)  NOT NULL,
    [CreateAt] datetime  NOT NULL,
    [DeleteAt] datetime  NOT NULL
);
GO

-- Creating table 'FormsModels'
CREATE TABLE [dbo].[FormsModels] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [FormId_Id] int  NOT NULL,
    [ModuleId_Id] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [CreateAt] datetime  NOT NULL,
    [DeleteAt] datetime  NOT NULL
);
GO

-- Creating table 'Rols'
CREATE TABLE [dbo].[Rols] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [CreateAt] datetime  NOT NULL,
    [DeleteAt] datetime  NOT NULL
);
GO

-- Creating table 'Rols_Users'
CREATE TABLE [dbo].[Rols_Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RolId_Id] int  NOT NULL,
    [UserId_Id] int  NOT NULL
);
GO

-- Creating table 'Permissions'
CREATE TABLE [dbo].[Permissions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Can_Read] nvarchar(max)  NOT NULL,
    [Can_Create] nvarchar(max)  NOT NULL,
    [Can_Update] nvarchar(max)  NOT NULL,
    [Can_Delete] nvarchar(max)  NOT NULL,
    [CreateAt] datetime  NOT NULL,
    [DeleteAt] datetime  NOT NULL
);
GO

-- Creating table 'Rol_Form_PermissionSet'
CREATE TABLE [dbo].[Rol_Form_PermissionSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [RolId_Id] int  NOT NULL,
    [FormId_Id] int  NOT NULL,
    [PermissionId_Id] int  NOT NULL
);
GO

-- Creating table 'Change_LogSet'
CREATE TABLE [dbo].[Change_LogSet] (
    [Id] int IDENTITY(1,1) NOT NULL
);
GO

-- Creating table 'Workers'
CREATE TABLE [dbo].[Workers] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [First_Name] nvarchar(max)  NOT NULL,
    [Last_Name] nvarchar(max)  NOT NULL,
    [Identity_Document] nvarchar(max)  NOT NULL,
    [Job_Title] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Hire_Date] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Clients'
CREATE TABLE [dbo].[Clients] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [First_Name] nvarchar(max)  NOT NULL,
    [Last_Name] nvarchar(max)  NOT NULL,
    [Identity_Document] nvarchar(max)  NOT NULL,
    [Client_Type] nvarchar(max)  NOT NULL,
    [Phone] nvarchar(max)  NOT NULL,
    [Email] nvarchar(max)  NOT NULL,
    [Address] nvarchar(max)  NOT NULL,
    [Socioeconomic_Stratification] nvarchar(max)  NOT NULL,
    [Registration_Date] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'PQRSet'
CREATE TABLE [dbo].[PQRSet] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PQR_Type] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Creation_Date] nvarchar(max)  NOT NULL,
    [PQR_Status] nvarchar(max)  NOT NULL,
    [Resolution_Date] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Logins'
CREATE TABLE [dbo].[Logins] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Workers_Logins'
CREATE TABLE [dbo].[Workers_Logins] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL,
    [Creation_Date] nvarchar(max)  NOT NULL,
    [Status] nvarchar(max)  NOT NULL,
    [Login_Id] int  NOT NULL,
    [Worker_Id] int  NOT NULL
);
GO

-- Creating table 'PQRWorker'
CREATE TABLE [dbo].[PQRWorker] (
    [PQR_Id] int  NOT NULL,
    [Worker_Id] int  NOT NULL
);
GO

-- Creating table 'PQRClient'
CREATE TABLE [dbo].[PQRClient] (
    [PQR_Id] int  NOT NULL,
    [Client_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'forms'
ALTER TABLE [dbo].[forms]
ADD CONSTRAINT [PK_forms]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Models'
ALTER TABLE [dbo].[Models]
ADD CONSTRAINT [PK_Models]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'FormsModels'
ALTER TABLE [dbo].[FormsModels]
ADD CONSTRAINT [PK_FormsModels]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Rols'
ALTER TABLE [dbo].[Rols]
ADD CONSTRAINT [PK_Rols]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Rols_Users'
ALTER TABLE [dbo].[Rols_Users]
ADD CONSTRAINT [PK_Rols_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Permissions'
ALTER TABLE [dbo].[Permissions]
ADD CONSTRAINT [PK_Permissions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Rol_Form_PermissionSet'
ALTER TABLE [dbo].[Rol_Form_PermissionSet]
ADD CONSTRAINT [PK_Rol_Form_PermissionSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Change_LogSet'
ALTER TABLE [dbo].[Change_LogSet]
ADD CONSTRAINT [PK_Change_LogSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Workers'
ALTER TABLE [dbo].[Workers]
ADD CONSTRAINT [PK_Workers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Clients'
ALTER TABLE [dbo].[Clients]
ADD CONSTRAINT [PK_Clients]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PQRSet'
ALTER TABLE [dbo].[PQRSet]
ADD CONSTRAINT [PK_PQRSet]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Logins'
ALTER TABLE [dbo].[Logins]
ADD CONSTRAINT [PK_Logins]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Workers_Logins'
ALTER TABLE [dbo].[Workers_Logins]
ADD CONSTRAINT [PK_Workers_Logins]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [PQR_Id], [Worker_Id] in table 'PQRWorker'
ALTER TABLE [dbo].[PQRWorker]
ADD CONSTRAINT [PK_PQRWorker]
    PRIMARY KEY CLUSTERED ([PQR_Id], [Worker_Id] ASC);
GO

-- Creating primary key on [PQR_Id], [Client_Id] in table 'PQRClient'
ALTER TABLE [dbo].[PQRClient]
ADD CONSTRAINT [PK_PQRClient]
    PRIMARY KEY CLUSTERED ([PQR_Id], [Client_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [FormId_Id] in table 'FormsModels'
ALTER TABLE [dbo].[FormsModels]
ADD CONSTRAINT [FK_FormModelForm]
    FOREIGN KEY ([FormId_Id])
    REFERENCES [dbo].[forms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormModelForm'
CREATE INDEX [IX_FK_FormModelForm]
ON [dbo].[FormsModels]
    ([FormId_Id]);
GO

-- Creating foreign key on [ModuleId_Id] in table 'FormsModels'
ALTER TABLE [dbo].[FormsModels]
ADD CONSTRAINT [FK_FormModuleModule]
    FOREIGN KEY ([ModuleId_Id])
    REFERENCES [dbo].[Models]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_FormModuleModule'
CREATE INDEX [IX_FK_FormModuleModule]
ON [dbo].[FormsModels]
    ([ModuleId_Id]);
GO

-- Creating foreign key on [RolId_Id] in table 'Rols_Users'
ALTER TABLE [dbo].[Rols_Users]
ADD CONSTRAINT [FK_Rol_UserRol]
    FOREIGN KEY ([RolId_Id])
    REFERENCES [dbo].[Rols]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Rol_UserRol'
CREATE INDEX [IX_FK_Rol_UserRol]
ON [dbo].[Rols_Users]
    ([RolId_Id]);
GO

-- Creating foreign key on [UserId_Id] in table 'Rols_Users'
ALTER TABLE [dbo].[Rols_Users]
ADD CONSTRAINT [FK_Rol_UserUser]
    FOREIGN KEY ([UserId_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Rol_UserUser'
CREATE INDEX [IX_FK_Rol_UserUser]
ON [dbo].[Rols_Users]
    ([UserId_Id]);
GO

-- Creating foreign key on [RolId_Id] in table 'Rol_Form_PermissionSet'
ALTER TABLE [dbo].[Rol_Form_PermissionSet]
ADD CONSTRAINT [FK_Rol_Form_PermissionRol]
    FOREIGN KEY ([RolId_Id])
    REFERENCES [dbo].[Rols]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Rol_Form_PermissionRol'
CREATE INDEX [IX_FK_Rol_Form_PermissionRol]
ON [dbo].[Rol_Form_PermissionSet]
    ([RolId_Id]);
GO

-- Creating foreign key on [FormId_Id] in table 'Rol_Form_PermissionSet'
ALTER TABLE [dbo].[Rol_Form_PermissionSet]
ADD CONSTRAINT [FK_Rol_Form_PermissionForm]
    FOREIGN KEY ([FormId_Id])
    REFERENCES [dbo].[forms]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Rol_Form_PermissionForm'
CREATE INDEX [IX_FK_Rol_Form_PermissionForm]
ON [dbo].[Rol_Form_PermissionSet]
    ([FormId_Id]);
GO

-- Creating foreign key on [PermissionId_Id] in table 'Rol_Form_PermissionSet'
ALTER TABLE [dbo].[Rol_Form_PermissionSet]
ADD CONSTRAINT [FK_Rol_Form_PermissionPermission]
    FOREIGN KEY ([PermissionId_Id])
    REFERENCES [dbo].[Permissions]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Rol_Form_PermissionPermission'
CREATE INDEX [IX_FK_Rol_Form_PermissionPermission]
ON [dbo].[Rol_Form_PermissionSet]
    ([PermissionId_Id]);
GO

-- Creating foreign key on [Login_Id] in table 'Workers_Logins'
ALTER TABLE [dbo].[Workers_Logins]
ADD CONSTRAINT [FK_Worker_LoginLogin]
    FOREIGN KEY ([Login_Id])
    REFERENCES [dbo].[Logins]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Worker_LoginLogin'
CREATE INDEX [IX_FK_Worker_LoginLogin]
ON [dbo].[Workers_Logins]
    ([Login_Id]);
GO

-- Creating foreign key on [Worker_Id] in table 'Workers_Logins'
ALTER TABLE [dbo].[Workers_Logins]
ADD CONSTRAINT [FK_Worker_LoginWorker]
    FOREIGN KEY ([Worker_Id])
    REFERENCES [dbo].[Workers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Worker_LoginWorker'
CREATE INDEX [IX_FK_Worker_LoginWorker]
ON [dbo].[Workers_Logins]
    ([Worker_Id]);
GO

-- Creating foreign key on [PQR_Id] in table 'PQRWorker'
ALTER TABLE [dbo].[PQRWorker]
ADD CONSTRAINT [FK_PQRWorker_PQR]
    FOREIGN KEY ([PQR_Id])
    REFERENCES [dbo].[PQRSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Worker_Id] in table 'PQRWorker'
ALTER TABLE [dbo].[PQRWorker]
ADD CONSTRAINT [FK_PQRWorker_Worker]
    FOREIGN KEY ([Worker_Id])
    REFERENCES [dbo].[Workers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PQRWorker_Worker'
CREATE INDEX [IX_FK_PQRWorker_Worker]
ON [dbo].[PQRWorker]
    ([Worker_Id]);
GO

-- Creating foreign key on [PQR_Id] in table 'PQRClient'
ALTER TABLE [dbo].[PQRClient]
ADD CONSTRAINT [FK_PQRClient_PQR]
    FOREIGN KEY ([PQR_Id])
    REFERENCES [dbo].[PQRSet]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Client_Id] in table 'PQRClient'
ALTER TABLE [dbo].[PQRClient]
ADD CONSTRAINT [FK_PQRClient_Client]
    FOREIGN KEY ([Client_Id])
    REFERENCES [dbo].[Clients]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PQRClient_Client'
CREATE INDEX [IX_FK_PQRClient_Client]
ON [dbo].[PQRClient]
    ([Client_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------