using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Threading;


//Model
namespace K_Means
{
    class KMeans
    {
        private readonly int ImageWidth;
        private readonly int ImageHeight;
        struct ClassWithColor
        {
            public ClassWithColor(Point point, Color color)
            {
                this.Color = color;
                this.Class = point;
            }
            public Point Class;
            public Color Color;
        }

        struct PointInClasster
        {
            public PointInClasster(Point point, Color color)
            {
                this.Point = point;
                this.Color = color;
            }
            public Point Point;
            public Color Color;
        }
        

        public KMeans(int imageWidth, int imageHeight)
        {
            this.ImageWidth = imageWidth;
            this.ImageHeight = imageHeight;
        }

        public DrawingImage GetDrawingImage(int pointsCount, int classesCount)
        {
            
            //Получение случайных точек
            List<Point> points = GetRandomPoints(pointsCount, ImageHeight, ImageWidth);
            //Получение случайных классов
            List<Point> classes = GetRandomClasses(classesCount, points);
            //Присвоение классам цвета
            List<ClassWithColor> listOfClassesWithColor = CreateListOfClassesWithColors(classes);
            //Позиционироание точек в класстеры
            List<PointInClasster> listOfPointsInClasster = CreateListOfPointsInClasster(points, listOfClassesWithColor);

            //Нормализация 
            Boolean isNecessaryRecalculate = true;
            while (isNecessaryRecalculate)
            {
                //Перерасчет классов
                var newListOfClasses = CalculateNewClasses(listOfClassesWithColor, listOfPointsInClasster);
                //Позиционирование точек в класстеры
                var newListOfPoints = CreateListOfPointsInClasster(points, newListOfClasses);

                //Проверка на сходство списков
                Boolean isSame = true;
                foreach (ClassWithColor newClassWithColor in newListOfClasses)
                {
                    foreach (ClassWithColor classWithColor in listOfClassesWithColor)
                    {
                        if (newClassWithColor.Color == classWithColor.Color && newClassWithColor.Class != classWithColor.Class)
                            isSame = false;                 
                    }
                }

                if (isSame) 
                    isNecessaryRecalculate = false;

                listOfClassesWithColor = newListOfClasses;
                listOfPointsInClasster = newListOfPoints;
                
            }

            var drawingGroup = new DrawingGroup();

            ////Рисование начальных точек
            #region
            //foreach (Point point in points)
            //{
            //    var ellipses = new GeometryGroup();
            //    int pointSize = 1;
            //    ellipses.Children.Add(new EllipseGeometry(point, pointSize, pointSize));
            //    var brush = new SolidColorBrush(GetColorFromNumber(0));
            //    var geometryDrawing = new GeometryDrawing(brush, new Pen(brush, 1), ellipses);
            //    geometryDrawing.Geometry = ellipses;
            //    drawingGroup.Children.Add(geometryDrawing);
            //}
            #endregion

            ////Рисование классов
            #region
            //foreach (ClassWithColor classWithColor in listOfClassesWithColor)
            //{
            //    var ellipses = new GeometryGroup();
            //    int pointSize = 4;
            //    ellipses.Children.Add(new EllipseGeometry(classWithColor.Class, pointSize, pointSize));
            //    var brush = new SolidColorBrush(classWithColor.Color);
            //    var geometryDrawing = new GeometryDrawing(brush, new Pen(brush, 1), ellipses);
            //    geometryDrawing.Geometry = ellipses;
            //    drawingGroup.Children.Add(geometryDrawing);
            //}
            #endregion

            //Рисование класстеров (разноцветных точек)
            foreach (PointInClasster pointInClasster in listOfPointsInClasster)
            {
                var ellipses = new GeometryGroup();
                int pointSize = 1;
                ellipses.Children.Add(new EllipseGeometry(pointInClasster.Point, pointSize, pointSize));
                var brush = new SolidColorBrush(pointInClasster.Color);
                var geometryDrawing = new GeometryDrawing(brush, new Pen(brush, 1), ellipses);
                geometryDrawing.Geometry = ellipses;
                drawingGroup.Children.Add(geometryDrawing);
            }

            //Рисование классов
            foreach (ClassWithColor classWithColor in listOfClassesWithColor)
            {
                var ellipses = new GeometryGroup();
                int pointSize = 7;
                ellipses.Children.Add(new EllipseGeometry(classWithColor.Class, pointSize, pointSize));
                var brush = new SolidColorBrush(classWithColor.Color);
                var geometryDrawing = new GeometryDrawing(brush, new Pen(brush, 1), ellipses);
                geometryDrawing.Geometry = ellipses;
                drawingGroup.Children.Add(geometryDrawing);
            }

            return new DrawingImage(drawingGroup);
        }

        private List<ClassWithColor> CalculateNewClasses(List<ClassWithColor> listOfClasses, List<PointInClasster> listOfPoints)
        {
            List<ClassWithColor> newListOfClasses = new List<ClassWithColor>();
            foreach(ClassWithColor _class in listOfClasses)
            {
                var listOfClass = new List<Point>();
                foreach(PointInClasster point in listOfPoints)
                {
                    if (point.Color == _class.Color)
                    {
                        listOfClass.Add(point.Point);
                    }
                }
                var classWithColor = new ClassWithColor(new Point(listOfClass.Average(x => x.X), listOfClass.Average(y => y.Y)), _class.Color);
                newListOfClasses.Add(classWithColor);
            }
            return newListOfClasses;
        }

        private double GetDistance(Point a, Point b)
        {
            return Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Y - b.Y), 2));
        }

        private List<PointInClasster> CreateListOfPointsInClasster(List<Point> points, List<ClassWithColor> classesWithColor)
        {
            List<PointInClasster> result = new List<PointInClasster>();
            foreach (Point point in points)
            {
                double minDistance = Double.MaxValue;
                Color color = new Color();
                foreach (ClassWithColor classWithColor in classesWithColor)
                {
                    if (minDistance > GetDistance(point, classWithColor.Class))
                    {
                        minDistance = GetDistance(point, classWithColor.Class);
                        color = classWithColor.Color;
                    }
                }
                result.Add(new PointInClasster(point, color));
            }
            return result;
        }

        private List<ClassWithColor> CreateListOfClassesWithColors(List<Point> classes)
        {
            var result = new List<ClassWithColor>();
            int colorStep = 256*256*256/(classes.Count);
            int i = 1;
            foreach (Point _class in classes)
            {
                ClassWithColor classWithColor;
                classWithColor.Class = _class;
                classWithColor.Color = GetColorFromNumber(i * colorStep);
                result.Add(classWithColor);
                i++;
            }
            return result;
        }

        //Выбирает среди существующих pounts
        private List<Point> GetRandomClasses(int countOfClasses, List<Point> points)
        {
            Random random = new Random();
            List<Point> randomClasses = new List<Point>(countOfClasses);
            for (int i = 0; i < countOfClasses; i++)
            {
                Point newClass = new Point();
                newClass = points[random.Next(points.Count)];
                //Проверка на наличие такого класса
                if (randomClasses.IndexOf(newClass) == -1)
                    randomClasses.Add(newClass);
                else
                    i--;
            }
            return randomClasses;
        }

        private List<Point> GetRandomPoints(int pointsCount, int height, int width)
        {
            var result = new List<Point>(pointsCount);
            var random = new Random();
            for (int i = 0; i < pointsCount; i++)
            {
                Point newPoint = new Point(random.Next(width), random.Next(height));
                //Проверка на наличие такой точки
                if (result.IndexOf(newPoint) == -1)
                    result.Add(newPoint);
                else
                    i--;
            }
            return result;
        }

        private Color GetColorFromNumber(int number)
        {
            Random rn = new Random();
            //byte blue = (byte)(number & 0x000000FF);
            //byte green = (byte)((number & 0x0000FF00) >> 8);
            //byte red = (byte)((number & 0x00FF0000) >> 16);
            //byte be = (byte)((number & 0xFF000000 >> 32));
            Thread.Sleep(30);
            return Color.FromRgb((byte)rn.Next(256), (byte)rn.Next(256), (byte)rn.Next(256));
        }

    }
}