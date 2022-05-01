using System;
using System.Runtime.CompilerServices;

namespace SHN
{
	public class SHNColumn
	{
		public int ID
		{
			get;
			set;
		}

		public int Length
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public uint Type
		{
			get;
			set;
		}

		public SHNColumn()
		{
		}

		public new System.Type GetType()
		{
			switch (this.Type)
			{
				case 1:
				case 12:
				{
					return typeof(byte);
				}
				case 2:
				{
					return typeof(ushort);
				}
				case 3:
				case 11:
				{
					return typeof(uint);
				}
				case 5:
				{
					return typeof(float);
				}
				case 9:
				case 24:
				case 26:
				{
					return typeof(string);
				}
				case 13:
				case 21:
				{
					return typeof(short);
				}
				case 16:
				{
					return typeof(byte);
				}
				case 18:
				case 27:
				{
					return typeof(uint);
				}
				case 20:
				{
					return typeof(sbyte);
				}
				case 22:
				{
					return typeof(int);
				}
				case 29:
				{
					return typeof(string);
				}
				default:
				{
					return typeof(object);
				}
			}
		}
	}
}