CREATE DATABASE [consultant-profile-db];
GO

USE [consultant-profile-db];
GO

CREATE TABLE [TitleCode] (
    [title_code_id] int IDENTITY (1,1) NOT NULL,
    [code] varchar(255) NOT NULL,
    [description] varchar(255) NOT NULL,
    CONSTRAINT [PK_TitleCode_title_code_id] PRIMARY KEY ([title_code_id])
);
GO


CREATE TABLE [Profile] (
    [profile_id] int IDENTITY (1,1) NOT NULL,
    [email_address] varchar(255) NOT NULL UNIQUE,
    [first_name] varchar(255) NOT NULL,
    [last_name] varchar(255) NOT NULL,
    [years_of_experience] int NOT NULL,
    [about_me] varchar(255),
    [title_code_id] int NOT NULL FOREIGN KEY REFERENCES [TitleCode]([title_code_id])
    CONSTRAINT [PK_Profiles_profile_id] PRIMARY KEY ([profile_id])
);
GO


CREATE TABLE [Certification] (
	[certification_id] int IDENTITY (1,1) NOT NULL,
	[title] varchar(255) NOT NULL,
	[year] int NOT NULL,
	[profile_id] int NOT NULL FOREIGN KEY REFERENCES [Profile]([profile_id])
    ON DELETE CASCADE
    ON UPDATE CASCADE,
    CONSTRAINT [PK_Certification_certification_id] PRIMARY KEY ([certification_id]),
);

CREATE TABLE [Education] (
	[education_id] int IDENTITY (1,1) NOT NULL,
	[school_name] varchar(255) NOT NULL,
	[graduation_year] int NOT NULL,
	[major_degree_name] varchar(255),
	[minor_degree_name] varchar(255),
	[profile_id] int NOT NULL FOREIGN KEY REFERENCES [Profile]([profile_id])
    ON DELETE CASCADE
    ON UPDATE CASCADE,
    CONSTRAINT [PK_Education_education_id] PRIMARY KEY ([education_id])

);

CREATE TABLE [Highlight] (
	[highlight_id] int IDENTITY (1,1) NOT NULL,
	[title] varchar(255) NOT NULL,
	[description] varchar(300) NOT NULL,
	[profile_id] int NOT NULL FOREIGN KEY REFERENCES [Profile]([profile_id])
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT [PK_Highlight_highlight_id] PRIMARY KEY ([highlight_id])

);

CREATE TABLE [SkillCode] (
    [skill_code_id] int IDENTITY (1,1) NOT NULL,
    [code] varchar(255) NOT NULL,
    [description] varchar(255) NOT NULL,
    CONSTRAINT [PK_SkillCode_skill_code_id] PRIMARY KEY ([skill_code_id])
);
GO

CREATE TABLE [Skill] (
	[skill_id] int IDENTITY (1,1) NOT NULL,
	[description] varchar(255) NOT NULL,
	[sort_order] int NOT NULL,
    [skill_code_id] int NOT NULL FOREIGN KEY REFERENCES [SkillCode]([skill_code_id]),
	[profile_id] int NOT NULL FOREIGN KEY REFERENCES [Profile]([profile_id])
    ON DELETE CASCADE
    ON UPDATE CASCADE,
    CONSTRAINT [PK_Skill_skill_id] PRIMARY KEY ([skill_id])
);
