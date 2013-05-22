
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 04/04/2013 21:19:47
-- Generated from EDMX file: c:\users\mic_3_000\documents\visual studio 2012\Projects\WindowsFormsApplication1\WindowsFormsApplication1\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;

IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');


-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[rentit27].[fk_ActionGroup_has_AllowedAction_ActionGroup1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[actiongroup_has_allowedaction] DROP CONSTRAINT [fk_ActionGroup_has_AllowedAction_ActionGroup1];

IF OBJECT_ID(N'[rentit27].[fk_ActionGroup_has_AllowedAction_AllowedAction1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[actiongroup_has_allowedaction] DROP CONSTRAINT [fk_ActionGroup_has_AllowedAction_AllowedAction1];

IF OBJECT_ID(N'[rentit27].[fk_Log_LogEntryType1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[log] DROP CONSTRAINT [fk_Log_LogEntryType1];

IF OBJECT_ID(N'[rentit27].[fk_Log_Loggable1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[log] DROP CONSTRAINT [fk_Log_Loggable1];

IF OBJECT_ID(N'[rentit27].[fk_Log_Loggable2]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[log] DROP CONSTRAINT [fk_Log_Loggable2];

IF OBJECT_ID(N'[rentit27].[fk_MetaData_MetaDataType1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[metadata] DROP CONSTRAINT [fk_MetaData_MetaDataType1];

IF OBJECT_ID(N'[rentit27].[fk_MetaData_Product1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[metadata] DROP CONSTRAINT [fk_MetaData_Product1];

IF OBJECT_ID(N'[rentit27].[fk_MimeTypes_ProductType1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[mimetype] DROP CONSTRAINT [fk_MimeTypes_ProductType1];

IF OBJECT_ID(N'[rentit27].[fk_Product_has_ActionGroup_ActionGroup1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[product_has_allowedaction] DROP CONSTRAINT [fk_Product_has_ActionGroup_ActionGroup1];

IF OBJECT_ID(N'[rentit27].[fk_Product_has_ActionGroup_Product1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[product_has_allowedaction] DROP CONSTRAINT [fk_Product_has_ActionGroup_Product1];

IF OBJECT_ID(N'[rentit27].[fk_Product_has_User_Product1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[productrating] DROP CONSTRAINT [fk_Product_has_User_Product1];

IF OBJECT_ID(N'[rentit27].[fk_Product_has_User_Product2]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[transaction] DROP CONSTRAINT [fk_Product_has_User_Product2];

IF OBJECT_ID(N'[rentit27].[fk_Product_has_User_User2]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[transaction] DROP CONSTRAINT [fk_Product_has_User_User2];

IF OBJECT_ID(N'[rentit27].[fk_Product_Loggable1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[product] DROP CONSTRAINT [fk_Product_Loggable1];

IF OBJECT_ID(N'[rentit27].[fk_Product_ProductType1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[product] DROP CONSTRAINT [fk_Product_ProductType1];

IF OBJECT_ID(N'[rentit27].[fk_Type_has_ActionGroup_ActionGroup1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[usertype_has_actiongroup] DROP CONSTRAINT [fk_Type_has_ActionGroup_ActionGroup1];

IF OBJECT_ID(N'[rentit27].[fk_Type_has_ActionGroup_Type1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[usertype_has_actiongroup] DROP CONSTRAINT [fk_Type_has_ActionGroup_Type1];

IF OBJECT_ID(N'[rentit27].[fk_User_Country1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[user] DROP CONSTRAINT [fk_User_Country1];

IF OBJECT_ID(N'[rentit27].[fk_User_Loggable1]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[user] DROP CONSTRAINT [fk_User_Loggable1];

IF OBJECT_ID(N'[rentit27].[fk_User_Type]', 'F') IS NOT NULL
    ALTER TABLE [rentit27].[user] DROP CONSTRAINT [fk_User_Type];


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[rentit27].[actiongroup]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[actiongroup];

IF OBJECT_ID(N'[rentit27].[actiongroup_has_allowedaction]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[actiongroup_has_allowedaction];

IF OBJECT_ID(N'[rentit27].[allowedaction]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[allowedaction];

IF OBJECT_ID(N'[rentit27].[country]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[country];

IF OBJECT_ID(N'[rentit27].[log]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[log];

IF OBJECT_ID(N'[rentit27].[logentrytype]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[logentrytype];

IF OBJECT_ID(N'[rentit27].[loggable]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[loggable];

IF OBJECT_ID(N'[rentit27].[metadata]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[metadata];

IF OBJECT_ID(N'[rentit27].[metadatatype]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[metadatatype];

IF OBJECT_ID(N'[rentit27].[mimetype]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[mimetype];

IF OBJECT_ID(N'[rentit27].[product]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[product];

IF OBJECT_ID(N'[rentit27].[product_has_allowedaction]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[product_has_allowedaction];

IF OBJECT_ID(N'[rentit27].[productrating]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[productrating];

IF OBJECT_ID(N'[rentit27].[producttype]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[producttype];

IF OBJECT_ID(N'[rentit27].[transaction]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[transaction];

IF OBJECT_ID(N'[rentit27].[user]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[user];

IF OBJECT_ID(N'[rentit27].[usertype]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[usertype];

IF OBJECT_ID(N'[rentit27].[usertype_has_actiongroup]', 'U') IS NOT NULL
    DROP TABLE [rentit27].[usertype_has_actiongroup];


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'actiongroup'
CREATE TABLE [dbo].[actiongroup] (
    [Name] varchar(45)  NOT NULL,
    [Description] text  NULL
);


-- Creating table 'allowedaction'
CREATE TABLE [dbo].[allowedaction] (
    [Name] varchar(45)  NOT NULL,
    [Description] text  NULL
);


-- Creating table 'country'
CREATE TABLE [dbo].[country] (
    [Name] varchar(74)  NOT NULL
);


-- Creating table 'log'
CREATE TABLE [dbo].[log] (
    [Time] datetime  NOT NULL,
    [ChangeFrom] varchar(45)  NULL,
    [ChangeTo] varchar(45)  NULL,
    [LogEntryType_name] varchar(45)  NOT NULL,
    [From_Loggable_id] int  NOT NULL,
    [To_Loggable_id] int  NOT NULL
);


-- Creating table 'logentrytype'
CREATE TABLE [dbo].[logentrytype] (
    [Name] varchar(45)  NOT NULL
);


-- Creating table 'loggable'
CREATE TABLE [dbo].[loggable] (
    [Id] int IDENTITY(1,1) NOT NULL
);


-- Creating table 'metadata'
CREATE TABLE [dbo].[metadata] (
    [Content] varchar(45)  NULL,
    [MetaDataType_Name] varchar(45)  NOT NULL,
    [Product_Id] int  NOT NULL
);


-- Creating table 'metadatatype'
CREATE TABLE [dbo].[metadatatype] (
    [Name] varchar(45)  NOT NULL
);


-- Creating table 'mimetype'
CREATE TABLE [dbo].[mimetype] (
    [ProductType_Name] varchar(45)  NOT NULL,
    [Type] varchar(45)  NOT NULL
);


-- Creating table 'product'
CREATE TABLE [dbo].[product] (
    [Id] int  NOT NULL,
    [Name] varchar(1000)  NULL,
    [Description] text  NULL,
    [Rent_price] int  NULL,
    [Buy_price] int  NULL,
    [ProductType_Name] varchar(45)  NOT NULL,
    [User_Username] varchar(100)  NOT NULL,
    [Published] bit  NOT NULL,
    [Created_date] datetime  NULL
);


-- Creating table 'productrating'
CREATE TABLE [dbo].[productrating] (
    [Product_Id] int  NOT NULL,
    [User_Username] varchar(100)  NOT NULL,
    [Rating] smallint  NOT NULL
);


-- Creating table 'producttype'
CREATE TABLE [dbo].[producttype] (
    [Name] varchar(45)  NOT NULL
);


-- Creating table 'transaction'
CREATE TABLE [dbo].[transaction] (
    [Id] int NOT NULL IDENTITY(1,1),
    [Product_Id] int  NOT NULL,
    [User_Username] varchar(100)  NOT NULL,
    [Purchased_Date] datetime  NULL,
    [Expires_Date] datetime  NULL,
    [Price_Paid] int  NULL,
    [Type] varchar(1) NOT NULL DEFAULT "B"
);


-- Creating table 'user'
CREATE TABLE [dbo].[user] (
    [Id] int  NOT NULL,
    [Username] varchar(100)  NOT NULL UNIQUE,
    [Email] varchar(254)  NOT NULL,
    [Address] varchar(1000)  NULL,
    [Date_of_birth] datetime  NULL,
    [Password] varchar(133)  NOT NULL,
    [Created_date] datetime  NULL,
    [Banned] tinyint  NULL,
    [About_me] text  NULL,
    [Type_name] varchar(45)  NOT NULL,
    [Balance] int  NULL,
    [Zipcode] int  NULL,
    [Country_Name] varchar(74)  NULL,
    [Name] varchar(1000)  NULL,
    [Usercol] varchar(45)  NULL
);


-- Creating table 'usertype'
CREATE TABLE [dbo].[usertype] (
    [Name] varchar(45)  NOT NULL
);


-- Creating table 'actiongroup_has_allowedaction'
CREATE TABLE [dbo].[actiongroup_has_allowedaction] (
    [actiongroup_Name] varchar(45)  NOT NULL,
    [allowedaction_Name] varchar(45)  NOT NULL
);


-- Creating table 'product_has_allowedaction'
CREATE TABLE [dbo].[product_has_allowedaction] (
    [allowedaction_Name] varchar(45)  NOT NULL,
    [product_Id] int  NOT NULL
);


-- Creating table 'usertype_has_actiongroup'
CREATE TABLE [dbo].[usertype_has_actiongroup] (
    [actiongroup_Name] varchar(45)  NOT NULL,
    [usertype_Name] varchar(45)  NOT NULL
);


-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Name] in table 'actiongroup'
ALTER TABLE [dbo].[actiongroup]
ADD CONSTRAINT [PK_actiongroup]
    PRIMARY KEY CLUSTERED ([Name] ASC);


-- Creating primary key on [Name] in table 'allowedaction'
ALTER TABLE [dbo].[allowedaction]
ADD CONSTRAINT [PK_allowedaction]
    PRIMARY KEY CLUSTERED ([Name] ASC);


-- Creating primary key on [Name] in table 'country'
ALTER TABLE [dbo].[country]
ADD CONSTRAINT [PK_country]
    PRIMARY KEY CLUSTERED ([Name] ASC);


-- Creating primary key on [Time], [From_Loggable_id], [To_Loggable_id] in table 'log'
ALTER TABLE [dbo].[log]
ADD CONSTRAINT [PK_log]
    PRIMARY KEY CLUSTERED ([Time], [From_Loggable_id], [To_Loggable_id] ASC);


-- Creating primary key on [Name] in table 'logentrytype'
ALTER TABLE [dbo].[logentrytype]
ADD CONSTRAINT [PK_logentrytype]
    PRIMARY KEY CLUSTERED ([Name] ASC);


-- Creating primary key on [Id] in table 'loggable'
ALTER TABLE [dbo].[loggable]
ADD CONSTRAINT [PK_loggable]
    PRIMARY KEY CLUSTERED ([Id] ASC);


-- Creating primary key on [MetaDataType_Name], [Product_Id] in table 'metadata'
ALTER TABLE [dbo].[metadata]
ADD CONSTRAINT [PK_metadata]
    PRIMARY KEY CLUSTERED ([MetaDataType_Name], [Product_Id] ASC);


-- Creating primary key on [Name] in table 'metadatatype'
ALTER TABLE [dbo].[metadatatype]
ADD CONSTRAINT [PK_metadatatype]
    PRIMARY KEY CLUSTERED ([Name] ASC);


-- Creating primary key on [ProductType_Name] in table 'mimetype'
ALTER TABLE [dbo].[mimetype]
ADD CONSTRAINT [PK_mimetype]
    PRIMARY KEY CLUSTERED ([ProductType_Name], [Type] ASC);


-- Creating primary key on [Id] in table 'product'
ALTER TABLE [dbo].[product]
ADD CONSTRAINT [PK_product]
    PRIMARY KEY CLUSTERED ([Id] ASC);


-- Creating primary key on [Product_Id], [User_Username] in table 'productrating'
ALTER TABLE [dbo].[productrating]
ADD CONSTRAINT [PK_productrating]
    PRIMARY KEY CLUSTERED ([Product_Id], [User_Username] ASC);


-- Creating primary key on [Name] in table 'producttype'
ALTER TABLE [dbo].[producttype]
ADD CONSTRAINT [PK_producttype]
    PRIMARY KEY CLUSTERED ([Name] ASC);


-- Creating primary key on [Id] in table 'transaction'
ALTER TABLE [dbo].[transaction]
ADD CONSTRAINT [PK_transaction]
    PRIMARY KEY CLUSTERED ([Id] ASC);


-- Creating primary key on [Id] in table 'user'
ALTER TABLE [dbo].[user]
ADD CONSTRAINT [PK_user]
    PRIMARY KEY CLUSTERED ([Id] ASC);


-- Creating primary key on [Name] in table 'usertype'
ALTER TABLE [dbo].[usertype]
ADD CONSTRAINT [PK_usertype]
    PRIMARY KEY CLUSTERED ([Name] ASC);


-- Creating primary key on [actiongroup_Name], [allowedaction_Name] in table 'actiongroup_has_allowedaction'
ALTER TABLE [dbo].[actiongroup_has_allowedaction]
ADD CONSTRAINT [PK_actiongroup_has_allowedaction]
    PRIMARY KEY NONCLUSTERED ([actiongroup_Name], [allowedaction_Name] ASC);


-- Creating primary key on [allowedaction_Name], [product_Id] in table 'product_has_allowedaction'
ALTER TABLE [dbo].[product_has_allowedaction]
ADD CONSTRAINT [PK_product_has_allowedaction]
    PRIMARY KEY NONCLUSTERED ([allowedaction_Name], [product_Id] ASC);


-- Creating primary key on [actiongroup_Name], [usertype_Name] in table 'usertype_has_actiongroup'
ALTER TABLE [dbo].[usertype_has_actiongroup]
ADD CONSTRAINT [PK_usertype_has_actiongroup]
    PRIMARY KEY NONCLUSTERED ([actiongroup_Name], [usertype_Name] ASC);


-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Country_Name] in table 'user'
ALTER TABLE [dbo].[user]
ADD CONSTRAINT [fk_User_Country1]
    FOREIGN KEY ([Country_Name])
    REFERENCES [dbo].[country]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_User_Country1'
CREATE INDEX [IX_fk_User_Country1]
ON [dbo].[user]
    ([Country_Name]);


-- Creating foreign key on [LogEntryType_name] in table 'log'
ALTER TABLE [dbo].[log]
ADD CONSTRAINT [fk_Log_LogEntryType1]
    FOREIGN KEY ([LogEntryType_name])
    REFERENCES [dbo].[logentrytype]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_Log_LogEntryType1'
CREATE INDEX [IX_fk_Log_LogEntryType1]
ON [dbo].[log]
    ([LogEntryType_name]);


-- Creating foreign key on [From_Loggable_id] in table 'log'
ALTER TABLE [dbo].[log]
ADD CONSTRAINT [fk_Log_Loggable1]
    FOREIGN KEY ([From_Loggable_id])
    REFERENCES [dbo].[loggable]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_Log_Loggable1'
CREATE INDEX [IX_fk_Log_Loggable1]
ON [dbo].[log]
    ([From_Loggable_id]);


-- Creating foreign key on [To_Loggable_id] in table 'log'
ALTER TABLE [dbo].[log]
ADD CONSTRAINT [fk_Log_Loggable2]
    FOREIGN KEY ([To_Loggable_id])
    REFERENCES [dbo].[loggable]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_Log_Loggable2'
CREATE INDEX [IX_fk_Log_Loggable2]
ON [dbo].[log]
    ([To_Loggable_id]);


-- Creating foreign key on [Id] in table 'product'
ALTER TABLE [dbo].[product]
ADD CONSTRAINT [fk_Product_Loggable1]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[loggable]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [Id] in table 'user'
ALTER TABLE [dbo].[user]
ADD CONSTRAINT [fk_User_Loggable1]
    FOREIGN KEY ([Id])
    REFERENCES [dbo].[loggable]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [MetaDataType_Name] in table 'metadata'
ALTER TABLE [dbo].[metadata]
ADD CONSTRAINT [fk_MetaData_MetaDataType1]
    FOREIGN KEY ([MetaDataType_Name])
    REFERENCES [dbo].[metadatatype]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [Product_Id] in table 'metadata'
ALTER TABLE [dbo].[metadata]
ADD CONSTRAINT [fk_MetaData_Product1]
    FOREIGN KEY ([Product_Id])
    REFERENCES [dbo].[product]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_MetaData_Product1'
CREATE INDEX [IX_fk_MetaData_Product1]
ON [dbo].[metadata]
    ([Product_Id]);


-- Creating foreign key on [ProductType_Name] in table 'mimetype'
ALTER TABLE [dbo].[mimetype]
ADD CONSTRAINT [fk_MimeTypes_ProductType1]
    FOREIGN KEY ([ProductType_Name])
    REFERENCES [dbo].[producttype]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [Product_Id] in table 'productrating'
ALTER TABLE [dbo].[productrating]
ADD CONSTRAINT [fk_Product_has_User_Product1]
    FOREIGN KEY ([Product_Id])
    REFERENCES [dbo].[product]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [Product_Id] in table 'productrating'
ALTER TABLE [dbo].[productrating]
ADD CONSTRAINT [fk_ProductRating_has_User_User_Username]
    FOREIGN KEY ([User_Username])
    REFERENCES [dbo].[user]
        ([Username])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [Product_Id] in table 'transaction'
ALTER TABLE [dbo].[transaction]
ADD CONSTRAINT [fk_Product_has_User_Product2]
    FOREIGN KEY ([Product_Id])
    REFERENCES [dbo].[product]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [ProductType_Name] in table 'product'
ALTER TABLE [dbo].[product]
ADD CONSTRAINT [fk_Product_ProductType1]
    FOREIGN KEY ([ProductType_Name])
    REFERENCES [dbo].[producttype]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_Product_ProductType1'
CREATE INDEX [IX_fk_Product_ProductType1]
ON [dbo].[product]
    ([ProductType_Name]);


-- Creating foreign key on [Product_Id] in table 'productrating'
ALTER TABLE [dbo].[product]
ADD CONSTRAINT [fk_Product_has_User_User_Username]
    FOREIGN KEY ([User_Username])
    REFERENCES [dbo].[user]
        ([Username])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [User_Username] in table 'transaction'
ALTER TABLE [dbo].[transaction]
ADD CONSTRAINT [fk_Product_has_User_User2]
    FOREIGN KEY ([User_Username])
    REFERENCES [dbo].[user]
        ([Username])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating foreign key on [Type_name] in table 'user'
ALTER TABLE [dbo].[user]
ADD CONSTRAINT [fk_User_Type]
    FOREIGN KEY ([Type_name])
    REFERENCES [dbo].[usertype]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'fk_User_Type'
CREATE INDEX [IX_fk_User_Type]
ON [dbo].[user]
    ([Type_name]);


-- Creating foreign key on [actiongroup_Name] in table 'actiongroup_has_allowedaction'
ALTER TABLE [dbo].[actiongroup_has_allowedaction]
ADD CONSTRAINT [FK_actiongroup_has_allowedaction_actiongroup]
    FOREIGN KEY ([actiongroup_Name])
    REFERENCES [dbo].[actiongroup]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [allowedaction_Name] in table 'actiongroup_has_allowedaction'
ALTER TABLE [dbo].[actiongroup_has_allowedaction]
ADD CONSTRAINT [FK_actiongroup_has_allowedaction_allowedaction]
    FOREIGN KEY ([allowedaction_Name])
    REFERENCES [dbo].[allowedaction]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_actiongroup_has_allowedaction_allowedaction'
CREATE INDEX [IX_FK_actiongroup_has_allowedaction_allowedaction]
ON [dbo].[actiongroup_has_allowedaction]
    ([allowedaction_Name]);


-- Creating foreign key on [allowedaction_Name] in table 'product_has_allowedaction'
ALTER TABLE [dbo].[product_has_allowedaction]
ADD CONSTRAINT [FK_product_has_allowedaction_allowedaction]
    FOREIGN KEY ([allowedaction_Name])
    REFERENCES [dbo].[allowedaction]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [product_Id] in table 'product_has_allowedaction'
ALTER TABLE [dbo].[product_has_allowedaction]
ADD CONSTRAINT [FK_product_has_allowedaction_product]
    FOREIGN KEY ([product_Id])
    REFERENCES [dbo].[product]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_product_has_allowedaction_product'
CREATE INDEX [IX_FK_product_has_allowedaction_product]
ON [dbo].[product_has_allowedaction]
    ([product_Id]);


-- Creating foreign key on [actiongroup_Name] in table 'usertype_has_actiongroup'
ALTER TABLE [dbo].[usertype_has_actiongroup]
ADD CONSTRAINT [FK_usertype_has_actiongroup_actiongroup]
    FOREIGN KEY ([actiongroup_Name])
    REFERENCES [dbo].[actiongroup]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;


-- Creating foreign key on [usertype_Name] in table 'usertype_has_actiongroup'
ALTER TABLE [dbo].[usertype_has_actiongroup]
ADD CONSTRAINT [FK_usertype_has_actiongroup_usertype]
    FOREIGN KEY ([usertype_Name])
    REFERENCES [dbo].[usertype]
        ([Name])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_usertype_has_actiongroup_usertype'
CREATE INDEX [IX_FK_usertype_has_actiongroup_usertype]
ON [dbo].[usertype_has_actiongroup]
    ([usertype_Name]);


-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------
