using System;
using System.Linq;
using NUnit.Framework;
using NGineer.BuildGenerators;
using System.Collections;
using System.Collections.Generic;
using NGineer.UnitTests.BuildGenerators;
using NGineer.BuildHelpers;
using Moq;
using NGineer.Utils;
using NGineer.UnitTests.BuilderTests;

namespace NGineer.UnitTests.BuildGenerators
{
	[TestFixture]
	public class UniqueCollectionGeneratorEnumerableTests
        : GeneratorTestFixture<UniqueCollectionGeneratorEnumerable<string>>
	{
		private readonly string[] _entries = new string[]{"ab1","kj7","lkj98","298jf"};
		
		protected override UniqueCollectionGeneratorEnumerable<string> Construct ()
		{
			return new UniqueCollectionGeneratorEnumerable<string>(_entries);
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
			Generator.Populate(null, entries, null, BuildSession());
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
			var session = BuildSession();
			for(int i = 0; i < 10; i++)
			{
				entries.Clear();
				Generator.Populate(null, entries, null, session);
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
