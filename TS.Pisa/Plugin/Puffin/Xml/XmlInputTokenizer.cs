using System.IO;

namespace TS.Pisa.Plugin.Puffin.Xml
{
    /// <summary>
    /// This class provides an Xml language input stream tokenizer for well-formed
    /// Xml expressions.
    /// </summary>
    /// <remarks>
    /// This class provides an Xml language input stream tokenizer for well-formed
    /// Xml expressions.  This is basically a cut-down Xml lexical analyser.  It
    /// cannot handle all elements of Xml but it can handle most; in particular it
    /// can handle the typical Xml documents that are used for Tradingscreen
    /// inter-process communication messaging.  It is designed to be fast rather
    /// than comprehensive.
    /// <p> The most notable Xml features that are not supported include: unicode
    /// characters, directives, processing instructions and comments.  None of
    /// these are required for IPC.
    /// </remarks>
    /// <author>Paul Sweeny</author>
    public class XmlInputTokenizer : XmlStringTokenizer
    {
        private Stream _in;

        /// <summary>Construct a new parser for the given input stream.</summary>
        /// <param name="in">the input stream to read Xml data from.</param>
        /// <param name="bufferSize">the internal buffer size.</param>
        public XmlInputTokenizer(Stream inStream, int bufferSize)
            : base(new byte[bufferSize], 0)
        {
            _in = inStream;
        }

        /// <summary>Construct a new parser for the given input stream.</summary>
        /// <param name="in">the input stream to read Xml data from.</param>
        public XmlInputTokenizer(Stream inStream)
            : this(inStream, 2048)
        {
        }

        /// <summary>Fill the buffer from the input stream.</summary>
        /// <returns>true if there is more data to read and false otherwise.</returns>
        /// <exception cref="XmlSyntaxException">
        /// if an I/O error occurred while refilling the
        /// buffer.
        /// </exception>
        /// <exception cref="XmlSyntaxException"/>
        public override bool FillBuffer()
        {
            return FillBuffer(_in);
        }

        /// <summary>Close the input stream.</summary>
        /// <exception cref="System.IO.IOException"/>
        public void Close()
        {
            _in.Close();
        }
    }
}