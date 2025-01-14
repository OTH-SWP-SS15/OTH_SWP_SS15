#region Copyright information
// <summary>
// <copyright file="UseCaseToScenarioCountConverter.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>22/06/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>
#endregion
using System.Linq;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.Converters
{
    /// <summary>
    /// converts the usecase graph to a description with a count of its scenarios
    /// </summary>
    public class UseCaseToScenarioCountConverter : GenericValueConverter<UseCaseGraph,string>
    {
        /// <summary>
        /// converts the source value of type TSource to a target value of type TTarget
        /// </summary>
        /// <param name="source">value to be converted</param>
        /// <returns>the converted value</returns>
        public override string Convert(UseCaseGraph source)
        {
            return string.Format("Scenarios ({0})", source.Scenarios.Count());
        }
    }
}