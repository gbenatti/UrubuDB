using System;
using System.Collections.Generic;
using SQLite;
using Newtonsoft.Json;

namespace UrubuDB
{
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
}

