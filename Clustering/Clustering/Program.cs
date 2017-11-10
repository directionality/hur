using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.FileIO;
using System.IO;

namespace Clustering
{
    class Program
    {
        public class Pair<T, U>
        {
            public Pair()
            {
            }

            public Pair(T first, U second)
            {
                this.First = first;
                this.Second = second;
            }

            public T First { get; set; }
            public U Second { get; set; }
        };

        class Hurricane
        {
         public            string id;
         public            int[]   bearing;

         private Dictionary<string, Pair<int,string>> cacheLCS=new Dictionary<string, Pair<int, string>>();

         public int LCS(Hurricane b, out string lcsMatch)
            {
                // Try to find this LCS pair from cache for speed
                int lcs;
                if (cacheLCS.TryGetValue(b.id, out Pair<int, string> prev))
                {
                    // Found a match
                    lcsMatch = prev.Second;
                    lcs = prev.First;
                }
                else
                {
                    // Not computed yet
                    lcs=findInt(bearing, b.bearing, out lcsMatch);
                    // Add computation
                    cacheLCS.Add(b.id, new Pair<int, string>(lcs,lcsMatch));
                }

                // Return new value
                return lcs;
            }

            public static int findInt(int[] Aint, int[] Bint, out string lcsMatch)
            {
                lcsMatch = "";
                int[,] LCS = new int[Aint.Length + 1, Bint.Length + 1];
                String[,] solution = new String[Aint.Length + 1, Bint.Length + 1];

                // if A is null then LCS of A, B =0
                for (int i = 0; i <= Bint.Length; i++)
                {
                    LCS[0, i] = 0;
                    solution[0, i] = "0";
                }

                // if B is null then LCS of A, B =0
                for (int i = 0; i <= Aint.Length; i++)
                {
                    LCS[i, 0] = 0;
                    solution[i, 0] = "0";
                }

                for (int i = 1; i <= Aint.Length; i++)
                {
                    for (int j = 1; j <= Bint.Length; j++)
                    {
                        if (Aint[i - 1] == Bint[j - 1])
                        {
                            LCS[i, j] = LCS[i - 1, j - 1] + 1;
                            solution[i, j] = "diagonal";
                        }
                        else
                        {
                            LCS[i, j] = Math.Max(LCS[i - 1, j], LCS[i, j - 1]);
                            if (LCS[i, j] == LCS[i - 1, j])
                            {
                                solution[i, j] = "top";
                            }
                            else
                            {
                                solution[i, j] = "left";
                            }
                        }
                    }
                }

                int lcsCnt = LCS[Aint.Length, Bint.Length];

                // below code is to just print the result
                String x = solution[Aint.Length, Bint.Length];
                String answer = "";
                int a = Aint.Length;
                int b = Bint.Length;
                while (x != "0")
                {
                    if (solution[a, b] == "diagonal")
                    {  
                        if (answer=="")
                        { answer = Aint[a - 1].ToString();
                        } else
                        { answer = Aint[a - 1].ToString() + "," + answer;
                        }
                        
                        a--;
                        b--;
                    }
                    else if (solution[a, b] == "left")
                    {
                        b--;
                    }
                    else if (solution[a, b] == "top")
                    {
                        a--;
                    }
                    x = solution[a, b];
                }
                lcsMatch=answer;

                for (int i = 0; i <= Aint.Length; i++)
                {
                    for (int j = 0; j <= Bint.Length; j++)
                    {
                        //Console.WriteLine(" " + LCS[i, j]);
                    }
                    //Console.WriteLine();
                }
                return lcsCnt;
            }
        };
        class Cluster
        {
            public int             id;
            public List<Hurricane> set = new List<Hurricane>();

            public void WriteIds(StreamWriter text)
            {
                string line="";
                foreach (Hurricane i in set)
                {
                    if (line== "")
                    { line = i.id;
                    } else
                    { line = line + "," + i.id.ToString();
                    }
                }

                text.WriteLine(line);
            }

            public void Print(StreamWriter text, bool withAveLCS)
            {
                // Print id of cluster and number of hurricanes in the cluster
                string line = "id=" + id.ToString().PadLeft(5) + " cnt=" + set.Count.ToString().PadLeft(5);

                if (withAveLCS)
                {
                    double aveLCS = AveLCS();
                    line = line + " aveLCS=" + aveLCS.ToString().PadLeft(15)+ ": ";
                }
                else
                {   line = line + ": ";
                }
                foreach (Hurricane i in set)
                {
                    line = line + i.id.ToString() + ",";
                }

                // If only two go ahead and print LCS
                if (set.Count==2)
                {   // Get hurricanes
                    Hurricane a = set[0];
                    Hurricane b = set[1];
                    // Get LCS
                    string lcsMatch;
                    a.LCS(b, out lcsMatch);
                    line = line + " LCS:" + lcsMatch;
                }

                text.WriteLine(line);
            }

            public void Combine(Cluster b)
            {   // Combine cluster b into this cluster
                foreach (Hurricane i in b.set)
                {   set.Add(i);
                }
            }

            public void CopyTo(Cluster to)
            {   // Copy this cluster to a new one making a duplicate except for the hurricane objects
                to.id = id;
                to.set = new List<Hurricane>();     // New list
                foreach (Hurricane i in set)
                {   // Point to existing hurricane
                    to.set.Add(i);
                }
            }

            public double AveLCS()
            {   // Compute average LCS map for the cluster
                double lcsSum = 0;
                double lcsCnt = 0;

                // Loop over all pairs (excluding diagonals and swapped pairs)
                for (int i = 0; i < set.Count; i++)
                {
                    for (int j = i + 1; j < set.Count; j++)
                    {   string lcsMatch;
                        int lcs = set[i].LCS(set[j], out lcsMatch);

                        // Save sum and count
                        lcsSum = lcsSum + lcs;
                        lcsCnt = lcsCnt + 1.0;
                    }
                }

                // Compute and return the average
                double lcsAve = lcsSum / lcsCnt;
                return lcsAve;
            }

            public int ScoreLCS(Cluster b)
            {   // Score based on minimum LCS or least common match
                int lcsMin = int.MaxValue;
                // Loop over all pairs
                foreach (Hurricane i in set)
                {   foreach (Hurricane j in b.set)
                    {   // Perform largest command substring
                        string lcsMatch;
                        int lcs = i.LCS(j, out lcsMatch);
                        if (lcs < lcsMin)
                        { lcsMin = lcs;
                        }
                    }
                }
                return lcsMin;
            }
        };

        class ClusterList
        {
            public List<Cluster> clusters = new List<Cluster>();

            private static int CompareByCount(Cluster a, Cluster b)
            {   // If count is equil sort best LCS first
                if (b.set.Count == a.set.Count)
                {   double lcsA = a.AveLCS();
                    double lcsB = b.AveLCS();
                    if (lcsA>lcsB)
                    { return -1;
                    } else if (lcsB>lcsA)
                    { return +1;
                    }
                    else
                    { return 0;
                    }
                }
                else
                { // Sort function which puts highest count first
                    return b.set.Count - a.set.Count;
                }
            }

            public void SortByCount()
            {   // Sort by number of items in cluster
                clusters.Sort(CompareByCount);
            }

            public int ClusterCount()
            {   // Return number of clusters
                return clusters.Count;

            }

            public void WriteIds(StreamWriter text)
            {
                foreach (Cluster i in clusters)
                {
                    i.WriteIds(text);
                }
            }

            public void Print(StreamWriter text, bool withAveLCS=false)
            {   // Print all clusters
                foreach (Cluster i in clusters)
                {
                    i.Print(text,withAveLCS);
                }
            }

            public ClusterList ConsolidateLCS(int minScoreAllowed, out int lcsBest)
            {   // Consolidate best LCS by combining 1 cluster with another
                int iBest = -1, jBest = -1;
                lcsBest = 0;
                for (int i = 0; i < clusters.Count; i++)
                {
                    for (int j = i + 1; j < clusters.Count; j++)
                    {
                        int lcs = clusters[i].ScoreLCS(clusters[j]);
                        if (lcs>=lcsBest)
                        {   lcsBest = lcs;
                            iBest = i;
                            jBest = j;
                        }
                    }
                }

                // Return this list unchanged
                if (lcsBest < minScoreAllowed)
                {   return this;
                } else
                {   // Get best candidates
                    Cluster iCluster = clusters[iBest];
                    Cluster jCluster = clusters[jBest];

                    // Combine clusters
                    ClusterList newClusters = new ClusterList();
                    for (int i = 0; i < clusters.Count; i++)
                    {   // Create new cluster copying the old one
                        Cluster cluster = new Cluster();
                        clusters[i].CopyTo(cluster);

                        // If this is the i match
                        if (i == iBest)
                        { // Combine j with cluster i
                            cluster.Combine(jCluster);
                            // Add combined cluster
                            newClusters.clusters.Add(cluster);
                        }
                        else if (i == jBest)
                        { // Do nothing already combined in a previous step above
                        }
                        else
                        { // Need to add the cluster to the list
                            newClusters.clusters.Add(cluster);
                        }
                    }
                    // Return new cluster list
                    return newClusters;
                }
            }

            public void PrintMatrix(StreamWriter text)
            {   // Form matrix
                double[,] lcsMatrix; // Match is largest common subsequence
                lcsMatrix = new double[clusters.Count, clusters.Count];

                // Loop over i
                for (int i = 0; i < clusters.Count; i++)
                {   // Diagonal is always zero LCS(A,A)
                    lcsMatrix[i, i] = 0;
                    // Loop over j
                    for (int j = i + 1; j < clusters.Count; j++)
                    {   // Get LCS
                        double lcs = clusters[i].ScoreLCS(clusters[j]);
                        // LCS(i,j) = LCS(j,i)
                        lcsMatrix[i, j] = lcs;
                        lcsMatrix[j, i] = lcs;
                    }
                }

                // Print matrix
                for (int i = 0; i < clusters.Count; i++)
                {
                    string line="";
                    for (int j = 0; j < clusters.Count; j++)
                    {   string entry = lcsMatrix[i, j].ToString().PadLeft(3);
                        line = line + entry;
                    }
                    text.WriteLine(line);
                }
            }
        }

        public static void Main(String[] args)
        {
            //String A = "ACBDEA";
            //String B = "ABCDA";
            //int[] Aint = { 12, 17, 15, 11, 6, 1, 2, 5, 5, 7, 11, 10, 12, 12, 15, 15, 17, -17, -18, 17, 17, 16, 13, 9, 6, 4, 4, 7, 9, 9, 9, 8, 14, 9, -1, 9, -16, 3, 3, 8, -11, -8, 9, -10, 9, -3, -9, -1, 3, -8, 1 };
            //int[] Bint = { 11, 11, 10, 9, 7, 4, 4, 3, 3, 4, 4, 3, 4, 1, -3, -3, 1, 5, 10, 16, 17, -18, 14, 9, 0, -2, -3, -2, -1, -1, -1, 0, 0, 0, 1, 1, 1, 2, 3, 4, 6, 8, 10, 11, 10, 10, 10, 8, 10, 10, 8, 8, 9, 8, 6, 8, 9, -18, 14, -9 };



            //int tempResult = findInt(Aint, Bint);
            //UNNAMED_01_1851,0,-1,0,1,-1,0,0,2,4,7,9,9,10,16,


            List<SortedSet<string>> resultSet = new List<SortedSet<string>>();

            using (TextFieldParser parser = new TextFieldParser(@"C:\Users\User\Desktop\HurricaneProject\sequences.txt"))
            {
                parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited;
                parser.SetDelimiters(",");

                Dictionary<string, int[]> sequenceDict = new Dictionary<string, int[]>();

                while (!parser.EndOfData)
                {
                    //Process row
                    string[] fields = parser.ReadFields();
                    string name = fields[0];
                    int length = fields.Length - 1;
                    while(fields[length]=="")
                    {   length--;
                    }
                    int[] tempInt = new int[length];
                    for (int i = 1; i <= length; i++)
                    {
                        tempInt[i - 1] = Int32.Parse(fields[i]);
                    }
                    sequenceDict.Add(name, tempInt);
                }

/*                int[] Aint = sequenceDict["UNNAMED_05_1864"];
                int[] Bint = sequenceDict["HUMBERTO_09_2007"];

                string lcsMatch;
                int lcs = Hurricane.findInt(Aint, Bint, out lcsMatch);*/



                StreamWriter logFile = new StreamWriter("C:\\Users\\User\\Desktop\\HurricaneProject\\ClusterLog.txt",false);

                ClusterList fullSet = new ClusterList();

                // Build set of clusters
                for (int i = 0; i < sequenceDict.Keys.Count; i++)
                {

     //if (i > 200) break;
                    string nameA = sequenceDict.Keys.ElementAt(i);
                    int[] intA = sequenceDict[sequenceDict.Keys.ElementAt(i)];

                    Hurricane hur = new Hurricane();
                    hur.id = nameA;
                    hur.bearing = intA;

                    Cluster cluster = new Cluster();
                    cluster.id = i;
                    cluster.set.Add(hur);

                    fullSet.clusters.Add(cluster);
                }

                // Test print matrix
                Console.WriteLine("Iter 0 (full matrix)");
                logFile.WriteLine("Iter 0 (full matrix)");
                fullSet.Print(logFile);
                //fullSet.PrintMatrix(logFile);

                // Reqiured minimum clustering score
                int minScoreAllowed = 5;

                // Begin clustering on full list
                ClusterList curCluster = fullSet;
                // Iterate
                for (int i = 1; ; i++)
                {
                    // Try to find two clusters to combine
                    int lcsScore;
                    ClusterList newCluster = curCluster.ConsolidateLCS(minScoreAllowed, out lcsScore);

                    // No more good clustering
                    if (newCluster==curCluster)
                    {   // No more clusters match to within minScoreAllowed
                        logFile.WriteLine("Clustering completed sorting by cluster size");
                        curCluster.SortByCount();
                        curCluster.Print(logFile, true);

                        // Write cluster groups
                        StreamWriter grpFile = new StreamWriter("C:\\Users\\User\\Desktop\\HurricaneProject\\ClusterGrps.txt", false);
                        curCluster.WriteIds(grpFile);
                        grpFile.Close();
                        break;
                    } else
                    {   // Accept the new cluster as the current
                        curCluster = newCluster;
                    }

                    // Log progress
                    logFile.WriteLine("Iter " + i.ToString() + " clusters " + curCluster.ClusterCount().ToString() + " LCSscore=" + lcsScore.ToString());
                    Console.WriteLine("Iter " + i.ToString() + " clusters " + curCluster.ClusterCount().ToString() + " LCSscore=" + lcsScore.ToString());
                    curCluster.Print(logFile);

                    // Print if reasonable
                    // curCluster.PrintMatrix(logFile);
                }
                Console.WriteLine("Done");

                logFile.Close();
            }

        }
    }
}
