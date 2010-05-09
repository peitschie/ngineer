using System;
using NGineer.Generators;
using NGineer.Utils;

namespace NGineer.BuildHelpers
{
    public class BuildSession : BaseBuilder, IDisposable
    {
        private readonly BaseBuilder _parent;
		private bool _closed;

        public BuildSession(BaseBuilder parent)
        {
            _parent = parent;
        }
		
		public int BuildDepth { get; private set; }
		
		public override object DoBuild (Type type, BuildSession session)
		{
			AssertNotDisposed();
			throw new System.NotImplementedException();
		}
		
		public override IBuilder CreateNew (BuildSession session)
		{
			return _parent.CreateNew(session);
		}
		
        public override object Build(Type type)
        {
			AssertNotDisposed();
			BuildDepth++;
            var obj = _parent.DoBuild(type, this);
			BuildDepth--;
			return obj;
        }

        public override IBuilder WithGenerator(IGenerator generator)
        {
            _parent.WithGenerator(generator);
            return this;
        }

        public override IBuilder SetMaximumDepth(int depth)
        {
            _parent.SetMaximumDepth(depth);
            return this;
        }

		public override IBuilder SetCollectionSize(int minimum, int maximum)
		{
			_parent.SetCollectionSize(minimum, maximum);
			return this;
		}
		
        public override IBuilder CreateNew()
        {
            return _parent.CreateNew(this);
        }

        public override TType Build<TType>()
        {
			AssertNotDisposed();
            return (TType) Build(typeof (TType));
        }

        public override IBuilder SetValuesFor<TType>(Action<TType> setter)
        {
            _parent.SetValuesFor(setter);
            return this;
        }

        public override IBuilder SetValuesFor<TType>(Func<TType, TType> setter)
        {
            _parent.SetValuesFor(setter);
            return this;
        }

        public override IBuilder SetValuesFor<TType>(Action<TType, IBuilder> setter)
        {
            _parent.SetValuesFor(setter);
            return this;
        }

        public override IBuilder SetValuesFor<TType>(Func<TType, IBuilder, TType> setter)
        {
            _parent.SetValuesFor(setter);
            return this;
        }

        public override IBuilder Sealed()
        {
            _parent.Sealed();
            return this;
        }
		
		public bool Disposed { get { return _closed; } }
		
		public void Dispose()
		{
			_closed = true;	
		}
		
		private void AssertNotDisposed()
		{
			if(_closed)
			{
				throw new InvalidOperationException("Attempting to use disposed session");
			}
		}
    }
}