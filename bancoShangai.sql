USE [master]
GO
/****** Object:  Database [TESTE]    Script Date: 08/10/2026 13:45:19 ******/

USE [TESTE]
GO
/****** Object:  Table [dbo].[Chamada]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Chamada](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[statusCallID] [int] NOT NULL,
	[callerName] [varchar](255) NULL,
	[callerNumber] [varchar](50) NULL,
	[reportedType] [varchar](50) NOT NULL,
	[descricao] [varchar](max) NULL,
	[locationID] [int] NULL,
	[startedAt] [datetime] NOT NULL,
	[answeredAt] [datetime] NULL,
	[endedAt] [datetime] NULL,
	[linkedIncidentId] [int] NULL,
 CONSTRAINT [PK_Chamada] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Cordenada]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cordenada](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[x] [decimal](7, 2) NOT NULL,
	[y] [decimal](7, 2) NOT NULL,
 CONSTRAINT [PK_Cordenada] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Estacao]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Estacao](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[typeUnitID] [int] NOT NULL,
	[name] [varchar](255) NOT NULL,
	[locationID] [int] NOT NULL,
	[availableUnitCount] [int] NOT NULL,
 CONSTRAINT [PK_Estacao] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Incidente]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Incidente](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[typeID] [int] NOT NULL,
	[subtypeID] [int] NOT NULL,
	[severityID] [int] NOT NULL,
	[statusID] [int] NOT NULL,
	[reportedAt] [datetime] NOT NULL,
	[locationID] [int] NOT NULL,
	[descricao] [varchar](max) NULL,
	[callID] [int] NULL,
	[resolvedAt] [datetime] NULL,
 CONSTRAINT [PK_Incidente] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IncidenteSubTipo]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IncidenteSubTipo](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tipoIncidenteID] [int] NOT NULL,
	[SubType] [varchar](255) NOT NULL,
 CONSTRAINT [PK_IncidenteSubTipo] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IncidenteUnidadeAtendimento]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IncidenteUnidadeAtendimento](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[incidenteID] [int] NOT NULL,
	[unidadeID] [int] NOT NULL,
	[atendidoEm] [datetime] NULL,
 CONSTRAINT [PK_IncidenteUnidadeAtendimento] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[IncidenteUnidadeAtribuida]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IncidenteUnidadeAtribuida](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[incidenteID] [int] NOT NULL,
	[unidadeID] [int] NOT NULL,
	[atribuidoEm] [datetime] NULL,
 CONSTRAINT [PK_IncidenteUnidadeAtribuida] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Severidade]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Severidade](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[severidade] [varchar](255) NOT NULL,
 CONSTRAINT [PK_Severidade] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StatusChamada]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusChamada](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[statusChamada] [varchar](255) NOT NULL,
 CONSTRAINT [PK_StatusChamada] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StatusIncidente]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusIncidente](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[incidente] [varchar](255) NOT NULL,
 CONSTRAINT [PK_StatusIncidente] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StatusUnidade]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StatusUnidade](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[unidade] [varchar](255) NOT NULL,
 CONSTRAINT [PK_StatusUnidade] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoIncidente]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoIncidente](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tipoIncidente] [varchar](255) NOT NULL,
 CONSTRAINT [PK_TipoIncidente] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[TipoUnidade]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TipoUnidade](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[tipoUnidade] [varchar](255) NOT NULL,
 CONSTRAINT [PK_TipoUnidade] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Unidade]    Script Date: 08/10/2026 13:45:20 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Unidade](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[estacaoID] [int] NOT NULL,
	[typeUnitID] [int] NOT NULL,
	[callSign] [varchar](255) NOT NULL,
	[statusUnitID] [int] NOT NULL,
	[locationID] [int] NOT NULL,
	[assignedIncidentId] [int] NULL,
 CONSTRAINT [PK_Unidade] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Chamada] ON 
GO
INSERT [dbo].[Chamada] ([id], [statusCallID], [callerName], [callerNumber], [reportedType], [descricao], [locationID], [startedAt], [answeredAt], [endedAt], [linkedIncidentId]) VALUES (1, 2, N'João Silva', N'11999991111', N'fire', N'Fumaça alta vista no prédio comercial', 7, CAST(N'2026-03-10T10:00:00.000' AS DateTime), CAST(N'2026-03-10T10:00:05.000' AS DateTime), CAST(N'2026-03-10T10:02:30.000' AS DateTime), 16)
GO
INSERT [dbo].[Chamada] ([id], [statusCallID], [callerName], [callerNumber], [reportedType], [descricao], [locationID], [startedAt], [answeredAt], [endedAt], [linkedIncidentId]) VALUES (2, 3, N'Maria Santos', N'11988882222', N'crime', N'Assalto em andamento na loja de conveniência', 5, CAST(N'2026-03-10T10:15:00.000' AS DateTime), CAST(N'2026-03-10T10:15:03.000' AS DateTime), NULL, NULL)
GO
INSERT [dbo].[Chamada] ([id], [statusCallID], [callerName], [callerNumber], [reportedType], [descricao], [locationID], [startedAt], [answeredAt], [endedAt], [linkedIncidentId]) VALUES (3, 3, N'Anônimo', NULL, N'medical', N'Pessoa desacordada na via pública', 6, CAST(N'2026-03-10T10:30:00.000' AS DateTime), NULL, NULL, 2)
GO
SET IDENTITY_INSERT [dbo].[Chamada] OFF
GO
SET IDENTITY_INSERT [dbo].[Cordenada] ON 
GO
INSERT [dbo].[Cordenada] ([id], [x], [y]) VALUES (1, CAST(100.50 AS Decimal(7, 2)), CAST(200.00 AS Decimal(7, 2)))
GO
INSERT [dbo].[Cordenada] ([id], [x], [y]) VALUES (2, CAST(450.00 AS Decimal(7, 2)), CAST(300.25 AS Decimal(7, 2)))
GO
INSERT [dbo].[Cordenada] ([id], [x], [y]) VALUES (3, CAST(800.75 AS Decimal(7, 2)), CAST(600.00 AS Decimal(7, 2)))
GO
INSERT [dbo].[Cordenada] ([id], [x], [y]) VALUES (4, CAST(150.00 AS Decimal(7, 2)), CAST(220.00 AS Decimal(7, 2)))
GO
INSERT [dbo].[Cordenada] ([id], [x], [y]) VALUES (5, CAST(500.00 AS Decimal(7, 2)), CAST(350.00 AS Decimal(7, 2)))
GO
INSERT [dbo].[Cordenada] ([id], [x], [y]) VALUES (6, CAST(850.00 AS Decimal(7, 2)), CAST(620.00 AS Decimal(7, 2)))
GO
INSERT [dbo].[Cordenada] ([id], [x], [y]) VALUES (7, CAST(300.00 AS Decimal(7, 2)), CAST(400.00 AS Decimal(7, 2)))
GO
INSERT [dbo].[Cordenada] ([id], [x], [y]) VALUES (8, CAST(110.00 AS Decimal(7, 2)), CAST(210.00 AS Decimal(7, 2)))
GO
SET IDENTITY_INSERT [dbo].[Cordenada] OFF
GO
SET IDENTITY_INSERT [dbo].[Estacao] ON 
GO
INSERT [dbo].[Estacao] ([id], [typeUnitID], [name], [locationID], [availableUnitCount]) VALUES (1, 1, N'Estação Central de Bombeiros', 1, 2)
GO
INSERT [dbo].[Estacao] ([id], [typeUnitID], [name], [locationID], [availableUnitCount]) VALUES (2, 2, N'1º Batalhão de Polícia Militar', 2, 1)
GO
INSERT [dbo].[Estacao] ([id], [typeUnitID], [name], [locationID], [availableUnitCount]) VALUES (3, 3, N'Base do SAMU Central', 3, 1)
GO
SET IDENTITY_INSERT [dbo].[Estacao] OFF
GO
SET IDENTITY_INSERT [dbo].[Incidente] ON 
GO
INSERT [dbo].[Incidente] ([id], [typeID], [subtypeID], [severityID], [statusID], [reportedAt], [locationID], [descricao], [callID], [resolvedAt]) VALUES (1, 1, 1, 3, 1, CAST(N'2026-03-10T00:00:00.000' AS DateTime), 4, N'Incêndio estrutural em galpão comercial', 1, NULL)
GO
INSERT [dbo].[Incidente] ([id], [typeID], [subtypeID], [severityID], [statusID], [reportedAt], [locationID], [descricao], [callID], [resolvedAt]) VALUES (2, 2, 6, 4, 6, CAST(N'2026-03-09T00:00:00.000' AS DateTime), 5, N'hdffdghdfgh', 2, NULL)
GO
INSERT [dbo].[Incidente] ([id], [typeID], [subtypeID], [severityID], [statusID], [reportedAt], [locationID], [descricao], [callID], [resolvedAt]) VALUES (3, 3, 12, 4, 6, CAST(N'2026-03-10T09:00:00.000' AS DateTime), 6, N'Atendimento de parada cardiorrespiratória em idoso', NULL, CAST(N'2026-03-10T09:45:00.000' AS DateTime))
GO
INSERT [dbo].[Incidente] ([id], [typeID], [subtypeID], [severityID], [statusID], [reportedAt], [locationID], [descricao], [callID], [resolvedAt]) VALUES (4, 1, 1, 1, 1, CAST(N'2026-10-08T13:42:17.770' AS DateTime), 1, N'teste', 1, CAST(N'2026-10-08T13:42:17.770' AS DateTime))
GO
INSERT [dbo].[Incidente] ([id], [typeID], [subtypeID], [severityID], [statusID], [reportedAt], [locationID], [descricao], [callID], [resolvedAt]) VALUES (5, 1, 1, 1, 1, CAST(N'2026-10-08T00:00:00.000' AS DateTime), 1, N'teste', 1, CAST(N'2026-10-01T00:00:00.000' AS DateTime))
GO
INSERT [dbo].[Incidente] ([id], [typeID], [subtypeID], [severityID], [statusID], [reportedAt], [locationID], [descricao], [callID], [resolvedAt]) VALUES (6, 1, 1, 1, 1, CAST(N'2026-10-08T13:42:17.770' AS DateTime), 1, N'teste', 1, CAST(N'2026-10-08T13:42:17.770' AS DateTime))
GO
INSERT [dbo].[Incidente] ([id], [typeID], [subtypeID], [severityID], [statusID], [reportedAt], [locationID], [descricao], [callID], [resolvedAt]) VALUES (7, 1, 1, 1, 1, CAST(N'2026-10-08T13:47:12.897' AS DateTime), 1, N'teseteeeeee', 1, CAST(N'2026-10-08T13:47:12.897' AS DateTime))
GO
INSERT [dbo].[Incidente] ([id], [typeID], [subtypeID], [severityID], [statusID], [reportedAt], [locationID], [descricao], [callID], [resolvedAt]) VALUES (8, 1, 1, 2, 1, CAST(N'2026-10-08T16:30:12.993' AS DateTime), 3, N'fdasfasdfsfadasdf', 1, CAST(N'2026-10-08T16:30:12.993' AS DateTime))
GO
INSERT [dbo].[Incidente] ([id], [typeID], [subtypeID], [severityID], [statusID], [reportedAt], [locationID], [descricao], [callID], [resolvedAt]) VALUES (16, 1, 1, 1, 1, CAST(N'2026-10-08T16:42:24.630' AS DateTime), 7, N'Fumaça alta vista no prédio comercial', 1, NULL)
GO
SET IDENTITY_INSERT [dbo].[Incidente] OFF
GO
SET IDENTITY_INSERT [dbo].[IncidenteSubTipo] ON 
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (1, 1, N'structure_fire')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (2, 1, N'vehicle_fire')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (3, 1, N'wildfire')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (4, 1, N'gas_leak')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (5, 1, N'fire_alarm')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (6, 2, N'robbery')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (7, 2, N'assault')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (8, 2, N'burglary')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (9, 2, N'vandalism')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (10, 2, N'disturbance')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (11, 2, N'traffic_incident')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (12, 3, N'cardiac_arrest')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (13, 3, N'trauma')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (14, 3, N'respiratory')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (15, 3, N'overdose')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (16, 3, N'fall')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (17, 3, N'maternity')
GO
INSERT [dbo].[IncidenteSubTipo] ([id], [tipoIncidenteID], [SubType]) VALUES (18, 1, N'unknown')
GO
SET IDENTITY_INSERT [dbo].[IncidenteSubTipo] OFF
GO
SET IDENTITY_INSERT [dbo].[IncidenteUnidadeAtendimento] ON 
GO
INSERT [dbo].[IncidenteUnidadeAtendimento] ([id], [incidenteID], [unidadeID], [atendidoEm]) VALUES (1, 3, 4, CAST(N'2026-03-10T09:45:00.000' AS DateTime))
GO
INSERT [dbo].[IncidenteUnidadeAtendimento] ([id], [incidenteID], [unidadeID], [atendidoEm]) VALUES (2, 2, 4, CAST(N'2026-10-07T16:16:17.443' AS DateTime))
GO
INSERT [dbo].[IncidenteUnidadeAtendimento] ([id], [incidenteID], [unidadeID], [atendidoEm]) VALUES (3, 2, 1, CAST(N'2026-10-07T16:23:20.357' AS DateTime))
GO
INSERT [dbo].[IncidenteUnidadeAtendimento] ([id], [incidenteID], [unidadeID], [atendidoEm]) VALUES (4, 2, 3, CAST(N'2026-10-07T16:23:45.250' AS DateTime))
GO
INSERT [dbo].[IncidenteUnidadeAtendimento] ([id], [incidenteID], [unidadeID], [atendidoEm]) VALUES (5, 2, 4, CAST(N'2026-10-07T16:23:45.253' AS DateTime))
GO
INSERT [dbo].[IncidenteUnidadeAtendimento] ([id], [incidenteID], [unidadeID], [atendidoEm]) VALUES (6, 2, 1, CAST(N'2026-10-07T16:25:12.663' AS DateTime))
GO
INSERT [dbo].[IncidenteUnidadeAtendimento] ([id], [incidenteID], [unidadeID], [atendidoEm]) VALUES (7, 2, 1, CAST(N'2026-10-07T16:25:23.610' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[IncidenteUnidadeAtendimento] OFF
GO
SET IDENTITY_INSERT [dbo].[Severidade] ON 
GO
INSERT [dbo].[Severidade] ([id], [severidade]) VALUES (1, N'low')
GO
INSERT [dbo].[Severidade] ([id], [severidade]) VALUES (2, N'medium')
GO
INSERT [dbo].[Severidade] ([id], [severidade]) VALUES (3, N'high')
GO
INSERT [dbo].[Severidade] ([id], [severidade]) VALUES (4, N'critical')
GO
SET IDENTITY_INSERT [dbo].[Severidade] OFF
GO
SET IDENTITY_INSERT [dbo].[StatusChamada] ON 
GO
INSERT [dbo].[StatusChamada] ([id], [statusChamada]) VALUES (1, N'ringing')
GO
INSERT [dbo].[StatusChamada] ([id], [statusChamada]) VALUES (2, N'active')
GO
INSERT [dbo].[StatusChamada] ([id], [statusChamada]) VALUES (3, N'ended')
GO
INSERT [dbo].[StatusChamada] ([id], [statusChamada]) VALUES (4, N'declined')
GO
INSERT [dbo].[StatusChamada] ([id], [statusChamada]) VALUES (5, N'missed')
GO
SET IDENTITY_INSERT [dbo].[StatusChamada] OFF
GO
SET IDENTITY_INSERT [dbo].[StatusIncidente] ON 
GO
INSERT [dbo].[StatusIncidente] ([id], [incidente]) VALUES (1, N'unacknowledged')
GO
INSERT [dbo].[StatusIncidente] ([id], [incidente]) VALUES (2, N'acknowledged')
GO
INSERT [dbo].[StatusIncidente] ([id], [incidente]) VALUES (3, N'assigned')
GO
INSERT [dbo].[StatusIncidente] ([id], [incidente]) VALUES (4, N'en_route')
GO
INSERT [dbo].[StatusIncidente] ([id], [incidente]) VALUES (5, N'on_scene')
GO
INSERT [dbo].[StatusIncidente] ([id], [incidente]) VALUES (6, N'resolved')
GO
SET IDENTITY_INSERT [dbo].[StatusIncidente] OFF
GO
SET IDENTITY_INSERT [dbo].[StatusUnidade] ON 
GO
INSERT [dbo].[StatusUnidade] ([id], [unidade]) VALUES (1, N'available')
GO
INSERT [dbo].[StatusUnidade] ([id], [unidade]) VALUES (2, N'assigned')
GO
INSERT [dbo].[StatusUnidade] ([id], [unidade]) VALUES (3, N'en_route')
GO
INSERT [dbo].[StatusUnidade] ([id], [unidade]) VALUES (4, N'on_scene')
GO
INSERT [dbo].[StatusUnidade] ([id], [unidade]) VALUES (5, N'returning')
GO
INSERT [dbo].[StatusUnidade] ([id], [unidade]) VALUES (6, N'out_of_service')
GO
SET IDENTITY_INSERT [dbo].[StatusUnidade] OFF
GO
SET IDENTITY_INSERT [dbo].[TipoIncidente] ON 
GO
INSERT [dbo].[TipoIncidente] ([id], [tipoIncidente]) VALUES (1, N'fire')
GO
INSERT [dbo].[TipoIncidente] ([id], [tipoIncidente]) VALUES (2, N'crime')
GO
INSERT [dbo].[TipoIncidente] ([id], [tipoIncidente]) VALUES (3, N'medical')
GO
SET IDENTITY_INSERT [dbo].[TipoIncidente] OFF
GO
SET IDENTITY_INSERT [dbo].[TipoUnidade] ON 
GO
INSERT [dbo].[TipoUnidade] ([id], [tipoUnidade]) VALUES (1, N'fire')
GO
INSERT [dbo].[TipoUnidade] ([id], [tipoUnidade]) VALUES (2, N'police')
GO
INSERT [dbo].[TipoUnidade] ([id], [tipoUnidade]) VALUES (3, N'medical')
GO
SET IDENTITY_INSERT [dbo].[TipoUnidade] OFF
GO
SET IDENTITY_INSERT [dbo].[Unidade] ON 
GO
INSERT [dbo].[Unidade] ([id], [estacaoID], [typeUnitID], [callSign], [statusUnitID], [locationID], [assignedIncidentId]) VALUES (1, 1, 1, N'Auto Bomba 01', 1, 8, 1)
GO
INSERT [dbo].[Unidade] ([id], [estacaoID], [typeUnitID], [callSign], [statusUnitID], [locationID], [assignedIncidentId]) VALUES (2, 1, 1, N'Auto Bomba 02', 1, 1, NULL)
GO
INSERT [dbo].[Unidade] ([id], [estacaoID], [typeUnitID], [callSign], [statusUnitID], [locationID], [assignedIncidentId]) VALUES (3, 2, 2, N'Viatura Policial 101', 1, 2, 2)
GO
INSERT [dbo].[Unidade] ([id], [estacaoID], [typeUnitID], [callSign], [statusUnitID], [locationID], [assignedIncidentId]) VALUES (4, 3, 3, N'UTI Móvel 05', 1, 3, NULL)
GO
SET IDENTITY_INSERT [dbo].[Unidade] OFF
GO
ALTER TABLE [dbo].[Estacao] ADD  DEFAULT ((0)) FOR [availableUnitCount]
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtendimento] ADD  DEFAULT (getdate()) FOR [atendidoEm]
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtribuida] ADD  DEFAULT (getdate()) FOR [atribuidoEm]
GO
ALTER TABLE [dbo].[Chamada]  WITH CHECK ADD  CONSTRAINT [FK_Chamada_Cordenada] FOREIGN KEY([locationID])
REFERENCES [dbo].[Cordenada] ([id])
GO
ALTER TABLE [dbo].[Chamada] CHECK CONSTRAINT [FK_Chamada_Cordenada]
GO
ALTER TABLE [dbo].[Chamada]  WITH CHECK ADD  CONSTRAINT [FK_Chamada_Incidente] FOREIGN KEY([linkedIncidentId])
REFERENCES [dbo].[Incidente] ([id])
GO
ALTER TABLE [dbo].[Chamada] CHECK CONSTRAINT [FK_Chamada_Incidente]
GO
ALTER TABLE [dbo].[Chamada]  WITH CHECK ADD  CONSTRAINT [FK_Chamada_Status] FOREIGN KEY([statusCallID])
REFERENCES [dbo].[StatusChamada] ([id])
GO
ALTER TABLE [dbo].[Chamada] CHECK CONSTRAINT [FK_Chamada_Status]
GO
ALTER TABLE [dbo].[Estacao]  WITH CHECK ADD  CONSTRAINT [FK_Estacao_Cordenada] FOREIGN KEY([locationID])
REFERENCES [dbo].[Cordenada] ([id])
GO
ALTER TABLE [dbo].[Estacao] CHECK CONSTRAINT [FK_Estacao_Cordenada]
GO
ALTER TABLE [dbo].[Estacao]  WITH CHECK ADD  CONSTRAINT [FK_Estacao_TipoUnidade] FOREIGN KEY([typeUnitID])
REFERENCES [dbo].[TipoUnidade] ([id])
GO
ALTER TABLE [dbo].[Estacao] CHECK CONSTRAINT [FK_Estacao_TipoUnidade]
GO
ALTER TABLE [dbo].[Incidente]  WITH CHECK ADD  CONSTRAINT [FK_Incidente_Cordenada] FOREIGN KEY([locationID])
REFERENCES [dbo].[Cordenada] ([id])
GO
ALTER TABLE [dbo].[Incidente] CHECK CONSTRAINT [FK_Incidente_Cordenada]
GO
ALTER TABLE [dbo].[Incidente]  WITH CHECK ADD  CONSTRAINT [FK_Incidente_Severidade] FOREIGN KEY([severityID])
REFERENCES [dbo].[Severidade] ([id])
GO
ALTER TABLE [dbo].[Incidente] CHECK CONSTRAINT [FK_Incidente_Severidade]
GO
ALTER TABLE [dbo].[Incidente]  WITH CHECK ADD  CONSTRAINT [FK_Incidente_Status] FOREIGN KEY([statusID])
REFERENCES [dbo].[StatusIncidente] ([id])
GO
ALTER TABLE [dbo].[Incidente] CHECK CONSTRAINT [FK_Incidente_Status]
GO
ALTER TABLE [dbo].[Incidente]  WITH CHECK ADD  CONSTRAINT [FK_Incidente_SubTipo] FOREIGN KEY([subtypeID])
REFERENCES [dbo].[IncidenteSubTipo] ([id])
GO
ALTER TABLE [dbo].[Incidente] CHECK CONSTRAINT [FK_Incidente_SubTipo]
GO
ALTER TABLE [dbo].[Incidente]  WITH CHECK ADD  CONSTRAINT [FK_Incidente_Tipo] FOREIGN KEY([typeID])
REFERENCES [dbo].[TipoIncidente] ([id])
GO
ALTER TABLE [dbo].[Incidente] CHECK CONSTRAINT [FK_Incidente_Tipo]
GO
ALTER TABLE [dbo].[IncidenteSubTipo]  WITH CHECK ADD  CONSTRAINT [FK_IncidenteSubTipo_TipoIncidente] FOREIGN KEY([tipoIncidenteID])
REFERENCES [dbo].[TipoIncidente] ([id])
GO
ALTER TABLE [dbo].[IncidenteSubTipo] CHECK CONSTRAINT [FK_IncidenteSubTipo_TipoIncidente]
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtendimento]  WITH CHECK ADD  CONSTRAINT [FK_Atendimento_Incidente] FOREIGN KEY([incidenteID])
REFERENCES [dbo].[Incidente] ([id])
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtendimento] CHECK CONSTRAINT [FK_Atendimento_Incidente]
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtendimento]  WITH CHECK ADD  CONSTRAINT [FK_Atendimento_Unidade] FOREIGN KEY([unidadeID])
REFERENCES [dbo].[Unidade] ([id])
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtendimento] CHECK CONSTRAINT [FK_Atendimento_Unidade]
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtribuida]  WITH CHECK ADD  CONSTRAINT [FK_Atribuida_Incidente] FOREIGN KEY([incidenteID])
REFERENCES [dbo].[Incidente] ([id])
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtribuida] CHECK CONSTRAINT [FK_Atribuida_Incidente]
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtribuida]  WITH CHECK ADD  CONSTRAINT [FK_Atribuida_Unidade] FOREIGN KEY([unidadeID])
REFERENCES [dbo].[Unidade] ([id])
GO
ALTER TABLE [dbo].[IncidenteUnidadeAtribuida] CHECK CONSTRAINT [FK_Atribuida_Unidade]
GO
ALTER TABLE [dbo].[Unidade]  WITH CHECK ADD  CONSTRAINT [FK_Unidade_Cordenada] FOREIGN KEY([locationID])
REFERENCES [dbo].[Cordenada] ([id])
GO
ALTER TABLE [dbo].[Unidade] CHECK CONSTRAINT [FK_Unidade_Cordenada]
GO
ALTER TABLE [dbo].[Unidade]  WITH CHECK ADD  CONSTRAINT [FK_Unidade_Estacao] FOREIGN KEY([estacaoID])
REFERENCES [dbo].[Estacao] ([id])
GO
ALTER TABLE [dbo].[Unidade] CHECK CONSTRAINT [FK_Unidade_Estacao]
GO
ALTER TABLE [dbo].[Unidade]  WITH CHECK ADD  CONSTRAINT [FK_Unidade_Incidente] FOREIGN KEY([assignedIncidentId])
REFERENCES [dbo].[Incidente] ([id])
GO
ALTER TABLE [dbo].[Unidade] CHECK CONSTRAINT [FK_Unidade_Incidente]
GO
ALTER TABLE [dbo].[Unidade]  WITH CHECK ADD  CONSTRAINT [FK_Unidade_StatusUnidade] FOREIGN KEY([statusUnitID])
REFERENCES [dbo].[StatusUnidade] ([id])
GO
ALTER TABLE [dbo].[Unidade] CHECK CONSTRAINT [FK_Unidade_StatusUnidade]
GO
ALTER TABLE [dbo].[Unidade]  WITH CHECK ADD  CONSTRAINT [FK_Unidade_TipoUnidade] FOREIGN KEY([typeUnitID])
REFERENCES [dbo].[TipoUnidade] ([id])
GO
ALTER TABLE [dbo].[Unidade] CHECK CONSTRAINT [FK_Unidade_TipoUnidade]
GO
USE [master]
GO
ALTER DATABASE [TESTE] SET  READ_WRITE 
GO
