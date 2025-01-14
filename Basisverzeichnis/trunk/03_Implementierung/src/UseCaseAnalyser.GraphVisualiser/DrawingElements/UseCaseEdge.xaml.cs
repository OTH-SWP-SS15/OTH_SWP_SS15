﻿#region Copyright information

// <summary>
// <copyright file="UseCaseEdge.xaml.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>09/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>

#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using GraphFramework.Interfaces;
using LogManager;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.GraphVisualiser.DrawingElements
{
    /// <summary>
    /// Class for displaying a edge as a Bezier curve within a UseCaseGraph. On selection line color changes.
    /// It contains the edge to display as a reference.
    /// </summary>
    internal partial class UseCaseEdge : ISelectableGraphElement
    {
        /// <summary>
        /// Reference of destination visual UseCaseNode 
        /// </summary>
        public readonly UseCaseNode mDestUseCaseNode;

        /// <summary>
        /// Reference of source visual UseCaseNode
        /// </summary>
        public readonly UseCaseNode mSourceUseCaseNode;

        #region constructors

        /// <summary>
        /// Creates a new instance of an visual presenter of an UseCaseEdge
        /// </summary>
        /// <param name="source">Source UseCaseNode</param>
        /// <param name="dest">Destination UseCaseNode</param>
        /// <param name="edge">Reference to the Edge in the Graph</param>
        public UseCaseEdge(UseCaseNode source, UseCaseNode dest, IEdge edge)
        {
            InitializeComponent();
            Edge = edge;
            mSourceUseCaseNode = source;
            mDestUseCaseNode = dest;
            //Add Edge Reference to visual UseCaseNodes
            mSourceUseCaseNode.AddEdge(this);
            mDestUseCaseNode.AddEdge(this);

            IAttribute nameAttribute = source.Node.GetAttributeByName(NodeAttributes.NodeType.AttributeName());

            ProcessType = nameAttribute != null && nameAttribute.Value.Equals(UseCaseGraph.NodeTypeAttribute.JumpNode)
                ? EdgeProcessType.BackwardEdge
                : EdgeProcessType.ForwardEdge;

            //Set properties for visual appearience
            Stroke = mUnselectDrawingBrush = new SolidColorBrush(Colors.Black);
            StrokeThickness = 1.5;           
            //This geometry will represent an arrow at the end of this edge
            EndCap = Geometry.Parse("M0,0 L6,-6 L6,6 z");

            RecalcBezier();
        }

        #endregion

        /// <summary>
        /// Reference to the Edge in the UseCaseGraph
        /// </summary>
        public IEdge Edge { get; private set; }

        /// <summary>
        /// Recaclulation of the Bezier curve and redraw the Edge 
        /// </summary>
        public void RecalcBezier()
        {
            //Set Start- and End-Position of Edge depending on the Position of the Edges
            switch (ProcessType)
            {
                case EdgeProcessType.ForwardEdge:
                    //Source node over destination node
                    if (Canvas.GetTop(mSourceUseCaseNode) + mSourceUseCaseNode.Height < Canvas.GetTop(mDestUseCaseNode))
                    {
                        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                        // Patrick Schiessl: keep if statement for better readability
                        if (Canvas.GetLeft(mSourceUseCaseNode) + mSourceUseCaseNode.Width < Canvas.GetLeft(mDestUseCaseNode))
                            DockPosDestElement = DockedStatus.Left;
                        else
                            DockPosDestElement = DockedStatus.Top;
                        
                        DockPosSourceElement = DockedStatus.Bottom;
                    }
                    //Source node under destination node
                    else if (Canvas.GetTop(mDestUseCaseNode) + mDestUseCaseNode.Height <
                             Canvas.GetTop(mSourceUseCaseNode))
                    {
                        DockPosSourceElement = DockedStatus.Top;
                        DockPosDestElement = DockedStatus.Bottom;
                    }
                    //Source node left of destination node
                    else if (Canvas.GetLeft(mSourceUseCaseNode) + mSourceUseCaseNode.Width <
                             Canvas.GetLeft(mDestUseCaseNode))
                    {
                        DockPosSourceElement = DockedStatus.Right;
                        DockPosDestElement = DockedStatus.Left;
                    }
                    //Source node right of destination node
                    else if (Canvas.GetLeft(mSourceUseCaseNode) >
                             Canvas.GetLeft(mDestUseCaseNode) + mDestUseCaseNode.Width)
                    {
                        DockPosSourceElement = DockedStatus.Left;
                        DockPosDestElement = DockedStatus.Right;
                    }
                    break;
                case EdgeProcessType.BackwardEdge:
                    if (Canvas.GetLeft(mSourceUseCaseNode) + mDestUseCaseNode.Width < Canvas.GetLeft(mDestUseCaseNode))
                    {
                        DockPosSourceElement = DockedStatus.Right;
                        DockPosDestElement = DockedStatus.Left;
                    }
                    else
                    {
                        DockPosSourceElement = DockedStatus.Right;
                        DockPosDestElement = DockedStatus.Right;
                    }

                    break;
            }
            // Calculate Start and End Position of the capped line depending on the amount of Lines 
            // which are already exist with the same dockedstatus.
            int indexStartElement = mSourceUseCaseNode.GetEdgeIndex(this);
            int indexDestElement = mDestUseCaseNode.GetEdgeIndex(this);
            int amountIndexStart = mSourceUseCaseNode.GetCountOfEdges(this);
            int amountIndexEnd = mDestUseCaseNode.GetCountOfEdges(this);


            BezierSegment bzSeg = new BezierSegment();
            PathFigure pthFigure = new PathFigure();


            Point startpoint;
            Point endpoint;
            switch (DockPosSourceElement)
            {
                case DockedStatus.Top:
                case DockedStatus.Bottom:
                    startpoint = new Point(
                        Canvas.GetLeft(mSourceUseCaseNode) +
                        (mSourceUseCaseNode.Width/amountIndexStart)*indexStartElement,
                        Canvas.GetTop(mSourceUseCaseNode));
                   if(double.IsNaN(startpoint.X) || double.IsNaN(startpoint.Y))
                       startpoint = new Point(0,0);
          
                    double heightStart = DockPosSourceElement == DockedStatus.Bottom ? mSourceUseCaseNode.Height : 0;
                    double heightEnd = DockPosDestElement == DockedStatus.Bottom ? mDestUseCaseNode.Height : 0;

                    if (DockPosDestElement == DockedStatus.Left)
                    {
                        endpoint = new Point(Canvas.GetLeft(mDestUseCaseNode) , Canvas.GetTop(mDestUseCaseNode) + (mDestUseCaseNode.Height / amountIndexEnd) * indexDestElement);
                        if (double.IsNaN(endpoint.X) || double.IsNaN(endpoint.Y))
                            endpoint = new Point(0, 0);
                        
                        bzSeg.Point1 = new Point(startpoint.X, startpoint.Y + (startpoint.Y - endpoint.Y) / 2 + heightStart);

                        bzSeg.Point2 = new Point(endpoint.X - (endpoint.X - startpoint.X) / 2, endpoint.Y + heightEnd);
                    }
                    else
                    {
                        endpoint = new Point(
                            Canvas.GetLeft(mDestUseCaseNode) + (mDestUseCaseNode.Width/amountIndexEnd)*indexDestElement,
                            Canvas.GetTop(mDestUseCaseNode));
                        if (double.IsNaN(endpoint.X) || double.IsNaN(endpoint.Y))
                            endpoint = new Point(0, 0);
                        bzSeg.Point1 = new Point(startpoint.X, endpoint.Y + heightEnd);
                        bzSeg.Point2 = new Point(endpoint.X, startpoint.Y + heightStart);
                    
                    }
                    
                    pthFigure.StartPoint = new Point(startpoint.X, startpoint.Y + heightStart);
                    bzSeg.Point1 = new Point(startpoint.X, endpoint.Y + heightEnd);
              
                    bzSeg.Point3 = new Point(endpoint.X, endpoint.Y + heightEnd);
                    break;

                case DockedStatus.Right:
                case DockedStatus.Left:
                    double widthStart = DockPosSourceElement == DockedStatus.Right ? mSourceUseCaseNode.Width : 0;
                    double widthEnd = DockPosDestElement == DockedStatus.Right ? mDestUseCaseNode.Width : 0;

                    startpoint = new Point(Canvas.GetLeft(mSourceUseCaseNode) + widthStart,
                        Canvas.GetTop(mSourceUseCaseNode) + (mSourceUseCaseNode.Height/amountIndexStart)*indexStartElement);
                    if (double.IsNaN(startpoint.X) || double.IsNaN(startpoint.Y))
                        startpoint = new Point(0, 0);
          
                    pthFigure.StartPoint = startpoint;

                    endpoint = new Point(Canvas.GetLeft(mDestUseCaseNode) + widthEnd,
                        Canvas.GetTop(mDestUseCaseNode) + (mDestUseCaseNode.Height/amountIndexEnd)*indexDestElement);
                    if (double.IsNaN(endpoint.X) || double.IsNaN(endpoint.Y))
                        endpoint = new Point(0, 0);

                    double middlePos = (endpoint.X - startpoint.X)/2;

                    //middlePos = middlePos < 0 ? middlePos * -1 : middlePos;
                    if (ProcessType == EdgeProcessType.BackwardEdge)
                    {
                        double resultEndPosY;

                        if (DockPosDestElement == DockedStatus.Right &&
                            startpoint.Y - mSourceUseCaseNode.Height/2 < endpoint.Y + mDestUseCaseNode.Height/2)
                        {
                            if (endpoint.Y > startpoint.Y + mDestUseCaseNode.Height)
                                resultEndPosY = endpoint.Y;
                            else if (startpoint.Y > endpoint.Y)
                                resultEndPosY = startpoint.Y - 1.5*mSourceUseCaseNode.Height;
                            else
                                resultEndPosY = endpoint.Y + 1.5 * mSourceUseCaseNode.Height;
                        }
                        else
                        {
                            resultEndPosY = endpoint.Y;
                        }

                        bzSeg.Point1 = new Point(startpoint.X + (mSourceUseCaseNode.Width/2), startpoint.Y);
                        bzSeg.Point2 = new Point(startpoint.X + (mDestUseCaseNode.Width/2), resultEndPosY);
                    }
                    else
                    {
                        bzSeg.Point1 = new Point(startpoint.X + middlePos, startpoint.Y);
                        bzSeg.Point2 = new Point(startpoint.X + middlePos, endpoint.Y);
                    }
                    bzSeg.Point3 = endpoint;
                    break;
            }

            if (LinePath == null)
                LinePath = new PathGeometry();

            //Exchange existing Geometry of the Capped Line 
            pthFigure.Segments.Add(bzSeg);
            LinePath.Figures.Clear();
            LinePath.Figures.Add(pthFigure);
        }

        /// <summary>
        /// Set new brush color to this Edge
        /// </summary>
        /// <param name="newBrush">future color which will be used for drawing</param>
        public void SetDrawingBrush(Brush newBrush)
        {
            if (Equals(newBrush, mUnselectDrawingBrush)) return;
            Stroke = mUnselectDrawingBrush = newBrush;
            RecalcBezier();
        }

        #region DockedStatus enum

        /// <summary>
        /// Docked status of CappedLine on UseCaseNode
        /// </summary>
        public enum DockedStatus
        {
            /// <summary>
            /// UseCaseEdge is visualised on top of UseCaseNode
            /// </summary>
            Top,

            /// <summary>
            /// UseCaseEdge is visualised on bottom of UseCaseNode
            /// </summary>
            Bottom,

            /// <summary>
            /// UseCaseEdge is visualised on the left of UseCaseNode
            /// </summary>
            Left,

            /// <summary>
            /// UseCaseEdge is visualised on the right of UseCaseNode
            /// </summary>
            Right
        }

        #endregion

        #region EdgeProcessType enum

        /// <summary>
        /// Type of UseCaseEdge which will be displayed
        /// </summary>
        public enum EdgeProcessType
        {
            /// <summary>
            /// UseCaseEdge is a forwarding edge
            /// </summary>
            ForwardEdge,

            /// <summary>
            /// UseCaseEdge is a backwarding edge (UseCaseNode with type JumpNode as source)
            /// </summary>
            BackwardEdge
        }

        #endregion

        #region Properties

        /// <summary>
        /// Selected status of the element
        /// </summary>
        public bool Selected { get; private set; }

        /// <summary>
        /// Dock position of CappedLine on the source Element
        /// </summary>
        internal DockedStatus DockPosSourceElement { get; set; }

        /// <summary>
        /// Dock Position Capped Line on the Destination Element
        /// </summary>
        internal DockedStatus DockPosDestElement { get; set; }

        /// <summary>
        /// Process type of Edge which will be displayed
        /// </summary>
        internal EdgeProcessType ProcessType { get; set; }

        /// <summary>
        /// Brush which will be displayed if the element is not selected
        /// </summary>
        private Brush mUnselectDrawingBrush;

        #endregion

        /// <summary>
        /// Select this element
        /// </summary>
        public void Select()
        {
            if(Selected)
                return;
            LoggingFunctions.Trace("Use Case Edge selected");
            Selected = true;
            Effect = new DropShadowEffect { ShadowDepth = 1, Color = Colors.DodgerBlue, Opacity = 100000, BlurRadius = 10 };
            RecalcBezier();
        }

        /// <summary>
        /// Unselect this element
        /// </summary>
        public void Unselect()
        {
            if (!Selected) 
                return;

            LoggingFunctions.Trace("Use Case Edge unselected");
            Selected = false;
            Effect = null;
            RecalcBezier();
        }

        /// <summary>
        /// Switch selection status of this element
        /// </summary>
        public void ChangeSelection()
        {
            if (Selected)
                Unselect();
            else
                Select();
        }

        /// <summary>
        /// Reference to the element in the UseCaseGraph
        /// </summary>
        public IGraphElement CurrentElement
        {
            get { return Edge; }
        }
    }
}