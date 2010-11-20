using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NGineer.Internal;

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

        private static byte NonBitConverterMethod_Byte(byte[] bytes, int startIndex)
        {
            return bytes[0];
        }

        private static sbyte NonBitConverterMethod_SByte(byte[] bytes, int startIndex)
        {
            return (sbyte)bytes[0];
        }

        static BlittableTypesGenerator()
        {
            var methods = typeof(BitConverter).GetMethods();
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
                    throw new InvalidCastException(string.Format("No converter found for {0}", type));
                }
                Converters.Add(type, method);
            }
            // call our methods to shut up the compilers
            NonBitConverterMethod_Byte(new byte[1], 0);
            NonBitConverterMethod_SByte(new byte[1], 0);
            Converters.Add(NonBitConverterTypes[0], typeof(BlittableTypesGenerator).GetMethod("NonBitConverterMethod_SByte", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));
            Converters.Add(NonBitConverterTypes[1], typeof(BlittableTypesGenerator).GetMethod("NonBitConverterMethod_Byte", BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy));
        }
        #endregion

        #region IGenerator Members

        public bool GeneratesType(Type type, IBuilder builder, BuildSession session)
        {
            return Converters.ContainsKey(type);
        }

        public ObjectBuildRecord CreateRecord(Type type, IBuilder builder, BuildSession session)
        {
            int size = Marshal.SizeOf(type);
            var bytes = new byte[size];
            session.Random.NextBytes(bytes);
            return new ObjectBuildRecord(type, Converters[type].Invoke(null, new object[] {bytes, 0}), false);
        }

        #endregion
    }
}