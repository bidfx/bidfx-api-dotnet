using System;
using System.Collections.Generic;
using System.IO;
using BidFX.Public.API.Price.Subject;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    internal class SubscriptionSync : IOutgoingPixieMessage
    {
        private const int CompressionLevel = 6;

        private const int CompressionBit = 0;
        private const int ControlsBit = 1;
        private const int UnchangedBit = 2;

        private uint _options;
        private readonly int _edition;
        private readonly int _size;
        public List<Subject.Subject> Subjects { get; internal set; }
        private readonly Dictionary<int, ControlOperation> _controls = new Dictionary<int, ControlOperation>();

        public int Edition
        {
            get { return _edition; }
        }

        public Dictionary<int, ControlOperation> Controls
        {
            get { return _controls; }
        }

        public SubscriptionSync(int edition, List<Subject.Subject> subjects)
        {
            _edition = Params.NotNegative(edition);
            Subjects = subjects;
            _size = subjects.Count;
        }

        public void AddControl(int sid, ControlOperation controlOperation)
        {
            if (sid < 0 || sid >= _size)
            {
                throw new ArgumentException("control SID (" + sid + ") not in range 0..<" + _size);
            }
            _controls[sid] = controlOperation;
            _options = BitSetter.SetBit(_options, ControlsBit);
        }

        public bool HasControls()
        {
            return BitSetter.IsSet(_options, ControlsBit);
        }

        public void SetChangedEdition(bool changed)
        {
            _options = BitSetter.ChangeBit(_options, UnchangedBit, !changed);
        }

        public bool IsChangedEdition()
        {
            return !BitSetter.IsSet(_options, UnchangedBit);
        }

        public void SetCompressed(bool compressed)
        {
            _options = BitSetter.ChangeBit(_options, CompressionBit, compressed);
        }

        public bool IsCompressed()
        {
            return BitSetter.IsSet(_options, CompressionBit);
        }

        public int Size
        {
            get { return _size; }
        }

        public MemoryStream Encode(int version)
        {
            if (version < 2 && !IsChangedEdition())
            {
                throw new IncompatibilityException(version, "SubscriptionSync with unchanged edition");
            }
            var memoryStream = new MemoryStream();
            memoryStream.WriteByte(PixieMessageType.SubscriptionSync);
            Varint.WriteU32(memoryStream, version < 2 ? _options & 1 : _options);
            Varint.WriteU32(memoryStream, _edition);
            Varint.WriteU32(memoryStream, _size);

            var appender = IsCompressed()
                ? (IStreamCompressor) new ZlibStreamCompressor(CompressionLevel)
                : new UncompressedStreamCompressor();

            var subjectBuffer = new MemoryStream();
            if (IsChangedEdition())
            {
                foreach (var subject in Subjects)
                {
                    Varint.WriteStringArray(subjectBuffer,
                        SubjectLobotomy.ExtractComponents(SubjectMutator.ToOldVersion(subject)));
                    appender.Compress(subjectBuffer);
                }
            }
            if (HasControls() && version >= 2)
            {
                Varint.WriteU32(subjectBuffer, _controls.Count);
                foreach (var control in _controls)
                {
                    Varint.WriteU32(subjectBuffer, control.Key);
                    var controlOperation = control.Value;
                    subjectBuffer.WriteByte(ControlOperationExtenstions.GetCode(controlOperation));
                }
                appender.Compress(subjectBuffer);
            }
            var compressed = appender.GetCompressed();
            memoryStream.Write(compressed, 0, compressed.Length);
            return memoryStream;
        }

        public string Summarize()
        {
            return "SubscriptionSync(edition=" + _edition
                   + ", compressed=" + IsCompressed()
                   + ", controls=" + _controls.Count
                   + ", changed=" + IsChangedEdition()
                   + ", subjects=" + _size + ')';
        }
    }
}