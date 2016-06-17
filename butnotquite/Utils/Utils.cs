namespace butnotquite.Utils
{
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;

    internal static class Utils
    {
        internal static T MakeDeepCopy<T>(T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();

                formatter.Serialize(ms, obj);

                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
