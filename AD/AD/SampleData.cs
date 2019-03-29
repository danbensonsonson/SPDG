using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace AD
{
    public class SampleData
    {
        public static ReadOnlyCollection<string> FirstNames;
        public static ReadOnlyCollection<string> LastNames;
        public static ReadOnlyCollection<string> Accounts;
        private static Random _randomGen = new Random();

        public static void PrepareSampleCollections()
        {
            Accounts = createSampleCollection("Accounts.csv");
            FirstNames = createSampleCollection("FirstName.csv");
            LastNames = createSampleCollection("LastName.csv");

        }

        private static ReadOnlyCollection<string> createSampleCollection(string csvFileName)
        {
            var reader = new StreamReader(File.OpenRead(@"SampleData\" + csvFileName));
            List<string> listA = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                listA.Add(line);
            }

            return new ReadOnlyCollection<string>(listA);
        }

        public static IList<string> GetRandomNonRepeatingValues(IList<string> collection, int valuesCount, int defaultValueIndex = 0)
        {
            var indexes = new List<int>();
            var collectionCount = collection.Count;

            for (var i = 0; i < collectionCount; i++)
            {
                indexes.Add(i);
            }

            indexes.Shuffle();

            var randomValues = new List<string>();
            for (var i = 0; i < valuesCount; i++)
            {
                if (i >= collectionCount)
                {
                    randomValues.Add(collection[defaultValueIndex]);
                }
                else
                {
                    randomValues.Add(collection[indexes[i]]);
                }
            }

            return randomValues;
        }

        public static string GetRandomName(IList<string> primaryCollection, IList<string> secondaryCollection, IList<string> tertiaryCollection, ref int attempt, out string baseName)
        {
            string retVal = "";
            attempt++;
            baseName = "";

            if ((secondaryCollection == null && tertiaryCollection == null) || attempt < 5)
            {
                baseName = GetSampleValueRandom(primaryCollection);
                retVal = baseName;
            }
            else if (tertiaryCollection == null)
            {
                baseName = GetSampleValueRandom(primaryCollection);
                retVal = string.Format("{0} {1}", baseName, GetSampleValueRandom(secondaryCollection));
            }
            else
            {
                if (attempt < 10)
                {
                    baseName = GetSampleValueRandom(primaryCollection);
                    retVal = string.Format("{0} {1}", baseName, GetSampleValueRandom(secondaryCollection));
                }
                else if (attempt < 20)
                {
                    baseName = GetSampleValueRandom(primaryCollection);
                    retVal = string.Format("{0} {1}", baseName, GetSampleValueRandom(tertiaryCollection));
                }
                else
                {
                    baseName = GetSampleValueRandom(primaryCollection);
                    retVal = string.Format("{0} {1} {2}", baseName, GetSampleValueRandom(secondaryCollection), GetSampleValueRandom(tertiaryCollection));
                }
            }



            if (attempt > 40)
            {
                retVal += string.Format("{0} {1}", retVal, _randomGen.Next(10000).ToString());
            }

            return retVal;
        }

        public static string GetSampleValueAt(List<string> sampleCollection, int index)
        {
            return sampleCollection[index];
        }

        public static string GetSampleValueRandom(IList<string> sampleCollection)
        {
            try
            {
                int randomNumber = _randomGen.Next(0, sampleCollection.Count - 1);

                return sampleCollection[randomNumber];
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }


}
