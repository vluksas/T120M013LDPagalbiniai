using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuessBaseParams
{
    public class Guesser
    {
        private const double FactorOffInsignificance = 0.05;
        private const double MaxBadness = 5f;
        int n;
        int N;
        double D;
        double Wtf;
        /// <summary>
        /// List of guesses sorted by accuracy value. Tuple values are: n1, n2, N1, N2
        /// </summary>
        private SortedList<double, Tuple<int, int, int, int>> results = new SortedList<double, Tuple<int, int, int, int>>();
        /// <summary>
        /// Only those that have 3/4 exact metric results
        /// </summary>
        private SortedList<double, Tuple<int, int, int, int>> hits = new SortedList<double, Tuple<int, int, int, int>>();
        private List<Tuple<int, int, int, int, double>> duplicates = new List<Tuple<int, int, int, int, double>>();
        public SortedList<double, Tuple<int, int, int, int>> CachedHits
        {
            get {
                return hits;
            }
        }
        public List<Tuple<int, int, int, int, double>> Duplicates {
            get {
                return duplicates;
            }
        }
        public Guesser(int n, int N, double D, double Wtf) {
            this.n = n;
            this.N = N;
            this.D = D;
            this.Wtf = Wtf;
        }
        public async Task<SortedList<double, Tuple<int, int, int, int>>> MakeGuesses(int guessAreaSize, Func<int, int, Task> progressCallback) {
            Tuple<int, int, int, int> tmp;
            for (int i = 0; i < guessAreaSize; i++) {
                for (int j = 0; j < guessAreaSize; j++)
                {
                    for (int k = 0; k < 4; k++) {
                        var val = Guess(out tmp, k % 2 == 0 ? i : -i, k > 1 ? j : -j);
                        if (CheckFeasibility(results, val, tmp) == true)
                        {
                            results.Add(val, tmp);
                        }
                    }
                }
                if(i%100 == 0)
                await progressCallback(guessAreaSize, i+1);
            }
            await progressCallback(guessAreaSize, guessAreaSize);
            return results;
        }
        private bool CheckFeasibility(SortedList<double, Tuple<int, int, int, int>> input,double key , Tuple<int,int,int,int> newVal) {
            if (key < 0.0 || key == double.NaN)
            {
                return false;
            }
            if (input.ContainsKey(key)) {
                duplicates.Add(new Tuple<int, int, int, int, double>(newVal.Item1, newVal.Item2, newVal.Item3, newVal.Item4, key));
                return false;
            }
            return true;
        }
        private double Guess(out Tuple<int, int, int, int> guessRes, int i, int j) {
            int n1 = n / 2 + i;
            int n2 = n - n1;
            int N1 = N / 2 + j;
            int N2 = N - N1;
            if (n1 < 0 || n2 < 0 || N1 < 0 || N2 < 0) {
                guessRes = null;
                return -1.0;
            }
            bool[] hits = new bool[4];
            double badness = GetnBadness(n1 + n2, out hits[0]) + GetNBadness(N1 + N2, out hits[1]) + GetDBadness(n1 / 2d * (N2 / (double)n2), out hits[2]) + GetWtfBadness(n1 * Math.Log(n1,2) + n2 * Math.Log(n2, 2), out hits[3]);
            if (badness > MaxBadness) {
                guessRes = null;
                return -1.0;//That's too bad
            }
            guessRes = new Tuple<int, int, int, int>(n1, n2, N1, N2);
            if (hits.Count(x => x == true) >= hits.Length-1 && CheckFeasibility(this.hits, badness, guessRes) == true) {
                this.hits.Add(badness, guessRes);
            }
            return badness;

        }
        private double GetnBadness(int cn, out bool isHit) {
            var res = Math.Abs(cn - n);
            isHit = res - FactorOffInsignificance <= 0;
            return res;
        }
        private double GetNBadness(int cN, out bool isHit) {
            var res = Math.Abs(cN - N);
            isHit = res - FactorOffInsignificance <= 0;
            return res;
        }
        private double GetDBadness(double cD, out bool isHit) {
            var res = Math.Abs(cD - D);
            isHit = res - FactorOffInsignificance <= 0;
            return res;
        }
        private double GetWtfBadness(double cWtf, out bool isHit) {
            var res = Math.Abs(cWtf - Wtf);
            isHit = res - FactorOffInsignificance <= 0;
            return res;
        }
    }
}
