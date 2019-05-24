// <copyright file="CompletionData.cs" company="SoluiNet">
// Copyright (c) SoluiNet. All rights reserved.
// </copyright>

namespace SoluiNet.DevTools.Core.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using ICSharpCode.AvalonEdit;
    using ICSharpCode.AvalonEdit.CodeCompletion;
    using ICSharpCode.AvalonEdit.Document;
    using ICSharpCode.AvalonEdit.Editing;

    /// <summary>
    /// Implements AvalonEdit ICompletionData interface to provide the entries in the
    /// completion drop down.
    /// </summary>
    public class CompletionData : ICompletionData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompletionData"/> class.
        /// </summary>
        /// <param name="text">The text which should be referenced for completion.</param>
        public CompletionData(string text)
        {
            this.Text = text;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        public System.Windows.Media.ImageSource Image
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the reference text for completion.
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// Gets the content.
        /// Use this property if you want to show a fancy UIElement in the list.
        /// </summary>
        public object Content
        {
            get { return this.Text; }
        }

        /// <summary>
        /// Gets the description.
        /// </summary>
        public object Description
        {
            get { return "Description for " + this.Text; }
        }

        /// <summary>
        /// Gets the priority.
        /// </summary>
        public double Priority
        {
            get { return 1.00; }
        }

        /// <summary>
        /// Complete the referenced text.
        /// </summary>
        /// <param name="textArea">The text area in which the reference text is present.</param>
        /// <param name="completionSegment">The segment in which the referenced text is present.</param>
        /// <param name="insertionRequestEventArgs">Arguments for insertion request.</param>
        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }

        /// <summary>
        /// Complete the referenced text.
        /// </summary>
        /// <param name="textEditor">The text editor in which the reference text is present.</param>
        /// <param name="completionSegment">The segment in which the referenced text is present.</param>
        /// <param name="insertionRequestEventArgs">Arguments for insertion request.</param>
        public void Complete(TextEditor textEditor, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textEditor.TextArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
