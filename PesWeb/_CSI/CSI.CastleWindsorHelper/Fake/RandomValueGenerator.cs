using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/*!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
 * have bugs when return value is ICollection<T>.
 * it does not return anything.
 */

namespace CSI.CastleWindsorHelper.Fake
{
    public class RandomValueGenerator : IFakeValueGenerator
    {
        private Random Rand = new Random();
        private static string[] RandStr = StringSeed.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        private int RandomElementCount()
        {
            return RandomInteger(5, 200);
        }
        private string RandomString()
        {
            return RandStr[Rand.Next() % RandStr.Length];
        }
        private DateTime RandomDateTime()
        {
            return DateTime.FromBinary(DateTime.Now.ToBinary() - (10000000000000 * Rand.Next() % 100));
        }
        private int RandomInteger(int min, int max)
        {
            return min >= max ? min : min + (Rand.Next() % (max - min));
        }
        private float RandomFloat(float min, float max)
        {
            float x = (((float)RandomInteger(ushort.MinValue, ushort.MaxValue)) / 17.0f);
            if (x > max)
                x = max;
            if (x < min)
                x = min;
            return x;
        }

        public T GenerateValuesFromType<T>()
        {
            return (T)GenerateValuesFromType(typeof(T));
        }

        public object GenerateValuesFromType(Type objectType)
        {
            if (objectType.IsGenericType)
            {
                Type genericDefType = objectType.GetGenericTypeDefinition();
                if (null != genericDefType)
                {
                    // Nullable
                    if (typeof(Nullable<>).IsAssignableFrom(genericDefType))
                        return GenerateValuesFromType(objectType.GetGenericArguments()[0]);

                    // ISet
                    Type[] interfaces = objectType.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISet<>)).ToArray();
                    if (interfaces.Length > 0)
                    {
                        Type elementType = objectType.GetGenericArguments()[0];
                        MethodInfo mi = objectType.GetMethod("Add");

                        var obj = Activator.CreateInstance(objectType);
                        int loopCount = RandomElementCount();
                        for (int i = 0; i < loopCount; i++)
                        {
                            var element = GenerateValuesFromType(elementType);
                            if (null != element)
                                mi.Invoke(obj, new object[] { element });
                        }
                        return obj;
                    }
                }
            }

            // Array
            if (objectType.IsArray)
            {
                Type elementType = objectType.GetElementType();
                int loopCount = RandomElementCount();
                Array obj = (Array)Activator.CreateInstance(objectType, loopCount);
                for (int i = 0; i < loopCount; i++)
                {
                    var element = GenerateValuesFromType(elementType);
                    if (null != element)
                        obj.SetValue(element, i);
                }
                return obj;
            }

            // IQueryable
            if (typeof(IQueryable).IsAssignableFrom(objectType))
            {

                Type elementType = objectType.IsGenericType ? objectType.GetGenericArguments()[0] : typeof(string);
                Type containerType = objectType.IsInterface ? typeof(List<>).MakeGenericType(elementType) : objectType;

                IList obj = (IList)Activator.CreateInstance(containerType);
                int loopCount = RandomElementCount();
                for (int i = 0; i < loopCount; i++)
                {
                    var element = GenerateValuesFromType(elementType);
                    if (null != element)
                        obj.Add(element);
                }

                return obj.AsQueryable();
            }

            // Dictionary
            if (typeof(IDictionary).IsAssignableFrom(objectType))
            {
                var generic = objectType.GetGenericArguments();
                Type keyType = objectType.IsGenericType ? generic[0] : typeof(string);
                Type valueType = objectType.IsGenericType ? generic[1] : typeof(string);
                Type containerType = objectType.IsInterface ? typeof(Dictionary<,>).MakeGenericType(generic) : objectType;

                IDictionary obj = (IDictionary)Activator.CreateInstance(containerType);
                int loopCount = RandomElementCount();
                for (int i = 0; i < loopCount; i++)
                {
                    var key = GenerateValuesFromType(keyType);
                    if (null != key)
                        if (false == obj.Contains(key))
                            obj.Add(key, GenerateValuesFromType(valueType));
                }

                return obj;
            }

            // List
            if (typeof(IList).IsAssignableFrom(objectType))
            {
                Type elementType = objectType.IsGenericType ? objectType.GetGenericArguments()[0] : typeof(string);
                Type containerType = objectType.IsInterface ? typeof(List<>).MakeGenericType(elementType) : objectType;

                IList obj = (IList)Activator.CreateInstance(containerType);
                int loopCount = RandomElementCount();
                for (int i = 0; i < loopCount; i++)
                {
                    var element = GenerateValuesFromType(elementType);
                    if (null != element)
                        obj.Add(element);
                }

                return obj;
            }

            // string
            if (typeof(string).IsAssignableFrom(objectType))
                return RandomString();

            // datetime
            if (typeof(DateTime).IsAssignableFrom(objectType))
                return RandomDateTime();

            // boolean
            if (typeof(bool) == objectType)
                return 0 == (RandomInteger(0, 9) % 2);

            // integer
            if (typeof(sbyte) == objectType)
                return (sbyte)RandomInteger(sbyte.MinValue, sbyte.MaxValue);
            if (typeof(byte) == objectType)
                return (byte)RandomInteger(byte.MinValue, byte.MaxValue);
            if (typeof(short) == objectType)
                return (short)RandomInteger(short.MinValue, short.MaxValue);
            if (typeof(ushort) == objectType)
                return (ushort)RandomInteger(ushort.MinValue, ushort.MaxValue);
            if (typeof(int) == objectType)
                return (int)RandomInteger(0, 1000);
            if (typeof(uint) == objectType)
                return (uint)RandomInteger(0, int.MaxValue);
            if (typeof(long) == objectType)
                return (long)RandomInteger(int.MinValue, int.MaxValue);
            if (typeof(ulong) == objectType)
                return (ulong)RandomInteger(0, int.MaxValue);

            // floating points
            if (typeof(float) == objectType)
                return RandomFloat(-1000f, 1000f);
            if (typeof(double) == objectType)
                return (double)RandomFloat(0f, 10000f);
            if (typeof(decimal) == objectType)
                return (decimal)RandomFloat(0f, 1000f);

            if (objectType.IsClass)
            {
                var obj = Activator.CreateInstance(objectType);
                foreach (PropertyInfo propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanWrite))
                    propertyInfo.SetValue(obj, GenerateValuesFromType(propertyInfo.PropertyType), null);
                return obj;
            }

            return null;
        }

        private const string StringSeed =
@"Come up to meet you
Tell you I'm sorry
You don't know how lovely you are
I had to find you
Tell you I need you
Tell you I set you apart

Tell me your secrets
And ask me your questions
Oh let's go back to the start
Running in circles; coming up tails
Heads on a silence apart 

Nobody said it was easy
It's such a shame for us to part
Nobody said it was easy
No one ever said it would be this hard
Oh take me back to the start

I was just guessing at numbers and figures
Pulling your puzzles apart
Questions of science; science and progress
Do not speak as loud as my heart

Tell me you love me
Come back and haunt me
Oh and I rush to the start
Running in circles, chasing our tails
Coming back as we are

Nobody said it was easy
Oh it's such a shame for us to part
Nobody said it was easy
No one ever said it would be so hard
I'm going back to the start

Look at the stars
Look how they shine for you
And everything you do
Yeah they were all yellow

I came along
I wrote a song for you
And all the things you do
And it was called yellow

So then I took my turn
Oh what a thing to have done
And it was all yellow

Your skin
Oh yeah your skin and bones
Turn into something beautiful
You know you know I love you so
You know I love you so

I swam across
I jumped across for you
Oh what a thing to do

Cause you were all yellow
I drew a line
I drew a line for you
Oh what a thing to do
And it was all yellow

Your skin
Oh yeah your skin and bones
Turn into something beautiful
And you know
For you I'd bleed myself dry
For you I'd bleed myself dry

It's true
Look how they shine for you

Look at the stars
Look how they shine for you
And all the things that you do";
    }
}
