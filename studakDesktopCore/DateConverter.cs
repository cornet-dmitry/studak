using System;
using Avalonia.Data.Converters;
using System.Globalization;


namespace studakDesktopCore;


/*
Метод `Convert` в интерфейсе `IValueConverter` служит для преобразования значения из одного типа в другой. 

Он принимает следующие параметры:
- `value`: значение, которое нужно преобразовать.
- `targetType`: тип, к которому нужно выполнить преобразование.
- `parameter`: дополнительный параметр, который можно передать в метод для настройки преобразования (опциональный).
- `culture`: объект `CultureInfo`, который определяет текущий языковой и культурный контекст для
 выполнения преобразования (опциональный).

Метод `Convert` возвращает преобразованное значение указанного `targetType`.

В случае конкретного класса конвертера, метод `Convert` принимает значение типа `DateTime` и преобразует
 его в строку с заданным форматом даты "dd.MM.yyyy" с помощью вызова метода `ToString()`.
 */

public class DateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        
        if (value is DateTime date)
        {
            return date.ToString("dd.MM.yyyy");
        }

        return value;
    }
        
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}