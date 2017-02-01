#NGineer.NET

NGineer.NET is an advanced hierarchal object builder exploiting some of the latest technologies available in C#3.0 and .NET 4.0. It has been heavily inspired by NBuilder, though designed to overcome what some of the perceived deficiencies are in existing test object builders.

**DISCLAIMER: This was started as a personal side project and is not actively maintained. Use at own risk**

NGineer.NET was designed with the following goals in mind:

* Be useful by default. NGineer.NET should not require a lot of code to build a simple (or even a complex) class
* Allow easy re-use and extension of builders. Similar test data is often required for multiple tests, so allow this to be easily and safely shared and overridden as required
* Make all object generation 100% reproducible. Running the same test should create the same object hierarchy (unless specifically configured otherwise)

The key features of NGineer.NET are:

* "Fluent" api allows easy configuration of builders
* Build any (supported) type with a single builder. No need to create an individual builder per type
* Automatic navigation and construction of object hierarchies. No need to manually define builders to build the properties on your class. Simply register some appropriate generators and let the NGineer figure it out. Default build depth is 5 levels deep, though this is easily configured using the SetMaximumDepth method on the builder object
* Automatic construction of lists, arrays, enumerable and other collections. The number of elements (min to max) each list contains is also configurable using the builder object
* Override generation of individual properties and fields using the AfterConstructionOf<> syntax
* Perform operations on objects after their construction and population is complete using the AfterPopulationOf<> syntax
* Easy support added for new types using the IGenerator interface


## Technical details

* Written for .NET 3.5 & 4, including support for Silverlight and Mono (last tested cira 2010!)
* Contains both micro benchmarks and unit tests to ensure correct behaviour

Further documentation is available on [the wiki](http://ngineer.tiddlyspot.com/#readOnly:yes)