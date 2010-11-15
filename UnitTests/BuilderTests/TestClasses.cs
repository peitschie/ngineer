using System;
using System.Collections.Generic;

namespace NGineer.UnitTests.BuilderTests
{
    public class BuilderOrderLevel1
    {
        public int B;
        public int Bp { get; set;}
    }

    public class BuilderOrderLevel2 : BuilderOrderLevel1
    {
        public int A;
        public int Ap { get; set; }
        public int C;
        public int Cp { get; set; }
    }

    public class BuilderOrderLevel3 : BuilderOrderLevel2
    {
        public int D;
        public int Dp { get; set; }
    }

    public class CountsPropertySets
    {
        private int _somePropertySets;
        public int GetSomePropertySets()
        {
            return _somePropertySets;
        }

        private int _someProperty;
        public int SomeProperty
        {
            get { return _someProperty; }
            set
            {
                _someProperty = value;
                _somePropertySets++;
            }
        }

        private int _recursivePropertySets;
        public int GetRecursivePropertySets()
        {
            return _recursivePropertySets;
        }

        private CountsPropertySets _recursiveProperty;
        public CountsPropertySets RecursiveProperty
        {
            get { return _recursiveProperty; }
            set
            {
                _recursiveProperty = value;
                _recursivePropertySets++;
            }
        }
    }


    public class RecursiveClass
    {
        public int IntProperty { get; set; }
        public RecursiveClass RecursiveReference { get; set; }
    }

    public class SimpleClassParent
    {
        public SimpleClass[] Child { get; set; }
        public SimpleClass[] UniqueChild { get; set; }
    }

    public class SimpleClass
    {
        public int IntProperty { get; set; }
        public string StringProperty { get; set; }
        public TestClass2 TestClass2Property { get; set; }
		
		public int IntField;
        public string StringField;
        public TestClass2 TestClass2Field;
    }

    public class TestClass2
    {
		public string StringProperty { get; set; }
    }

    public class ClassWithNullableInt
    {
        public int? Property1 { get; set; }
    }

    public class ClassWithNullableDateTime
    {
        public DateTime? Property1 { get; set; }
    }

    public class InheritsFromClassWithNullableDateTime : ClassWithNullableDateTime
    {
    }

    public class InheritsFromClassWithNullableDateTime2 : ClassWithNullableDateTime
    {
    }

    public class TestClass
    {
        public int Property1 { get; set; }

        public TestClass2 Property2 { get; set; }
    }
	
	public class TestClassThreeDeep
	{
		public TestClass PropertyTestClass { get; set; }
	}
	
	public class TestClassFourDeep
	{
		public TestClassThreeDeep PropertyTestClass { get; set; }
    }
	
	public enum SimpleEnum
	{
		First,
		Second,
		Third
	}
	
	public class ClassWithEnumAndProperties
	{
		public SimpleEnum EnumProperty { get; set;}
		public string StringProperty { get; set; }
	}

    public class ListOfClassWithEnumAndProperties
    {
         public IList<ClassWithEnumAndProperties> Entries { get; set; }
    }
	
	public class TopLevel : IInterfaceTest
	{
		public int TopLevelProperty { get; set; }
		public int InterfaceProperty { get; set; }
	}
	
	public class SecondLevel : TopLevel
	{
		public string SecondLevelProperty { get; set; }
	}
	
	public class AnotherLevel : IInterfaceTest
	{
		public int InterfaceProperty { get; set; }
	}
	
	public interface IInterfaceTest
	{
		int InterfaceProperty { get; set; }
	}
	
	public class CombinedClasses
	{
		public TopLevel Property1 { get; set; }
		public SecondLevel Property2 { get; set; }
		public AnotherLevel Property3 { get; set; }
	}
}
