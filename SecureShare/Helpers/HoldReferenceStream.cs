using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

/*
 * Stream to hold a reference to object (prevent it from being disposed by the garbage collector)
 * Used with Amazon S3 to not kill the HTTP connection when the response from GetObject is disposed
 * 
 * Bad design from Amazon's SDK team if you ask me...
 */
namespace ShareGrid.Helpers
{
	public class HoldReferenceStream<T>
		: Stream
	{
		public Stream stream;
		public T heldObject;

		public HoldReferenceStream(Stream streamBase, T hold)
		{
			heldObject = hold;
			stream = streamBase;
		}

		public override bool CanRead
		{
			get { return stream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return stream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return stream.CanWrite; }
		}

		public override void Flush()
		{
			stream.Flush();
		}

		public override long Length
		{
			get { return stream.Length; }
		}

		public override long Position
		{
			get
			{
				return stream.Position;
			}
			set
			{
				stream.Position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return stream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return stream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			stream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			stream.Write(buffer, offset, count);
		}
	}
}