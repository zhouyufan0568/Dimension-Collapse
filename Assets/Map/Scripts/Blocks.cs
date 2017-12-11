using UnityEngine;

public enum BlockType : byte
{
	Default = 0,
	Stone_andesite_smooth= 1,
	BookShelf = 2,
	Glass_magenta = 3,
	Glass_cyan = 4,
	Concrete_green=5,
	Concrete_blue=6,
	Wool_Colored_orange=7,
	Concrete_red=8,
	Concrete_silver=9,
	Shulker_top_blue=10,
	Shulker_top_line=11,
	Shulker_top_yellow=12,
	Wool_colored_yellow=13,
	Cobblestone=14,
	Hardened_clay_staind_brown=15,
	Hardened_clay_staind_white=16,
	Pumpkin_face_on=17,
	Shulker_top_brown=18,
	Stone_granite_smooth=19,
	Trandoor=20,
	Wool_colored_lime=21,
	Wool_colored_red=22,
	black=23,
	Wool_colored_pink=24,
	grass=25,
	sea=26
}

public enum BlockFace : byte
{
	Top = 0,
	Side = 1,
	Bottom = 2
}

public class BlockUVCoordinates
{
	private readonly Rect[] m_BlockFaceUvCoordinates = new Rect[3];

	public BlockUVCoordinates(Rect topUvCoordinates, Rect sideUvCoordinates, Rect bottomUvCoordinates)
	{
		BlockFaceUvCoordinates[(int)BlockFace.Top] = topUvCoordinates;
		BlockFaceUvCoordinates[(int)BlockFace.Side] = sideUvCoordinates;
		BlockFaceUvCoordinates[(int)BlockFace.Bottom] = bottomUvCoordinates;
	}


	public Rect[] BlockFaceUvCoordinates
	{
		get { return m_BlockFaceUvCoordinates; }
	}
}