using System;
using System.Collections.Generic;
using SQLite;
using Newtonsoft.Json;

namespace UrubuDB
{
	public class DocumentCollection<T> : IEnumerable<T>
	{
		SQLiteConnection _conn;
		Dictionary<object, int> _mapping;
		
		public DocumentCollection (SQLiteConnection conn, Dictionary<object, int> mapping)
		{
			_conn = conn;
			_mapping = mapping;
		}
		
		#region IEnumerable[T] implementation
		public IEnumerator<T> GetEnumerator ()
		{
			var documentType = typeof(T).Name;
	
			foreach(Document doc in _conn.Table<Document>().Where(doc => doc.DocumentType == documentType))
			{
				var data = JsonConvert.DeserializeObject<T>(doc.Json);
				_mapping[data] = doc.Id;
				
				yield return data;
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
}

