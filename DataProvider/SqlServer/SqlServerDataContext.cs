﻿using System;
using System.Data.Entity;
using WpfApp.Domain;

namespace WpfApp.DataProvider.SqlServer
{
	/// <summary>
	/// Контекст для доступа к базе SqlServer
	/// </summary>
	public class SqlServerDataContext : DbContext
	{
		/// <summary>
		/// Экземпляр контекста
		/// </summary>
		public static SqlServerDataContext Instance => new SqlServerDataContext();

		public DbSet<Storage> Storages { get; set; }
		public DbSet<Box> Boxes { get; set; }
		public DbSet<Contract> Contracts { get; set; }

		private const string ConnectionString = "DefaultConnection";

		private SqlServerDataContext() : base(ConnectionString)
		{
		}
	}
}