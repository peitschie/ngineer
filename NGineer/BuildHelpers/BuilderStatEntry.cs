using System;

namespace NGineer.BuildHelpers
{
    public class BuilderStatEntry
    {
        private readonly Type _type;
        private readonly int _count;

        public BuilderStatEntry(Type type, int count)
        {
            _type = type;
            _count = count;
        }

        public Type Type
        {
            get { return _type; }
        }

        public int Count
        {
            get { return _count; }
        }

        public bool Equals(BuilderStatEntry other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._type, _type) && other._count == _count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (BuilderStatEntry)) return false;
            return Equals((BuilderStatEntry) obj);
        }

        public override string ToString()
        {
            return base.ToString() + "{" + Type +"," + Count + "}";
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_type.GetHashCode()*397) ^ _count;
            }
        }
    }
}