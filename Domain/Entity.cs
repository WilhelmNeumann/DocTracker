﻿using MongoDB.Bson.Serialization.Attributes;

namespace WpfApp.Domain
{
	public abstract class Entity
	{
		/// <summary>
		/// Идентификатор
		/// </summary>
		[BsonId]
		public int Id { get; set; }

		/// <summary>
		/// Описание
		/// </summary>
		public string Description { get; set; }
	}
}