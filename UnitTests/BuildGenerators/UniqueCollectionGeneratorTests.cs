using System;
using NUnit.Framework;
using NGineer.BuildGenerators;
using System.Collections;
using System.Collections.Generic;
using NGineer.UnitTests.BuildGenerators;
using NGineer.BuildHelpers;

namespace NGineer.UnitTests.BuildGenerators
{
	[TestFixture]
	public class UniqueCollectionGeneratorTests : GeneratorTestFixture<UniqueCollectionGenerator<string>>
	{
		private readonly string[] _entries = new string[]{"ab1","kj7","lkj98","298jf"};
		
		protected override UniqueCollectionGenerator<string> Construct ()
		{
			return new UniqueCollectionGenerator<string>(10, _entries);
		}

		protected override Type[] SupportedTypes ()
		{
			return new []{
				typeof(ICollection<string>),
				typeof(IList<string>),
				typeof(IEnumerable<string>),
				typeof(List<string>),
			};
		}
		
		protected override Type[] UnsupportedTypes ()
		{
			return new[] {
				typeof(ICollection<int>),
				typeof(List<int>),
				typeof(string),
				typeof(IList),
				typeof(string[]),
				typeof(object)
			};
		}
		
		[Test]
		public void Populate_PopulatesWithCorrectNumberOfEntries()
		{
			var entries = new List<string>();
			Generator.Populate(null, entries, null, null);
			Assert.AreEqual(_entries.Length, entries.Count);
			foreach(var item in _entries)
			{
				Assert.Contains(item, entries);
				entries.Remove(item);
			}
		}
		
		[Test]
		public void Populate_OrderChanges()
		{
			bool different = false;
			string lastOrder = null;
			var entries = new List<string>();
			for(int i = 0; i < 10; i++)
			{
				entries.Clear();
				Generator.Populate(null, entries, null, null);
				Assert.AreEqual(_entries.Length, entries.Count);
				var current = string.Join(":", entries.ToArray());
				if(lastOrder != null)
				{
					different = different || !string.Equals(lastOrder, current);
				}
				lastOrder = current;
			}
			Assert.IsTrue(different, "Sequence was always identical");
		}
	}
}
