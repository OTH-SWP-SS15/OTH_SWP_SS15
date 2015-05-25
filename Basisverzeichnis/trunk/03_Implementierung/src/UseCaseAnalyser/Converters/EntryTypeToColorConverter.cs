#region Copyright information
// <summary>
// <copyright file="EntryTypeToColorConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>20/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// convertes the report entry type to a color 
    /// log --> blue,
    /// warning --> yellow,
    /// error --> red
    /// default --> none
    /// </summary>
    public class EntryTypeToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Report.Entrytype? type = value as Report.Entrytype?;
            if (type == null || type.Value == Report.Entrytype.DEFAULT)
            {
                return null;
            }


            Brush entryTypeBrush = type.Value == Report.Entrytype.LOG
                ? Brushes.LightGreen : type.Value == Report.Entrytype.WARNING ? Brushes.LightYellow : Brushes.LightSalmon;

            return entryTypeBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}