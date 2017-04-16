using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace GuessBaseParams
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async Task UpdateProgress(int total, int done) {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                double score = ((double)done / total) * 100;
                textBlock1.Text = string.Format("{0:N2}%", score);
                Pb.Value = score;
            }
            );
        }
        private async void button_Click(object sender, RoutedEventArgs e)
        {
            await HandleIt();
        }
        private async Task HandleIt() {
            int n = int.Parse(textBox.Text);
            int N = int.Parse(textBox_Copy2.Text);
            double D = double.Parse(textBox_Copy1.Text);
            double wtf = double.Parse(textBox_Copy.Text);
            Guesser g = new Guesser(n, N, D, wtf);
            var res = await g.MakeGuesses(Math.Max(n,N)/2+1, UpdateProgress);
            Output(g, res);

        }
        private void Output(Guesser g, SortedList<double, Tuple<int, int, int, int>> res) {
            textBox1.Text = EatStrings(res);
            var hits = g.CachedHits;
            if (hits != null && hits.Count > 0)
            {
                textBox2.Text = EatStrings(hits);
            }
            var duplicates = g.Duplicates;
            if (duplicates != null && duplicates.Count > 0)
            {
                textBox2_Copy.Text = EatStrings(duplicates);
            }
            var exacts = hits.Select(a => new Tuple<int,int,int,int,double>(a.Value.Item1, a.Value.Item2, a.Value.Item3, a.Value.Item4,a.Key)).Where(x => x.Item5 <= 0.05);
            if (exacts != null && exacts.Count() > 0)
            {
                textBox2_Copy1.Text = EatStrings(exacts.ToList());
            }
        }
        private string EatStrings(SortedList<double, Tuple<int, int, int, int>> list) {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0,-5} => {1,-10}|{2,-10}|{3,-10}|{4,-10}", "Scr", "n1", "n2", "N1", "N2"));
            foreach (var tpl in list) {
                var val = tpl.Value;
                sb.AppendLine(string.Format("{0,-5} => {1,-10}|{2,-10}|{3,-10}|{4,-10}", tpl.Key, val.Item1, val.Item2, val.Item3, val.Item4));
            }
            return sb.ToString();
        }
        private string EatStrings(List<Tuple<int, int, int, int, double>> list)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Format("{0,-5} => {1,-10}|{2,-10}|{3,-10}|{4,-10}", "Scr", "n1", "n2", "N1", "N2"));
            foreach (var tpl in list)
            {
                var val = tpl;
                sb.AppendLine(string.Format("{0,-5} => {1,-10}|{2,-10}|{3,-10}|{4,-10}", tpl.Item5, val.Item1, val.Item2, val.Item3, val.Item4));
            }
            return sb.ToString();
        }
    }
}
