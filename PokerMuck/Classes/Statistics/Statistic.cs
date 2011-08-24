using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PokerMuck
{
    public class Statistic
    {
        private StatisticsData mainData;
        public StatisticsData MainData { get { return mainData; } }

        private List<StatisticsData> subData;
        public List<StatisticsData> SubData { get { return subData; } }

        private String category;
        public String Category { get { return category; } }

        public int Order { get; set; }
        public String Name { get { return mainData.Name; } }

        public Statistic(StatisticsData mainData, String category)
        {
            this.mainData = mainData;
            this.category = category;
            this.Order = 0;
            this.subData = new List<StatisticsData>();
        }

        public Statistic(StatisticsData mainData)
            : this(mainData, "")
        {
        }

        public Statistic(StatisticsData mainData, String category, List<StatisticsData> subData)
            : this(mainData, category)
        {
            this.subData = subData;
        }


        public static Statistic CreateUnknown(String name, string category)
        {
            return new Statistic(new StatisticsUnknownData(name), category);
        }

        public bool HasSubStatistics()
        {
            return subData.Count > 0;
        }

        public StatisticsData FindSubStatistic(String name)
        {
            foreach (StatisticsData data in subData)
            {
                if (data.Name == name)
                {
                    return data;
                }
            }

            return null;
        }

        public void AddSubStatistic(StatisticsData subStatistic){
            subData.Add(subStatistic);
        }

        public Statistic Average(String category, int precision, params Statistic[] stats)
        {
            int paramsCount = stats.Count<Statistic>();

            // Populate param list for main data
            StatisticsData[] mainStats = new StatisticsData[paramsCount];
            for (int i = 0; i < paramsCount; i++)
            {
                mainStats[i] = stats[i].MainData;
            }

            // Compute average for main data
            StatisticsData mainDataAverage = MainData.Average(MainData.Name, precision, mainStats);

            // Assume the sub statistics are ordered and are of the same size of the other sub statistics
            List<StatisticsData> subDataAverages = new List<StatisticsData>();
            for (int i = 0; i < SubData.Count; i++)
            {
                // Populate a param list with the values for each sub value
                StatisticsData[] subStats = new StatisticsData[paramsCount];
                for (int j = 0; j < paramsCount; j++)
                {
                    Trace.Assert(SubData.Count == stats[j].SubData.Count, "Trying to compute the average of sub statistics values when their sizes are different.");
                    subStats[j] = stats[j].SubData[i];
                }

                subDataAverages.Add(SubData[i].Average(SubData[i].Name, precision, subStats));
            }
            
            return new Statistic(mainDataAverage, category, subDataAverages);
        }

        public override String ToString()
        {
            String result = Name + " (" + Category + "): " + mainData.GetValue();
            foreach (StatisticsData data in subData)
            {
                result += "\n" + data.Name + ": " + data.GetValue();
            }
            return result;
        }
    }
}
