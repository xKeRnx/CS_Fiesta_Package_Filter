using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;

namespace SHN
{
	public class SHNFile
	{
		public string LoadPath;

		public Encoding SHNEncoding;

		public SHNType Type;

		public DataTable Table;

		public List<SHNColumn> SHNColumns = new List<SHNColumn>();

		private byte[] CryptoHeader;

		private byte[] Data;

		private int DataLength;

		private uint Header;

		private uint RowCount;

		private uint DefaultRowLength;

		private uint ColumnCount;

		public SHNFile(string LP)
		{
			this.LoadPath = LP;
			try
			{
				this.Type = (SHNType)Enum.Parse(typeof(SHNType), Path.GetFileNameWithoutExtension(this.LoadPath));
			}
			catch
			{
				this.Type = SHNType.Unknown;
			}
		}

		private uint GetColumnLengths()
		{
			uint length = 2;
			foreach (SHNColumn sHNColumn in this.SHNColumns)
			{
				length = length + (uint)sHNColumn.Length;
			}
			return length;
		}

        public void Read()
        {
            this.Table = new DataTable();
            SHNBinaryReader sHNBinaryReader = new SHNBinaryReader(File.OpenRead(this.LoadPath), this.SHNEncoding);
            SHNBinaryReader sHNBinaryReader1 = sHNBinaryReader;
            using (sHNBinaryReader)
            {
                this.CryptoHeader = sHNBinaryReader1.ReadBytes(32);
                this.DataLength = sHNBinaryReader1.ReadInt32();
                this.Data = sHNBinaryReader1.ReadBytes(this.DataLength - 36);
            }
            SHNCrypto.CryptoDefault(this.Data);
            sHNBinaryReader1 = new SHNBinaryReader(new MemoryStream(this.Data), this.SHNEncoding);
            this.Header = sHNBinaryReader1.ReadUInt32();
            this.RowCount = sHNBinaryReader1.ReadUInt32();
            this.DefaultRowLength = sHNBinaryReader1.ReadUInt32();
            this.ColumnCount = sHNBinaryReader1.ReadUInt32();
            int num = 0;
            int num1 = 0;
            for (uint i = 0; i < this.ColumnCount; i++)
            {
                string str = sHNBinaryReader1.ReadString(48);
                uint num2 = sHNBinaryReader1.ReadUInt32();
                int num3 = sHNBinaryReader1.ReadInt32();
                if (str.Length == 0 || string.IsNullOrWhiteSpace(str))
                {
                    str = string.Format("Unknown: {0}", num);
                    num++;
                }
                SHNColumn sHNColumn = new SHNColumn()
                {
                    ID = num1,
                    Name = str,
                    Type = num2,
                    Length = num3
                };
                SHNColumn sHNColumn1 = sHNColumn;
                this.SHNColumns.Add(sHNColumn1);
                DataColumn dataColumn = new DataColumn()
                {
                    ColumnName = str,
                    DataType = sHNColumn1.GetType()
                };
                this.Table.Columns.Add(dataColumn);
                num1++;
            }
            object[] objArray = new object[this.ColumnCount];
            for (uint j = 0; j < this.RowCount; j++)
            {
                sHNBinaryReader1.ReadUInt16();
                foreach (SHNColumn sHNColumn2 in this.SHNColumns)
                {
                    switch (sHNColumn2.Type)
                    {
                        case 1:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadByte();
                                continue;
                            }
                        case 2:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadUInt16();
                                continue;
                            }
                        case 3:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadUInt32();
                                continue;
                            }
                        case 5:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadSingle();
                                continue;
                            }
                        case 9:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadString(sHNColumn2.Length);
                                continue;
                            }
                        case 11:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadUInt32();
                                continue;
                            }
                        case 12:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadByte();
                                continue;
                            }
                        case 13:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadInt16();
                                continue;
                            }
                        case 16:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadByte();
                                continue;
                            }
                        case 18:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadUInt32();
                                continue;
                            }
                        case 20:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadSByte();
                                continue;
                            }
                        case 21:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadInt16();
                                continue;
                            }
                        case 22:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadInt32();
                                continue;
                            }
                        case 24:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadString(sHNColumn2.Length);
                                continue;
                            }
                        case 26:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadString();
                                continue;
                            }
                        case 27:
                            {
                                objArray[sHNColumn2.ID] = sHNBinaryReader1.ReadUInt32();
                                continue;
                            }
                        case 29:
                            {
                                objArray[sHNColumn2.ID] = string.Concat(sHNBinaryReader1.ReadUInt32(), ":", sHNBinaryReader1.ReadUInt32());
                                continue;
                            }
                        default:
                            {
                                continue;
                            }
                    }
                }
                this.Table.Rows.Add(objArray);
            }

        }

		public void Write(string WritePath)
		{
			if (WritePath == this.LoadPath)
			{
				if (File.Exists(this.LoadPath))
				{
					if (File.Exists(string.Format("{0}.bak", this.LoadPath)))
					{
						File.Delete(string.Format("{0}.bak", this.LoadPath));
					}
					File.Move(this.LoadPath, string.Format("{0}.bak", this.LoadPath));
				}
			}
			else if (File.Exists(WritePath) && File.Exists(WritePath))
			{
				if (File.Exists(string.Format("{0}.bak", WritePath)))
				{
					File.Delete(string.Format("{0}.bak", WritePath));
				}
				File.Move(WritePath, string.Format("{0}.bak", WritePath));
			}
			MemoryStream memoryStream = new MemoryStream();
			SHNBinaryWriter sHNBinaryWriter = new SHNBinaryWriter(memoryStream, this.SHNEncoding);
			sHNBinaryWriter.Write(this.Header);
			sHNBinaryWriter.Write(this.Table.Rows.Count);
			sHNBinaryWriter.Write(this.GetColumnLengths());
			sHNBinaryWriter.Write(this.Table.Columns.Count);
			foreach (SHNColumn sHNColumn in this.SHNColumns)
			{
				if (!sHNColumn.Name.StartsWith("Unknown"))
				{
					sHNBinaryWriter.WriteString(sHNColumn.Name, 48);
				}
				else
				{
					sHNBinaryWriter.Write(new byte[48]);
				}
				sHNBinaryWriter.Write(sHNColumn.Type);
				sHNBinaryWriter.Write(sHNColumn.Length);
			}
			foreach (DataRow row in this.Table.Rows)
			{
				long position = sHNBinaryWriter.BaseStream.Position;
				sHNBinaryWriter.Write((ushort)0);
				foreach (SHNColumn sHNColumn1 in this.SHNColumns)
				{
					object itemArray = row.ItemArray[sHNColumn1.ID];
					if (itemArray == null)
					{
						itemArray = "0";
					}
					switch (sHNColumn1.Type)
					{
						case 1:
						{
							if (itemArray is string)
							{
								itemArray = byte.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((byte)itemArray);
							goto case 28;
						}
						case 2:
						{
							if (itemArray is string)
							{
								itemArray = ushort.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((ushort)itemArray);
							goto case 28;
						}
						case 3:
						{
							if (itemArray is string)
							{
								itemArray = uint.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((uint)itemArray);
							goto case 28;
						}
						case 4:
						case 6:
						case 7:
						case 8:
						case 10:
						case 14:
						case 15:
						case 17:
						case 19:
						case 23:
						case 25:
						case 28:
						{
							long num = sHNBinaryWriter.BaseStream.Position - position;
							long position1 = sHNBinaryWriter.BaseStream.Position;
							sHNBinaryWriter.BaseStream.Seek(position, SeekOrigin.Begin);
							sHNBinaryWriter.Write((ushort)num);
							sHNBinaryWriter.BaseStream.Seek(position1, SeekOrigin.Begin);
							continue;
						}
						case 5:
						{
							if (itemArray is string)
							{
								itemArray = float.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((float)itemArray);
							goto case 28;
						}
						case 9:
						{
							if (!string.IsNullOrWhiteSpace(itemArray.ToString()))
							{
								sHNBinaryWriter.WriteString((string)itemArray, sHNColumn1.Length);
								goto case 28;
							}
							else
							{
								sHNBinaryWriter.WriteString(itemArray.ToString(), sHNColumn1.Length);
								goto case 28;
							}
						}
						case 11:
						{
							if (itemArray is string)
							{
								itemArray = uint.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((uint)itemArray);
							goto case 28;
						}
						case 12:
						{
							if (itemArray is string)
							{
								itemArray = byte.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((byte)itemArray);
							goto case 28;
						}
						case 13:
						{
							if (itemArray is string)
							{
								itemArray = short.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((short)itemArray);
							goto case 28;
						}
						case 16:
						{
							if (itemArray is string)
							{
								itemArray = byte.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((byte)itemArray);
							goto case 28;
						}
						case 18:
						{
							if (itemArray is string)
							{
								itemArray = uint.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((uint)itemArray);
							goto case 28;
						}
						case 20:
						{
							if (itemArray is string)
							{
								itemArray = sbyte.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((sbyte)itemArray);
							goto case 28;
						}
						case 21:
						{
							if (itemArray is string)
							{
								itemArray = short.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((short)itemArray);
							goto case 28;
						}
						case 22:
						{
							if (itemArray is string)
							{
								itemArray = int.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((int)itemArray);
							goto case 28;
						}
						case 24:
						{
							sHNBinaryWriter.WriteString((string)itemArray, sHNColumn1.Length);
							goto case 28;
						}
						case 26:
						{
							sHNBinaryWriter.WriteString((string)itemArray, -1);
							goto case 28;
						}
						case 27:
						{
							if (itemArray is string)
							{
								itemArray = uint.Parse((string)itemArray);
							}
							sHNBinaryWriter.Write((uint)itemArray);
							goto case 28;
						}
						case 29:
						{
							if (!itemArray.ToString().Contains(":"))
							{
								goto case 28;
							}
							string[] strArrays = itemArray.ToString().Split(new char[] { ':' });
							sHNBinaryWriter.Write(uint.Parse(strArrays[0]));
							sHNBinaryWriter.Write(uint.Parse(strArrays[1]));
							goto case 28;
						}
						default:
						{
							goto case 28;
						}
					}
				}
			}
		    byte[] buffer = memoryStream.GetBuffer();
		    byte[] numArray = new byte[checked(memoryStream.Length)];
			Array.Copy(buffer, numArray, memoryStream.Length);
			SHNCrypto.CryptoDefault(numArray);
			sHNBinaryWriter.Close();
			sHNBinaryWriter = new SHNBinaryWriter(File.Create(WritePath), this.SHNEncoding);
			sHNBinaryWriter.Write(this.CryptoHeader);
			sHNBinaryWriter.Write((int)numArray.Length + 36);
			sHNBinaryWriter.Write(numArray);
			sHNBinaryWriter.Close();
			this.LoadPath = WritePath;
		}
	}
}