using System.Collections.Generic;

using SQLite;
using Newtonsoft.Json;

namespace UrubuDB
{
	public class DocumentStore
	{
		SQLiteConnection _db;
		Dictionary<object, int> _objectMapping;
		
		public DocumentStore (string path)
		{
			_db =  new SQLiteConnection(path);
			_db.CreateTable<Document>();
			
			_objectMapping = new Dictionary<object, int>();
		}
		
		public void Insert<T>(T data)
		{
			string json = JsonConvert.SerializeObject(data);
			int id = SaveDocument<T>(json);
			SetObjectId (id, data);
		}
		
		public void Update<T>(T data)
		{
			string json = JsonConvert.SerializeObject(data);
			int id = GetObjectId (data);
			UpdateDocument<T>(id, json);
		}

		public void Delete<T>(T data)
		{
			int id = GetObjectId (data);
			DeleteDocument(id);
			DeleteObjectId(data);
		}
		
		public IEnumerable<Document> All()
		{
			return new DocumentCollection<Document>(_db, _objectMapping);
		}
		
		public IEnumerable<T> All<T>()
		{
			return new DocumentCollection<T>(_db, _objectMapping);
		}
		
		public void Close ()
		{
			if (_db != null) 
			{
				_db.Close();
				_db = null;
			}
		}

		private int SaveDocument<T> (string json)
		{
			var documentType = typeof(T).Name;
			return _db.Insert(new Document { DocumentType = documentType, Json = json });
		}

		public void UpdateDocument<T> (int id, string json)
		{
			var documentType = typeof(T).Name;
			_db.Update(new Document { Id = id, DocumentType = documentType, Json = json });
		}

		public void DeleteDocument (int id)
		{
			_db.Delete(new Document { Id = id });
		}

		int GetObjectId<T>(T data)
		{
			return _objectMapping[data];
		}

		void SetObjectId<T> (int id, T data)
		{
			_objectMapping[data] = id;
		}

		public void DeleteObjectId<T> (T data)
		{
			_objectMapping.Remove(data);
		}
	}
}

