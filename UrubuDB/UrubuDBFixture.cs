using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace UrubuDB
{
	class Person
	{
		public string Name  { get; set; }
		public int Age { get; set; }
	}
	
	[TestFixture]
	public class UrubuDBFixture
	{
		string storePath;
		
		[SetUp]
		public void Setup()
		{
			storePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "test.db");
			if (File.Exists(storePath)) File.Delete(storePath);
		}
		
		[Test]
		public void ShouldCreateADocumentStoreBackendFile()
		{
			Assert.IsFalse(File.Exists(storePath));	

			var store = new DocumentStore(storePath);
			store.Close();
			
			Assert.IsTrue(File.Exists(storePath));	
		}
		
		[Test]
		public void ShouldSaveObject()
		{
			var store = new DocumentStore(storePath);

			store.Insert( new Person { Name = "Georges", Age = 35 } );
			
			var person = store.All<Person>().First();
			
			Assert.AreEqual(1, store.All<Person>().Count());
			Assert.AreEqual("Georges", person.Name);
			Assert.AreEqual(35, person.Age);
			
			store.Close();
		}
	
		[Test]
		public void ShouldSupportLinq()
		{
			var store = new DocumentStore(storePath);

			store.Insert( new Person { Name = "Georges", Age = 35 } );
			store.Insert( new Person { Name = "Vivi", Age = 32 } );
			
			var names = from p in store.All<Person>() where p.Age > 33 select p.Name;
			
			Assert.AreEqual(2, store.All<Person>().Count());
			Assert.AreEqual(1, names.Count());
			Assert.AreEqual("Georges", names.First());
			
			store.Close();
		}
	}
}