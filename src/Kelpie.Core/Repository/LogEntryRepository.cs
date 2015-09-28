﻿using System;
using System.Collections.Generic;
using System.Linq;
using Kelpie.Core.Domain;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Kelpie.Core.Repository
{
	public class LogFilePath
	{
		public string ApplicationName { get; set; }
		public string FilePath { get; set; }
	}

	public class LogEntryRepository
	{
		private readonly MongoClient _mongoClient;
		private readonly IMongoDatabase _database;
		private readonly IMongoCollection<LogEntry> _collection;

		public LogEntryRepository(MongoClient mongoClient, string databaseName = "Kelpie")
		{
			_mongoClient = mongoClient;
			_database = _mongoClient.GetDatabase(databaseName);
			_collection = _database.GetCollection<LogEntry>("LogEntry");
		}

		public void Save(LogEntry entry)
		{
			_collection.InsertOneAsync(entry);
		}

		public void BulkSave(IEnumerable<LogEntry> entries)
		{
			_collection.InsertManyAsync(entries);
		}

		public void DeleteAll()
		{
			_database.DropCollectionAsync("LogEntry");
		}

		public IEnumerable<LogEntry> GetEntriesForApp(string logApplication)
		{
            return _collection.AsQueryable<LogEntry>().Where(x => x.ApplicationName.Equals(logApplication));
		}

		public IEnumerable<LogEntry> GetEntriesToday(string applicationName)
		{
			var items = _collection.AsQueryable<LogEntry>().Where(x => x.ApplicationName.Equals(applicationName) && x.DateTime > DateTime.Today);

			return items.ToList().OrderByDescending(x => x.DateTime);
		}

		public IEnumerable<LogEntry> GetEntriesThisWeek(string logApplication)
		{
			var items =
				_collection.AsQueryable<LogEntry>()
					.Where(x => x.ApplicationName.Equals(logApplication) && x.DateTime > DateTime.Today.AddDays(-7));

			return items.ToList().OrderByDescending(x => x.DateTime);
		}

		public LogEntry GetEntry(Guid id)
		{
			return _collection.AsQueryable<LogEntry>().FirstOrDefault(x => x.Id == id);
		}
	}
}