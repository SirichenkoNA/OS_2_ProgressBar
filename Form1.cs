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

        private readonly static bool DEBUG = false; //включение опции дебаггинга

        private readonly static object block = new(); // создание ключа для 

        private readonly static string FILE_NAME = @"C:\Users\p3n3k\source\repos\OS_2_ProgressBar\ReportFile.txt"; //прописываем путь к файлу для записи результата

        public Form1()
        {
            InitializeComponent();
        }

        public void Button1_Click(object sender, EventArgs e)
        {
            _n = 0; // начальное значение для вычислений        

            if (NInput.Text != "") //проверяем, введено ли значение
            {
                Stopwatch stopWatch = new(); //создание таймера программы
                stopWatch.Start(); //запуск таймера программы

                //очищаем прогресс бары
                {
                    BarA.Value = 0;
                    BarB.Value = 0;
                    BarC.Value = 0;
                    BarD.Value = 0;
                    BarE.Value = 0;
                    BarF.Value = 0;
                    BarG.Value = 0;
                    BarH.Value = 0;
                    BarK.Value = 0;
                }

                ConsoleOutPut(); //вывод строки-разделителя

                using (StreamWriter streamWriter = new(FILE_NAME, true)) //запись в файл даты теста
                {
                    streamWriter.WriteLine("\t-----------------------------------------");
                    streamWriter.WriteLine("\t Testcase | Date: {0:d} at {0:t}", DateTime.Now);
                    streamWriter.WriteLine("\t-----------------------------------------");
                }

                _n = Convert.ToInt32(NInput.Text); //чтение данных от пользователя

                Task<List<List<int>>> Task_A = Task.Run(GenerateM); //запуск задачи A
                Task<List<bool>> Task_B = Task.Run(GenerateR); //запуск задачи B
                Task.WaitAll(Task_A, Task_B); //ожидание выполнения парралельной работы

                ConsoleOutPut(); //вывод строки-разделителя

                Task<List<List<int>>> Task_C = Task.Run(() => Foo1(Task_A.Result, Task_B.Result)); //запуск задачи C

                Task_C.Wait(); //ожидание выполнения задачи С

                ConsoleOutPut(); //вывод строки-разделителя

                Task<List<int>> Task_D = Task_C.ContinueWith(_Function1 => Foo2(Task_C.Result)); //запуск задачи D, после завершения C
                Task<List<int>> Task_E = Task_C.ContinueWith(_Function1 => Foo3(Task_C.Result)); //запуск задачи E, после завершения C
                Task<List<int>> Task_F = Task_C.ContinueWith(_Function1 => Foo4(Task_C.Result)); //запуск задачи F, после завершения C

                Task<int> Task_G = Task_D.ContinueWith(_Function5 => Foo5(Task_D.Result)); //запуск задачи G, после завершения D
                Task<int> Task_H = Task_E.ContinueWith(_Function4 => Foo6(Task_E.Result)); //запуск задачи H, после завершения E

                Task.WaitAll(Task_G, Task_H, Task_F); //ожидание всех задач к завершению вычислений

                ConsoleOutPut(); //вывод строки-разделителя

                Task<string> Task_K = Task.Run(() => Foo7(Task_G.Result, Task_H.Result)); //запуск задачи K

                Task_K.Wait(); //ожидание задачи К, для вывода результата

                ConsoleOutPut(); //вывод строки-разделителя

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

                Task.Run(async () => //запуск отдельного аснхронной задачи для значений результата
                {
                    await Task.Delay(5000);
                    this.BeginInvoke((Action)delegate ()
                    {
                        Result.Text = $"{Task_K.Result}"; //вывод результата в элемента TextBox
                    });
                });

                if (DEBUG) //дебагер для проверки работы алгоритма
                {
                    Task Task_Check = Task.Run(() => CheckForResult(Task_C.Result));
                    Task_Check.Wait();
                }

                Console.WriteLine("\tMain Task Has Finished"); //завершение основного потока (Main)            
                Console.WriteLine("\tProgram execution time: {0}", stopWatch.Elapsed); //выводим время выполнения всей программы

                ConsoleOutPut();
            }

        }
        private List<List<int>> GenerateM()
        { //создает двумерный массив из чисел в промежутке (-10, 10) 
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<List<int>> _GeneratedList = new();

            Random randomInt = new();

            for (int i = 0; i < _n; i++)
            {
                List<int> _list = new();
                _GeneratedList.Add(_list);
                for (int j = 0; j < _n; j++)
                {
                    _GeneratedList[i].Add(randomInt.Next(-10, 11));
                }
            }

            Console.WriteLine($"\tTask (A) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                this.BeginInvoke((Action)delegate ()
                {
                    BarA.Value = 100;
                });
            });

            return _GeneratedList;
        }
        private List<bool> GenerateR()
        { //создает массив из true/false размером n
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<bool> _GeneratedList = new();
            Random randomBool = new();

            for (int i = 0; i < _n; i++)
            {
                _GeneratedList.Add(Convert.ToBoolean(randomBool.Next(2)));
            }

            Console.WriteLine($"\tTask (B) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                this.BeginInvoke((Action)delegate ()
                {
                    BarB.Value = 100;
                });
            });

            return _GeneratedList;
        }
        private List<List<int>> Foo1(List<List<int>> _GeneratedM, List<bool> _GeneratedR)
        { //меняет элементы массива на -1;0;1 от true/false
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            for (int i = 0; i < _GeneratedM.Count; i++)
            {
                for (int j = 0; j < _GeneratedM.Count; j++)
                {
                    if (_GeneratedM[i][j] > 0 && _GeneratedR[j] == true)
                        _GeneratedM[i][j] = 1;
                    else if (_GeneratedM[i][j] < 0 && _GeneratedR[j] == true)
                        _GeneratedM[i][j] = -1;
                    else
                        _GeneratedM[i][j] = 0;
                }
            }

            Console.WriteLine($"\tTask (C) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                this.BeginInvoke((Action)delegate ()
                {
                    BarC.Value = 100;
                });
            });

            return _GeneratedM;
        }
        private List<int> Foo2(List<List<int>> _Function1)
        { // записывает индексы всех элементов {-1}
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<int> _FunctionTwoResult = new();

            for (int i = 0; i < _Function1.Count; i++)
            {
                for (int j = 0; j < _Function1.Count; j++)
                {
                    if (_Function1[i][j] == -1)
                        _FunctionTwoResult.Add(i * _Function1.Count + j);


                }
            }

            Console.WriteLine($"\tTask (D) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                this.BeginInvoke((Action)delegate ()
                {
                    BarD.Value = 100;
                });
            });

            return _FunctionTwoResult;
        }
        private List<int> Foo3(List<List<int>> _Function1)
        { // записывает индексы всех элементов {0}
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<int> _FunctionThreeResult = new();

            for (int i = 0; i < _Function1.Count; i++)
            {
                for (int j = 0; j < _Function1.Count; j++)
                {
                    if (_Function1[i][j] == 0)
                        _FunctionThreeResult.Add(i * _Function1.Count + j);
                }
            }

            Console.WriteLine($"\tTask (E) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                this.BeginInvoke((Action)delegate ()
                {
                    BarE.Value = 100;
                });
            });

            return _FunctionThreeResult;
        }
        private List<int> Foo4(List<List<int>> _Function1)
        { // записывает индексы всех элементов {1}
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            List<int> _FunctionFourResult = new();

            for (int i = 0; i < _Function1.Count; i++)
            {
                for (int j = 0; j < _Function1.Count; j++)
                {
                    if (_Function1[i][j] == 1)
                        _FunctionFourResult.Add(i * _Function1.Count + j);
                }
            }
            Console.WriteLine($"\tTask (F) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                this.BeginInvoke((Action)delegate ()
                {
                    BarF.Value = 100;
                });
            });

            return _FunctionFourResult;
        }
        private int Foo5(List<int> _Function2)
        { //функция 5 будет отдавать сумму индексов для {-1}
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            int _FunctionFiveResult = 0;

            for (int i = 0; i < _Function2.Count; i++)
            {
                _FunctionFiveResult += _Function2[i];


            }

            Console.WriteLine($"\tTask (G) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                this.BeginInvoke((Action)delegate ()
                {
                    BarG.Value = 100;
                });
            });

            return _FunctionFiveResult;
        }
        private int Foo6(List<int> _Function4)
        { //функция 6 будет отдавать сумму индексов для {1}
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            int _FunctionSixResult = 0;

            for (int i = 0; i < _Function4.Count; i++)
            {
                _FunctionSixResult += _Function4[i];


            }

            Console.WriteLine($"\tTask (H) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            Task.Run(async () =>
            {
                await Task.Delay(3000);
                this.BeginInvoke((Action)delegate ()
                {
                    BarH.Value = 100;
                });
            });

            return _FunctionSixResult;
        }
        private string Foo7(int _Function5, int _Function6)
        { //функция 7 бдует сравнивать две суммы, и возращать ответ, каких значений больше {-1} | {1} | Equal
            Stopwatch stopWatch = new(); //запуск таймера в начале задачи
            stopWatch.Start();

            string _FunctionSevenResult = _Function5 > _Function6 ? "-1" : _Function5 == _Function6 ? "Equal" : "1";
            
            Task.Run(async () =>
            {
                await Task.Delay(3500);
                this.BeginInvoke((Action)delegate ()
                {
                    BarK.Value = 100;
                });
            });

            Console.WriteLine($"\tTask (K) With Id: {Task.CurrentId} Has Finished Generating With Time: {stopWatch.Elapsed}"); //вывод данных по текущей задаче

            Report();

            return _FunctionSevenResult;
        }
        static void Report()
        {
            lock (block) //критический блок кода для разных Task'ов, блокируем, пока его выполняет 1 Task
            {
                using StreamWriter streamWriter = new(FILE_NAME, true); //открываем файл на запись
                streamWriter.WriteLine($"\t Task With Id: {Task.CurrentId} Has Finished Generating"); //записываем данные Task'а в файл
                streamWriter.Close(); //закрываем запись в файл
            }
        }
        static void CheckForResult(List<List<int>> _Task_C_Result)
        { //метод для ручной проверки вычислений (пред-конечная матрица)

            for (int i = 0; i < _Task_C_Result.Count; i++)
            {
                for (int j = 0; j < _Task_C_Result.Count; j++)
                {
                    Console.Write("\t{0}", _Task_C_Result[i][j]);
                }
                Console.WriteLine();
            }
        }
        static void ConsoleOutPut()
        { // строки-разделители для консоли
            Console.WriteLine("\t-----------------------------------------------------------------------");
        }
        public void Form1_Load(object sender, EventArgs e) {}

    }
}