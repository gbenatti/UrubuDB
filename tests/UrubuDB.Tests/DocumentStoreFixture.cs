using NUnit.Framework;
using System;
using System.IO;
using System.Linq;

namespace UrubuDB.Tests
{
	class Person
	{
		public string Name  { get; set; }
		public int Age { get; set; }
	}
	
	[TestFixture]
	public class DocumentStoreFixture
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
		public void ShouldSupportInsert()
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
		public void ShouldSupportUpdate()
		{
			var gb = new Person { Name = "Georges", Age = 35 }; 

			var store = new DocumentStore(storePath);
			
			store.Insert(gb);
			
			Assert.AreEqual(1, store.All<Person>().Count());
			Assert.AreEqual("Georges", gb.Name);
			Assert.AreEqual(35, gb.Age);
			
			gb.Name = "GB";
			store.Update(gb);
			
			var person = store.All<Person>().First();
			
			Assert.AreEqual(1, store.All<Person>().Count());
			Assert.AreEqual("GB", person.Name);
			Assert.AreEqual(35, person.Age);
			
			store.Close();
		}
			
		[Test]
		public void ShouldSupportUpdateBetweenSessions()
		{
			var gb = new Person { Name = "Georges", Age = 35 }; 

			var store = new DocumentStore(storePath);
			
			store.Insert(gb);
			
			var person = store.All<Person>().First();
			
			Assert.AreEqual(1, store.All<Person>().Count());
			Assert.AreEqual("Georges", person.Name);
			Assert.AreEqual(35, person.Age);
			
			store.Close();
			
			store = new DocumentStore(storePath);

			gb = store.All<Person>().First();

			gb.Name = "GB";
			store.Update(gb);
			
			store.Close();
			
			store = new DocumentStore(storePath);

			person = store.All<Person>().First();
			
			Assert.AreEqual(1, store.All<Person>().Count());
			Assert.AreEqual("GB", person.Name);
			Assert.AreEqual(35, person.Age);
			
			store.Close();
		}
		
		[Test]
		public void ShouldSupportDelete()
		{
			var gb = new Person { Name = "Georges", Age = 35 }; 

			var store = new DocumentStore(storePath);

			store.Insert(gb);
			
			Assert.AreEqual(1, store.All<Person>().Count());

			store.Delete(gb);

			Assert.AreEqual(0, store.All<Person>().Count());
		}

		[Test]
		public void ShouldPersistDataBetweenSessions()
		{
			var store = new DocumentStore(storePath);
			store.Insert( new Person { Name = "Georges", Age = 35 } );
			
			Assert.AreEqual(1, store.All<Person>().Count());
			
			store.Close();

			store = new DocumentStore(storePath);
			
			Assert.AreEqual(1, store.All<Person>().Count());
			
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
