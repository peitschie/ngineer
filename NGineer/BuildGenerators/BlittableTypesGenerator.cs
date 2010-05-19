using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NGineer.BuildHelpers;
using NGineer.Utils;

namespace NGineer.BuildGenerators
{
    /// <summary>
    /// See http://msdn.microsoft.com/en-us/library/75dwhxf7.aspx
    /// </summary>
    public class BlittableTypesGenerator : IGenerator
    {
        #region Byte conversion cache

        private static readonly Type[] NonBitConverterTypes = new []
            {
                typeof (sbyte),
                typeof (byte),
            };

        private static readonly Type[] BitConverterTypes = new[]
            {
                typeof (double),
                typeof (float),
                typeof (int),
                typeof (uint),
                typeof (long),
                typeof (ulong),
                typeof (short),
                typeof (ushort),
            };

        private static readonly Dictionary<Type, MethodBase> Converters = new Dictionary<Type, MethodBase>();

        private static byte NonBitConverterMethod(byte[] bytes, int startIndex)
        {
            return bytes[0];
        }

        static BlittableTypesGenerator()
        {
            var methods = typeof (BitConverter).GetMethods();
            foreach (var type in BitConverterTypes)
            {
                var method = methods.Where(m => 
                        m.ReturnType == type 
                        && m.GetParameters().Length == 2 
                        && m.GetParameters()[0].ParameterType == typeof(byte[])
                        && m.GetParameters()[1].ParameterType == typeof(int)
                    ).FirstOrDefault();
                if (method == null)
                {
                    throw new InvalidCastException("No converter found for {0}".With(type));
                }
                Converters.Add(type, method);
            }
			// call our methods to shut up the compilers!
			NonBitConverterMethod(new byte[1], 0);
            var nonBitMethod = typeof(BlittableTypesGenerator).GetMethod("NonBitConverterMethod", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if(nonBitMethod == null)
            {
                throw new MissingMethodException(typeof(BlittableTypesGenerator).Name, "NonBitConverterMethod");
            }
            foreach (var type in NonBitConverterTypes)
            {
                Converters.Add(type, nonBitMethod);
            }
        }
        #endregion

        private readonly Random _random;

        public BlittableTypesGenerator(int seed)
        {
            _random = new Random(seed);
        }

        #region IGenerator Members

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return Converters.ContainsKey(type);
        }

        public object Create(Type type, IBuilder builder, BuildSession session)
        {
            int size = Marshal.SizeOf(type);
            var bytes = new byte[size];
            _random.NextBytes(bytes);
            return Converters[type].Invoke(null, new object[] {bytes, 0});
        }

        public void Populate(Type type, object obj, IBuilder builder, BuildSession session)
        {
        }

        #endregion
    }
}