using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Timers;

namespace OS_2_ProgressBar
{
    public partial class Form1 : Form
    {
        private static int _n; //значение размера массива данных

        private readonly static object block = new();

        private readonly static string FILE_NAME = @"D:\6 semestr\Operating System\lab2\OS_2_ProgressBar\ReportFile.txt"; //путь к файлу для записи результата

        public Form1()
        {
            InitializeComponent();
        }

        public void Button1_Click(object sender, EventArgs e)
        {
            _n = 0; // начальное значение для вычислений        

            if (Input.Text != "" && Input.Text  != "0") //проверяем, введено ли значение, отличное от нуля
            {
                Stopwatch stopWatch = new(); //создание таймера программы
                stopWatch.Start(); //запуск таймера программы

                //очищаем прогресс бары
                {
                    this.A.Value = 0;
                    this.B.Value = 0;
                    this.C.Value = 0;
                    this.D.Value = 0;
                    this.E.Value = 0;
                    this.F.Value = 0;
                    this.G.Value = 0;
                    this.H.Value = 0;
                    this.K.Value = 0;
                }

                ConsoleOutPut(); //вывод строки-разделителя

                using (StreamWriter streamWriter = new(FILE_NAME, true)) //запись в файл даты теста
                {
                    streamWriter.WriteLine("\t-----------------------------------------");
                    streamWriter.WriteLine("\t Testcase | Date: {0:d} at {0:t}", DateTime.Now);
                    streamWriter.WriteLine("\t-----------------------------------------");
                }

                _n = Convert.ToInt32(Input.Text); //принятие числа введёенного пользователем

                Task<List<List<int>>> Task_A = Task.Run(() => Gnrt()); //запуск задачи A
                List<List<int>> A = Task_A.Result;
                ConsoleOutPut();

                Task<List<List<int>>> Task_B = Task_A.ContinueWith(Fun1 => F1(A)); //запуск задачи B, после завершения A
                List<List<int>> B = Task_B.Result;
                ConsoleOutPut();

                Task<List<List<int>>> Task_C = Task_A.ContinueWith(Fun2 => F2(A)); //запуск задачи C, после завершения A
                List<List<int>> C = Task_C.Result;
                ConsoleOutPut();

                Task<List<List<int>>> Task_D = Task_A.ContinueWith(Fun3 => F3(A)); //запуск задачи D, после завершения A
                List<List<int>> D = Task_D.Result;
                ConsoleOutPut(); 

                Task<List<List<int>>> Task_E = Task_C.ContinueWith(_Func4 => F4(C)); //запуск задачи E, после завершения C
                List<List<int>> E = Task_E.Result;
                ConsoleOutPut(); 

                Task<List<List<int>>> Task_F = Task_C.ContinueWith(_Func5 => F5(C)); //запуск задачи F, после завершения C
                List<List<int>> F = Task_F.Result;
                ConsoleOutPut(); 

                Task<List<List<int>>> Task_G = Task_C.ContinueWith(_Func6 => F6(C)); //запуск задачи G, после завершения C
                List<List<int>> G = Task_G.Result;
                ConsoleOutPut(); 

                Task.WaitAll(Task_F, Task_G);
                Task<List<List<int>>> Task_H = Task.Run(() => F7(F, G)); //запуск задачи H после завершения F и G
                List<List<int>> H = Task_H.Result;
                ConsoleOutPut(); //вывод строки-разделителя

                Task.WaitAll(Task_B, Task_E, Task_H, Task_D); 
                Task<double> Task_K = Task.Run(() => F8(B, E, H, D)); //запуск задачи K после завершения B, E, H, D
                ConsoleOutPut(); //вывод строки-разделителя

                Task_K.Wait();

                using StreamWriter TaskSelectorWriter = new(FILE_NAME, true);
                { //открываем файл на запись
                    //записываем данные ID всех Task'ов в файл
                    TaskSelectorWriter.WriteLine("\t-----------------------------------------");
                    TaskSelectorWriter.WriteLine($"\t Task A Id: {Task_A.Id}");
                    TaskSelectorWriter.WriteLine($"\t Task B Id: {Task_B.Id}");
                    TaskSelectorWriter.WriteLine($"\t Task C Id: {Task_C.Id}");
                    TaskSelectorWriter.WriteLine($"\t Task D Id: {Task_D.Id}");
                    TaskSelectorWriter.WriteLine($"\t Task E Id: {Task_E.Id}");
                    TaskSelectorWriter.WriteLine($"\t Task F Id: {Task_F.Id}");
                    TaskSelectorWriter.WriteLine($"\t Task G Id: {Task_G.Id}");
                    TaskSelectorWriter.WriteLine($"\t Task H Id: {Task_H.Id}");
                    TaskSelectorWriter.WriteLine($"\t Task K Id: {Task_K.Id}");
                }

                Task.Run(async () => //вывод результата в TextBox
                {
                    await Task.Delay(5000);
                    this.BeginInvoke((Action)delegate ()
                    {
                        Result.Text = $"{Task_K.Result}"; 
                    });
                });

                Console.WriteLine("\tMain Task Has Finished"); //завершение основного потока (Main)            
                Console.WriteLine("\tProgram execution time: {0}", stopWatch.Elapsed); //выводим время выполнения всей программы

                ConsoleOutPut();
            }

        }

        //--------------------------------------------------------------------------------------
        private List<List<int>> Gnrt()
        { //создает массив из трёх массивов bool
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<List<int>> GnrtL = new();


            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<int> _list = new();
                GnrtL.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    Random randomBool = new Random((int)DateTime.Now.Ticks);
                    GnrtL[i].Add(randomBool.Next(0, 2));
                }
            }

            Console.WriteLine($"\tTask (A) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                this.BeginInvoke((Action)delegate ()
                {
                    A.Value = 100;
                });
            });

            return GnrtL;
        }
        //-------------------------------------------------------------------------------------

        private List<List<int>> F1(List<List<int>> GnrtL)
        { //инвертирует

            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<List<bool>> GnrtLLL = new();


            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list = new();
                GnrtLLL.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL[i][j] == 1) GnrtLLL[i].Add(true);
                    if (GnrtL[i][j] == 0) GnrtLLL[i].Add(false);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    GnrtLLL[i][j] = !GnrtLLL[i][j];
                }
            }

            Console.WriteLine($"\tTask (B) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3500);
                this.BeginInvoke((Action)delegate ()
                {
                    B.Value = 100;
                });
            });

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtLLL[i][j] == true) GnrtL[i][j] = 1;
                    if (GnrtLLL[i][j] == false) GnrtL[i][j] = 0;
                }
            }
            return GnrtL;
        }

        private List<List<int>> F2(List<List<int>> GnrtL)
        { //сложение по модулю два с единицей
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<List<bool>> GnrtLLL = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list = new();
                GnrtLLL.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL[i][j] == 1) GnrtLLL[i].Add(true);
                    if (GnrtL[i][j] == 0) GnrtLLL[i].Add(false);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    GnrtLLL[i][j] = GnrtLLL[i][j] ^ true;
                }
            }

            Console.WriteLine($"\tTask (B) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3500);
                this.BeginInvoke((Action)delegate ()
                {
                    C.Value = 100;
                });
            });

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtLLL[i][j] == true) GnrtL[i][j] = 1;
                    if (GnrtLLL[i][j] == false) GnrtL[i][j] = 0;
                }
            }

            return GnrtL;
        }

        private List<List<int>> F3(List<List<int>> GnrtL)
        { //"И-НЕ" с единицей
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<List<bool>> GnrtLLL = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list = new();
                GnrtLLL.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL[i][j] == 1) GnrtLLL[i].Add(true);
                    if (GnrtL[i][j] == 0) GnrtLLL[i].Add(false);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    GnrtLLL[i][j] = !(GnrtLLL[i][j] & true);
                }
            }

            Console.WriteLine($"\tTask (B) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3500);
                this.BeginInvoke((Action)delegate ()
                {
                    D.Value = 100;
                });
            });

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtLLL[i][j] == true) GnrtL[i][j] = 1;
                    if (GnrtLLL[i][j] == false) GnrtL[i][j] = 0;
                }
            }

            return GnrtL;
        }

        private List<List<int>> F4(List<List<int>> GnrtL)
        { //импликация c нулём
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<List<bool>> GnrtLLL = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list = new();
                GnrtLLL.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL[i][j] == 1) GnrtLLL[i].Add(true);
                    if (GnrtL[i][j] == 0) GnrtLLL[i].Add(false);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {

                    GnrtLLL[i][j] = !GnrtLLL[i][j] | false;
                }
            }

            Console.WriteLine($"\tTask (B) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(4000);
                this.BeginInvoke((Action)delegate ()
                {
                    E.Value = 100;
                });
            });

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtLLL[i][j] == true) GnrtL[i][j] = 1;
                    if (GnrtLLL[i][j] == false) GnrtL[i][j] = 0;
                }
            }

            return GnrtL;
        }

        private List<List<int>> F5(List<List<int>> GnrtL)
        { //логическое умножение на самого себя 
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<List<bool>> GnrtLLL = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list = new();
                GnrtLLL.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL[i][j] == 1) GnrtLLL[i].Add(true);
                    if (GnrtL[i][j] == 0) GnrtLLL[i].Add(false);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {

                    GnrtLLL[i][j] = GnrtLLL[i][j] & GnrtLLL[i][j];
                }
            }

            Console.WriteLine($"\tTask (B) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(4000);
                this.BeginInvoke((Action)delegate ()
                {
                    F.Value = 100;
                });
            });

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtLLL[i][j] == true) GnrtL[i][j] = 1;
                    if (GnrtLLL[i][j] == false) GnrtL[i][j] = 0;
                }
            }

            return GnrtL;
        }

        private List<List<int>> F6(List<List<int>> GnrtL)
        { //эквивалентность с единицей
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<List<bool>> GnrtLLL = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list = new();
                GnrtLLL.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL[i][j] == 1) GnrtLLL[i].Add(true);
                    if (GnrtL[i][j] == 0) GnrtLLL[i].Add(false);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtLLL[i][j] == true)
                    {
                        GnrtLLL[i][j] = true;
                    }
                    else {
                        GnrtLLL[i][j] = false;
                    }
                }
            }

            Console.WriteLine($"\tTask (B) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(4000);
                this.BeginInvoke((Action)delegate ()
                {
                    G.Value = 100;
                });
            });

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtLLL[i][j] == true) GnrtL[i][j] = 1;
                    if (GnrtLLL[i][j] == false) GnrtL[i][j] = 0;
                }
            }

            return GnrtL;
        }

        private List<List<int>> F7(List<List<int>> GnrtL, List<List<int>> GnrtL1)
        { //логическое умножение
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<List<bool>> GnrtLLL = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list = new();
                GnrtLLL.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL[i][j] == 1) GnrtLLL[i].Add(true);
                    if (GnrtL[i][j] == 0) GnrtLLL[i].Add(false);
                }
            }

            List<List<bool>> GnrtLLD = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list1 = new();
                GnrtLLD.Add(_list1);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL1[i][j] == 1) GnrtLLD[i].Add(true);
                    if (GnrtL1[i][j] == 0) GnrtLLD[i].Add(false);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    GnrtLLL[i][j] = GnrtLLL[i][j] & GnrtLLD[i][j];
                }
            }

            Console.WriteLine($"\tTask (B) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(4250);
                this.BeginInvoke((Action)delegate ()
                {
                    H.Value = 100;
                });
            });

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtLLL[i][j] == true) GnrtL[i][j] = 1;
                    if (GnrtLLL[i][j] == false) GnrtL[i][j] = 0;
                }
            }

            return GnrtL;
        }

        private double F8(List<List<int>> GnrtL, List<List<int>> GnrtL1, List<List<int>> GnrtL2, List<List<int>> GnrtL3)
        { //логическое умножение
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();
            double count = 0;

            List<List<bool>> GnrtLLL = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list = new();
                GnrtLLL.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL[i][j] == 1) GnrtLLL[i].Add(true);
                    if (GnrtL[i][j] == 0) GnrtLLL[i].Add(false);
                }
            }

            List<List<bool>> GnrtLLA = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list1 = new();
                GnrtLLA.Add(_list1);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL1[i][j] == 1) GnrtLLA[i].Add(true);
                    if (GnrtL1[i][j] == 0) GnrtLLA[i].Add(false);
                }
            }

            List<List<bool>> GnrtLLB = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list2 = new();
                GnrtLLB.Add(_list2);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL2[i][j] == 1) GnrtLLB[i].Add(true);
                    if (GnrtL2[i][j] == 0) GnrtLLB[i].Add(false);
                }
            }

            List<List<bool>> GnrtLLC = new();
            for (int i = 0; i < 3; i++) //создание трёх массивов буля
            {
                List<bool> _list3 = new();
                GnrtLLC.Add(_list3);
                for (int j = 0; j < _n; j++)
                {
                    if (GnrtL3[i][j] == 1) GnrtLLC[i].Add(true);
                    if (GnrtL3[i][j] == 0) GnrtLLC[i].Add(false);
                }
            }


            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < _n; j++)
                {
                    GnrtLLL[i][j] = GnrtLLL[i][j] & GnrtLLA[i][j] & GnrtLLB[i][j] & GnrtLLC[i][j];
                    if (GnrtLLL[i][j] == true) {
                        count++;
                    }
                }
            }

            count = (count / (_n * 3)) * 100; //находим сколько процентов элементов из всех массивов были равны true
            count = Math.Round(count, 2); //ограничение на количество выводимых знаков

            Console.WriteLine($"\tTask (B) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(4500);
                this.BeginInvoke((Action)delegate ()
                {
                    K.Value = 100;
                });
            });
            return count;
        }

        static void Report()
        {
            lock (block)
            {
                using StreamWriter streamWriter = new(FILE_NAME, true); //открываем файл на запись
                streamWriter.WriteLine($"\t Task With Id: {Task.CurrentId} Has Finished Generating"); //записываем данные Task'а в файл
                streamWriter.Close(); //закрываем запись в файл
            }
        }
        static void ConsoleOutPut()
        { // строки-разделители для консоли
            Console.WriteLine("\t-----------------------------------------------------------------------");
        }
        public void Form1_Load(object sender, EventArgs e) {}

    }
}