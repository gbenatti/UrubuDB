using System;
using System.IO;

using Newtonsoft.Json;
using System.Collections.Generic;

using SQLite;

namespace UrubuDB
{
	public class Document
	{
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

		public string DocumentType { get; set; }
        public string Json { get; set; }
	}
	
	public class DocumentCollection<T> : IEnumerable<T>
	{
		SQLiteConnection _conn;
		
		public DocumentCollection (SQLiteConnection conn)
		{
			_conn = conn;
		}
		
		#region IEnumerable[T] implementation
		public IEnumerator<T> GetEnumerator ()
		{
			var documentType = typeof(T).Name;
	
			foreach(Document doc in _conn.Table<Document>().Where(doc => doc.DocumentType == documentType))
			{
				yield return JsonConvert.DeserializeObject<T>(doc.Json);
			}
		}
		#endregion

		#region IEnumerable implementation
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator ()
		{
			throw new NotImplementedException();
		}
		#endregion
	}
	
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

