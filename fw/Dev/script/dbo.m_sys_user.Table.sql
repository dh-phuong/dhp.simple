USE [BUS]
GO
/****** Object:  Table [dbo].[m_sys_user]    Script Date: 6/4/2015 9:57:07 PM ******/
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
	[ver_no] [int] NOT NULL,
 CONSTRAINT [SYS_USER_PK] PRIMARY KEY CLUSTERED 
(
	[c_seq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [SYS_USER_UNQ] UNIQUE NONCLUSTERED 
(
	[app_id] ASC,
	[login_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Index [SYS_USER_INDENTITY]    Script Date: 6/4/2015 9:57:07 PM ******/
CREATE NONCLUSTERED INDEX [SYS_USER_INDENTITY] ON [dbo].[m_sys_user]
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [SYS_USER_SEQ]    Script Date: 6/4/2015 9:57:07 PM ******/
CREATE UNIQUE NONCLUSTERED INDEX [SYS_USER_SEQ] ON [dbo].[m_sys_user]
(
	[c_seq] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[m_sys_user] ADD  CONSTRAINT [DF_m_sys_user_ver_no]  DEFAULT ((1)) FOR [ver_no]
GO
