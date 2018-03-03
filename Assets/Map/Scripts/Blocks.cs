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
	sea=26,
    grass_path_top = 27,
    grass_path_top_deep = 28,
    grass_path_top_s = 29,
    stonebrick_mossy = 30,
    door_iron_upper_s = 31,
    Tnt=32,
    Tnt_side=33,
    Tnt_bottom=34,
    wheat_stage_3 = 35,
    sandstone_carved = 36,
    brick1=37,
    brick2=38,
    brickside=39,
    cobblestone=40,
    Concrete_cyan=41,
    Concrete_green1=42,
    Concrete_powder_light_blue =43,
    Concrete_lime=44,
    Concrete_powder_orange=45,
    Concrete_powder_yellow=46,
    Concrete_red1=47,
    Crafting_table_top=48,
    Daylight_detector_top=49,
    Dispenser_front_horizontal=50,
    Door_wood_upper=51,
    dropper_front_horizontal=52,
    end_stone=53,
    grass_top=54,
    hardened_clay_stained_brown=55,
    hopper_top=56,
    leaves_acacia=57,
    leaves_jungle_opaque=58,
    liie_oak_gate_side=59,
    log_oak=60,
    log_oak1=61,
    observer_front=62,
    obsidian=63,
    planks_oak=64,
    purpur_block=65,
    stone=66,
    stonebrick_carved=67,
    stonebrick_cracked=68,
    tnt_top1=69,
    wheat_stage_7=70,
    wool_colored_brown=71,
    wool_colored_cyan=72,
    wool_colored_gray=73,
    wool_colored_orange=74,
    wool_colored_red=75,
    wool_colored_yellow=76
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