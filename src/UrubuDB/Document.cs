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
}

