using System;
using System.IO;

namespace Elements
{
	public class SimpleMemoryStream
	{
		private byte[] data;

		private long position;

		public long Position
		{
			[global::OpenPCR.Migration("04f87394-f7ea-4625-858e-4d02478cf2de")]
			get
			{
				return position;
			}
			[global::OpenPCR.Migration("36665faf-b50b-4063-927b-decf118bfba4")]
			set
			{
				position = value;
			}
		}

		[global::OpenPCR.Migration("a32ca72e-c8e9-4b9b-8418-b2a98da37e2e")]
		public SimpleMemoryStream(byte[] _data)
		{
			data = _data;
		}

		[global::OpenPCR.Migration("3ab7562e-97c7-4729-94d7-0b0de2af1cc3")]
		public int ReadByte()
		{
			return data[position++];
		}

		[global::OpenPCR.Migration("d496477c-1b5e-4472-b2cc-4c489a4b0d63")]
		public int Read(byte[] _dest_buffer, int _dest_offset, int _copy_size)
		{
			Buffer.BlockCopy(data, (int)position, _dest_buffer, _dest_offset, _copy_size);
			position += _copy_size;
			return _copy_size;
		}

		[global::OpenPCR.Migration("573cbac8-c4f9-442d-af13-2b36d619ca8e")]
		public long Seek(long _position, SeekOrigin _origin)
		{
			switch (_origin)
			{
				case SeekOrigin.Begin:
					position = _position;
					break;
				case SeekOrigin.Current:
					position += _position;
					break;
			}
			return position;
		}
		[global::OpenPCR.Migration("b6e68a11-5690-4779-8e86-05cee8c9fa19")]
		public void Close()
		{
		}

	}
}
