USE [BUS]
GO
/****** Object:  Table [dbo].[m_sys_message]    Script Date: 6/4/2015 9:57:06 PM ******/
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
ALTER TABLE [dbo].[m_sys_message] ADD  CONSTRAINT [DF_m_sys_message_ver_no]  DEFAULT ((1)) FOR [ver_no]
GO
