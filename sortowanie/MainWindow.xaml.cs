using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace sortowanie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public delegate void Sort(int[] arr);
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Button_Click_AddValues(object sender, RoutedEventArgs e)
        {
            ListBox_BeforeSort.Items.Clear();
            try
            {
                foreach (var i in Generator(int.Parse(TextBox_Podaj_liczbe.Text))){
                    ListBox_BeforeSort.Items.Add(i);
                }
                //CreteTables();
            }
            catch 
            {
                MessageBox.Show("TO nie jest liczba");
            }
            
        }
        private void Button_Click_BubbleSort(object sender, RoutedEventArgs e)
        {
            int[] tablica1 = (ListBox_BeforeSort.Items.OfType<int>().ToArray());
            int[] tablica2 = (ListBox_BeforeSort.Items.OfType<int>().ToArray());
            ClearResultLists();

            var watch1 = System.Diagnostics.Stopwatch.StartNew();
            BubbleSort(tablica1);
            watch1.Stop();
            var elapsedMs1 = watch1.ElapsedMilliseconds;


            foreach ( var i in tablica1)
            {
                ListBox_BubbleSort.Items.Add(i);
            }

            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            QuickSort(tablica2, 0, tablica2.Length -1);
            watch2.Stop();
            var elapsedMs2 = watch2.ElapsedMilliseconds;

            foreach (var i in tablica2)
            {
                ListBox_QuickSort.Items.Add(i);
            }

            BubbleSort_time.Content = "czas: " + elapsedMs1.ToString() + " ms";
            QuickSort_time.Content = "czas: " + elapsedMs2.ToString() + " ms";
            All_time.Content = "czas: " + (elapsedMs2 + elapsedMs1).ToString() + " ms";

            }

        static void BubbleSort(int[] arr)
        {
           
            int temp = 0;

            for (int write = 0; write < arr.Length; write++)
            {
                for (int sort = 0; sort < arr.Length - 1; sort++)
                {
                    if (arr[sort] > arr[sort + 1])
                    {
                        temp = arr[sort + 1];
                        arr[sort + 1] = arr[sort];
                        arr[sort] = temp;
                    }
                }
            }

        }

        public static int[] Generator(int n)
        {
            int[] table = new int[n];
            Random random = new Random();
            for (int i = 0; i < n; i++)
            {
                table[i] = random.Next();
            }

            return table;
        }
        public static void QuickSort(int[] array, int left, int right)
        {
            var i = left;
            var j = right;
            var pivot = array[(left + right) / 2];
            while (i < j)
            {
                while (array[i] < pivot) i++;
                while (array[j] > pivot) j--;
                if (i <= j)
                {
                    // swap
                    var tmp = array[i];
                    array[i++] = array[j];  // ++ and -- inside array braces for shorter code
                    array[j--] = tmp;
                }
            }
            if (left < j) QuickSort(array, left, j);
            if (i < right) QuickSort(array, i, right);
        }

        

       




        private void Button_Click(object sender, RoutedEventArgs e) //sortowanie równoległe
        {
            int[] tablica1 = (ListBox_BeforeSort.Items.OfType<int>().ToArray());
            int[] tablica2 = (ListBox_BeforeSort.Items.OfType<int>().ToArray());
            ClearResultLists();


            // Create a background thread to execute BubbleSort() method and pass values to listbox
            BackgroundWorker bw1 = new BackgroundWorker();
            bw1.WorkerReportsProgress = true;
            bw1.DoWork += worker1_dowork;
            bw1.RunWorkerCompleted += worker1_Completed;

            // Create a background thread to execute QuickSort() method and pass values to listbox
            BackgroundWorker bw2 = new BackgroundWorker();
            bw2.WorkerReportsProgress = true;
            bw2.DoWork += worker2_dowork;
            bw2.RunWorkerCompleted += worker2_Completed;
            
            

            bw1.RunWorkerAsync(argument: tablica1);
            bw2.RunWorkerAsync(argument: tablica2);



        }

        private void worker1_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Container container = (Container)e.Result;
            foreach (var i in container.Table)
            {
                ListBox_BubbleSort.Items.Add(i);
            }
            BubbleSort_time.Content = "czas: " + container.Ms + " ms";
        }

        private void worker1_dowork(object sender, DoWorkEventArgs e)
        {
            int[] table = (int[])e.Argument;
            var watch1 = System.Diagnostics.Stopwatch.StartNew();
            BubbleSort((int[])e.Argument);
            watch1.Stop();
            var elapsedMs1 = watch1.ElapsedMilliseconds;

            Container container = new Container(table, elapsedMs1);
            e.Result = container;
            
           
        }

        private void worker2_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            Container container = (Container)e.Result;
            foreach (var i in container.Table)
            {
                ListBox_QuickSort.Items.Add(i);
            }
            QuickSort_time.Content = "czas: " + container.Ms + " ms";
        }

        private void worker2_dowork(object sender, DoWorkEventArgs e)
        {
            int[] table = (int[])e.Argument;
            var watch2 = System.Diagnostics.Stopwatch.StartNew();
            QuickSort(table, 0, table.Length - 1);
            watch2.Stop();
            var elapsedMs2 = watch2.ElapsedMilliseconds;

            Container container = new Container(table, elapsedMs2);
            e.Result = container;
        }
        class Container
        {
            public int[] Table { get; set; }
            public long Ms { get; set; }

            public Container(int[] table, long ms)
            {
                Table = table;
                Ms = ms;
            }
        }
        void ClearResultLists()
        {
            ListBox_BubbleSort.Items.Clear();
            ListBox_QuickSort.Items.Clear();
        }





    }
}
