using Calculator.Model;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Calculator.ViewModel
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        //1.1 Событие изменение свойств в XAML
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }

        bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
        //1.2 Коллекция для сохранения вычислений в файл
        private Collection<string> computationHistory;

        //2 Поля
        private Calc calculation;
        private string lastOperation;
        private bool newDisplayRequired = false;
        private string fullExpression;
        private string display;


        //3 Конструктор
        public MainWindowViewModel()
        {
            calculation = new Calc();
            //Коллекция для сохранения вычислений в файл
            computationHistory = new Collection<string>();
            //Поля
            display = "0";
            FirstOperand = string.Empty;
            SecondOperand = string.Empty;
            Operation = string.Empty;
            lastOperation = string.Empty;
            fullExpression = string.Empty;
            //Команды
            SaveCommand = new RelayCommand(OnSaveCommandExecute, CanSaveCommandExecuted);
            CopyCommand = new RelayCommand(OnCopyCommandExecute, CanCopyCommandExecuted);
            OperationButtonPressCommand = new RelayCommand(OnOperationButtonPressCommandExecute, CanOperationButtonPressCommandExecuted);
            SingularOperationButtonPressCommand = new RelayCommand(OnSingularOperationButtonPressCommandExecute, CanSingularOperationButtonPressCommandExecuted);
            DigitButtonPressCommand = new RelayCommand(OnDigitButtonPressCommandExecute, CanDigitButtonPressCommandExecuted);
        }


        //4.Свойства

        //4.1 Первый операнд
        public string FirstOperand
        {
            get => calculation.FirstOperand;
            set { calculation.FirstOperand = value; }
        }
        //4.2 Второй операнд
        public string SecondOperand
        {
            get => calculation.SecondOperand;
            set { calculation.SecondOperand = value; }
        }
        //4.3 Операция
        public string Operation
        {
            get => calculation.Operation;
            set { calculation.Operation = value; }
        }
        //4.4 Последняя операция
        public string LastOperation
        {
            get => lastOperation;
            set { lastOperation = value; }
        }
        //4.5 Результат
        public string Result
        {
            get => calculation.Result;
        }
        //4.6 Экран калькулятора
        public string Display
        {
            get => display;
            set => Set(ref display, value);
        }
        //4.7 История вычислений
        public string FullExpression
        {
            get => fullExpression;
            set => Set(ref fullExpression, value);
        }


        //5 Команды

        //5.1 Команда сохранения истории вычислений в файл
        public ICommand SaveCommand { get; }

        private void OnSaveCommandExecute(object p)
        {
            string logFilePath = Directory.GetCurrentDirectory() + @"\" + "Вычисления_" + DateTime.Now.ToString("dd.MM.yy") + '_' + DateTime.Now.ToString("hh.mm.ss") + ".txt";
            using (StreamWriter streamWriter = new StreamWriter(logFilePath))
            {
                int i = 0;
                foreach (string computation in computationHistory)
                {
                    i++;
                    streamWriter.WriteLine(i.ToString() + ". " + computation);
                }
                MessageBox.Show("История действий была сохранена в файле: " + logFilePath);
            }
        }

        private bool CanSaveCommandExecuted(object p)
        {
            if (computationHistory.Count != 0) return true;
            else return false;
        }

        //5.2 Команда копирования результата вычислений с экрана калькулятора в буфер обмена
        public ICommand CopyCommand { get; }

        private void OnCopyCommandExecute(object p)
        {
            Clipboard.SetText(Display);
        }

        private bool CanCopyCommandExecuted(object p) => true;

        //5.3 Команда обработчик нажатия на кнопки оперций с двумя операндами
        public ICommand OperationButtonPressCommand { get; }

        private void OnOperationButtonPressCommandExecute(object p)
        {
            if (FirstOperand == string.Empty || LastOperation == "=")
            {
                FirstOperand = display;
                LastOperation = p.ToString();
            }
            else
            {
                SecondOperand = display;
                Operation = lastOperation;
                calculation.CalculateResult();

                if (Operation == "/" && SecondOperand == "0")
                {
                    Display = "Ошибка";
                    newDisplayRequired = true;
                    return;
                }
                else
                {
                    FullExpression = Math.Round(Convert.ToDouble(FirstOperand), 10) + Operation
                        + Math.Round(Convert.ToDouble(SecondOperand), 10) + "="
                        + Math.Round(Convert.ToDouble(Result), 10);

                    computationHistory.Add(FullExpression);
                    LastOperation = p.ToString();
                    Display = Result;
                    FirstOperand = display;
                }
            }
            newDisplayRequired = true;
        }

        private bool CanOperationButtonPressCommandExecuted(object p) => true;

        //5.4 Команда обработчик нажатия на кнопки оперций с одним операндом
        public ICommand SingularOperationButtonPressCommand { get; }

        private void OnSingularOperationButtonPressCommandExecute(object p)
        {
            FirstOperand = Display;
            Operation = p.ToString();
            calculation.CalculateResult();

            if (Operation == "1/x" && FirstOperand == "0" || Operation == "√х" && Convert.ToDouble(FirstOperand) < 0)
            {
                Display = "Ошибка";
                newDisplayRequired = true;
                return;
            }
            else
            {
                FullExpression = Operation + "(" + Math.Round(Convert.ToDouble(FirstOperand), 10) + ")="
                    + Math.Round(Convert.ToDouble(Result), 10);

                computationHistory.Add(FullExpression);
                LastOperation = "=";
                Display = Result;
                FirstOperand = display;
                newDisplayRequired = true;
            }
        }

        private bool CanSingularOperationButtonPressCommandExecuted(object p) => true;

        //5.5 Команда обработчик нажатия на цифровые кнопки
        public ICommand DigitButtonPressCommand { get; }

        private void OnDigitButtonPressCommandExecute(object p)
        {
            switch (p)
            {
                case "C":
                    Display = "0";
                    FirstOperand = string.Empty;
                    SecondOperand = string.Empty;
                    Operation = string.Empty;
                    LastOperation = string.Empty;
                    FullExpression = string.Empty;
                    break;
                case "CE":
                    Display = "0";
                    SecondOperand = string.Empty;
                    break;
                case "←":
                    Display = display.Length > 1 ? display.Substring(0, display.Length - 1) : "0";
                    break;
                case "±":
                    Display = display.Contains("-") ? display.Remove(display.IndexOf("-"), 1) : "-" + display;
                    break;
                case ",":
                    if (newDisplayRequired)
                    {
                        Display = "0,";
                    }
                    else
                    {
                        if (!display.Contains(","))
                        {
                            Display = display + ",";
                        }
                    }
                    break;
                default:
                    Display = display == "0" || newDisplayRequired ? p.ToString() : display + p.ToString();
                    break;
            }
            newDisplayRequired = false;
        }

        private bool CanDigitButtonPressCommandExecuted(object p) => true;
        
    }
}