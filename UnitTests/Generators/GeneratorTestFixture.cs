
using System;
using NUnit.Framework;

namespace NGineer.UnitTests
{
	public abstract class GeneratorTestFixture
	{
		public abstract void GeneratesTypes_AcceptsTypes();
		public abstract void GeneratesTypes_RejectsTypes();
	}
}
