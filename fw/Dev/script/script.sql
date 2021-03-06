USE [BUS]
GO
/****** Object:  StoredProcedure [dbo].[p_m_sys_user_c_seq]    Script Date: 6/13/2015 6:28:36 PM ******/
DROP PROCEDURE [dbo].[p_m_sys_user_c_seq]
GO
ALTER TABLE [dbo].[m_sys_message] DROP CONSTRAINT [DF_m_sys_message_ver_no]
GO
/****** Object:  Index [SYS_USER_SEQ]    Script Date: 6/13/2015 6:28:36 PM ******/
DROP INDEX [SYS_USER_SEQ] ON [dbo].[m_sys_user]
GO
/****** Object:  Index [SYS_USER_INDENTITY]    Script Date: 6/13/2015 6:28:36 PM ******/
DROP INDEX [SYS_USER_INDENTITY] ON [dbo].[m_sys_user]
GO
/****** Object:  Table [dbo].[m_sys_user]    Script Date: 6/13/2015 6:28:36 PM ******/
DROP TABLE [dbo].[m_sys_user]
GO
/****** Object:  Table [dbo].[m_sys_message]    Script Date: 6/13/2015 6:28:36 PM ******/
DROP TABLE [dbo].[m_sys_message]
GO
/****** Object:  UserDefinedTableType [dbo].[ins_ids]    Script Date: 6/13/2015 6:28:36 PM ******/
DROP TYPE [dbo].[ins_ids]
GO
USE [master]
GO
/****** Object:  Database [BUS]    Script Date: 6/13/2015 6:28:36 PM ******/
DROP DATABASE [BUS]
GO
/****** Object:  Database [BUS]    Script Date: 6/13/2015 6:28:36 PM ******/
CREATE DATABASE [BUS] ON  PRIMARY 
( NAME = N'BUS', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\BUS.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'BUS_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\BUS_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BUS].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [BUS] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [BUS] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [BUS] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [BUS] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [BUS] SET ARITHABORT OFF 
GO
ALTER DATABASE [BUS] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [BUS] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [BUS] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [BUS] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [BUS] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [BUS] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [BUS] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [BUS] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [BUS] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [BUS] SET  DISABLE_BROKER 
GO
ALTER DATABASE [BUS] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [BUS] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [BUS] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [BUS] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [BUS] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [BUS] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [BUS] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [BUS] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [BUS] SET  MULTI_USER 
GO
ALTER DATABASE [BUS] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [BUS] SET DB_CHAINING OFF 
GO
USE [BUS]
GO
/****** Object:  UserDefinedTableType [dbo].[ins_ids]    Script Date: 6/13/2015 6:28:36 PM ******/
CREATE TYPE [dbo].[ins_ids] AS TABLE(
	[id] [int] NOT NULL
)
GO
/****** Object:  Table [dbo].[m_sys_message]    Script Date: 6/13/2015 6:28:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[m_sys_message](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[app_id] [nchar](5) NOT NULL,
	[c_seq] [char](10) NOT NULL,
	[msg_cd] [char](10) NOT NULL,
	[msg_text] [nvarchar](255) NOT NULL,
	[msg_help] [nvarchar](255) NOT NULL,
	[msg_type] [tinyint] NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_u_id] [int] NOT NULL,
	[crt_time] [datetime] NOT NULL,
	[upd_u_id] [int] NOT NULL,
	[upd_time] [datetime] NOT NULL,
	[ver_no] [int] NOT NULL,
 CONSTRAINT [SYS_MSG_PK] PRIMARY KEY CLUSTERED 
(
	[c_seq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [SYS_MSG_UNQ] UNIQUE NONCLUSTERED 
(
	[app_id] ASC,
	[msg_cd] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[m_sys_user]    Script Date: 6/13/2015 6:28:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[m_sys_user](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[app_id] [nchar](5) NOT NULL,
	[c_seq] [char](10) NOT NULL,
	[login_id] [varchar](20) NOT NULL,
	[password] [char](32) NOT NULL,
	[full_name] [nvarchar](50) NOT NULL,
	[short_name] [nvarchar](25) NOT NULL,
	[group_id] [int] NOT NULL,
	[del_flg] [bit] NOT NULL,
	[crt_u_id] [int] NOT NULL,
	[crt_time] [datetime] NOT NULL,
	[upd_u_id] [int] NOT NULL,
	[upd_time] [datetime] NOT NULL,
	[ver_no] [int] NOT NULL CONSTRAINT [DF_m_sys_user_ver_no]  DEFAULT ((0)),
 CONSTRAINT [SYS_USER_PK] PRIMARY KEY CLUSTERED 
(
	[c_seq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [SYS_USER_UNQ] UNIQUE NONCLUSTERED 
(
	[app_id] ASC,
	[login_id] ASC,
	[del_flg] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [SYS_USER_INDENTITY]    Script Date: 6/13/2015 6:28:37 PM ******/
CREATE NONCLUSTERED INDEX [SYS_USER_INDENTITY] ON [dbo].[m_sys_user]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [SYS_USER_SEQ]    Script Date: 6/13/2015 6:28:37 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [SYS_USER_SEQ] ON [dbo].[m_sys_user]
(
	[c_seq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[m_sys_message] ADD  CONSTRAINT [DF_m_sys_message_ver_no]  DEFAULT ((1)) FOR [ver_no]
GO
/****** Object:  StoredProcedure [dbo].[p_m_sys_user_c_seq]    Script Date: 6/13/2015 6:28:37 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROC [dbo].[p_m_sys_user_c_seq]
AS
	SELECT NEXT VALUE FOR seq_m_sys_user_c_seq as c_seq

GO
USE [master]
GO
ALTER DATABASE [BUS] SET  READ_WRITE 
GO
