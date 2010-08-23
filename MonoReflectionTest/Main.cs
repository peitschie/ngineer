using System;
using System.Reflection;

namespace MonoReflectionTest
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var inherit1 = typeof(InheritsFromClassWithNullableDateTime);
			var siblingProperty = inherit1.GetProperty("Property1");
			PrintMember("Child1", siblingProperty);
			PrintMember("Child1", siblingProperty);
		}
		
		private static void PrintMember(string name, MemberInfo member)
		{
			Console.Write(name);
			Console.Write(" ");
			Console.Write(member.DeclaringType.FullName);
			Console.Write(" ");
			Console.WriteLine(member.ReflectedType.FullName);
		}
	}
	
	public class ClassWithNullableDateTime
	{
		public DateTime? Property1 { get; set; }
	}

	public class InheritsFromClassWithNullableDateTime : ClassWithNullableDateTime
	{
	}
}

