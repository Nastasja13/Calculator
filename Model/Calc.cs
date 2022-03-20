using System;

namespace Calculator.Model
{
    public class Calc
    {
        private string result;

        //Конструкторы калькулятора

        public Calc(string firstOperand, string secondOperand, string operation)
        {
            FirstOperand = firstOperand;
            SecondOperand = secondOperand;
            Operation = operation;
            result = string.Empty;
        }

        public Calc(string firstOperand, string operation)
        {
            FirstOperand = firstOperand;
            SecondOperand = string.Empty;
            Operation = operation;
            result = string.Empty;
        }

        public Calc()
        {
            FirstOperand = string.Empty;
            SecondOperand = string.Empty;
            Operation = string.Empty;
            result = string.Empty;
        }
        

        //Свойства и методы

        public string FirstOperand { get; set; }
        public string SecondOperand { get; set; }
        public string Operation { get; set; }
        public string Result { get { return result; } }

        public void CalculateResult()
        {
            switch (Operation)
            {
                case ("+"):
                    result = (Convert.ToDouble(FirstOperand) + Convert.ToDouble(SecondOperand)).ToString();
                    break;

                case ("-"):
                    result = (Convert.ToDouble(FirstOperand) - Convert.ToDouble(SecondOperand)).ToString();
                    break;

                case ("*"):
                    result = (Convert.ToDouble(FirstOperand) * Convert.ToDouble(SecondOperand)).ToString();
                    break;

                case ("/"):
                    result = (Convert.ToDouble(FirstOperand) / Convert.ToDouble(SecondOperand)).ToString();
                    break;

                case ("%"):
                    result = (Convert.ToDouble(FirstOperand) / 100).ToString();
                    break;

                case ("√х"):
                    result = Convert.ToDouble(FirstOperand) < 0 ? "Ошибка" : Math.Sqrt(Convert.ToDouble(FirstOperand)).ToString();
                    break;

                case ("х²"):
                    result = Math.Pow(Convert.ToDouble(FirstOperand), 2).ToString();
                    break;

                case ("1/x"):
                    result = Convert.ToDouble(FirstOperand) == 0 ? "Ошибка" : (1 / Convert.ToDouble(FirstOperand)).ToString();
                    break;
            }
        }
        
    }
}