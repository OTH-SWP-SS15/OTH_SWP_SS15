﻿#region Copyright information

// <summary>
// <copyright file="UseCaseNode.xaml.cs">Copyright (c) 2015</copyright>
// 
// <creationDate>09/05/2015</creationDate>
// 
// <professor>Prof. Dr. Kurt Hoffmann</professor>
// <studyCourse>Angewandte Informatik</studyCourse>
// <branchOfStudy>Industrieinformatik</branchOfStudy>
// <subject>Software Projekt</subject>
// </summary>

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Effects;
using GraphFramework.Interfaces;
using LogManager;
using UseCaseAnalyser.Model.Model;

namespace UseCaseAnalyser.GraphVisualiser.DrawingElements
{
    /// <summary>
    /// Class for displaying a node as rectangule within a UseCaseGraph. On selection border color changes.
    /// It contains the node to display as a reference.
    /// </summary>
    internal partial class UseCaseNode : ISelectableGraphElement
    {
        /// <summary>
        /// List of UseCaseEdge that either start or end in this UseCaseNode
        /// </summary>
        public readonly List<UseCaseEdge> mEdges = new List<UseCaseEdge>();

        /// <summary>
        /// Wrapper class for GraphFrameworks's INode which is used to define how a node will be displayed in UseCaseGraphVisualiser.
        /// </summary>
        /// <param name="node">INode object that will be wrapped by UseCaseNode</param>
        public UseCaseNode(INode node)
        {
            InitializeComponent();
            //initalize member
            IAttribute indexAttribute = node.GetAttributeByName(NodeAttributes.NormalIndex.AttributeName());
            IAttribute variantAttribute = node.GetAttributeByName(NodeAttributes.VariantIndex.AttributeName());
            IAttribute varSeqStepAttribute = node.GetAttributeByName(NodeAttributes.VarSeqStep.AttributeName());
           
            if (indexAttribute != null)
                LblIndex.Content = indexAttribute.Value.ToString();
            if (varSeqStepAttribute != null && variantAttribute != null)
                LblIndex.Content += variantAttribute.Value.ToString() + varSeqStepAttribute.Value;

            Node = node;
            NodeBorder.BorderBrush = mUnselectDrawingBrush = Brushes.Black;
            Unselect();
        }

        /// <summary>
        /// Node property for GraphFrameworks INode element which is wrapped by this class.
        /// </summary>
        public INode Node { get; private set; }

        /// <summary>
        /// Property to check if UseCaseNode is marked as selected
        /// </summary>
        public bool Selected { get; private set; }

        /// <summary>
        /// Recursive function for rendering edges of this node by using RecalcBezier and its neighbours.
        /// </summary>
        /// <param name="notRenderNode">Optional parameter for use case node which prevents rendering of specified node's edges.</param>
        public void RenderEdges(UseCaseNode notRenderNode = null)
        {
            foreach (UseCaseEdge ucEdge in mEdges)
            {
                //render this node
                if (notRenderNode == null)
                {
                    //prevent re-rendering in recursive call by setting current node as not to render
                    if (!Equals(ucEdge.mDestUseCaseNode, this))
                        ucEdge.mDestUseCaseNode.RenderEdges(this);
                    else
                        ucEdge.mSourceUseCaseNode.RenderEdges(this);
                }
                //check if rendering of notRenderNode should be skipped (otherwise infinite recusive call of this function)
                else if (Equals(ucEdge.mSourceUseCaseNode, notRenderNode) ||
                         Equals(ucEdge.mDestUseCaseNode, notRenderNode))
                    continue;

                ucEdge.RecalcBezier();
            }
        }

        /// <summary>
        /// Add an edge to UseCaseNode if not already contained and nodes is either starting or endpoint of the specified edge.
        /// </summary>
        /// <param name="newEdge">UseCaseEgde that should be added to the UseCaseNode</param>
        public void AddEdge(UseCaseEdge newEdge)
        {
            if (mEdges.Contains(newEdge)) 
                return;

            if (Equals(newEdge.mSourceUseCaseNode, this))
            {
                if(mEdges.Count<=1)
                    mEdges.Add(newEdge);
                else
                    mEdges.Insert(2,newEdge);
            }

            if(Equals(newEdge.mDestUseCaseNode, this))
            {
                mEdges.Insert(0, newEdge);
            }
        }

        /// <summary>
        /// Return index of specified edge corresponding to its DockedStatus.
        /// </summary>
        /// <param name="sourceEdge">Edge for determine index.</param>
        /// <returns>Number of index.</returns>
        public int GetEdgeIndex(UseCaseEdge sourceEdge)
        {
            UseCaseEdge.DockedStatus currentDockedStatus = sourceEdge.mSourceUseCaseNode.Equals(this)
                ? sourceEdge.DockPosSourceElement
                : sourceEdge.DockPosDestElement;

            IEnumerable<UseCaseEdge> normalEdges = mEdges.FindAll(edge =>
                edge.mSourceUseCaseNode.Node.GetAttributeByName(NodeAttributes.VariantIndex.AttributeName()) == null
                && edge.mDestUseCaseNode.Node.GetAttributeByName(NodeAttributes.VariantIndex.AttributeName()) == null);

            int index = 1;
            //find index of normal edge
            foreach (UseCaseEdge edge in normalEdges)
            {
                if ((!edge.mSourceUseCaseNode.Equals(this) || edge.DockPosSourceElement != currentDockedStatus) &&
                    (!edge.mDestUseCaseNode.Equals(this) || edge.DockPosDestElement != currentDockedStatus))
                    continue;
                if (sourceEdge.Equals(edge))
                    return index;
                index++;
            }
            //get index of all other edges
            for (int i = 0 ; i < mEdges.Count; i++)
            {
                if (normalEdges.Contains(mEdges[i]))
                    continue;
                if ((!mEdges[i].mSourceUseCaseNode.Equals(this) || mEdges[i].DockPosSourceElement != currentDockedStatus) &&
                    (!mEdges[i].mDestUseCaseNode.Equals(this) || mEdges[i].DockPosDestElement != currentDockedStatus))
                    continue;
                if (sourceEdge.Equals(mEdges[i]))
                    return index;
                index++;
            }
            return 0;
        }

        /// <summary>
        /// Counts the amount of edges in depending of the docking status
        /// </summary>
        /// <param name="sourceEdge">Elements will be counted by the position of this element </param>
        /// <returns>amount of Edges at the same docking status of this node</returns>
        public int GetCountOfEdges(UseCaseEdge sourceEdge)
        {
            int index = 1;

            UseCaseEdge.DockedStatus currentDockedStatus = sourceEdge.mSourceUseCaseNode.Equals(this)
                ? sourceEdge.DockPosSourceElement
                : sourceEdge.DockPosDestElement;


            // ReSharper disable once LoopCanBeConvertedToQuery
            // [Fettstorch] keep more readable version of loop
            foreach (UseCaseEdge useCaseEdge in mEdges)
            {
                if (useCaseEdge.mSourceUseCaseNode.Equals(this) &&
                    useCaseEdge.DockPosSourceElement == currentDockedStatus ||
                    useCaseEdge.mDestUseCaseNode.Equals(this) && useCaseEdge.DockPosDestElement == currentDockedStatus)
                {
                    index++;
                }
            }
            return index;
        }

        /// <summary>
        /// Brush which will be displayed if the element is not selected
        /// </summary>
        private Brush mUnselectDrawingBrush;

        /// <summary>
        /// Color for specific scenario will be set
        /// </summary>
        /// <param name="toColorEdges">List of Edges which will be colored</param>
        /// <param name="newBrush">Brush which will be used to highlite the specific scenario</param>
        public void SetDrawingBrush(IEnumerable<IEdge> toColorEdges, Brush newBrush)
        {
            NodeBorder.BorderBrush = mUnselectDrawingBrush = newBrush;
            IEnumerable<IEdge> colorEdges = toColorEdges as IList<IEdge> ?? toColorEdges.ToList();
            for (int i = 0; i < mEdges.Count(); i++)
            {
                mEdges[i].SetDrawingBrush(colorEdges.Contains(mEdges[i].Edge) ? newBrush : Brushes.Black);
            }
        }

        /// <summary>
        /// Select this element (visualised by glowing)
        /// </summary>
        public void Select()
        {
            if(Selected)
                return;
            LoggingFunctions.Trace("Use Case Node selected");
            Selected = true;
            NodeBorder.Effect = new DropShadowEffect { ShadowDepth = 1, Color = Colors.DodgerBlue, Opacity = 1, BlurRadius = 30 };
        }

        /// <summary>
        /// Unselect this element (remove glow effect)
        /// </summary>
        public void Unselect()
        {
            if(!Selected)
                return;
            LoggingFunctions.Trace("Use Case Node unselected");
            Selected = false;
            NodeBorder.BorderBrush = mUnselectDrawingBrush;
            NodeBorder.Effect = null;
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
        /// Reference to the element in the Graph
        /// </summary>
        public IGraphElement CurrentElement
        {
            get { return Node; }
        }
    }
}