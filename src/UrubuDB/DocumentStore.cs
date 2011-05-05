using System.Collections.Generic;

using SQLite;
using Newtonsoft.Json;

namespace UrubuDB
{
	public class DocumentStore
	{
		SQLiteConnection _db;
		
		public DocumentStore (string path)
		{
			_db =  new SQLiteConnection(path);
			_db.CreateTable<Document>();
		}
		
		public void Insert<T>(T data)
		{
			string json = JsonConvert.SerializeObject(data);
			SaveDocument<T>(json);
		}
		
		public IEnumerable<Document> All()
		{
			return new DocumentCollection<Document>(_db);
		}
		
		public IEnumerable<T> All<T>()
		{
			return new DocumentCollection<T>(_db);
		}
		
		public void Close ()
		{
			if (_db != null) 
			{
				_db.Close();
				_db = null;
			}
		}

		private void SaveDocument<T> (string json)
		{
			var documentType = typeof(T).Name;
			_db.Insert(new Document { DocumentType = documentType, Json = json });
		}
	}
}

