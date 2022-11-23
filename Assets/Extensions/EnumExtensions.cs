using System;
using System.IO;
using System.Linq;

namespace Extensions
{
    public static class EnumExtensions
    {
        public static T Random <T> () where T : Enum
        {
            Array values = Enum.GetValues (typeof(T));

            return (T) values.GetValue (UnityEngine.Random.Range (0, values.Length));
        }

        public static T RandomIgnore <T> (params T[] ignore) where T : Enum
        {
            // get the list of values
            T[] values = (T[]) Enum.GetValues (typeof(T));
            // ignore the values requested
            values = values.Except (ignore).ToArray ();

            if (values.Length == 0)
                throw new InvalidDataException ("Trying to get a random value from an enum that doesn't have enough members");
            
            // finally get a random value from the list left
            return (T) values.GetValue (UnityEngine.Random.Range (0, values.Length));
        }

        public static T RandomArray <T> (params T [] values) where T : Enum
        {
            return values [UnityEngine.Random.Range (0, values.Length)];
        }
    }
}