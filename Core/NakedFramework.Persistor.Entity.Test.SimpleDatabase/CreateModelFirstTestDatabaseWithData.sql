USE [master]
GO
/****** Object:  Database [ModelFirst]    Script Date: 04/14/2014 11:34:27 ******/
CREATE DATABASE [ModelFirst] ON  PRIMARY 
( NAME = N'ModelFirst', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\ModelFirst.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'ModelFirst_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10.SQLEXPRESS\MSSQL\DATA\ModelFirst_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [ModelFirst] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [ModelFirst].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [ModelFirst] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [ModelFirst] SET ANSI_NULLS OFF
GO
ALTER DATABASE [ModelFirst] SET ANSI_PADDING OFF
GO
ALTER DATABASE [ModelFirst] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [ModelFirst] SET ARITHABORT OFF
GO
ALTER DATABASE [ModelFirst] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [ModelFirst] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [ModelFirst] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [ModelFirst] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [ModelFirst] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [ModelFirst] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [ModelFirst] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [ModelFirst] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [ModelFirst] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [ModelFirst] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [ModelFirst] SET  DISABLE_BROKER
GO
ALTER DATABASE [ModelFirst] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [ModelFirst] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [ModelFirst] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [ModelFirst] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [ModelFirst] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [ModelFirst] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [ModelFirst] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [ModelFirst] SET  READ_WRITE
GO
ALTER DATABASE [ModelFirst] SET RECOVERY SIMPLE
GO
ALTER DATABASE [ModelFirst] SET  MULTI_USER
GO
ALTER DATABASE [ModelFirst] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [ModelFirst] SET DB_CHAINING OFF
GO
USE [ModelFirst]
GO
/****** Object:  Table [dbo].[PersonSet]    Script Date: 04/14/2014 11:34:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PersonSet](
	[Id] [int] NOT NULL,
	[ComplexProperty_Firstname] [nvarchar](max) NOT NULL,
	[ComplexProperty_Surname] [nvarchar](max) NOT NULL,
	[ComplexProperty_1_s1] [nvarchar](max) NOT NULL,
	[ComplexProperty_1_s2] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_PersonSet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[PersonSet] ([Id], [ComplexProperty_Firstname], [ComplexProperty_Surname], [ComplexProperty_1_s1], [ComplexProperty_1_s2]) VALUES (1, N'Joe', N'Bloggs', N'a', N'b')
INSERT [dbo].[PersonSet] ([Id], [ComplexProperty_Firstname], [ComplexProperty_Surname], [ComplexProperty_1_s1], [ComplexProperty_1_s2]) VALUES (2, N'Fred', N'Smith', N'c', N'd')
/****** Object:  Table [dbo].[FoodSet]    Script Date: 04/14/2014 11:34:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FoodSet](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](max) NOT NULL,
	[Person_Id] [int] NOT NULL,
 CONSTRAINT [PK_FoodSet] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[FoodSet] ([Id], [Name], [Person_Id]) VALUES (1, N'Apple', 1)
INSERT [dbo].[FoodSet] ([Id], [Name], [Person_Id]) VALUES (2, N'Orange', 2)
/****** Object:  Table [dbo].[FoodSet_Fruit]    Script Date: 04/14/2014 11:34:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FoodSet_Fruit](
	[Organic] [bit] NOT NULL,
	[Id] [int] NOT NULL,
 CONSTRAINT [PK_FoodSet_Fruit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[FoodSet_Fruit] ([Organic], [Id]) VALUES (1, 1)
INSERT [dbo].[FoodSet_Fruit] ([Organic], [Id]) VALUES (0, 2)
/****** Object:  ForeignKey [FK_PersonFood]    Script Date: 04/14/2014 11:34:27 ******/
ALTER TABLE [dbo].[FoodSet]  WITH CHECK ADD  CONSTRAINT [FK_PersonFood] FOREIGN KEY([Person_Id])
REFERENCES [dbo].[PersonSet] ([Id])
GO
ALTER TABLE [dbo].[FoodSet] CHECK CONSTRAINT [FK_PersonFood]
GO
/****** Object:  ForeignKey [FK_Fruit_inherits_Food]    Script Date: 04/14/2014 11:34:27 ******/
ALTER TABLE [dbo].[FoodSet_Fruit]  WITH CHECK ADD  CONSTRAINT [FK_Fruit_inherits_Food] FOREIGN KEY([Id])
REFERENCES [dbo].[FoodSet] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[FoodSet_Fruit] CHECK CONSTRAINT [FK_Fruit_inherits_Food]
GO
