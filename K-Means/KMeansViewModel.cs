using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Input;

//viewModel
namespace K_Means
{
    class KMeansViewModel : INotifyPropertyChanged
    {
        const int SourceImageHeight = (int)(450 * 0.9);
        const int SourceImageWidth = 800;

        //При инициализации через конструктор проверка не нужна, IntegerUpDown обеспечивает число(без условия колКлассов > колПоинтов)
        //Если не инициализировать через конструктор, то нужна проверка на число и нужно сделать button недоступным через IsEnabled
        private int _pointsCount;
        public int PointsCount
        {
            get
            {
                return _pointsCount;
            }
            set
            {
                _pointsCount = value;
                //if ((value > 0) && (ClassesCount > 0) && (value >= PointsCount))
                //    ButtonStart.IsEnabled = true;
                //else
                //    ButtonStart.IsEnabled = false;
            }
        }
        private int _classesCount;
        public int ClassesCount
        {
            get
            {
                return _classesCount;
            }
            set
            {
                _classesCount = value;
                //if ((value > 0) && (PointsCount > 0) && (value <= PointsCount))
                //    ButtonStart.IsEnabled = true;
                //else
                //    ButtonStart.IsEnabled = false;
            }
        }

        public string ButtonContent { get; set; }
        public DrawingImage SourceMainImage { get; set; }
        public ButtonCommandBinding ButtonStart { get; set; }

        public KMeansViewModel()
        {
            PointsCount = 1000;
            ClassesCount = 10;
            ButtonContent = "Старт";
            //Call the button command binding class to
            //register the button click event with the handler
            ButtonStart = new ButtonCommandBinding(Start)
            {
                //Enable the button click event
                IsEnabled = true
                //IsEnabled = false
            };
        }

        public void Start()
        {
            KMeans kMeans = new KMeans(SourceImageWidth, SourceImageHeight);
            SourceMainImage = kMeans.GetDrawingImage(PointsCount, ClassesCount);
            OnPropertyChanged("SourceMainImage");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


    public class ButtonCommandBinding : ICommand
    {
        //delegate command to register method to be executed
        private readonly Action handler;
        private bool isEnabled;

        /// <summary>
        /// Bind method to be executed to the handler
        /// So that it can direct on event execution
        /// </summary>
        /// <param name="handler"></param>
        public ButtonCommandBinding(Action handler)
        {
            // Assign the method name to the handler
            this.handler = handler;
        }

        //Property that helps to
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value != isEnabled)
                {
                    isEnabled = value;
                    if (CanExecuteChanged != null)
                    {
                        CanExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        /// method to specify if the event will execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return IsEnabled;
        }

        public event EventHandler CanExecuteChanged;

        // Helps to execute the respective method using the handler
        public void Execute(object parameter)
        {
            //calls the respective method that has been registered with the handler
            handler();
        }
    }

}