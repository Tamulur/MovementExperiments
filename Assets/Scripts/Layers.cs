
public class Layers 
{
	public enum Layer
	{
		Default					= 0,
		TransparentFX			= 1,
		Ignore_Raycast		= 2,
		Builtin_3					= 3,
		Water					= 4,
		UI							= 5,
		Builtin_6					= 6,
		Builtin_7					= 7,
		PlayerMain				= 8,
		PlayerLimbs				= 9,
		PlayerHead				= 10
	}


	public static int Mask( Layer layer )
	{
		return 1 << (int) layer;
	}
}
