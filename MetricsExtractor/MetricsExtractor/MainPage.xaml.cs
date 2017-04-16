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
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MetricsExtractor
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

        private async void button_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.FileTypeFilter.Add(".txt");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                System.Diagnostics.Debug.WriteLine(file);
                var res = await ExtractMetric(file);
                DisplayInTextbox(textBox, res.Keys.ToArray());
                DisplayInTextbox(textBox_Copy, res.Values.ToArray());
                textBlock.Text = string.Format("{0}:{1}. Total: {2} classes",file.DisplayName, ((ComboBoxItem)comboBox.SelectedValue).Content, res.Keys.Count);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("fail");
            }
        }

        private async Task<SortedList<string,string>> ExtractMetric(StorageFile file)
        {
            var read = await FileIO.ReadLinesAsync(file);
            int selectedMetric = comboBox.SelectedIndex;
            SortedList<string, string> results = new SortedList<string, string>(read.Count);
            foreach (var data in read) {
                var pRes = PrepareValue(data, selectedMetric);
                results.Add(pRes.Item1, pRes.Item2);
            }
            
            return results;
        }
        private Tuple<string, string> PrepareValue(string initString, int metricId) {
            string[] vals = initString.Split(' ');
            return new Tuple<string, string>(vals[0], vals[metricId + 1]);
        }
        private void DisplayInTextbox(TextBox tb, string[] values) {
            StringBuilder sb = new StringBuilder();
            foreach (var s in values) {
                sb.AppendLine(s);
            }
            tb.Text = sb.ToString();
        }
    }
}
